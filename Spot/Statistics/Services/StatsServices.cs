using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Calculators;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Services {
    public static class StatsServices {
        public static void CalculateAndWriteStats(IAlgorithmInterface algorithmInterface, ISpotScenario scenario, SpotSolution solution, ISpotUserParameters parameters) {
            var summaryStats = new SummaryStatsCalculator(scenario).Calculate(solution);
            var relationsStats = new RelationsStatsCalculator().Calculate(scenario, solution);

            new SummaryStatsWriter(algorithmInterface).WriteToViriato(summaryStats);
            new RelationsStatsWriter(algorithmInterface).WriteToViriato(relationsStats, parameters.MaximalNumberOfTransfers);
        }
    }
}