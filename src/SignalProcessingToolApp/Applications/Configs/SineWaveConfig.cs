namespace SignalProcessingToolApp.Applications.Configs;

public record SineWaveConfig
{
    public double Frequency { get; }

    public SineWaveConfig(double frequency)
    {
        Frequency = frequency;
    }
}
