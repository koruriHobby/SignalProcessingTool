#nullable enable

namespace SignalProcessingToolApp.Domain.WaveformGeneration;

public class SineWaveGenerator
{
    public virtual double[] Generate(
        SineWaveParameter parameter,
        CommonParameter commonParameter,
        int startIndex = 0)
    {
        double[] waveformData = new double[commonParameter.DataSize];
        double normalizedAngularFrequency = 2.0 * Math.PI * parameter.Frequency / commonParameter.SamplingFrequency;

        for (int i = 0; i < commonParameter.DataSize; i++)
        {
            double phase = normalizedAngularFrequency * (startIndex + i);
            waveformData[i] = Math.Sin(phase);
        }

        return waveformData;
    }
}
