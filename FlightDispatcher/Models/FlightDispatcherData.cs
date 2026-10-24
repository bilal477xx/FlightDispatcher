using System.Collections.Generic;

namespace FlightDispatcher.Models;

public class FlightDispatcherData
{
    public List<Flight> Flights { get; }
    public List<string> FlightEvents { get; }

    public FlightDispatcherData(List<Flight> flights, List<string> flightEvents)
    {
        Flights = flights;
        FlightEvents = flightEvents;
    }
}