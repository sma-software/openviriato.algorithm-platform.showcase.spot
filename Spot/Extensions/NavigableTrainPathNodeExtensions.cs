using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Timetable.DataStructures;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions {
    public static class NavigableTrainPathNodeExtensions {
        public static SpotPathNodeConstraint ToSpotLinePathNode(this INavigableTrainPathNode node) {
            return new SpotPathNodeConstraint(
                (int)node.ID,
                (int)node.NodeID,
                node.MinimumRunTime,
                node.MinimumStopTime,
                node.SequenceNumber);
        }
    }
}