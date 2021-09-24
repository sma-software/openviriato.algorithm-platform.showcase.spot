using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface IPassengerTravelRoute {
        long ID { get; }
        IImmutableList<IPassengerTravelRoutePart> TravelRouteParts { get; }
        int NumberTransfers { get; }
    }

    public class PassengerTravelRoute : IPassengerTravelRoute {
        public PassengerTravelRoute(long id, IImmutableList<IPassengerTravelRoutePart> travelRouteParts) {
            TravelRouteParts = travelRouteParts;
            ID = id;
        }

        public long ID { get; }
        public IImmutableList<IPassengerTravelRoutePart> TravelRouteParts { get; }
        public int NumberTransfers => TravelRouteParts.Count - 1;
    }
}