using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddTravelDurationsOnRoutePartsConstraintCommand : SpotConstraintCommandBase {
        private readonly IPassengerRelation _passengerRelation;

        public AddTravelDurationsOnRoutePartsConstraintCommand(IPassengerRelation passengerRelation) {
            _passengerRelation = passengerRelation;
        }

        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var travelRoute in _passengerRelation.TravelRoutes) {
                foreach (var travelRoutePart in travelRoute.TravelRouteParts) {
                    AddConstraintsForTravelPart(ctx, travelRoutePart);
                }
            }
        }

        private static void AddConstraintsForTravelPart(SpotMilpGenerationContext ctx, IPassengerTravelRoutePart travelRoutePart) {
            var partStart = ctx.VariableFactory.CreateDeparture(travelRoutePart.SpotLineConstraint, travelRoutePart.StartLinePathNodeConstraint);
            var partEnd = ctx.VariableFactory.CreateArrival(travelRoutePart.SpotLineConstraint, travelRoutePart.EndLinePathNodeConstraint);
            var routePartDuration = ctx.VariableFactory.CreateRoutePartDuration(travelRoutePart);

            var cycleTimeShiftsForStopTimes = travelRoutePart
                .VisitedNodes
                .SkipLast(1)
                .Skip(1)
                .Select(node => ctx.VariableFactory.CreateCycleTimeShift(node, node))
                .ToImmutableList();
            var cycleTimeShiftsForRunTimes = travelRoutePart
                .VisitedNodes
                .Skip(1)
                .Select(node => ctx.VariableFactory.CreateCycleTimeShift(node.Predecessor, node))
                .ToImmutableList();

            var scenario = ctx.Scenario;
            var consName = SpotConstraintNameFactory.CreateTravelRoutePartDuration(ctx.GetCurrentConstraintIndex());
            var routePartDurationWithoutCycleTimeShifts = ImmutableListUtils.FromArray(
                new Monomial(partEnd),
                new Monomial(-1, partStart));
            var cycleTimeShifts = cycleTimeShiftsForStopTimes
                .Concat(cycleTimeShiftsForRunTimes)
                .Select(cycleTimeShift => new Monomial(scenario.TimeConverter.ToModelTime(scenario.CycleTime), cycleTimeShift))
                .ToImmutableList();

            var travelRoutePartsDurationConstraint = new Constraint(
                consName,
                new Term(new Monomial(routePartDuration)),
                ConstraintType.Eq,
                new Term(routePartDurationWithoutCycleTimeShifts.Concat(cycleTimeShifts)));

            ctx.Problem.Add(travelRoutePartsDurationConstraint);
        }
    }
}