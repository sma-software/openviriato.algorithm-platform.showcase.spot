using System;
using System.Collections.Generic;
using System.Linq;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util;
using SMA.Algorithms.Utils;
using SMA.Apps.Utils.Collections.Generic.Extensions;
using SMA.Apps.Utils.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Definitions {
    public class RelationsDefinition {
        public const string TravelTimeKey = "TravelTime";
        public const string TransferTimeKey = "TransferTime";
        public const string TotalTimeKey = "TotalTime";
        public const string RelationOriginNodeKey = "RelationOriginNode";
        public const string RelationDestinationNodeKey = "RelationDestionationNode";
        private const string UsedTrainKeyBase = "utrain_";
        private const string TransferNodeKeyBase = "tnode_";

        private const string RelationsTableLabel = "PassengerRelations";
        private const string OriginNodeLabel = "Origin Node";
        private const string DestinationNodeLabel = "Destination Node";
        private const string TravelTimeLabel = "Total Travel Time";
        private const string TransferTimeLabel = "Transfer Time";
        private const string TotalTimeLabel = "Total Time";
        private const string UsedTrain = "Used Train";
        private const string TransferNode = "Transfer Node";

        public static string GetUsedTrainKey(int i) {
            return UsedTrainKeyBase + i.ToInvariantString();
        }

        public static string GetTransferNodeKey(int i) {
            return TransferNode + i.ToInvariantString();
        }

        public ITableDefinition RelationsTableDefinition(int numberOfChangesAllowed) {
            var fixedColumnDefinitions = ImmutableListUtils.FromArray(
                new TableColumnDefinition(
                    RelationOriginNodeKey,
                    new TextCell(RelationOriginNodeKey, OriginNodeLabel),
                    TableCellDataType.String,
                    TableCellDataType.AlgorithmNode),
                new TableColumnDefinition(
                    RelationDestinationNodeKey,
                    new TextCell(RelationDestinationNodeKey, DestinationNodeLabel),
                    TableCellDataType.String,
                    TableCellDataType.AlgorithmNode),
                new TableColumnDefinition(
                    TravelTimeKey,
                    new TextCell(TravelTimeKey, TravelTimeLabel),
                    TableCellDataType.String,
                    TableCellDataType.Duration),
                new TableColumnDefinition(
                    TransferTimeKey,
                    new TextCell(TransferTimeKey, TransferTimeLabel),
                    TableCellDataType.String,
                    TableCellDataType.Duration),
                new TableColumnDefinition(
                    TotalTimeKey,
                    new TextCell(TotalTimeKey, TotalTimeLabel),
                    TableCellDataType.String,
                    TableCellDataType.Duration));
            var usedTrainColumns = Enumerable
                .Range(1, numberOfChangesAllowed + 1)
                .Select(i => string.Join(" ", UsedTrain, i))
                .Select((usedTrainLabel, i) => new TableColumnDefinition(GetUsedTrainKey(i), new TextCell(GetUsedTrainKey(i), usedTrainLabel), TableCellDataType.String, TableCellDataType.AlgorithmTrain))
                .ToImmutableList();
            var transferNodeColumns = Enumerable
                .Range(1, numberOfChangesAllowed)
                .Select(i => string.Join(" ", TransferNode, i))
                .Select((usedTrainLabel, i) => new TableColumnDefinition(GetTransferNodeKey(i), new TextCell(GetTransferNodeKey(i), usedTrainLabel), TableCellDataType.String, TableCellDataType.AlgorithmNode))
                .ToImmutableList();

            var allColumnDefinitions = new List<ITableColumnDefinition>(fixedColumnDefinitions);
            allColumnDefinitions.Add(usedTrainColumns.First());
            foreach (var (node, train) in transferNodeColumns.Zip(usedTrainColumns.Skip(1)).ToImmutableList()) {
                allColumnDefinitions.Add(node);
                allColumnDefinitions.Add(train);
            }

            return new TableDefinition(RelationsTableLabel, allColumnDefinitions.ToImmutableList());
        }
    }
}