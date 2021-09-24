using System.Globalization;
using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Infrastructure.Caches;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Services;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Services;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters;
using SMA.Algorithms.Utils.LPCreation.Model;
using SMA.Algorithms.Utils.LPCreation.Solving;
using SMA.Algorithms.Utils.LPCreation.Solving.Gurobi;
using SMA.Algorithms.Utils.LPCreation.Solving.Parameters;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot {
    public class SpotAlgorithm {
        private readonly IAlgorithmInterface _algorithmInterface;

        public SpotAlgorithm(IAlgorithmInterface algorithmInterface) {
            _algorithmInterface = algorithmInterface;
        }

        public void Run() {
            _algorithmInterface.NotifyUser("Spot", "Running Spot");

            var spotAlgorithmParameters = new SpotUserParametersFactory(_algorithmInterface).CreateParameters();
            if (spotAlgorithmParameters.NumberOfCycles <= 0) {
                _algorithmInterface.NotifyUser("Spot", "Number of Cycles must be at least one");
                return;
            }

            _algorithmInterface.ShowStatusMessage("Reading nodes from Viriato.");
            var nodeLookup = new NodeLookupFactory().CreateNodeLookup(_algorithmInterface.GetAllNodes());

            var transferTimesLookup = new TransferTimesLookup(spotAlgorithmParameters.DefaultMinimumTransferTime);

            var scenarioResult = new SpotScenarioFactory(_algorithmInterface, nodeLookup).Create(spotAlgorithmParameters, transferTimesLookup);
            if (!scenarioResult.Success) {
                _algorithmInterface.NotifyUser("Error occurred", scenarioResult.Answers.ToPrettyString());
                return;
            }

            var scenario = scenarioResult.Value;

            var builderFactory = new ProblemAndSolutionBuilderFactory();
            var (problemBuilder, solutionBuilder) = builderFactory.Create(scenario);
            var solverFactory = SolverFactory.CreateInstance();
            var solverResult = solverFactory.CreateSolver();
            if (!solverResult.Success) {
                _algorithmInterface.NotifyUser("Spot", "Could not create solver. Aborted.");
                return;
            }

            using (var solver = solverResult.Value) {
                _algorithmInterface.ShowStatusMessage("Preparing SPOT Optimization Model");
                var problem = problemBuilder.Build();
                _algorithmInterface.ShowStatusMessage("Solving SPOT Optimization Model");

                var solution = solver.Solve(problem, CreateParametersLookup(problem, spotAlgorithmParameters));

                _algorithmInterface.NotifyUser("Spot", string.Format(CultureInfo.InvariantCulture, "Instance solved. State is: {0}", solution.State));

                var spotSolution = solutionBuilder.Build(scenario, solution);
                _algorithmInterface.NotifyUser("Spot Solution", string.Format(CultureInfo.InvariantCulture, "The Spot solution contains {0} trains", spotSolution.ScheduledTrains.Count));
                var trainPersistenceService = new TrainPersistenceService(_algorithmInterface);
                trainPersistenceService.WriteToViriato(spotSolution, scenario.TimeConverter.TimeWindowFirstCycleTime, scenario.NumberOfPeriods);
                StatsServices.CalculateAndWriteStats(_algorithmInterface, scenario, spotSolution, spotAlgorithmParameters);
            }
        }

        private SingleObjectiveParametersLookup CreateParametersLookup(SingleObjectiveProblem problem, ISpotUserParameters spotAlgorithmAlgorithmParameters) {
            if (spotAlgorithmAlgorithmParameters.SolverTimeout.HasValue) {
                var objectiveSpecificParameters = new ObjectiveSpecificParametersBuilder().SetTimeLimit(Duration.FromSeconds(spotAlgorithmAlgorithmParameters.SolverTimeout.Value)).Build();
                var objectiveParameterLookup = new SingleObjectiveParametersLookup();
                objectiveParameterLookup.SetObjectiveSpecificParameters(objectiveSpecificParameters, problem.Objective);
                return objectiveParameterLookup;
            } else {
                return null;
            }
        }
    }
}