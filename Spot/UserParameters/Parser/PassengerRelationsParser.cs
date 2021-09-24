using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Infrastructure.Caches;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util;
using SMA.Apps.Utils.Answers;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters.Parser {
    public class PassengerRelationsParser {
        private const int TotalDemandPerRelation = 1;
        private const int NumberOfHeaderLines = 10;
        private readonly IAlgorithmInterface _algorithmInterface;
        private readonly IImmutableList<ISpotLineConstraint> _spotLineConstraints;
        private readonly INodeLookup _nodeLookup;
        private readonly int? _maximalNumberOfTransfers;

        public PassengerRelationsParser(IAlgorithmInterface algorithmInterface, IImmutableList<ISpotLineConstraint> spotLineConstraints, INodeLookup nodeLookup, int? maximalNumberOfTransfers) {
            _algorithmInterface = algorithmInterface;
            _spotLineConstraints = spotLineConstraints;
            _nodeLookup = nodeLookup;
            _maximalNumberOfTransfers = maximalNumberOfTransfers;
        }

        public Result<IImmutableList<IPassengerRelation>> ReadRelationsFromFile(string fileName) {
            var linesWithNodesVisitedTwice = _spotLineConstraints
                .Where(cons => cons.PathNodes.Select(p => p.NodeID).Distinct().Count() < cons.PathNodes.Count)
                .ToImmutableList();

            foreach (var line in linesWithNodesVisitedTwice) {
                _algorithmInterface.NotifyUser("Line filtered", string.Format(CultureInfo.InvariantCulture, "Line {0} has been filtered because it visits one node twice. Discarding routes using this line.", line.Code));
            }

            var spotPathNodeLookup = _spotLineConstraints
                .Except(linesWithNodesVisitedTwice)
                .SelectMany(line => line.PathNodes.Select(pathNodeConstraint => (line.Code, _nodeLookup.Get(pathNodeConstraint.NodeID).Code, pathNodeConstraint)))
                .ToDictionary(triple => (triple.Item1, triple.Item2), l => l.pathNodeConstraint);

            var spotLineLookup = _spotLineConstraints
                .Except(linesWithNodesVisitedTwice)
                .ToDictionary(l => l.Code, l => l);

            var fileContentsResult = ParserUtil.ReadAllLineSafely(fileName, "passenger routes file");

            if (!fileContentsResult.Success) {
                return fileContentsResult.Answers.ToFailedResult();
            }

            var fileContents = fileContentsResult.Value.Select((line, lineNumber) => (lineNumber, line)).Skip(NumberOfHeaderLines).ToImmutableList();
            var routesByOd = new Dictionary<(long, long), List<IPassengerTravelRoute>>();
            var numberOfFilteredRoutes = 0;
            var foundOdPairs = new HashSet<(string, string)>();
            foreach (var (lineNumber, line) in fileContents) {
                var parserHelper = new ParserHelper(line.Split(';').ToImmutableList());
                var originNodeCode = parserHelper.Skip(1).Next();
                var originNodeID = _nodeLookup.Find(originNodeCode)?.ID;
                if (originNodeID == null) {
                    return Result.FromString(string.Format(CultureInfo.InvariantCulture, "Did not find a node with code {0}", originNodeCode));
                }

                var destinationNodeCode = parserHelper.Skip(2).Next();
                var destinationNodeID = _nodeLookup.Find(destinationNodeCode)?.ID;
                if (destinationNodeID == null) {
                    return Result.FromString(string.Format(CultureInfo.InvariantCulture, "Did not find a node with code {0}", destinationNodeCode));
                }

                foundOdPairs.Add((originNodeCode, destinationNodeCode));

                var passengerRoute = ParseRouteFromPassengerTravelRoutePartsSplitIfPossible(routesByOd.Values.Select(v => v.Count).Sum(), spotLineLookup, spotPathNodeLookup, parserHelper.Skip(3).Take(), lineNumber);
                if (passengerRoute.Success) {
                    if (passengerRoute.Value.NumberTransfers > _maximalNumberOfTransfers) {
                        numberOfFilteredRoutes++;
                    } else {
                        routesByOd.AddOrUpdate((originNodeID.Value, destinationNodeID.Value), passengerRoute.Value);
                    }
                } else {
                    _algorithmInterface.NotifyUser("Warning", string.Format(CultureInfo.InvariantCulture, "Parsing failed: {0}", passengerRoute.Answers.ToPrettyString()));
                }
            }

            var relations = new List<IPassengerRelation>();
            foreach (var (odPair, routes) in routesByOd) {
                relations.Add(new PassengerRelation(_nodeLookup.Get(odPair.Item1), _nodeLookup.Get(odPair.Item2), FilterDuplicates(routes.ToImmutableList()), TotalDemandPerRelation));
            }

            var odPairsWithRoutes = routesByOd
                .Select(odPair => (_nodeLookup.Get(odPair.Key.Item1).Code, _nodeLookup.Get(odPair.Key.Item2).Code))
                .ToImmutableList();

            var odPairsWithoutRoutes = foundOdPairs
                .Except(odPairsWithRoutes)
                .ToImmutableList();

            if (odPairsWithoutRoutes.Any()) {
                _algorithmInterface.NotifyUser("Warning", string.Format(CultureInfo.InvariantCulture, "The following OD-Relations are ignored, as no routes were found:\n{0}", string.Join("\n", odPairsWithoutRoutes.Select(odPair => string.Format(CultureInfo.InvariantCulture, "From {0} to {1}", odPair.Item1, odPair.Item2)))));
            }

            var relationsWithNoRoutes = odPairsWithoutRoutes
                .Select(ToRelationWithoutRoutes)
                .ToImmutableList();

            if (numberOfFilteredRoutes > 0) {
                _algorithmInterface.NotifyUser("Route Filtering", string.Format(CultureInfo.InvariantCulture, "Filtered {0} out of {1} Routes because they require more than {2} transfers", numberOfFilteredRoutes, fileContents.Count(), _maximalNumberOfTransfers));
            }

            return relations.Concat(relationsWithNoRoutes).ToImmutableList();
        }

        private IImmutableList<IPassengerTravelRoute> FilterDuplicates(ImmutableList<IPassengerTravelRoute> allRoutes) {
            var lookup = new TravelRouteLookup();
            allRoutes.Apply(lookup.Add);
            return lookup.GetAll();
        }

        private PassengerRelation ToRelationWithoutRoutes((string, string) odPair) {
            var originNode = _nodeLookup.Find(odPair.Item1);
            var destinationNode = _nodeLookup.Find(odPair.Item2);
            return new PassengerRelation(originNode, destinationNode, ImmutableList<IPassengerTravelRoute>.Empty, TotalDemandPerRelation);
        }

        private static Result<IPassengerTravelRoute> ParseRouteFromPassengerTravelRoutePartsSplitIfPossible(int numberOfCreatedRoutes, IDictionary<string, ISpotLineConstraint> spotLineLookup, IDictionary<(string, string), ISpotPathNodeConstraint> spotPathNodeLookup, IImmutableList<string> splitWithAllTravelRouteParts, int parserLineNumber) {
            var passengerRouteParts = new List<IPassengerTravelRoutePart>();
            var parser = new ParserHelper(splitWithAllTravelRouteParts);
            while (parser.Any()) {
                var nextRoutePart = parser.Take(10);
                var passengerRoutePartsResult = ParseOneRoutePartIfLineExists(spotLineLookup, spotPathNodeLookup, nextRoutePart, parserLineNumber);
                if (passengerRoutePartsResult.Success) {
                    passengerRouteParts.Add(passengerRoutePartsResult.Value);
                } else {
                    return passengerRoutePartsResult.Answers.ToFailedResult();
                }
            }

            return new PassengerTravelRoute(numberOfCreatedRoutes + 1, passengerRouteParts.ToImmutableList());
        }

        private static Result<IPassengerTravelRoutePart> ParseOneRoutePartIfLineExists(IDictionary<string, ISpotLineConstraint> spotLineLookup, IDictionary<(string, string), ISpotPathNodeConstraint> spotPathNodeLookup, IImmutableList<string> singleRoutePartSplits, int parserLineNumber) {
            int ParsedFileLineNumberToDisplayForUser() {
                return parserLineNumber + NumberOfHeaderLines + 1;
            }

            var parser = new ParserHelper(singleRoutePartSplits);
            var partStartNodeCode = parser.Skip(1).Next();
            var lineCode = parser.Skip(1).Next();
            var partEndNodeCode = parser.Skip(3).Next();

            if (!spotLineLookup.TryGetValue(lineCode, out var spotLine)) {
                return Result<IPassengerTravelRoutePart>.FromString(string.Format(CultureInfo.InvariantCulture, "Error when parsing trips: Could not find train {0} on file-line {1}", lineCode, ParsedFileLineNumberToDisplayForUser()));
            }

            if (!spotPathNodeLookup.TryGetValue((lineCode, partStartNodeCode), out var startLinePathNodeConstraint)) {
                return Result<IPassengerTravelRoutePart>.FromString(string.Format(CultureInfo.InvariantCulture, "Error when parsing trips: Could not find start node {0} of routePart on file-line {1}", partStartNodeCode, ParsedFileLineNumberToDisplayForUser()));
            }

            if (!spotPathNodeLookup.TryGetValue((lineCode, partEndNodeCode), out var endLinePathNodeConstraint)) {
                return Result<IPassengerTravelRoutePart>.FromString(string.Format(CultureInfo.InvariantCulture, "Error when parsing trips: Could not find end node {0} of routePart on file-line {1}", partEndNodeCode, ParsedFileLineNumberToDisplayForUser()));
            }

            var visitedNodes = spotLine
                .PathNodes
                .SkipWhile(pn => pn.ID != startLinePathNodeConstraint.ID)
                .TakeWhile(pn => pn.ID != endLinePathNodeConstraint.ID)
                .Append(endLinePathNodeConstraint)
                .ToImmutableList();

            return new PassengerTravelRoutePart(spotLine, visitedNodes);
        }
    }
}