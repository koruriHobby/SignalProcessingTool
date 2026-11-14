using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.WaveformGeneration;

public class SineWaveGeneratorTests
{
    [Fact]
    public void Generate_FillsOutputWithSineWave()
    {
        var generator = new SineWaveGenerator();
        var output = new double[4];
        var parameter = new SineWaveParameter(1000.0, 8000.0);
        var commonParameter = new CommonParameter(8000.0, 4, 4);

        generator.Generate(output, parameter, commonParameter);

        for (int index = 0; index < output.Length; index++)
        {
            Assert.Equal(Math.Sin(index * Math.PI / 4), output[index], 10);
        }
    }

    [Fact]
    public void Generate_WithStartIndex_OffsetsPhase()
    {
        var generator = new SineWaveGenerator();
        var output = new double[2];
        var parameter = new SineWaveParameter(1000.0, 8000.0);
        var commonParameter = new CommonParameter(8000.0, 2, 2);

        generator.Generate(output, parameter, commonParameter, 1);

        Assert.Equal(Math.Sin(Math.PI / 4), output[0], 10);
        Assert.Equal(Math.Sin(2 * Math.PI / 4), output[1], 10);
    }

    [Fact]
    public void Generate_WithEmptyOutput_ThrowsArgumentException()
    {
        var generator = new SineWaveGenerator();
        var parameter = new SineWaveParameter(1000.0, 8000.0);
        var commonParameter = new CommonParameter(8000.0, 4, 4);

        var exception = Assert.Throws<ArgumentException>(() =>
            generator.Generate(Array.Empty<double>(), parameter, commonParameter));

        Assert.Equal("output", exception.ParamName);
    }
}
