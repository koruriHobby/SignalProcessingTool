using System;
using SignalProcessingToolApp.Domains.Parameters;

namespace SignalProcessingToolApp.Domains.SignalProcessing;

public class DelayProcessor
{
    private readonly double[] _buffer;
    private readonly int _sampleCount;
    private int _writeIndex = 0;

    public DelayProcessor(DelayParameter delayParameter)
    {
        _buffer = new double[delayParameter.BufferSize];
        _sampleCount = delayParameter.SampleCount;
    }

    public void ProcessFrame(
        ReadOnlySpan<double> input,
        Span<double> output)
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

        for (int index = 0; index < input.Length; index++)
        {
            output[index] = Process(input[index]);
        }
    }

    private double Process(double input)
    {
        if (_sampleCount == 0)
        {
            return input;
        }

        int readIndex = (_writeIndex - _sampleCount + _buffer.Length) % _buffer.Length;
        double output = _buffer[readIndex];

        _buffer[_writeIndex] = input;
        _writeIndex = (_writeIndex + 1) % _buffer.Length;

        return output;
    }
}
