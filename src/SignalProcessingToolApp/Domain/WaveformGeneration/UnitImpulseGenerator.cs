#nullable enable

namespace SignalProcessingToolApp.Domain.WaveformGeneration;

public class UnitImpulseGenerator
{
    public virtual double[] Generate(
        CommonParameter commonParameter,
        int startIndex = 0)
    {
        double[] waveformData = new double[commonParameter.DataSize];

        for (int i = 0; i < commonParameter.DataSize; i++)
        {
            waveformData[i] = (startIndex + i == 0) ? 1.0 : 0.0;
        }

        return waveformData;
    }
}
