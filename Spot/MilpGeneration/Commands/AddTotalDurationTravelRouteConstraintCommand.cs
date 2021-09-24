using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTotalDurationTravelRouteConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _travelRelation;

        public AddTotalDurationTravelRouteConstraintCommand(IPassengerRelation travelRelation) {
            _travelRelation = travelRelation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var travelRoute in _travelRelation.TravelRoutes) {
                CreateForOneTravelRoute(ctx, travelRoute);
            }
        }

        private static void CreateForOneTravelRoute(SpotMilpGenerationContext ctx, IPassengerTravelRoute travelRoute) {
            var transfersBetweenTravelRouteParts = travelRoute
                .TravelRouteParts
                .IterateInConsecutivePairs()
                .Select(pair => (arrivalLine: pair.Previous.SpotLineConstraint, arrivalNode: pair.Previous.EndLinePathNodeConstraint, departureLine: pair.Next.SpotLineConstraint, departureNode: pair.Next.StartLinePathNodeConstraint))
                .ToImmutableList();

            var travelDurations = travelRoute.TravelRouteParts.Select(part => ctx.VariableFactory.CreateRoutePartDuration(part)).ToImmutableList();
            var transferDurations = transfersBetweenTravelRouteParts.Select(transfer => ctx.VariableFactory.CreateRoutePartTransferDuration(transfer.arrivalLine, transfer.arrivalNode, transfer.departureLine, transfer.departureNode)).ToImmutableList();

            var consName = SpotConstraintNameFactory.CreateTravelRouteTotalDurationEquation(ctx.GetCurrentConstraintIndex());

            var lhs = new Monomial(ctx.VariableFactory.CreateRouteTotalDuration(travelRoute));
            var rhs = travelDurations
                .Concat(transferDurations)
                .Select(v => new Monomial(v))
                .ToImmutableList();

            var travelRouteDurationConstraint = new Constraint(consName, new Term(lhs), ConstraintType.Eq, new Term(rhs));
            ctx.Problem.Add(travelRouteDurationConstraint);
        }
    }
}