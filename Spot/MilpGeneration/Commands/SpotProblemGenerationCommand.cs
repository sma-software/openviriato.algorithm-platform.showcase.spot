namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class SpotProblemGenerationCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            new AddObjectiveCommand().Execute(ctx);
            new AddTrainRunConstraintsCommand().Execute(ctx);
            new AddRelationsConstraintsCommand().Execute(ctx);
        }
    }
}