namespace FlightDispatcher.Events;

public class FlightAddedEventArgs : FlightEventArgs
{

    public FlightAddedEventArgs(string flightId) : base(flightId)
    {
        
    }
}