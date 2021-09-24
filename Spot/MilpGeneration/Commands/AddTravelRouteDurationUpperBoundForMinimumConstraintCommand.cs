using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTravelRouteDurationUpperBoundForMinimumConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _relation;
        private readonly IPassengerTravelRoute _travelRoute;

        public AddTravelRouteDurationUpperBoundForMinimumConstraintCommand(IPassengerRelation relation, IPassengerTravelRoute travelRoute) {
            _relation = relation;
            _travelRoute = travelRoute;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            var minConsName = SpotConstraintNameFactory.CreateMinimumTravelRouteDuration(ctx.GetCurrentConstraintIndex());

            var minimalDurationConstraint = new Constraint(
                minConsName,
                new Term(new Monomial(ctx.VariableFactory.CreateSelectedRouteTotalTravelDuration(_relation))),
                ConstraintType.Leq,
                new Term(new Monomial(ctx.VariableFactory.CreateRouteTotalDuration(_travelRoute))));
            ctx.Problem.Add(minimalDurationConstraint);
        }
    }
}