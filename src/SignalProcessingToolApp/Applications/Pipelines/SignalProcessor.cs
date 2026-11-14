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

    public SignalProcessor(DelayConfig? delayConfig)
    {
        if (delayConfig is { IsEnabled: true })
        {
            _delayProcessor = new DelayProcessor(new DelayParameter(delayConfig.BufferSize, delayConfig.SampleCount));
        }
    }

    public void Process(
        ReadOnlySpan<double> input,
        Span<double> output,
        SignalProcessingConfig processingConfig)
    {
        ReadOnlySpan<double> source = input;

        if (processingConfig.GainConfig.IsEnabled)
        {
            EnsureBufferLength(ref _gainBuffer, input.Length);
            Span<double> gainOutput = _gainBuffer.AsSpan(0, input.Length);

            _gainProcessor.Apply(
                source,
                gainOutput,
                processingConfig.GainConfig.GainDb);

            source = gainOutput;
        }

        if (processingConfig.DelayConfig is { IsEnabled: true })
        {
            if (_delayProcessor is null)
            {
                throw new InvalidOperationException("ディレイが有効ですが、ディレイ設定なしでSignalProcessorが生成されています。");
            }

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