using System;
using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Timetable.DataStructures;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public class SpotLineConstraintFactory {
        public ISpotLineConstraint CreateFrom(INavigableTrain train) {
            var spotLineNodes = train.TrainPathNodes.Select(node => node.ToSpotLinePathNode()).ToImmutableList();
            DoubleLinkNodes(spotLineNodes);
            return new SpotLineConstraint(train.ID, spotLineNodes, train.Code);
        }

        private static void DoubleLinkNodes(IImmutableList<SpotPathNodeConstraint> nodes) {
            if (nodes.Count > 1) {
                nodes.First().Successor = nodes.Skip(1).First();
                nodes.Last().Predecessor = nodes.SkipLast(1).Last();
            }

            var nodesWithSucceedingNode = nodes.SkipLast(1).Zip(nodes.Skip(1));
            foreach (var (precedingNode, succeedingNode) in nodesWithSucceedingNode) {
                succeedingNode.Predecessor = precedingNode;
                precedingNode.Successor = succeedingNode;
            }
        }
    }
}