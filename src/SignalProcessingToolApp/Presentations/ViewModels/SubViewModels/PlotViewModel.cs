#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class PlotViewModel : ObservableObject
{
    [ObservableProperty]
    private double[]? _inputWaveformData;

    [ObservableProperty]
    private double[]? _processedWaveformData;

    public void Clear()
    {
        InputWaveformData = null;
        ProcessedWaveformData = null;
    }

    public void UpdateData(double[] input, double[] processed)
    {
        InputWaveformData = input;
        ProcessedWaveformData = processed;
    }
}