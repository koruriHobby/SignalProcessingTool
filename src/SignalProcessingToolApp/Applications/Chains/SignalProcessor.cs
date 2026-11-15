#nullable enable

using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;

namespace SignalProcessingToolApp.Applications.Pipelines;

public class SignalProcessor
{
    private readonly GainProcessor _gainProcessor = new();
    private readonly DelayProcessor? _delayProcessor;
    private double[] _gainBuffer = Array.Empty<double>();
    private double[] _delayBuffer = Array.Empty<double>();

    public SignalProcessor(DelayParameter? delayParameter)
    {
        if (delayParameter is not null)
        {
            _delayProcessor = new DelayProcessor(delayParameter);
        }
    }

    public void Process(
        ReadOnlySpan<double> input,
        Span<double> output,
        GainConfig gainConfig)
    {
        ReadOnlySpan<double> source = input;

        if (gainConfig.IsEnabled)
        {
            EnsureBufferLength(ref _gainBuffer, input.Length);
            Span<double> gainOutput = _gainBuffer.AsSpan(0, input.Length);

            _gainProcessor.Apply(
                source,
                gainOutput,
                gainConfig.GainDb);

            source = gainOutput;
        }

        if (_delayProcessor is not null)
        {
            EnsureBufferLength(ref _delayBuffer, input.Length);
            Span<double> delayOutput = _delayBuffer.AsSpan(0, input.Length);

            _delayProcessor.ProcessFrame(
                source,
                delayOutput);

            source = delayOutput;
        }

        source.CopyTo(output);
    }

    private static void EnsureBufferLength(
        ref double[] buffer,
        int length)
    {
        if (buffer.Length < length)
        {
            buffer = new double[length];
        }
    }
}