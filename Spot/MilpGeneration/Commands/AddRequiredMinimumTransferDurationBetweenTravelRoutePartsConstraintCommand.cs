using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddRequiredMinimumTransferDurationBetweenTravelRoutePartsConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _passengerRelation;

        public AddRequiredMinimumTransferDurationBetweenTravelRoutePartsConstraintCommand(IPassengerRelation passengerRelation) {
            _passengerRelation = passengerRelation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var travelRoute in _passengerRelation.TravelRoutes) {
                CreateForOneTravelRoute(ctx, travelRoute);
            }
        }

        private static void CreateForOneTravelRoute(SpotMilpGenerationContext ctx, IPassengerTravelRoute travelRoute) {
            var transfersBetweenTravelRouteParts = travelRoute
                .TravelRouteParts
                .IterateInConsecutivePairs()
                .Select(pair => (arrivalLine: pair.Previous.SpotLineConstraint, arrivalNode: pair.Previous.EndLinePathNodeConstraint, departureLine: pair.Next.SpotLineConstraint, departureNode: pair.Next.StartLinePathNodeConstraint))
                .ToImmutableList();
            foreach (var transfersBetweenTravelRoutePart in transfersBetweenTravelRouteParts) {
                CreateForSucceedingRouteParts(
                    ctx,
                    transfersBetweenTravelRoutePart.arrivalLine,
                    transfersBetweenTravelRoutePart.arrivalNode,
                    transfersBetweenTravelRoutePart.departureLine,
                    transfersBetweenTravelRoutePart.departureNode);
            }
        }

        private static void CreateForSucceedingRouteParts(SpotMilpGenerationContext ctx, ISpotLineConstraint arrivalLine, ISpotPathNodeConstraint arrivalNode, ISpotLineConstraint departureLine, ISpotPathNodeConstraint departureNode) {
            var routePartTransferDuration = ctx.VariableFactory.CreateRoutePartTransferDuration(arrivalLine, arrivalNode, departureLine, departureNode);
            var requiredMinimumTransferDuration = ctx.Scenario.TransferTimeLookup.GetRequiredMinimumTransferDuration(arrivalLine, arrivalNode, departureLine, departureNode);

            var scenario = ctx.Scenario;
            var consName = SpotConstraintNameFactory.CreateRequiredTransferDurationBetweenSucceedingRouteParts(ctx.GetCurrentConstraintIndex());
            var travelRoutePartTransferMinimumTransferDurationConstraint = new Constraint(
                consName,
                new Term(new Monomial(routePartTransferDuration)),
                ConstraintType.Geq,
                new Term(new Monomial(scenario.TimeConverter.ToModelTime(requiredMinimumTransferDuration))));

            ctx.Problem.Add(travelRoutePartTransferMinimumTransferDurationConstraint);
        }
    }
}