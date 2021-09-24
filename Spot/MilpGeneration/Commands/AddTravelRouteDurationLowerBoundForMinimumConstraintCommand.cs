using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Services;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTravelRouteDurationLowerBoundForMinimumConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerTravelRoute _travelRoute;
        private readonly IPassengerRelation _relation;

        public AddTravelRouteDurationLowerBoundForMinimumConstraintCommand(IPassengerRelation relation, IPassengerTravelRoute travelRoute) {
            _travelRoute = travelRoute;
            _relation = relation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            var consName = SpotConstraintNameFactory.CreateMaximumTravelRouteDuration(ctx.GetCurrentConstraintIndex());
            var lhs = new Term(new Monomial(ctx.VariableFactory.CreateSelectedRouteTotalTravelDuration(_relation)));
            var bigM = ctx.Scenario.TimeConverter.ToModelTime(TravelTimesCalculationServices.CalculateMaximumTravelTimeOnTravelRoute(_travelRoute, ctx.Scenario.MaximumTransferTime, ctx.Scenario.CycleTime));

            var rhs = new Term(new Monomial(bigM, ctx.VariableFactory.CreateRouteSelectionVariable(_travelRoute)), new Monomial(-bigM), new Monomial(ctx.VariableFactory.CreateRouteTotalDuration(_travelRoute)));

            var asLongAsChosenDurationConstraint = new Constraint(consName, lhs, ConstraintType.Geq, rhs);
            ctx.Problem.Add(asLongAsChosenDurationConstraint);
        }
    }
}