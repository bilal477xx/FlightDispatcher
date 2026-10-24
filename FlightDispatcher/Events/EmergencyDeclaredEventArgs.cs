using System;

namespace FlightDispatcher.Events;

public class EmergencyDeclaredEventArgs : FlightEventArgs
{
    
    public string Message { get; }
    public string AirTrafficControllerName { get; }
    public DateTime DelayTime { get; }
    
    public EmergencyDeclaredEventArgs(string flightId, string airline, string message, string airTrafficControllerName, DateTime delayTime) : base(flightId, airline)
    {
        Message = message;
        AirTrafficControllerName = airTrafficControllerName;
        DelayTime = delayTime;
    }
}