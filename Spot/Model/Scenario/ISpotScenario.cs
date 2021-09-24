using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario {
    public interface ISpotScenario {
        IImmutableList<ISpotLineConstraint> Lines { get; }
        Duration CycleTime { get; }
        Duration MaximumTransferTime { get; }
        int NumberOfPeriods { get; }
        TimeConverter TimeConverter { get; }
        IImmutableList<IPassengerRelation> PassengerRelations { get; }
        IImmutableList<IPassengerRelation> PassengerRelationsWithRoutes { get; }
        ITransferTimeLookup TransferTimeLookup { get; }
        double AdditionalRunTimeFactor { get; }
    }
}