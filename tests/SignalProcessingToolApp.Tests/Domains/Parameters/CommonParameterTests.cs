using SignalProcessingToolApp.Domains.Parameters;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.Parameters;

public class CommonParameterTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var parameter = new CommonParameter(44100.0, 8, 16);

        Assert.Equal(44100.0, parameter.SamplingFrequency);
        Assert.Equal(8, parameter.FrameSize);
        Assert.Equal(16, parameter.DataSize);
    }

    [Fact]
    public void Constructor_WithNonPositiveSamplingFrequency_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CommonParameter(0.0, 8, 16));

        Assert.Equal("samplingFrequency", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNonPositiveFrameSize_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CommonParameter(44100.0, 0, 16));

        Assert.Equal("frameSize", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNonPositiveDataSize_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CommonParameter(44100.0, 8, 0));

        Assert.Equal("dataSize", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenFrameSizeExceedsDataSize_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CommonParameter(44100.0, 16, 8));

        Assert.Equal("frameSize", exception.ParamName);
    }
}
