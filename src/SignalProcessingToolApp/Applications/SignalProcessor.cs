#nullable enable

using SignalProcessingToolApp;
using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain.SignalProcessing;

namespace SignalProcessingToolApp.Application;

public class SignalProcessor
{
    private readonly GainProcessor _gainProcessor;

    public SignalProcessor(GainProcessor gainProcessor)
    {
        _gainProcessor = gainProcessor ?? throw new ArgumentNullException(nameof(gainProcessor));
    }

    public void Process(
        ReadOnlySpan<double> input,
        Span<double> output,
        SignalProcessingConfig processingConfig)
    {
        if (processingConfig.GainConfig.IsEnabled)
        {
            _gainProcessor.Apply(
                input,
                output,
                processingConfig.GainConfig.GainDb);
        }
        else
        {
            input.CopyTo(output);
        }
    }
}