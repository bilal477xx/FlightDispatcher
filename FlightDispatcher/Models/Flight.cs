using System;
using FlightDispatcher.Events;
using FlightDispatcher.Models.Enums;

namespace FlightDispatcher.Models;

public class Flight
{
    
    public string FlightId
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Flight ID is required.");
            
            field = value;
        }
    }
    public string Airline { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }

    public double FlightDuration
    {
        get;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(FlightDuration), "Flight duration must be greater than zero.");
            
            field = value;
        }
    }
    public FlightStatus Status { get; set; }

    public Flight()
    {
        FlightId = string.Empty;
        Airline = string.Empty;
        Origin = string.Empty;
        Destination = string.Empty;
        DepartureTime = DateTime.Now;
        ArrivalTime = DateTime.Now;
        FlightDuration = 0.0;
        Status = FlightStatus.Landed;
    }

    public Flight(string flightId, string airline, string origin, string destination, DateTime departureTime, DateTime arrivalTime)
    {
        FlightId = flightId;
        Airline = airline;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        FlightDuration = 0.0;
    }

    public void MarkAsDeparted(DateTime departureTime)
    {
        if (Status == FlightStatus.InAir)
            throw new InvalidOperationException($"{FlightId} is already in the air.");

        Status = FlightStatus.InAir;
        DepartureTime = departureTime;
        ArrivalTime = null;
    }

    public void MarkAsLanded(DateTime arrivalTime)
    {
        if (Status != FlightStatus.InAir)
            throw new InvalidOperationException($"{FlightId} is not in the air.");
        
        Status = FlightStatus.Landed;
        ArrivalTime = arrivalTime;
        Destination = "Home";
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            $"Flight ID: {FlightId}, Airline: {Airline}",
            $"Origin: {Origin}, Destination: {Destination}",
            $"Departure Time: {DepartureTime}, Arrival Time: {ArrivalTime},  Flight Duration: {FlightDuration}",
            $"Status: {Status}");
    }
}