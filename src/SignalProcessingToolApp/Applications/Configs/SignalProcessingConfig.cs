#nullable enable

namespace SignalProcessingToolApp.Applications.Configs;

public record SignalProcessingConfig
{
    public GainConfig GainConfig { get; }
    public DelayConfig? DelayConfig { get; }
    public IirConfig? IirConfig { get; }

    public SignalProcessingConfig(
        GainConfig gainConfig,
        DelayConfig? delayConfig = null,
        IirConfig? iirConfig = null)
    {
        GainConfig = gainConfig ?? throw new ArgumentNullException(nameof(gainConfig));
        DelayConfig = delayConfig;
        IirConfig = iirConfig;
    }
}
