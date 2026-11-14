#nullable enable

using SignalProcessingToolApp.Domains.Parameters;

namespace SignalProcessingToolApp.Domains.WaveformGeneration;

public class UnitImpulseGenerator
{
    private const double ImpulseAmplitude = 1.0;
    private const double NonImpulseAmplitude = 0.0;
    private const int ImpulseSampleIndex = 0;

    public void Generate(
        Span<double> output,
        CommonParameter commonParameter,
        int startIndex)
    {
        if (output.IsEmpty)
        {
            throw new ArgumentException("Output span cannot be empty.", nameof(output));
        }

        for (int index = 0; index < output.Length; index++)
        {
            output[index] = (startIndex + index == ImpulseSampleIndex) ? ImpulseAmplitude : NonImpulseAmplitude;
        }
    }
}
