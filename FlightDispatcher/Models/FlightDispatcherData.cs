using System.Collections.Generic;

namespace FlightDispatcher.Models;

public class FlightDispatcherData
{
    public List<Flight> Flights { get; }
    public List<string> FlightEvents { get; }
    
    public AirTrafficController AirTrafficController { get; }

    public FlightDispatcherData(List<Flight> flights, List<string> flightEvents, AirTrafficController airTrafficController)
    {
        Flights = flights;
        FlightEvents = flightEvents;
        AirTrafficController = airTrafficController;
    }
}