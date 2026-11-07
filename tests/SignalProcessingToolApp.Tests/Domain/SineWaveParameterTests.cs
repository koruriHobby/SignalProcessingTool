#nullable enable

using Xunit;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Tests.Domain;

public class SineWaveParameterTests
{
    [Fact]
    public void Constructor_WithValidFrequency_ShouldInitialize()
    {
        // Arrange
        double frequency = 1000.0;
        double samplingFrequency = 44100.0;

        // Act
        var parameter = new SineWaveParameter(frequency, samplingFrequency);

        // Assert
        Assert.Equal(frequency, parameter.Frequency);
    }

    [Fact]
    public void Constructor_WithFrequencyAboveNyquist_ShouldThrowArgumentException()
    {
        // Arrange
        double frequency = 30000.0;  // > 44100/2
        double samplingFrequency = 44100.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SineWaveParameter(frequency, samplingFrequency));
    }

    [Theory]
    [InlineData(-1000.0, 44100.0)]
    [InlineData(1000.0, -44100.0)]
    public void Constructor_WithNegativeValues_ShouldThrowArgumentException(double frequency, double samplingFrequency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SineWaveParameter(frequency, samplingFrequency));
    }
}
