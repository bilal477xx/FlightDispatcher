using System;
using FlightDispatcher.Models.Enums;

namespace FlightDispatcher.Events;

public class FlightStatusChangedEventArgs : FlightEventArgs
{
    public FlightStatus OldStatus { get; }
    public FlightStatus NewStatus { get; }
    public DateTime Time { get; }

    public FlightStatusChangedEventArgs(string flightId, FlightStatus oldStatus, FlightStatus newStatus, DateTime time) : base(flightId)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Time = time;
    }
}