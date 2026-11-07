#nullable enable

namespace SignalProcessingToolApp.Domain.SignalProcessing.Gain;

public class GainProcessor
{
    private const double MinusInfinityDb = -200.0;

    public virtual double[] Apply(
        double[] waveform,
        double gainDb)
    {
        if (waveform == null)
        {
            throw new ArgumentNullException(nameof(waveform));
        }

        if (gainDb == MinusInfinityDb)
        {
            return new double[waveform.Length];
        }

        double linearGain = Math.Pow(10.0, gainDb / 20.0);
        double[] processedWaveform = new double[waveform.Length];

        for (int i = 0; i < waveform.Length; i++)
        {
            processedWaveform[i] = waveform[i] * linearGain;
        }

        return processedWaveform;
    }
}
