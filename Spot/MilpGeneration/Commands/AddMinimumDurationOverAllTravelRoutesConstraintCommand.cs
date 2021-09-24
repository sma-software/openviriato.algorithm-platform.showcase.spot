using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddMinimumDurationOverAllTravelRoutesConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _relation;

        public AddMinimumDurationOverAllTravelRoutesConstraintCommand(IPassengerRelation relation) {
            _relation = relation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var travelRoute in _relation.TravelRoutes) {
                new AddTravelRouteDurationUpperBoundForMinimumConstraintCommand(_relation, travelRoute).Execute(ctx);
                new AddTravelRouteDurationLowerBoundForMinimumConstraintCommand(_relation, travelRoute).Execute(ctx);
            }

            new AddTravelRouteDurationSelectAtLeastOneForMinimumConstraintCommand(_relation).Execute(ctx);
        }
    }
}