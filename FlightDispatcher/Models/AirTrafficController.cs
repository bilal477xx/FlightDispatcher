using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Threading;
using FlightDispatcher.Events;
using FlightDispatcher.Managers;

namespace FlightDispatcher.Models;

public class AirTrafficController
{
    public string EmployeeId { get; set; }
    public string Name { get; set; }
    public int NumberOfFlightsHandled { get; set; }
    
    public event EventHandler<FlightDelayedEventArgs>? FlightDelayed;
    public event EventHandler<EmergencyDeclaredEventArgs>? EmergencyDeclared;
    public event EventHandler<RunwayClearedEventArgs>? RunwayCleared;
    
    private readonly HashSet<string> _airborneFlights = new HashSet<string>();
    private readonly DispatcherTimer _monitorTimer = new DispatcherTimer();
    private readonly Random _random = new Random();

    public AirTrafficController() : this(string.Empty, string.Empty, 0) { }

    public AirTrafficController(string employeeId, string name, int numberOfFlightsHandled)
    {
        EmployeeId = employeeId;
        Name = name;
        NumberOfFlightsHandled = numberOfFlightsHandled;
        
        _monitorTimer.Interval = TimeSpan.FromSeconds(1);
        _monitorTimer.Tick += MonitorTimerTick;
    }

    public void StartMonitoring(FlightDispatcherManager flightDispatcherManager)
    {
        flightDispatcherManager.TakeOff += FlightTakeOff;
        flightDispatcherManager.Landed += FlightLanded;
        _monitorTimer.Start();
    }

    public void StopMonitoring(FlightDispatcherManager flightDispatcherManager)
    {
        flightDispatcherManager.TakeOff -= FlightTakeOff;
        flightDispatcherManager.Landed -= FlightLanded;
        _monitorTimer.Stop();
    }
    
    private void FlightTakeOff(object? sender, TakeOffEventArgs e)
    {
        _airborneFlights.Add(e.FlightId);
        OnRunwayCleared(new RunwayClearedEventArgs(e.FlightId, e.Airline, Name, DateTime.Now));
    }

    private void FlightLanded(object? sender, LandedEventArgs e)
    {
        NumberOfFlightsHandled++;
        _airborneFlights.Remove(e.FlightId);
        OnRunwayCleared(new RunwayClearedEventArgs(e.FlightId, e.Airline, Name, DateTime.Now));
    }
    
    
    private void MonitorTimerTick(object? sender, EventArgs e)
    {
        foreach (string flightId in _airborneFlights.ToList())
        {
            int roll = _random.Next(100);
            if (roll <= 2)
                OnEmergencyDeclared(new EmergencyDeclaredEventArgs(flightId, "", "Engine failure", Name, DateTime.Now));
            else if (roll <= 7)
                OnFlightDelayed(new FlightDelayedEventArgs(flightId, "", 2, "Holding pattern", Name, DateTime.Now));
        }
    }
    
    private void OnFlightDelayed(FlightDelayedEventArgs e) => FlightDelayed?.Invoke(this, e);
    
    private void OnEmergencyDeclared(EmergencyDeclaredEventArgs e) => EmergencyDeclared?.Invoke(this, e);

    private void OnRunwayCleared(RunwayClearedEventArgs e) => RunwayCleared?.Invoke(this, e);
}