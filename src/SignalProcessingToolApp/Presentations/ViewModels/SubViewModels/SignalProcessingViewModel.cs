#nullable enable
using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Domains.Filters;

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

    [ObservableProperty]
    private bool _isIirFilterEnabled = false;

    [ObservableProperty]
    private string _cutoffFrequencyText = "100";

    public List<string> FilterTypes { get; } = new()
    {
        "b0確認用",
        "b1確認用",
        "b2確認用",
        "a1確認用",
        "a2確認用",
        "LPF(butter)",
        "HPF(butter)"
    };

    [ObservableProperty]
    private int _filterTypeIndex = -1;

    [ObservableProperty]
    private string _b0Text = "b0";

    [ObservableProperty]
    private string _b1Text = "b1";

    [ObservableProperty]
    private string _b2Text = "b2";

    [ObservableProperty]
    private string _a1Text = "a1";

    [ObservableProperty]
    private string _a2Text = "a2";

    public bool IsCutoffFrequencyEnabled => FilterTypeIndex == 5 || FilterTypeIndex == 6;

    partial void OnFilterTypeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsCutoffFrequencyEnabled));
    }

    public void UpdateCoefficientTexts(FilterCoefficientsDto coefficients)
    {
        B0Text = FormatCoefficient(coefficients.B0);
        B1Text = FormatCoefficient(coefficients.B1);
        B2Text = FormatCoefficient(coefficients.B2);
        A1Text = FormatCoefficient(coefficients.A1);
        A2Text = FormatCoefficient(coefficients.A2);
    }

    private static string FormatCoefficient(double coefficient)
    {
        double absoluteValue = Math.Abs(coefficient);
        if (absoluteValue != 0.0 && absoluteValue < 0.01)
        {
            return coefficient.ToString("0.00E+0", CultureInfo.InvariantCulture);
        }

        return coefficient.ToString("F2", CultureInfo.InvariantCulture);
    }

    public void ClearCoefficientTexts()
    {
        B0Text = "b0";
        B1Text = "b1";
        B2Text = "b2";
        A1Text = "a1";
        A2Text = "a2";
    }

    public FilterType GetFilterType()
    {
        return FilterTypeIndex switch
        {
            0 => FilterType.B0Check,
            1 => FilterType.B1Check,
            2 => FilterType.B2Check,
            3 => FilterType.A1Check,
            4 => FilterType.A2Check,
            5 => FilterType.Lpf,
            6 => FilterType.Hpf,
            _ => throw new InvalidOperationException("フィルタタイプが選択されていません。")
        };
    }

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

    public double ParseCutoffFrequency()
    {
        if (!double.TryParse(CutoffFrequencyText, out double value))
        {
            throw new FormatException("カットオフ周波数は数値で入力してください。");
        }

        return value;
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