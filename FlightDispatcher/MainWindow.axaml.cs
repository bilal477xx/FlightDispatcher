using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FlightDispatcher.Events;
using FlightDispatcher.Managers;
using FlightDispatcher.Models;
using MsBox.Avalonia;

namespace FlightDispatcher;

public partial class MainWindow : Window
{

    private readonly FlightDispatcherManager _flightDispatcher = new();
    private Flight _currentFlight;
    
    public MainWindow()
    {
        InitializeComponent();
        _currentFlight = new Flight();
        FlightForm.DataContext = _currentFlight;

        RefreshFlightList();
        
        _flightDispatcher.FlightAdded += OnFlightAdded;
        _flightDispatcher.TakeOff += OnTakeOff;
        _flightDispatcher.Landed += OnLanded;
        _flightDispatcher.FlightStatusChanged += OnFlightStatusChanged;
    }

    private async void BtnAddFlight_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_currentFlight.ValidFlight())
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "All the plane fields are required").ShowWindowDialogAsync(this);
            return;
        }
        
        try
        {
            _flightDispatcher.AddFlight(_currentFlight);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", ex.Message).ShowWindowDialogAsync(this);
        }
    }
    
    private async void BtnTakeOff_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!(LbFlights.SelectedItem is Flight selectedFlight))
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "You must select a flight first.").ShowWindowDialogAsync(this);
            return;
        }
        
        try
        {
            _flightDispatcher.Depart(selectedFlight.FlightId);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", ex.Message).ShowWindowDialogAsync(this);
        }
    }
    
    private void OnFlightAdded(object? sender, FlightAddedEventArgs e)
    {
        _currentFlight = new Flight();
        FlightForm.DataContext = _currentFlight;
        
        LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) has been added with origin {e.Origin} and destination {e.Destination}.");
        
        RefreshFlightList();
    }
    
    private void OnTakeOff(object? sender, TakeOffEventArgs e)
    {
        LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) has departed at {e.DepartureTime}");
    }

    private void OnLanded(object? sender, LandedEventArgs e)
    {
        LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) has landed in {e.LandedIn} at {e.ArrivalTime}");
    }
    
    private void OnFlightStatusChanged(object? sender, FlightStatusChangedEventArgs e)
    {
        LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) status has been changed from {e.OldStatus} to {e.NewStatus} at {e.Time}");
        RefreshFlightList();
    }

    private void RefreshFlightList()
    {
        LbFlights.ItemsSource = null;
        LbFlights.ItemsSource = _flightDispatcher.GetFlights();
    }

    private void MnuNew_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void MnuSave_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}