using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FlightDispatcher;

public partial class ReportWindow : Window
{
    public ReportWindow(string? report)
    {
        InitializeComponent();
        TbReport.Text = report;
    }
}