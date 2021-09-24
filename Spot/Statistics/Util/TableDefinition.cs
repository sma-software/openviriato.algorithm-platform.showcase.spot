using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    internal class TableDefinition : ITableDefinition {
        public TableDefinition(string name, IImmutableList<ITableColumnDefinition> columns) {
            Name = name;
            Columns = columns;
        }

        public string Name { get; }
        public IImmutableList<ITableColumnDefinition> Columns { get; }
    }
}