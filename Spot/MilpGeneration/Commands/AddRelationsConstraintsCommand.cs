namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddRelationsConstraintsCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var relation in ctx.Scenario.PassengerRelationsWithRoutes) {
                new AddTravelDurationsOnRoutePartsConstraintCommand(relation).Execute(ctx);
                new AddTransferDurationsBetweenRoutePartsConstraintCommand(relation).Execute(ctx);
                new AddTotalDurationTravelRouteConstraintCommand(relation).Execute(ctx);
                new AddMinimumDurationOverAllTravelRoutesConstraintCommand(relation).Execute(ctx);
                new AddRequiredMinimumTransferDurationBetweenTravelRoutePartsConstraintCommand(relation).Execute(ctx);
            }
        }
    }
}