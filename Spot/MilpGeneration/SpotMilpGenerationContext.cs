using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Algorithms.Utils.LPCreation.ModelGeneration;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration {
    public class SpotMilpGenerationContext : MilpGenerationContext<SingleObjectiveProblem> {
        public SpotMilpGenerationContext(ISpotScenario scenario, SpotVariableFactory variableFactory) : base(new SingleObjectiveProblem()) {
            Scenario = scenario;
            VariableFactory = variableFactory;
        }
        public ISpotScenario Scenario { get; }

        public SpotVariableFactory VariableFactory { get; }
    }
}