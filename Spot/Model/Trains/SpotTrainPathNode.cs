using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public class SpotTrainPathNode : ISpotTrainPathNode {
        public SpotTrainPathNode(long trainPathNodeId, LocalDateTime arrivalTime, LocalDateTime departureTime, StopStatus? stopStatus, int sequenceNumber) {
            TrainPathNodeID = trainPathNodeId;
            ArrivalTime = arrivalTime;
            DepartureTime = departureTime;
            StopStatus = stopStatus;
            SequenceNumber = sequenceNumber;
        }

        public long TrainPathNodeID { get; }
        public LocalDateTime ArrivalTime { get; }
        public LocalDateTime DepartureTime { get; }
        public StopStatus? StopStatus { get; }
        public int SequenceNumber { get; }
    }
}