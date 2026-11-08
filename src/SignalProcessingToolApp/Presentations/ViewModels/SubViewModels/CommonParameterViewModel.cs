#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using SignalProcessingToolApp.Domain;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class CommonParameterViewModel : ObservableObject
{
    [ObservableProperty]
    private int _samplingFrequencyIndex = 0;

    [ObservableProperty]
    private int _frameSizeIndex = 0;

    [ObservableProperty]
    private int _dataSizeIndex = 2;

    public List<int> SamplingFrequencies { get; } = new() { 44100, 48000, 96000 };
    public List<int> FrameSizes { get; } = new() { 8, 16, 32, 64, 128, 256 };
    public List<int> DataSizes { get; } = new() { 8, 16, 32, 64, 128, 256, 512, 1024 };

    public CommonParameter CreateParameter()
    {
        double samplingFrequency = SamplingFrequencies[SamplingFrequencyIndex];
        int frameSize = FrameSizes[FrameSizeIndex];
        int dataSize = DataSizes[DataSizeIndex];

        return new CommonParameter(samplingFrequency, frameSize, dataSize);
    }
}