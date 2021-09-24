using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration {
    public class ProblemAndSolutionBuilderFactory {
        public (SpotProblemBuilder, SpotSolutionBuilder) Create(ISpotScenario scenario) {
            var variableFactory = new SpotVariableFactory(scenario.TimeConverter, scenario.CycleTime, scenario.MaximumTransferTime);
            return (new SpotProblemBuilder(scenario, variableFactory), new SpotSolutionBuilder(variableFactory));
        }
    }
}