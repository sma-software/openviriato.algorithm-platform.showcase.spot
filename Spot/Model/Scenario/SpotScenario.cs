using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario {
    public class SpotScenario : ISpotScenario {
        public SpotScenario(TimeWindow timeWindow, int numberOfPeriods, IImmutableList<ISpotLineConstraint> lines, IImmutableList<IPassengerRelation> passengerRelations, ITransferTimeLookup transferTimeLookup, double additionalRunTimeFactor) {
            Lines = lines;
            PassengerRelations = passengerRelations;
            PassengerRelationsWithRoutes = PassengerRelations.Where(r => r.HasRoutes).ToImmutableList();
            TransferTimeLookup = transferTimeLookup;
            AdditionalRunTimeFactor = additionalRunTimeFactor;
            TimeConverter = new TimeConverter(timeWindow.StartTime, timeWindow.Duration);
            NumberOfPeriods = numberOfPeriods;
        }

        public IImmutableList<ISpotLineConstraint> Lines { get; }
        public Duration CycleTime => TimeConverter.CycleTime;
        public Duration MaximumTransferTime => CycleTime.Times(2);
        public int NumberOfPeriods { get; }
        public TimeConverter TimeConverter { get; }
        public IImmutableList<IPassengerRelation> PassengerRelations { get; }
        public IImmutableList<IPassengerRelation> PassengerRelationsWithRoutes { get; }
        public ITransferTimeLookup TransferTimeLookup { get; }
        public double AdditionalRunTimeFactor { get; }
    }
}