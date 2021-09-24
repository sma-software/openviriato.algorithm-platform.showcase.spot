using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    internal class IntegerCell : ITableIntegerCell {
        public IntegerCell(string columnKey, int? value) {
            ColumnKey = columnKey;
            Value = value;
        }

        public string ColumnKey { get; }
        public int? Value { get; }
    }
}