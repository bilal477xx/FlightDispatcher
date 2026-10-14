using System;
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
    } = string.Empty;

    public string Airline
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Airline is required.");
            
            field = value;
        }
    } = string.Empty;

    public string Origin
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Origin is required.");
            
            field = value;
        }
    } = string.Empty;

    public string Destination
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Destination is required.");
            
            field = value;
        }
    } = string.Empty;
    public DateTime? DepartureTime { get; private set; }
    public DateTime? ArrivalTime { get; private set; }

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
    public FlightStatus Status { get; private set; }

    public Flight()
    {
        DepartureTime = null;
        ArrivalTime = null;
        Status = FlightStatus.Landed;
    }

    public Flight(string flightId, string airline, string origin, string destination, DateTime departureTime, DateTime arrivalTime, double flightDuration, FlightStatus status)
    {
        FlightId = flightId;
        Airline = airline;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        FlightDuration = flightDuration;
        Status = status; 
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

    public bool ValidFlight()
    {
        if (string.IsNullOrWhiteSpace(FlightId) || string.IsNullOrWhiteSpace(Airline) || string.IsNullOrWhiteSpace(Origin) ||
            string.IsNullOrWhiteSpace(Destination) || FlightDuration <= 0.0)
        {
            return false;
        }
        
        return true;
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            $"Flight ID: {FlightId}, Airline: {Airline}",
            $"Origin: {Origin}, Destination: {Destination}",
            //$"Departure Time: {DepartureTime}, Arrival Time: {ArrivalTime},  Flight Duration: {FlightDuration}",
            $"Status: {Status}");
    }
}