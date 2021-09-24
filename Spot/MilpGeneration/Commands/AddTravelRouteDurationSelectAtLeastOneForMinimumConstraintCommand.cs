using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTravelRouteDurationSelectAtLeastOneForMinimumConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _relation;

        public AddTravelRouteDurationSelectAtLeastOneForMinimumConstraintCommand(IPassengerRelation relation) {
            _relation = relation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            var conName = SpotConstraintNameFactory.CreateTravelRouteDurationSelectionEquation(ctx.GetCurrentConstraintIndex());
            var atLeastOneRouteConstraint = new Constraint(
                conName,
                new Term(new Monomial(1)),
                ConstraintType.Eq,
                new Term(_relation.TravelRoutes.Select(route => new Monomial(ctx.VariableFactory.CreateRouteSelectionVariable(route)))));
            ctx.Problem.Add(atLeastOneRouteConstraint);
        }
    }
}