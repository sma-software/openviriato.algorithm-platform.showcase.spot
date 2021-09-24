using NodaTime;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public class SpotPathNodeConstraint : ISpotPathNodeConstraint {
        public SpotPathNodeConstraint(long id, long nodeID, Duration? minRunTime, Duration minStopTime, int sequenceNumber) {
            ID = id;
            MinRunTime = minRunTime;
            MinStopTime = minStopTime;
            NodeID = nodeID;
            SequenceNumber = sequenceNumber;
        }

        public long ID { get; }
        public long NodeID { get; }
        public Duration MinStopTime { get; }
        public Duration? MinRunTime { get; }
        public int SequenceNumber { get; }
        public ISpotPathNodeConstraint Predecessor { get; set; }
        public ISpotPathNodeConstraint Successor { get; set; }
    }
}