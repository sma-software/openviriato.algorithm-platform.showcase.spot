using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats;
using SMA.Algorithms.Utils.TimeWindow;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Calculators {
    public class SummaryStatsCalculator {
        private readonly ISpotScenario _scenario;

        public SummaryStatsCalculator(ISpotScenario scenario) {
            _scenario = scenario;
        }

        public ISummaryStats Calculate(SpotSolution solution) {
            var totalTravelTime = CalculateTotalTravelTime(solution);
            var totalNumberOfPassengersWithoutARoute = _scenario.PassengerRelations.Except(_scenario.PassengerRelationsWithRoutes).Select(relation => relation.TotalDemand).Sum();

            return new SummaryStats(totalTravelTime, totalNumberOfPassengersWithoutARoute);
        }

        private static Duration CalculateTotalTravelTime(SpotSolution solution) {
            var weightedTravelTimePerRelation = solution.PassengerRelationCharacteristics
                .SelectMany(r => r.TravelRouteCharacteristics.Select(CalculateWeightedRouteTravelTime))
                .Sum();

            return weightedTravelTimePerRelation;
        }

        private static Duration CalculateWeightedRouteTravelTime(ITravelRouteCharacteristics routeCharacteristics) {
            return (routeCharacteristics.TotalTrainTravelTime + routeCharacteristics.TotalTransferTime).Times(routeCharacteristics.NumberOfPassengers);
        }
    }
}