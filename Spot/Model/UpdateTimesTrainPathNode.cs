using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model {
    public class UpdateTimesTrainPathNode : IUpdateTimesTrainPathNode {
        public UpdateTimesTrainPathNode(long trainPathNodeId, LocalDateTime arrivalTime, LocalDateTime departureTime, Duration? minimumRunTime, Duration? minimumStopTime, StopStatus? stopStatus) {
            TrainPathNodeID = trainPathNodeId;
            ArrivalTime = arrivalTime;
            DepartureTime = departureTime;
            MinimumRunTime = minimumRunTime;
            MinimumStopTime = minimumStopTime;
            StopStatus = stopStatus;
        }

        public long TrainPathNodeID { get; }
        public LocalDateTime ArrivalTime { get; }
        public LocalDateTime DepartureTime { get; }
        public Duration? MinimumRunTime { get; }
        public Duration? MinimumStopTime { get; }
        public StopStatus? StopStatus { get; }
    }
}