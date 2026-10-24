using System;

namespace FlightDispatcher.Events;

public class RunwayClearedEventArgs : FlightEventArgs
{
    public string AirTrafficControllerName { get; }
    public DateTime DelayTime { get; }
    
    public RunwayClearedEventArgs(string flightId, string airline, string arAirTrafficControllerName, DateTime delayTime) : base(flightId, airline)
    {
        AirTrafficControllerName = arAirTrafficControllerName;
        DelayTime = delayTime;
    }
}