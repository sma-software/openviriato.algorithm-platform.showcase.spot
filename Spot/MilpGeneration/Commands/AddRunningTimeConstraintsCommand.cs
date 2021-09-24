namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddRunningTimeConstraintsCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            new AddRunningTimeEqualityCommand().Execute(ctx);
            new AddAdditionalRunningTimeBoundCommand().Execute(ctx);
        }
    }
}