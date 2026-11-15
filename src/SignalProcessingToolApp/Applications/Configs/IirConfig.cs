using SignalProcessingToolApp.Domains.Filters;

namespace SignalProcessingToolApp.Applications.Configs;

public record IirConfig
{
    public bool IsEnabled { get; }
    public FilterType FilterType { get; }
    public double CutoffFrequency { get; }

    public IirConfig(
        bool isEnabled,
        FilterType filterType,
        double cutoffFrequency)
    {
        IsEnabled = isEnabled;
        FilterType = filterType;
        CutoffFrequency = cutoffFrequency;
    }
}
