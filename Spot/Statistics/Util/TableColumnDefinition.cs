using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    internal class TableColumnDefinition : ITableColumnDefinition {
        public TableColumnDefinition(string key, ITableCell header, TableCellDataType headerDataType, TableCellDataType columnDataType) {
            Key = key;
            Header = header;
            HeaderDataType = headerDataType;
            ColumnDataType = columnDataType;
        }

        public string Key { get; }
        public ITableCell Header { get; }
        public TableCellDataType HeaderDataType { get; }
        public TableCellDataType ColumnDataType { get; }
    }
}