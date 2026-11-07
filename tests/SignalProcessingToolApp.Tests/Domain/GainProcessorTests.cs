#nullable enable

using Xunit;
using SignalProcessingToolApp.Domain.SignalProcessing.Gain;

namespace SignalProcessingToolApp.Tests.Domain;

public class GainProcessorTests
{
    [Fact]
    public void Apply_With0dB_ShouldReturnSameWaveform()
    {
        // Arrange
        var processor = new GainProcessor();
        var inputWaveform = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 };

        // Act
        var result = processor.Apply(inputWaveform, 0.0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputWaveform.Length, result.Length);
        for (int i = 0; i < inputWaveform.Length; i++)
        {
            Assert.Equal(inputWaveform[i], result[i], precision: 6);
        }
    }

    [Fact]
    public void Apply_WithNegativeGain_ShouldReduceAmplitude()
    {
        // Arrange
        var processor = new GainProcessor();
        var inputWaveform = new double[] { 1.0, 1.0, 1.0 };

        // Act
        var result = processor.Apply(inputWaveform, -6.0);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, sample => Assert.True(sample < 1.0 && sample > 0.0));
    }

    [Fact]
    public void Apply_WithMinusInfinityGain_ShouldReturnZeros()
    {
        // Arrange
        var processor = new GainProcessor();
        var inputWaveform = new double[] { 1.0, 1.0, 1.0 };

        // Act
        var result = processor.Apply(inputWaveform, -200.0);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.All(sample => sample == 0.0));
    }
}
