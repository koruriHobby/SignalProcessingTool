using System;
using SignalProcessingToolApp.Domains.Parameters;

namespace SignalProcessingToolApp.Domains.SignalProcessing;

public class IirProcessor
{
    private readonly IirParameter _iirParameter;
    private double _inputPrevious1 = 0.0;
    private double _inputPrevious2 = 0.0;
    private double _outputPrevious1 = 0.0;
    private double _outputPrevious2 = 0.0;

    public IirProcessor(IirParameter iirParameter)
    {
        _iirParameter = iirParameter ?? throw new ArgumentNullException(nameof(iirParameter));
    }

    public void ProcessFrame(
        ReadOnlySpan<double> input,
        Span<double> output)
    {
        if (input.IsEmpty)
        {
            throw new ArgumentException("Input span cannot be empty.", nameof(input));
        }

        if (output.Length < input.Length)
        {
            throw new ArgumentException("Output span must be at least as long as input span.", nameof(output));
        }

        for (int index = 0; index < input.Length; index++)
        {
            output[index] = Process(input[index]);
        }
    }

    private double Process(double input)
    {
        double output = _iirParameter.B0 * input
            + _iirParameter.B1 * _inputPrevious1
            + _iirParameter.B2 * _inputPrevious2
            + _iirParameter.A1 * _outputPrevious1
            + _iirParameter.A2 * _outputPrevious2;

        _inputPrevious2 = _inputPrevious1;
        _inputPrevious1 = input;
        _outputPrevious2 = _outputPrevious1;
        _outputPrevious1 = output;

        return output;
    }
}
