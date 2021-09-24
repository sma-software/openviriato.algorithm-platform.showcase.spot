using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario;
using SMA.Algorithms.Utils.LPCreation.Solving;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution {
    public class SpotSolutionBuilder {
        private readonly SpotVariableFactory _variableFactory;

        public SpotSolutionBuilder(SpotVariableFactory variableFactory) {
            _variableFactory = variableFactory;
        }

        public SpotSolution Build(ISpotScenario scenario, SingleObjectiveSolution solution) {
            if (!solution.FeasibleSolutionFound()) {
                return SpotSolution.CreateEmpty();
            }

            var spotTrains = new SpotTrainFactory(_variableFactory).CreateSpotTrainsFromSolution(scenario, solution);
            var passengerRelationCharacteristics = new PassengerRelationCharacteristicsFactory(_variableFactory)
                .CreatePassengerRelationCharacteristicsFromSolution(scenario, solution);

            return new SpotSolution(spotTrains, passengerRelationCharacteristics);
        }
    }
}