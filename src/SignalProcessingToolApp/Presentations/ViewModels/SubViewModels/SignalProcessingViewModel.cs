#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class SignalProcessingViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isGainEnabled = false;

    [ObservableProperty]
    private int _gainIndex = 0;

    public List<int> GainValues { get; } = new() { 0, -3, -6, -9, -12, -15, -18, -200 };
}