using System;

namespace FlightDispatcher.Events;

public class FlightDelayedEventArgs : FlightEventArgs
{
    
    public int DelayDuration { get; }
    public string Message { get; }
    public string AirTrafficControllerName { get; }
    public DateTime DelayTime { get; }
    
    public FlightDelayedEventArgs(string flightId, string airline, int delayDuration, string message, string airTrafficControllerName, DateTime delayTime) : base(flightId, airline)
    {
        DelayDuration = delayDuration;
        Message = message;
        AirTrafficControllerName = airTrafficControllerName;
        DelayTime = delayTime;
    }
}