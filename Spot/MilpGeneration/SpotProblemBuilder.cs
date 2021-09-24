using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration {
    public class SpotProblemBuilder {
        private readonly SpotMilpGenerationContext _context;
        public SpotProblemBuilder(ISpotScenario scenario, SpotVariableFactory variableFactory) {
            _context = new SpotMilpGenerationContext(scenario, variableFactory);
        }

        public SingleObjectiveProblem Build() {
            new SpotProblemGenerationCommand().Execute(_context);
            return _context.Problem;
        }
    }
}