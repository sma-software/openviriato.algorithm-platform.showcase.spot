using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Calculators {
    public class RelationsStatsCalculator {
        public IImmutableList<IRelationsStats> Calculate(ISpotScenario scenario, SpotSolution solution) {
            var relationsWithoutRoutes = scenario.PassengerRelations.Except(scenario.PassengerRelationsWithRoutes).ToImmutableList();

            return solution
                .PassengerRelationCharacteristics
                .SelectMany(c => c.TravelRouteCharacteristics.Select(ch => CreateRelationStats(c, ch)))
                .Concat(relationsWithoutRoutes.Select(r => new RelationStats(r.OriginNode.ID, r.DestinationNode.ID, null, null, null, ImmutableList<long>.Empty, ImmutableList<long>.Empty)))
                .OrderBy(r => r.StartNodeID)
                .ThenBy(r => r.EndNodeID)
                .ToImmutableList();
        }

        private static RelationStats CreateRelationStats(IPassengerRelationCharacteristics c, ITravelRouteCharacteristics ch) {
            return new RelationStats(
                c.PassengerRelation.OriginNode.ID,
                c.PassengerRelation.DestinationNode.ID,
                ch.TotalTrainTravelTime,
                ch.TotalTransferTime,
                ch.TotalTrainTravelTime + ch.TotalTransferTime,
                ch.TravelRoute.TravelRouteParts.Select(p => p.SpotLineConstraint.ID).ToImmutableList(),
                ch.TravelRoute.TravelRouteParts.Skip(1).Select(p => p.StartLinePathNodeConstraint.NodeID).ToImmutableList());
        }
    }
}