using NodaTime;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats {
    public interface IRelationsStats {
        long StartNodeID { get; }
        long EndNodeID { get; }
        Duration? TotalTravelTime { get; }
        Duration? TransferTime { get; }
        Duration? TotalTime { get; }
        IImmutableList<long> TransferStations { get; }
        IImmutableList<long> UsedTrainIDs { get; }
    }

    public class RelationStats : IRelationsStats {
        public RelationStats(long startNodeId, long endNodeId, Duration? totalTravelTime, Duration? transferTime, Duration? totalTime, IImmutableList<long> usedTrainIDs, IImmutableList<long> transferStations) {
            StartNodeID = startNodeId;
            EndNodeID = endNodeId;
            TotalTravelTime = totalTravelTime;
            TransferTime = transferTime;
            TotalTime = totalTime;
            UsedTrainIDs = usedTrainIDs;
            TransferStations = transferStations;
        }

        public long StartNodeID { get; }
        public long EndNodeID { get; }
        public Duration? TotalTravelTime { get; }
        public Duration? TransferTime { get; }
        public Duration? TotalTime { get; }
        public IImmutableList<long> UsedTrainIDs { get; }
        public IImmutableList<long> TransferStations { get; }
    }
}