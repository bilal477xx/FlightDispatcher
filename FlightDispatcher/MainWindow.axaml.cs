using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FlightDispatcher.Events;
using FlightDispatcher.Managers;
using FlightDispatcher.Models;
using FlightDispatcher.Models.Enums;
using FlightDispatcher.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;

namespace FlightDispatcher;

public partial class MainWindow : Window
{

    private readonly FlightDispatcherManager _flightDispatcher = new();
    private AirTrafficController _airTrafficController = new("ATC-001", "Bilal", 100);
    private Flight _currentFlight;
    
    private string _currentSaveFilePath = Path.Combine(AppContext.BaseDirectory, "FlightDispatcherData.json");
    private string _savedDataSnapshot = string.Empty;
    
    private bool _closeConfirmed = false;
    
    public MainWindow()
    {
        InitializeComponent();
        _currentFlight = new Flight();
        FlightForm.DataContext = _currentFlight;
        _airTrafficController.StartMonitoring(_flightDispatcher);
        _flightDispatcher.AssignController(_airTrafficController);

        RefreshFlightList();
        
        _flightDispatcher.FlightAdded += OnFlightAdded;
        _flightDispatcher.TakeOff += OnTakeOff;
        _flightDispatcher.Landed += OnLanded;
        _flightDispatcher.FlightStatusChanged += OnFlightStatusChanged;
        _flightDispatcher.FlightEventsUpdated += OnFlightEventUpdated;
        _airTrafficController.FlightDelayed += OnFlightDelayed;
        _airTrafficController.EmergencyDeclared += OnEmergencyDeclared;
        _airTrafficController.RunwayCleared += OnRunwayCleared;
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

    private void OnFlightDelayed(object? sender, FlightDelayedEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"$Flight with flight id {e.FlightId} has been delayed for {e.DelayDuration} seconds.");
    }

    private void OnEmergencyDeclared(object? sender, EmergencyDeclaredEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"Flight with flight id {e.FlightId} has declared an emergency.");
    }

    private void OnRunwayCleared(object? sender, RunwayClearedEventArgs e)
    {
        _flightDispatcher.AddFlightEvent($"Flight with flight id {e.FlightId} has been cleared.");
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
        await Save();
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_currentSaveFilePath))
        {
            await SaveAs();
            return;
        }

        try
        {
            FlightDispatcherData flightDispatcherData = new FlightDispatcherData(_flightDispatcher.GetFlights(), _flightDispatcher.GetAllFlightEvents().ToList(), _airTrafficController);
            await JsonFileService<FlightDispatcherData>.SaveAsync(flightDispatcherData, _currentSaveFilePath);
            
            _savedDataSnapshot = CreateSnapshot();
            
            await MessageBoxManager.GetMessageBoxStandard("Info", "The data has been saved.").ShowWindowDialogAsync(this);
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", exception.Message).ShowWindowDialogAsync(this);
        }
    }

    private async void MnuSaveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        await SaveAs();
    }

    private async Task SaveAs()
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
            
            FlightDispatcherData flightDispatcherData = new FlightDispatcherData(_flightDispatcher.GetFlights(), _flightDispatcher.GetAllFlightEvents().ToList(), _airTrafficController);
            await JsonFileService<FlightDispatcherData>.SaveAsync(flightDispatcherData, path);

            _currentSaveFilePath = path;
            _savedDataSnapshot = CreateSnapshot();
            
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
            
            string? path = files.Count > 0 ? files[0].TryGetLocalPath() : null;
            if (path == null) return;
            
            FlightDispatcherData? data = await JsonFileService<FlightDispatcherData>.LoadAsync(path);
            
            if (data == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Info", "The file is empty.").ShowWindowDialogAsync(this);
                return;
            }
            
            _flightDispatcher.LoadFrom(data);
            _airTrafficController.Name = data.AirTrafficController.Name;
            _airTrafficController.EmployeeId = data.AirTrafficController.EmployeeId;
            _airTrafficController.NumberOfFlightsHandled = data.AirTrafficController.NumberOfFlightsHandled;
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

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_closeConfirmed || !HasUnsavedChanges()) return;
        e.Cancel = true;
        
        ButtonResult result = await MessageBoxManager
            .GetMessageBoxStandard("Save", "Do you want to save the changes?", ButtonEnum.YesNo)
            .ShowWindowDialogAsync(this);
        

        if (result == ButtonResult.Yes)
        {
            await Save();
        }
        
        _closeConfirmed = true;
        Close();
    }
    
    private string CreateSnapshot()
    {
        FlightDispatcherData data = new FlightDispatcherData(
            _flightDispatcher.GetFlights(),
            _flightDispatcher.GetAllFlightEvents().ToList(),
            _airTrafficController);

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    private bool HasUnsavedChanges() => CreateSnapshot() != _savedDataSnapshot;

    private void MnuReport_OnClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<string> report = _flightDispatcher.GetAllFlightEvents();
        StringBuilder stringReport = new StringBuilder();
        stringReport.AppendLine($"FLIGHT OPERATIONS REPORT – {DateTime.Now:yyyy-MM-dd HH:mm}");
        stringReport.AppendLine($"Controller: {_airTrafficController.Name} ({_airTrafficController.EmployeeId}), flights handled: {_airTrafficController.NumberOfFlightsHandled}");

        List<Flight> flights = _flightDispatcher.GetFlights();
        stringReport.AppendLine($"\nRECORDED FLIGHTS ({flights.Count})");
        foreach (Flight flight in flights)
            stringReport.AppendLine($"  {flight.FlightId}  {flight.Airline}  {flight.Origin} -> {flight.Destination}  {flight.Status}");

        stringReport.AppendLine("\nSTATISTICS");
        stringReport.AppendLine($"  In air: {flights.Count(f => f.Status == FlightStatus.InAir)}, landed: {flights.Count(f => f.Status == FlightStatus.Landed)}");
        
        foreach (string item in report)
        {
            stringReport.AppendLine(item);
        }
        
        ReportWindow reportWindow = new ReportWindow(stringReport.ToString());
        reportWindow.ShowDialog(this);
    }

    private async void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        try
        {
            if (File.Exists(_currentSaveFilePath))
            {
                FlightDispatcherData? data = await JsonFileService<FlightDispatcherData>.LoadAsync(_currentSaveFilePath);
                if (data != null) _flightDispatcher.LoadFrom(data);
            }
            else
            {
                await JsonFileService<FlightDispatcherData>.SaveAsync(new FlightDispatcherData(_flightDispatcher.GetFlights(), _flightDispatcher.GetAllFlightEvents().ToList(), _airTrafficController), _currentSaveFilePath);
            }
            
            _savedDataSnapshot = CreateSnapshot();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                       or NotSupportedException or JsonException or ArgumentException)
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Error", $"The file could not be loaded:\n{ex.Message}")
                .ShowWindowDialogAsync(this);
        }
    }
}