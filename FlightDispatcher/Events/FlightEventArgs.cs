using System;

namespace FlightDispatcher.Events;

public class FlightEventArgs : EventArgs
{
    public string FlightId  { get; }
    public string Airline { get; }
    
    protected FlightEventArgs(string flightId, string airline)
    {
        FlightId = flightId;
        Airline = airline;
    }
}