using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    public class NodeCell : ITableAlgorithmNodeCell {
        public NodeCell(string columnKey, long nodeId) {
            ColumnKey = columnKey;
            NodeID = nodeId;
        }
        public string ColumnKey { get; }
        public long? NodeID { get; }
    }
}