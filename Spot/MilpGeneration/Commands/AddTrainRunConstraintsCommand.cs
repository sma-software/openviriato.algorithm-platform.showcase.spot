namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTrainRunConstraintsCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            new AddStopTimeConstraintCommand().Execute(ctx);
            new AddRunningTimeConstraintsCommand().Execute(ctx);
        }
    }
}