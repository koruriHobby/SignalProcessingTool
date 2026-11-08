#nullable enable

namespace SignalProcessingToolApp.Domain.WaveformGeneration;

public class UnitImpulseGenerator
{
    public void Generate(
        Span<double> output,
        CommonParameter commonParameter,
        int startIndex)
    {
        if (output.IsEmpty)
        {
            throw new ArgumentException("Output span cannot be empty.", nameof(output));
        }
    
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (startIndex + i == 0) ? 1.0 : 0.0;
        }
    }
}
