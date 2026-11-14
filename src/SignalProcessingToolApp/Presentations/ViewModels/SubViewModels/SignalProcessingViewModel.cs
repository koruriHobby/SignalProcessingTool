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

    [ObservableProperty]
    private bool _isDelayEnabled = false;

    [ObservableProperty]
    private string _delayBufferSizeText = string.Empty;

    [ObservableProperty]
    private string _delaySampleCountText = string.Empty;

    public int ParseDelayBufferSize()
    {
        return ParseInteger(
            DelayBufferSizeText,
            "ディレイバッファサイズ");
    }

    public int ParseDelaySampleCount()
    {
        return ParseInteger(
            DelaySampleCountText,
            "ディレイサンプル数");
    }

    private static int ParseInteger(
        string text,
        string parameterName)
    {
        if (!int.TryParse(text, out int value))
        {
            throw new FormatException($"{parameterName}は整数で入力してください。");
        }

        return value;
    }
}