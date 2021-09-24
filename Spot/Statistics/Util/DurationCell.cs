using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    public class DurationCell : ITableDurationCell {
        public DurationCell(string columnKey, Duration? value) {
            Value = value;
            ColumnKey = columnKey;
        }

        public string ColumnKey { get; }
        public Duration? Value { get; }
    }
}