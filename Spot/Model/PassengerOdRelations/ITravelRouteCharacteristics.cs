using NodaTime;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface ITravelRouteCharacteristics {
        IPassengerTravelRoute TravelRoute { get; }
        double NumberOfPassengers { get; }
        Duration TotalTrainTravelTime { get; }
        Duration TotalTransferTime { get; }
    }

    public class TravelRouteCharacteristics : ITravelRouteCharacteristics {
        public TravelRouteCharacteristics(IPassengerTravelRoute travelRoute, double numberOfPassengers, Duration totalTransferTime, Duration totalTrainTravelTime) {
            TravelRoute = travelRoute;
            NumberOfPassengers = numberOfPassengers;
            TotalTrainTravelTime = totalTrainTravelTime;
            TotalTransferTime = totalTransferTime;
        }

        public IPassengerTravelRoute TravelRoute { get; }
        public double NumberOfPassengers { get; }
        public Duration TotalTrainTravelTime { get; }
        public Duration TotalTransferTime { get; }
    }
}