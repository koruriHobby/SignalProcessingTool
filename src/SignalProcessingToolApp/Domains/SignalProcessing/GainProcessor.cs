namespace SignalProcessingToolApp.Domain.SignalProcessing;

public class GainProcessor
{
    private const double MinusInfinityDb = -200.0;

    public void Apply(
        ReadOnlySpan<double> input,
        Span<double> output,
        double gainDb)
    {
        if (input.IsEmpty)
        {
            throw new ArgumentException("Input span cannot be empty.", nameof(input));
        }

        if (output.IsEmpty)
        {
            throw new ArgumentException("Output span cannot be empty.", nameof(output));
        }

        if (output.Length < input.Length)
        {
            throw new ArgumentException("Output span must be at least as long as input span.", nameof(output));
        }

        double linearGain = (gainDb <= MinusInfinityDb) ? 0.0 : Math.Pow(10.0, gainDb / 20.0);

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = input[i] * linearGain;
        }
    }
}
