using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Timetable.Factories;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions {
    public static class SpotLineAlgorithmExtensions {
        public static ISpotLineConstraint ToSpotLine(this IAlgorithmTrain train) {
            var navigableTrain = NavigableTrainFactory.CreateFromAlgorithmTrain(train);
            return new SpotLineConstraintFactory().CreateFrom(navigableTrain);
        }
    }
}