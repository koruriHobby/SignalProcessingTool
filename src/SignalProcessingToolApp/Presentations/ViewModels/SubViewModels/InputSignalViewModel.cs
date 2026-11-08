#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using SignalProcessingToolApp.Application;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class InputSignalViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWaveformSineWave))]
    [NotifyPropertyChangedFor(nameof(IsWaveformImpulse))]
    [NotifyPropertyChangedFor(nameof(SineWaveFrequencyVisibility))]
    private int _waveformTypeIndex = 0;

    [ObservableProperty]
    private int _frequencyIndex = 3;

    public List<string> WaveformTypes { get; } = new() { "正弦波", "インパルス" };
    public List<int> SineWaveFrequencies { get; } = new() { 100, 200, 500, 1000, 2000, 5000, 10000, 20000 };

    public bool IsWaveformSineWave => WaveformTypeIndex == 0;
    public bool IsWaveformImpulse => WaveformTypeIndex == 1;
    public System.Windows.Visibility SineWaveFrequencyVisibility => IsWaveformSineWave ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

    public WaveformType GetWaveformType()
    {
        return WaveformTypeIndex == 0 ? WaveformType.SineWave : WaveformType.UnitImpulse;
    }

    public SineWaveParameter? CreateSineWaveParameter(double samplingFrequency)
    {
        if (GetWaveformType() != WaveformType.SineWave)
        {
            return null;
        }

        double frequency = SineWaveFrequencies[FrequencyIndex];
        return new SineWaveParameter(frequency, samplingFrequency);
    }
}