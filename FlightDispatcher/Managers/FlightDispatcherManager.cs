using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Threading;
using FlightDispatcher.Events;
using FlightDispatcher.Models;
using FlightDispatcher.Models.Enums;

namespace FlightDispatcher.Managers;

public class FlightDispatcherManager
{
    
    public event EventHandler<TakeOffEventArgs>? TakeOff;
    public event EventHandler<LandedEventArgs>? Landed;
    public event EventHandler<FlightStatusChangedEventArgs>? FlightStatusChanged;
    public event EventHandler<FlightAddedEventArgs>? FlightAdded;
    
    private readonly Dictionary<string, Flight> _flights = new();
    private readonly Dictionary<DispatcherTimer, Flight> _activeTimers = new();

    public void AddFlight(Flight flight)
    {
        if (!_flights.ContainsKey(flight.FlightId))
            throw new ArgumentException($"Flight with ID {flight.FlightId} does exist.");
        
        _flights.Add(flight.FlightId, flight);
        
        OnFlightAdded(new FlightAddedEventArgs(flight.FlightId, flight.Airline, flight.Origin, flight.Destination));
    }

    public List<Flight> GetFlights() => _flights.Values.ToList();

    public void Depart(string flightId)
    {
        if (!_flights.TryGetValue(flightId, out Flight? flight))
        {
            throw new KeyNotFoundException($"No flight with ID {flightId}.");
        }
        
        FlightStatus oldStatus = flight.Status;
        DateTime takeOffTime = DateTime.Now;
        flight.MarkAsDeparted(takeOffTime);
        
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(flight.FlightDuration);
        timer.Tick += Timer_Tick;
        _activeTimers.Add(timer, flight);
        timer.Start();
        
        OnTakeOff(new TakeOffEventArgs(flightId, flight.Airline, flight.Destination, takeOffTime));
        OnFlightStatusChanged(new FlightStatusChangedEventArgs(flightId, flight.Airline, oldStatus, flight.Status, takeOffTime));
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        DispatcherTimer timer = (DispatcherTimer) sender!;
        timer.Stop();
        timer.Tick -= Timer_Tick;
        
        Flight flight = _activeTimers[timer];
        _activeTimers.Remove(timer);
        
        string landedAt = flight.Destination;
        FlightStatus oldStatus = flight.Status;
        DateTime arrivalTime = DateTime.Now;
        flight.MarkAsLanded(arrivalTime);
        
        OnLanded(new LandedEventArgs(flight.FlightId, flight.Airline, landedAt, arrivalTime));
        OnFlightStatusChanged(new FlightStatusChangedEventArgs(flight.FlightId, flight.Airline, oldStatus, flight.Status, arrivalTime));
    }

    private void OnFlightAdded(FlightAddedEventArgs e)
    {
        FlightAdded?.Invoke(this, e);
    }
    
    private void OnTakeOff(TakeOffEventArgs e)
    {
        TakeOff?.Invoke(this, e);
    }

    private void OnLanded(LandedEventArgs e)
    {
        Landed?.Invoke(this, e);
    }

    private void OnFlightStatusChanged(FlightStatusChangedEventArgs e)
    {
        FlightStatusChanged?.Invoke(this, e);
    }
    
}