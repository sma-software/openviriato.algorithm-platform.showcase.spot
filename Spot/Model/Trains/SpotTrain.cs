using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains {
    public interface ISpotTrain {
        long ID { get; }
        string Code { get; }
        IImmutableList<ISpotTrainPathNode> TrainPathNodes { get; }
    }

    public class SpotTrain : ISpotTrain {
        public SpotTrain(long id, IImmutableList<ISpotTrainPathNode> spotTrainPathNodes, string code) {
            ID = id;
            TrainPathNodes = spotTrainPathNodes;
            Code = code;
        }

        public long ID { get; }
        public string Code { get; }
        public IImmutableList<ISpotTrainPathNode> TrainPathNodes { get; }
    }
}