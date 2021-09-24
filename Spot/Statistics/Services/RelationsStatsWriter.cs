using System.Linq;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Definitions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Stats;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util;
using SMA.Algorithms.Utils;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Services {
    public class RelationsStatsWriter {
        private readonly IAlgorithmInterface _algorithmInterface;

        public RelationsStatsWriter(IAlgorithmInterface algorithmInterface) {
            _algorithmInterface = algorithmInterface;
        }

        public void WriteToViriato(IImmutableList<IRelationsStats> stats, int numberOfTransfers) {
            var tableID = _algorithmInterface.CreateTable(new RelationsDefinition().RelationsTableDefinition(numberOfTransfers));

            var tableRows = stats
                .Select(CreateCellsForOneRow)
                .Select(e => new TableRow(e))
                .ToImmutableList();

            _algorithmInterface.AddRowsToTable(tableID, tableRows);
        }

        private IImmutableList<ITableCell> CreateCellsForOneRow(IRelationsStats statsPerRelation) {
            var fixedCells = ImmutableListUtils.FromArray<ITableCell>(
                    new NodeCell(RelationsDefinition.RelationOriginNodeKey, statsPerRelation.StartNodeID),
                    new NodeCell(RelationsDefinition.RelationDestinationNodeKey, statsPerRelation.EndNodeID),
                    new DurationCell(RelationsDefinition.TravelTimeKey, statsPerRelation.TotalTravelTime),
                    new DurationCell(RelationsDefinition.TransferTimeKey, statsPerRelation.TransferTime),
                    new DurationCell(RelationsDefinition.TotalTimeKey, statsPerRelation.TotalTime));

            var trainCells = statsPerRelation
                .UsedTrainIDs
                .Select((usedTrainID, i) => new TrainCell(RelationsDefinition.GetUsedTrainKey(i), usedTrainID))
                .ToImmutableList();
            var nodeCells = statsPerRelation
                .TransferStations
                .Select((transferNodeID, i) => new NodeCell(RelationsDefinition.GetTransferNodeKey(i), transferNodeID))
                .ToImmutableList();
            return fixedCells.Concat(trainCells).Concat(nodeCells).ToImmutableList();
        }
    }
}