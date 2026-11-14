#nullable enable

using SignalProcessingToolApp.Domains.Parameters;

namespace SignalProcessingToolApp.Domains.WaveformGeneration;

public class SineWaveGenerator
{
    private const double FullCycleRadians = 2.0 * Math.PI;
    private const int DefaultStartIndex = 0;

    public void Generate(
        Span<double> output,
        SineWaveParameter parameter,
        CommonParameter commonParameter,
        int startIndex = DefaultStartIndex)
    {
        if (output.IsEmpty)
        {
            throw new ArgumentException("Output span cannot be empty.", nameof(output));
        }

        double normalizedAngularFrequency = FullCycleRadians * parameter.Frequency / commonParameter.SamplingFrequency;

        for (int index = 0; index < output.Length; index++)
        {
            double phase = normalizedAngularFrequency * (startIndex + index);
            output[index] = Math.Sin(phase);
        }
    }
}
