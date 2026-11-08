#nullable enable

using SignalProcessingToolApp;

namespace SignalProcessingToolApp.Application.Config;

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
