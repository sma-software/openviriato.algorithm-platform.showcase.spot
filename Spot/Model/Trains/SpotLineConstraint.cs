using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public class SpotLineConstraint : ISpotLineConstraint {
        public SpotLineConstraint(long id, IImmutableList<ISpotPathNodeConstraint> pathNodes, string code) {
            Code = code;
            ID = id;
            PathNodes = pathNodes;
        }

        public long ID { get; }
        public string Code { get; }
        public IImmutableList<ISpotPathNodeConstraint> PathNodes { get; }
    }
}