using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTransferDurationsBetweenRoutePartsConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _passengerRelation;

        public AddTransferDurationsBetweenRoutePartsConstraintCommand(IPassengerRelation passengerRelation) {
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
                CreateForSucceedingTravelRouteParts(
                    ctx,
                    transfersBetweenTravelRoutePart.arrivalLine,
                    transfersBetweenTravelRoutePart.arrivalNode,
                    transfersBetweenTravelRoutePart.departureLine,
                    transfersBetweenTravelRoutePart.departureNode);
            }
        }

        private static void CreateForSucceedingTravelRouteParts(SpotMilpGenerationContext ctx, ISpotLineConstraint arrivingLineConstraint, ISpotPathNodeConstraint arrivingNodeConstraint, ISpotLineConstraint departureLineConstraint, ISpotPathNodeConstraint departureNodeConstraint) {
            var arrivingPart = ctx.VariableFactory.CreateArrival(arrivingLineConstraint, arrivingNodeConstraint);
            var departingPart = ctx.VariableFactory.CreateDeparture(departureLineConstraint, departureNodeConstraint);
            var cycleTimeShift = ctx.VariableFactory.CreateCycleTimeShift(arrivingNodeConstraint, departureNodeConstraint);
            var routePartTransferDuration = ctx.VariableFactory.CreateRoutePartTransferDuration(arrivingLineConstraint, arrivingNodeConstraint, departureLineConstraint, departureNodeConstraint);

            var scenario = ctx.Scenario;
            var consName = SpotConstraintNameFactory.CreateTransferDurationBetweenSucceedingTravelRouteParts(ctx.GetCurrentConstraintIndex());
            var rhs = ImmutableListUtils.FromArray(
                new Monomial(departingPart),
                new Monomial(-1, arrivingPart),
                new Monomial(scenario.TimeConverter.ToModelTime(scenario.CycleTime), cycleTimeShift));

            var travelRoutePartTransferDurationConstraint = new Constraint(
                consName,
                new Term(new Monomial(routePartTransferDuration)),
                ConstraintType.Eq,
                new Term(rhs));

            ctx.Problem.Add(travelRoutePartTransferDurationConstraint);
        }
    }
}