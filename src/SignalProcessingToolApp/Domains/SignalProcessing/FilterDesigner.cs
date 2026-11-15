#nullable enable

using System;
using SignalProcessingToolApp.Domains.Filters;
using SignalProcessingToolApp.Domains.Parameters;

namespace SignalProcessingToolApp.Domains.SignalProcessing;

public static class FilterDesigner
{
    private const double A1CheckCoefficient = 0.5;
    private static readonly double SqrtTwo = Math.Sqrt(2.0);

    public static IirParameter Design(
        double samplingFrequency,
        double cutoffFrequency,
        FilterType filterType)
    {
        return filterType switch
        {
            FilterType.B0Check => new IirParameter(1.0, 0.0, 0.0, 0.0, 0.0),
            FilterType.B1Check => new IirParameter(0.0, 1.0, 0.0, 0.0, 0.0),
            FilterType.B2Check => new IirParameter(0.0, 0.0, 1.0, 0.0, 0.0),
            FilterType.A1Check => new IirParameter(1.0, 0.0, 0.0, A1CheckCoefficient, 0.0),
            FilterType.A2Check => new IirParameter(1.0, 0.0, 0.0, 0.0, A1CheckCoefficient),
            FilterType.Lpf => DesignButterworth(samplingFrequency, cutoffFrequency, isLowPass: true),
            FilterType.Hpf => DesignButterworth(samplingFrequency, cutoffFrequency, isLowPass: false),
            _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null)
        };
    }

    private static IirParameter DesignButterworth(
        double samplingFrequency,
        double cutoffFrequency,
        bool isLowPass)
    {
        if (cutoffFrequency < 0.0)
        {
            throw new ArgumentException(
                $"カットオフ周波数は0以上の値である必要があります。入力値: {cutoffFrequency}",
                nameof(cutoffFrequency));
        }

        double nyquistFrequency = samplingFrequency / 2.0;
        if (cutoffFrequency >= nyquistFrequency)
        {
            throw new ArgumentException(
                $"カットオフ周波数はナイキスト周波数未満である必要があります。カットオフ周波数: {cutoffFrequency}, ナイキスト周波数: {nyquistFrequency}",
                nameof(cutoffFrequency));
        }

        double k = Math.Tan(Math.PI * cutoffFrequency / samplingFrequency);
        double kSquared = k * k;
        double norm = 1.0 / (1.0 + SqrtTwo * k + kSquared);

        double a1 = 2.0 * (1.0 - kSquared) * norm;
        double a2 = -(1.0 - SqrtTwo * k + kSquared) * norm;

        if (isLowPass)
        {
            double b0 = kSquared * norm;
            return new IirParameter(b0, 2.0 * b0, b0, a1, a2);
        }

        return new IirParameter(norm, -2.0 * norm, norm, a1, a2);
    }
}
