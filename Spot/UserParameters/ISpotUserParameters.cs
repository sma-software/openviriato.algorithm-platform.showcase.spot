using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters {
    public interface ISpotUserParameters {
        int? SolverTimeout { get; }
        int MaximalNumberOfTransfers { get; }
        Duration DefaultMinimumTransferTime { get; }
        int NumberOfCycles { get; }
        TimeWindow CycleTimeWindow { get; }
        string FilePathToRoutesAsCsvFile { get; }
        double AdditionalRunTimeFactor { get; }
        IImmutableList<IAlgorithmTrain> AlgorithmTrainsForSpotLineConstraints { get; }
    }
}