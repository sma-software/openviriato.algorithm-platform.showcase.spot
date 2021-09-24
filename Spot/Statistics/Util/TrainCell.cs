using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    public class TrainCell : ITableAlgorithmTrainCell {
        public TrainCell(string columnKey, long? trainID) {
            ColumnKey = columnKey;
            TrainID = trainID;
        }

        public string ColumnKey { get; }
        public long? TrainID { get; }
    }
}