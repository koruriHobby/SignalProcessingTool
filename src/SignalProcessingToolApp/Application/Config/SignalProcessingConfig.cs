#nullable enable

namespace SignalProcessingToolApp.Application.Config;

public record SignalProcessingConfig
{
    public GainConfig GainConfig { get; }

    public SignalProcessingConfig(GainConfig gainConfig)
    {
        GainConfig = gainConfig ?? throw new ArgumentNullException(nameof(gainConfig));
    }
}
