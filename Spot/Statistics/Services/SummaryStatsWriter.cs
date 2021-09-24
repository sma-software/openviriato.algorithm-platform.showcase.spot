using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Definitions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util;
using SMA.Algorithms.Utils;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Services {
    public class SummaryStatsWriter {
        private readonly IAlgorithmInterface _algorithmInterface;

        public SummaryStatsWriter(IAlgorithmInterface algorithmInterface) {
            _algorithmInterface = algorithmInterface;
        }

        public void WriteToViriato(ISummaryStats stats) {
            var tableID = _algorithmInterface.CreateTable(new SummaryDefinition().CreateSummaryTableDefinition());

            var tableRow = new TableRow(
                ImmutableListUtils.FromArray<ITableCell>(
                    new DurationCell(SummaryDefinition.TotalTravelTimeKey, stats.TotalTravelTime),
                    new IntegerCell(SummaryDefinition.PassengersWithoutARouteKey, stats.PassengersWithoutARoute)));
            _algorithmInterface.AddRowsToTable(tableID, ImmutableListUtils.FromArray(tableRow));
        }
    }
}