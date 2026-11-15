namespace SignalProcessingToolApp.Applications.Configs;

public record CommonConfig
{
    public double SamplingFrequency { get; }
    public int FrameSize { get; }
    public int DataSize { get; }

    public CommonConfig(
        double samplingFrequency,
        int frameSize,
        int dataSize)
    {
        SamplingFrequency = samplingFrequency;
        FrameSize = frameSize;
        DataSize = dataSize;
    }
}
