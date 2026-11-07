#nullable enable

using Xunit;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Tests.Domain;

public class SineWaveGeneratorTests
{
    [Fact]
    public void Generate_WithValidParameters_ShouldReturnValidWaveform()
    {
        // Arrange
        var generator = new SineWaveGenerator();
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var commonParameter = new CommonParameter(44100.0, 256, 1024);

        // Act
        var waveform = generator.Generate(sineParameter, commonParameter, startIndex: 0);

        // Assert
        Assert.NotNull(waveform);
        Assert.Equal(256, waveform.Length);
        Assert.True(waveform.All(sample => sample >= -1.0 && sample <= 1.0));
    }

    [Fact]
    public void Generate_WithDifferentStartIndex_ShouldGenerateDifferentWaveform()
    {
        // Arrange
        var generator = new SineWaveGenerator();
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var commonParameter = new CommonParameter(44100.0, 256, 1024);

        // Act
        var waveform1 = generator.Generate(sineParameter, commonParameter, startIndex: 0);
        var waveform2 = generator.Generate(sineParameter, commonParameter, startIndex: 256);

        // Assert
        Assert.NotNull(waveform1);
        Assert.NotNull(waveform2);
        Assert.NotEqual(waveform1, waveform2);
    }
}
