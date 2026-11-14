using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.WaveformGeneration;

public class SineWaveParameterTests
{
    [Fact]
    public void Constructor_WithValidValues_SetsFrequency()
    {
        var parameter = new SineWaveParameter(1000.0, 44100.0);

        Assert.Equal(1000.0, parameter.Frequency);
    }

    [Fact]
    public void Constructor_WithNonPositiveFrequency_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new SineWaveParameter(0.0, 44100.0));

        Assert.Equal("frequency", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNonPositiveSamplingFrequency_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new SineWaveParameter(1000.0, 0.0));

        Assert.Equal("samplingFrequency", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenFrequencyReachesNyquist_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new SineWaveParameter(22050.0, 44100.0));

        Assert.Equal("frequency", exception.ParamName);
    }
}
