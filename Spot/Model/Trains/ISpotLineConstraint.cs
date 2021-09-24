using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public interface ISpotLineConstraint {
        long ID { get; }
        string Code { get; }
        IImmutableList<ISpotPathNodeConstraint> PathNodes { get; }
    }
}