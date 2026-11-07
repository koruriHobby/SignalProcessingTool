#nullable enable

using Xunit;
using SignalProcessingToolApp.Domain;

namespace SignalProcessingToolApp.Tests.Domain;

public class CommonParameterTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange
        double samplingFrequency = 44100.0;
        int frameSize = 256;
        int dataSize = 1024;

        // Act
        var parameter = new CommonParameter(samplingFrequency, frameSize, dataSize);

        // Assert
        Assert.Equal(samplingFrequency, parameter.SamplingFrequency);
        Assert.Equal(frameSize, parameter.FrameSize);
        Assert.Equal(dataSize, parameter.DataSize);
    }

    [Fact]
    public void Constructor_WithInvalidFrameSize_ShouldThrowArgumentException()
    {
        // Arrange
        double samplingFrequency = 44100.0;
        int frameSize = 2048;
        int dataSize = 1024;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CommonParameter(samplingFrequency, frameSize, dataSize));
    }

    [Theory]
    [InlineData(-1.0, 256, 1024)]
    [InlineData(44100.0, -1, 1024)]
    [InlineData(44100.0, 256, -1)]
    public void Constructor_WithNegativeValues_ShouldThrowArgumentException(double samplingFrequency, int frameSize, int dataSize)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CommonParameter(samplingFrequency, frameSize, dataSize));
    }
}
