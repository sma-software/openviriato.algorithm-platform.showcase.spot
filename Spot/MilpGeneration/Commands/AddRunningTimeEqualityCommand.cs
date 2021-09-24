using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddRunningTimeEqualityCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var line in ctx.Scenario.Lines) {
                CreateForNodes(ctx, line);
            }
        }

        private static void CreateForNodes(SpotMilpGenerationContext ctx, ISpotLineConstraint lineConstraint) {
            var scenario = ctx.Scenario;

            foreach (var currentNode in lineConstraint.PathNodes.Skip(1)) {
                var lastDep = ctx.VariableFactory.CreateDeparture(lineConstraint, currentNode.Predecessor);
                var currentArr = ctx.VariableFactory.CreateArrival(lineConstraint, currentNode);
                var cycleTimeShift = ctx.VariableFactory.CreateCycleTimeShift(currentNode.Predecessor, currentNode);
                var consName = SpotConstraintNameFactory.CreateRunningTimeEquation(ctx.GetCurrentConstraintIndex());
                var minRt = scenario.TimeConverter.ToModelTime(currentNode.MinRunTime.Value);
                var addRt = ctx.VariableFactory.CreateAdditionalRunningTime(lineConstraint, currentNode);

                var consRt = new Constraint(
                    consName,
                    new Term(new Monomial(currentArr)),
                    ConstraintType.Eq,
                    new Term(
                        new Monomial(lastDep),
                        new Monomial(minRt),
                        new Monomial(addRt),
                        new Monomial(-1 * scenario.TimeConverter.ToModelTime(scenario.CycleTime), cycleTimeShift)));
                ctx.Problem.Add(consRt);
            }
        }
    }
}