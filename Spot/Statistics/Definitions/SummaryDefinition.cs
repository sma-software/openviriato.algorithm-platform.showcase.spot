using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util;
using SMA.Algorithms.Utils;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Definitions {
    public class SummaryDefinition {
        public const string TotalTravelTimeKey = "totalTravelTime";
        public const string PassengersWithoutARouteKey = "PassengersWithoutARoute";

        private const string SummaryTableLabel = "Summary";
        private const string TotalTravelTimeLabel = "Total Travel Time";
        private const string PassengersWithoutARouteLabel = "Passengers without a Route";

        public ITableDefinition CreateSummaryTableDefinition() {
            var columnDefinitions = ImmutableListUtils.FromArray(
                new TableColumnDefinition(
                    TotalTravelTimeKey,
                    new TextCell(TotalTravelTimeKey, TotalTravelTimeLabel),
                    TableCellDataType.String,
                    TableCellDataType.Duration),
                new TableColumnDefinition(
                    PassengersWithoutARouteKey,
                    new TextCell(PassengersWithoutARouteKey, PassengersWithoutARouteLabel),
                    TableCellDataType.String,
                    TableCellDataType.Integer));
            return new TableDefinition(SummaryTableLabel, columnDefinitions);
        }
    }
}