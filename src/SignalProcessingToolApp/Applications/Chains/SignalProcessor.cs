#nullable enable

using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;

namespace SignalProcessingToolApp.Applications.Chains;

public class SignalProcessor
{
    private readonly GainProcessor _gainProcessor = new();
    private readonly DelayProcessor? _delayProcessor;
    private readonly IirProcessor? _iirProcessor;
    private double[] _gainBuffer = Array.Empty<double>();
    private double[] _delayBuffer = Array.Empty<double>();
    private double[] _iirBuffer = Array.Empty<double>();

    public SignalProcessor(
        DelayParameter? delayParameter,
        IirParameter? iirParameter = null)
    {
        if (delayParameter is not null)
        {
            _delayProcessor = new DelayProcessor(delayParameter);
        }

        if (iirParameter is not null)
        {
            _iirProcessor = new IirProcessor(iirParameter);
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

        if (_iirProcessor is not null)
        {
            EnsureBufferLength(ref _iirBuffer, input.Length);
            Span<double> iirOutput = _iirBuffer.AsSpan(0, input.Length);

            _iirProcessor.ProcessFrame(
                source,
                iirOutput);

            source = iirOutput;
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