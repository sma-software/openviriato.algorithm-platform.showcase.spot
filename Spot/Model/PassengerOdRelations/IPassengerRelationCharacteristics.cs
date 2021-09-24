using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface IPassengerRelationCharacteristics {
        IPassengerRelation PassengerRelation { get; }
        ImmutableList<ITravelRouteCharacteristics> TravelRouteCharacteristics { get; }
    }

    public class PassengerRelationCharacteristics : IPassengerRelationCharacteristics {
        public PassengerRelationCharacteristics(IPassengerRelation passengerRelation, ImmutableList<ITravelRouteCharacteristics> travelRouteCharacteristics) {
            PassengerRelation = passengerRelation;
            TravelRouteCharacteristics = travelRouteCharacteristics;
        }

        public IPassengerRelation PassengerRelation { get; }
        public ImmutableList<ITravelRouteCharacteristics> TravelRouteCharacteristics { get; }
    }
}