using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.MilpGeneration;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Algorithms.Utils.LPCreation.Solving;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions {
    public static class SolutionExtensions {
        public static bool IsRouteSelected(this ISolution solution, IPassengerTravelRoute route, SpotVariableFactory variableFactory) {
            return solution.GetTruthValue(variableFactory.CreateRouteSelectionVariable(route));
        }
    }
}