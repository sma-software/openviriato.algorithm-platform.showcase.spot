using System.Collections.Generic;
using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.LPCreation.Solving;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution {
    public class SpotTrainFactory {
        private readonly SpotVariableFactory _variableFactory;

        public SpotTrainFactory(SpotVariableFactory variableFactory) {
            _variableFactory = variableFactory;
        }

        public ImmutableList<ISpotTrain> CreateSpotTrainsFromSolution(ISpotScenario scenario, SingleObjectiveSolution solution) {
            var pathNodesVisitedByAnyRoute = ComputePathNodesVisitedByAnyRoute(scenario, solution);
            var findSpotLineConstraintsSelectedInAnyRoute = FindSpotLineConstraintsSelectedInAnyRoute(scenario, solution);

            var spotTrains = findSpotLineConstraintsSelectedInAnyRoute
                .Select(line => CreateTrimmedTrainFromLine(scenario, solution, line, pathNodesVisitedByAnyRoute))
                .ToImmutableList();
            return spotTrains;
        }

        private ISpotTrain CreateTrimmedTrainFromLine(ISpotScenario scenario, ISolution solution, ISpotLineConstraint spotLineConstraint, ISet<ISpotPathNodeConstraint> pathNodesUsedByAnyRoute) {
            var firstUsedNodeByAnyRoute = spotLineConstraint.PathNodes.First(pathNodesUsedByAnyRoute.Contains);
            var lastUsedNodeByAnyRoute = spotLineConstraint.PathNodes.Last(pathNodesUsedByAnyRoute.Contains);
            var updateNodes = CreateVisitedSpotTrainPathNodes(scenario, solution, spotLineConstraint, firstUsedNodeByAnyRoute, lastUsedNodeByAnyRoute);
            return new SpotTrain(spotLineConstraint.ID, updateNodes.ToImmutableList(), spotLineConstraint.Code);
        }

        private ISet<ISpotPathNodeConstraint> ComputePathNodesVisitedByAnyRoute(ISpotScenario scenario, ISolution solution) {
            return scenario
                .PassengerRelationsWithRoutes
                .SelectMany(relation => relation.TravelRoutes.Where(route => solution.IsRouteSelected(route, _variableFactory)))
                .SelectMany(route => route.TravelRouteParts.SelectMany(routePart => routePart.VisitedNodes))
                .ToHashSet();
        }

        private IImmutableList<ISpotTrainPathNode> CreateVisitedSpotTrainPathNodes(ISpotScenario scenario, ISolution solution, ISpotLineConstraint line, ISpotPathNodeConstraint firstPathNodeConstraintUsedByAnyRoute, ISpotPathNodeConstraint lastPathNodeConstraintUsedByAnyRoute) {
            var trainPathNodes = new List<SpotTrainPathNode>();
            var currentCycleTimeOffset = Duration.Zero;
            var visitedTrainPathNodesFromLine = line
                .PathNodes
                .SkipWhile(pn => pn != firstPathNodeConstraintUsedByAnyRoute)
                .TakeWhile(pn => pn != lastPathNodeConstraintUsedByAnyRoute)
                .Append(lastPathNodeConstraintUsedByAnyRoute)
                .ToImmutableList();

            foreach (var tpn in visitedTrainPathNodesFromLine) {
                var arrivalTime = scenario
                    .TimeConverter
                    .FromModelTime(solution.GetValue(_variableFactory.CreateArrival(line, tpn)))
                    .Plus(currentCycleTimeOffset);
                if (tpn.SequenceNumber > 0 && solution.GetTruthValue(_variableFactory.CreateCycleTimeShift(tpn.Predecessor, tpn))) {
                    // Current assumption: shift by only one cycleTime
                    arrivalTime = arrivalTime.Plus(scenario.CycleTime);
                    currentCycleTimeOffset = currentCycleTimeOffset.Plus(scenario.CycleTime);
                }

                var departureTime = scenario
                    .TimeConverter
                    .FromModelTime(solution.GetValue(_variableFactory.CreateDeparture(line, tpn)))
                    .Plus(currentCycleTimeOffset);
                if (tpn.MinStopTime > Duration.Zero && solution.GetTruthValue(_variableFactory.CreateCycleTimeShift(tpn, tpn))) {
                    departureTime = departureTime.Plus(scenario.CycleTime);
                    currentCycleTimeOffset = currentCycleTimeOffset.Plus(scenario.CycleTime);
                }

                trainPathNodes.Add(new SpotTrainPathNode(tpn.ID, arrivalTime, departureTime, null, tpn.SequenceNumber));
            }

            return trainPathNodes.ToImmutableList();
        }

        private IImmutableList<ISpotLineConstraint> FindSpotLineConstraintsSelectedInAnyRoute(ISpotScenario scenario, ISolution solution) {
            return scenario
                .PassengerRelationsWithRoutes
                .SelectMany(relation => relation.TravelRoutes.Where(route => solution.IsRouteSelected(route, _variableFactory)))
                .SelectMany(route => route.TravelRouteParts.Select(routePart => routePart.SpotLineConstraint))
                .Distinct()
                .ToImmutableList();
        }
    }
}