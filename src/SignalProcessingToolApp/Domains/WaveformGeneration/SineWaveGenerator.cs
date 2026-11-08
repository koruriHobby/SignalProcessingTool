#nullable enable

namespace SignalProcessingToolApp.Domain.WaveformGeneration;

public class SineWaveGenerator
{
    public void Generate(
        Span<double> output,
        SineWaveParameter parameter,
        CommonParameter commonParameter,
        int startIndex = 0)
    {
        if (output.IsEmpty)
        {
            throw new ArgumentException("Output span cannot be empty.", nameof(output));
        }

        double normalizedAngularFrequency = 2.0 * Math.PI * parameter.Frequency / commonParameter.SamplingFrequency;

        for (int i = 0; i < output.Length; i++)
        {
            double phase = normalizedAngularFrequency * (startIndex + i);
            output[i] = Math.Sin(phase);
        }
    }
}
