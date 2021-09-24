using System;
using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.Algorithms.Utils;
using SMA.Algorithms.Utils.LPCreation.Solving;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution {
    public class PassengerRelationCharacteristicsFactory {
        private readonly SpotVariableFactory _variableFactory;

        public PassengerRelationCharacteristicsFactory(SpotVariableFactory variableFactory) {
            _variableFactory = variableFactory;
        }

        public IImmutableList<IPassengerRelationCharacteristics> CreatePassengerRelationCharacteristicsFromSolution(ISpotScenario scenario, SingleObjectiveSolution solution) {
            return scenario.PassengerRelationsWithRoutes.Select(r => ComputeResultingRelation(scenario, r, solution)).ToImmutableList();
        }

        private IPassengerRelationCharacteristics ComputeResultingRelation(ISpotScenario scenario, IPassengerRelation passengerRelation, SingleObjectiveSolution solution) {
            var onlyUsedTravelRoutes = RemoveUnusedRoutesPerPassengerTravelRoutes(passengerRelation, solution, _variableFactory).TravelRoutes;
            if (onlyUsedTravelRoutes.Count > 1) {
                // TODO: VPLAT-9033
                throw new InvalidOperationException("Only a single route per relation may be selected, SPOT model must be extended otherwise");
            }

            var travelRouteCharacteristics = ComputeTravelRouteCharacteristics(scenario, onlyUsedTravelRoutes.First(), solution, (double)passengerRelation.TotalDemand);
            return new PassengerRelationCharacteristics(passengerRelation, ImmutableListUtils.FromArray(travelRouteCharacteristics).ToImmutableList());
        }

        private static IPassengerRelation RemoveUnusedRoutesPerPassengerTravelRoutes(IPassengerRelation relation, SingleObjectiveSolution solution, SpotVariableFactory variableFactory) {
            var selectedTravelRoutes = relation
                .TravelRoutes
                .Where(route => solution.IsRouteSelected(route, variableFactory))
                .ToImmutableList();

            return new PassengerRelation(
                relation.OriginNode,
                relation.DestinationNode,
                selectedTravelRoutes,
                relation.TotalDemand);
        }

        private ITravelRouteCharacteristics ComputeTravelRouteCharacteristics(ISpotScenario scenario, IPassengerTravelRoute travelRoute, SingleObjectiveSolution solution, double numberOfPassengers) {
            var totalTravelTimeOfRouteParts = travelRoute
                .TravelRouteParts
                .Select(part => solution.GetValue(_variableFactory.CreateRoutePartDuration(part)))
                .Select(durationAsModelTime => scenario.TimeConverter.ToDuration(durationAsModelTime))
                .Sum();

            var totalTransferTimBetweenRouteParts = ExtractTransfersBetweenTravelRouteParts(travelRoute, scenario, solution);

            return new TravelRouteCharacteristics(travelRoute, numberOfPassengers, totalTransferTimBetweenRouteParts, totalTravelTimeOfRouteParts);
        }

        private Duration ExtractTransfersBetweenTravelRouteParts(IPassengerTravelRoute travelRoute, ISpotScenario scenario, ISolution solution) {
            var subsequentRoutePartsWithTransferBetween = travelRoute
                .TravelRouteParts
                .IterateInConsecutivePairs()
                .Select(pair => (arrivalLine: pair.Previous.SpotLineConstraint, arrivalNode: pair.Previous.EndLinePathNodeConstraint, departureLine: pair.Next.SpotLineConstraint, departureNode: pair.Next.StartLinePathNodeConstraint))
                .ToImmutableList();

            var transfersBetweenTravelRoutePartDurationValues = subsequentRoutePartsWithTransferBetween
                .Select(transfersBetweenTravelRoutePart => solution.GetValue(_variableFactory.CreateRoutePartTransferDuration(transfersBetweenTravelRoutePart.arrivalLine, transfersBetweenTravelRoutePart.arrivalNode, transfersBetweenTravelRoutePart.departureLine, transfersBetweenTravelRoutePart.departureNode)))
                .Select(transferTimeAsModelTime => scenario.TimeConverter.ToDuration(transferTimeAsModelTime))
                .Sum();

            return transfersBetweenTravelRoutePartDurationValues;
        }
    }
}