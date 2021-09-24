using System.Linq;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddObjectiveCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            var routedPassengerTravelTime = ctx.Scenario.PassengerRelationsWithRoutes.Select(relation => new Monomial(relation.TotalDemand, ctx.VariableFactory.CreateSelectedRouteTotalTravelDuration(relation))).ToImmutableList();
            ctx.Problem.SetObjective(new Objective(ObjectiveType.Min, new Term(routedPassengerTravelTime)));
        }
    }
}