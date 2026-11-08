#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class SignalProcessingViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isGainEnabled = false;

    [ObservableProperty]
    private int _gainIndex = 0;

    public List<int> GainValues { get; } = new() { 0, -3, -6, -9, -12, -15, -18, -200 };

    public SignalProcessingConfig CreateConfig()
    {
        int gainDb = GainValues[GainIndex];
        var gainConfig = new GainConfig(IsGainEnabled, gainDb);
        return new SignalProcessingConfig(gainConfig);
    }
}