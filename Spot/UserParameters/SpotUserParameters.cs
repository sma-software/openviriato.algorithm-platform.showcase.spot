using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters {
    public class SpotUserParameters : ISpotUserParameters {
        public SpotUserParameters(int? solverTimeOut, int maximalNumberOfTransfers, Duration defaultMinimumTransferTime, int numberOfCycles, string filePathToRoutesAsCsvFile, TimeWindow cycleTimeWindow, double additionalRunTimeFactor, IImmutableList<IAlgorithmTrain> algorithmTrainsForSpotLineConstraints) {
            SolverTimeout = solverTimeOut;
            MaximalNumberOfTransfers = maximalNumberOfTransfers;
            DefaultMinimumTransferTime = defaultMinimumTransferTime;
            NumberOfCycles = numberOfCycles;
            FilePathToRoutesAsCsvFile = filePathToRoutesAsCsvFile;
            CycleTimeWindow = cycleTimeWindow;
            AdditionalRunTimeFactor = additionalRunTimeFactor;
            AlgorithmTrainsForSpotLineConstraints = algorithmTrainsForSpotLineConstraints;
        }

        public int? SolverTimeout { get; }
        public int MaximalNumberOfTransfers { get; }
        public Duration DefaultMinimumTransferTime { get; }
        public int NumberOfCycles { get; }
        public TimeWindow CycleTimeWindow { get; }
        public string FilePathToRoutesAsCsvFile { get; }
        public double AdditionalRunTimeFactor { get; }
        public IImmutableList<IAlgorithmTrain> AlgorithmTrainsForSpotLineConstraints { get; }
    }
}