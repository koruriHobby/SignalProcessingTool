#nullable enable

using Xunit;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Tests.Domain;

public class UnitImpulseGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnUnitImpulseWaveform()
    {
        // Arrange
        var generator = new UnitImpulseGenerator();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);

        // Act
        var waveform = generator.Generate(commonParameter, startIndex: 0);

        // Assert
        Assert.NotNull(waveform);
        Assert.Equal(256, waveform.Length);
        Assert.Equal(1.0, waveform[0]);  // First sample should be 1.0
        Assert.True(waveform.Skip(1).All(sample => sample == 0.0));  // Rest should be 0.0
    }

    [Fact]
    public void Generate_WithNonZeroStartIndex_ShouldReturnAllZeros()
    {
        // Arrange
        var generator = new UnitImpulseGenerator();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);

        // Act
        var waveform = generator.Generate(commonParameter, startIndex: 256);

        // Assert
        Assert.NotNull(waveform);
        Assert.Equal(256, waveform.Length);
        Assert.True(waveform.All(sample => sample == 0.0));
    }
}
