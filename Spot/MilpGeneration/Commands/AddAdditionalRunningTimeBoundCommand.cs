using System;
using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddAdditionalRunningTimeBoundCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var line in ctx.Scenario.Lines) {
                CreateForNodes(ctx, line);
            }
        }

        private static void CreateForNodes(SpotMilpGenerationContext ctx, ISpotLineConstraint lineConstraint) {
            var scenario = ctx.Scenario;

            foreach (var currentNode in lineConstraint.PathNodes.Skip(1)) {
                var minRunTimeAsModelTime = scenario.TimeConverter.ToModelTime(currentNode.MinRunTime.Value);
                var addRunTime = ctx.VariableFactory.CreateAdditionalRunningTime(lineConstraint, currentNode);
                var addRunTimeBound = Math.Floor(scenario.AdditionalRunTimeFactor * minRunTimeAsModelTime);
                var consName = SpotConstraintNameFactory.CreateAdditionalRunningTimeBoundEquation(ctx.GetCurrentConstraintIndex());

                var constraint = new Constraint(
                    consName,
                    new Term(new Monomial(addRunTime)),
                    ConstraintType.Leq,
                    new Term(new Monomial(addRunTimeBound)));
                ctx.Problem.Add(constraint);
            }
        }
    }
}