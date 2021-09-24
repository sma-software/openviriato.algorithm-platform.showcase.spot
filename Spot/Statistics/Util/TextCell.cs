using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    internal class TextCell : ITableTextCell {
        public TextCell(string columnKey, string value) {
            ColumnKey = columnKey;
            Value = value;
        }

        public string ColumnKey { get; }
        public string Value { get; }
    }
}