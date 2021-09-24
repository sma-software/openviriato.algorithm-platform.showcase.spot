using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.LPCreation.Model;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration.Commands {
    public class AddStopTimeConstraintCommand : SpotConstraintCommandBase {
        public override void Execute(SpotMilpGenerationContext ctx) {
            foreach (var line in ctx.Scenario.Lines) {
                CreateForNodes(ctx, line);
            }
        }

        private static void CreateForNodes(SpotMilpGenerationContext ctx, ISpotLineConstraint lineConstraint) {
            var scenario = ctx.Scenario;
            foreach (var nodeConstraint in lineConstraint.PathNodes) {
                if (nodeConstraint.MinStopTime > Duration.Zero) {
                    AddConstrainForPathNodesWithStop(ctx, lineConstraint, nodeConstraint, scenario);
                } else {
                    AddConstrainForPathNodesWithoutStop(ctx, lineConstraint, nodeConstraint);
                }
            }
        }

        private static void AddConstrainForPathNodesWithoutStop(SpotMilpGenerationContext ctx, ISpotLineConstraint lineConstraint, ISpotPathNodeConstraint nodeConstraint) {
            var arrivalVar = ctx.VariableFactory.CreateArrival(lineConstraint, nodeConstraint);
            var departureVar = ctx.VariableFactory.CreateDeparture(lineConstraint, nodeConstraint);
            var consName = SpotConstraintNameFactory.CreateStoppingTimeEquationWithoutStop(ctx.GetCurrentConstraintIndex());

            var stopTimeConstraint = new Constraint(
                consName,
                new Term(new Monomial(departureVar)),
                ConstraintType.Eq,
                new Term(new Monomial(arrivalVar)));

            ctx.Problem.Add(stopTimeConstraint);
        }

        private static void AddConstrainForPathNodesWithStop(SpotMilpGenerationContext ctx, ISpotLineConstraint lineConstraint, ISpotPathNodeConstraint nodeConstraint, ISpotScenario scenario) {
            var arrivalVar = ctx.VariableFactory.CreateArrival(lineConstraint, nodeConstraint);
            var departureVar = ctx.VariableFactory.CreateDeparture(lineConstraint, nodeConstraint);
            var cycleTimeShift = ctx.VariableFactory.CreateCycleTimeShift(nodeConstraint, nodeConstraint);
            var consName = SpotConstraintNameFactory.CreateStoppingTimeEquationWithStop(ctx.GetCurrentConstraintIndex());
            var minStopTime = scenario.TimeConverter.ToModelTime(nodeConstraint.MinStopTime);

            var stopTimeConstraint = new Constraint(
                consName,
                new Term(new Monomial(departureVar)),
                ConstraintType.Geq,
                new Term(
                    new Monomial(arrivalVar),
                    new Monomial(minStopTime),
                    new Monomial(-1 * scenario.TimeConverter.ToModelTime(scenario.CycleTime), cycleTimeShift)));
            ctx.Problem.Add(stopTimeConstraint);
        }
    }
}