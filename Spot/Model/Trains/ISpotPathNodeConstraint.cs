using NodaTime;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public interface ISpotPathNodeConstraint {
        long ID { get; }
        long NodeID { get; }
        Duration MinStopTime { get; }
        Duration? MinRunTime { get; }
        int SequenceNumber { get; }
        ISpotPathNodeConstraint Predecessor { get; }
        ISpotPathNodeConstraint Successor { get; }
    }
}