#nullable enable

namespace SignalProcessingToolApp.Applications.Configs;

public record GainConfig
{
    public bool IsEnabled { get; }
    public double GainDb { get; }

    public GainConfig(
        bool isEnabled,
        double gainDb)
    {
        IsEnabled = isEnabled;
        GainDb = gainDb;
    }
}
