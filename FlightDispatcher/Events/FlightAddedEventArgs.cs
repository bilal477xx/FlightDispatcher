namespace FlightDispatcher.Events;

public class FlightAddedEventArgs : FlightEventArgs
{

    public string Origin { get; }
    public string Destination { get; }
    
    public FlightAddedEventArgs(string flightId, string airline, string origin, string destination) : base(flightId, airline)
    {
        Origin = origin;
        Destination = destination;
    }
}