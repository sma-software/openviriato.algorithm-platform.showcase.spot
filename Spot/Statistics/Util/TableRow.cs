using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    internal class TableRow : ITableRow {
        public TableRow(IImmutableList<ITableCell> cells) {
            Cells = cells;
        }

        public IImmutableList<ITableCell> Cells { get; }
    }
}