namespace SignalProcessingToolApp.Domains.SignalProcessing;

public class GainProcessor
{
    private const double MinusInfinityDb = -200.0;
    private const double DecibelBase = 10.0;
    private const double AmplitudeDecibelBase = 20.0;

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

        double linearGain = (gainDb <= MinusInfinityDb) ? 0.0 : Math.Pow(DecibelBase, gainDb / AmplitudeDecibelBase);

        for (int index = 0; index < input.Length; index++)
        {
            output[index] = input[index] * linearGain;
        }
    }
}
