#nullable enable

namespace SignalProcessingToolApp.Domain.WaveformGeneration;

public record SineWaveParameter
{
    public double Frequency { get; }

    public SineWaveParameter(
        double frequency,
        double samplingFrequency)
    {
        if (frequency <= 0)
        {
            throw new ArgumentException(
                $"周波数は0より大きい値である必要があります。入力値: {frequency}",
                nameof(frequency));
        }

        if (samplingFrequency <= 0)
        {
            throw new ArgumentException(
                $"サンプリング周波数は0より大きい値である必要があります。入力値: {samplingFrequency}",
                nameof(samplingFrequency));
        }

        double nyquistFrequency = samplingFrequency / 2.0;
        if (frequency >= nyquistFrequency)
        {
            throw new ArgumentException(
                $"ナイキスト定理により、周波数は {nyquistFrequency} Hz より小さい値である必要があります。入力値: {frequency}",
                nameof(frequency));
        }

        Frequency = frequency;
    }
}
