using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public interface ISpotTrainPathNode {
        long TrainPathNodeID { get; }
        LocalDateTime ArrivalTime { get; }
        LocalDateTime DepartureTime { get; }
        StopStatus? StopStatus { get; }
        int SequenceNumber { get; }
    }
}