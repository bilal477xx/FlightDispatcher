using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FlightDispatcher.Events;
using FlightDispatcher.Managers;
using FlightDispatcher.Models;
using FlightDispatcher.Services;
using MsBox.Avalonia;

namespace FlightDispatcher;

public partial class MainWindow : Window
{

    private readonly FlightDispatcherManager _flightDispatcher = new();
    private Flight _currentFlight;
    
    private string _currentSaveFilePath = string.Empty;
    private string _savedDataSnapshot = string.Empty;
    
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
        _flightDispatcher.FlightEventsUpdated += OnFlightEventUpdated;
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
        
        _flightDispatcher.AddFlightEvent($"Flight {e.Airline} (flight id {e.FlightId}) has been added with origin {e.Origin} and destination {e.Destination}.");
        //LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) has been added with origin {e.Origin} and destination {e.Destination}.");
        
        RefreshFlightList();
    }
    
    private void OnTakeOff(object? sender, TakeOffEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"Flight {e.Airline} (flight id {e.FlightId}) has departed at {e.DepartureTime}");
        //LbFlightUpdates.Items.Add($"Flight {e.Airline} (flight id {e.FlightId}) has departed at {e.DepartureTime}");
    }

    private void OnLanded(object? sender, LandedEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"Flight {e.Airline} (flight id {e.FlightId}) has landed in {e.LandedIn} at {e.ArrivalTime}");
    }
    
    private void OnFlightStatusChanged(object? sender, FlightStatusChangedEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"Flight {e.Airline} (flight id {e.FlightId}) status has been changed from {e.OldStatus} to {e.NewStatus} at {e.Time}");
        RefreshFlightList();
    }

    private void RefreshFlightList()
    {
        LbFlights.ItemsSource = null;
        LbFlights.ItemsSource = _flightDispatcher.GetFlights();
    }

    private void ResetForm()
    {
        _currentFlight = new Flight();
    }

    private void MnuNew_OnClick(object? sender, RoutedEventArgs e)
    {
        _flightDispatcher.ClearAllFlights();
        RefreshFlightList();
        ResetForm();
    }

    private async void MnuSave_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentSaveFilePath))
        {
            SaveAs();
            return;
        }
        
        try
        {
            FlightDispatcherData flightDispatcherData = new FlightDispatcherData(_flightDispatcher.GetFlights(), _flightDispatcher.GetAllFlightEvents().ToList());
            await JsonFileService<FlightDispatcherData>.SaveAsync(flightDispatcherData, _currentSaveFilePath);
            await MessageBoxManager.GetMessageBoxStandard("Info", "The data has been saved.").ShowWindowDialogAsync(this);
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", exception.Message).ShowWindowDialogAsync(this);
        }
    }

    private void MnuSaveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        SaveAs();
    }

    private async void SaveAs()
    {
        try
        {
            IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save as",
                SuggestedFileName = "FlightDispatcherData.json",
                DefaultExtension =  "json",
                ShowOverwritePrompt = true,
                FileTypeChoices = [
                    new FilePickerFileType("JSON file") { Patterns = ["*.json"]}
                ]
            });
            
            string? path = file?.TryGetLocalPath();
            if (path == null) return;
            
            FlightDispatcherData flightDispatcherData = new FlightDispatcherData(_flightDispatcher.GetFlights(), _flightDispatcher.GetAllFlightEvents().ToList());
            await JsonFileService<FlightDispatcherData>.SaveAsync(flightDispatcherData, path);
            await MessageBoxManager.GetMessageBoxStandard("Info", "The data has been saved.").ShowWindowDialogAsync(this);
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", exception.Message).ShowWindowDialogAsync(this);
        }
    }

    private async void MnuLoad_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Pick file",
                AllowMultiple = false,
                FileTypeFilter = [
                    new FilePickerFileType("JSON file") { Patterns = ["*.json"] }
                ]
            });
            
            var path = files.Count > 0 ? files[0].TryGetLocalPath() : null;
            if (path == null) return;
            
            FlightDispatcherData? data = await JsonFileService<FlightDispatcherData>.LoadAsync(path);
            
            if (data == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Info", "The file is empty.").ShowWindowDialogAsync(this);
                return;
            }
            
            _flightDispatcher.LoadFrom(data);
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", exception.Message).ShowWindowDialogAsync(this);
        }
    }

    private void OnFlightEventUpdated()
    {
        LbFlightUpdates.ItemsSource = _flightDispatcher.GetAllFlightEvents();
    }
}