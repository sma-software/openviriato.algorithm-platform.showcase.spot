using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Algorithms.Utils.LPCreation.ModelGeneration;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public abstract class SpotConstraintCommandBase : IModelGenerationCommand<SingleObjectiveProblem, SpotMilpGenerationContext> {
        public abstract void Execute(SpotMilpGenerationContext ctx);
    }
}