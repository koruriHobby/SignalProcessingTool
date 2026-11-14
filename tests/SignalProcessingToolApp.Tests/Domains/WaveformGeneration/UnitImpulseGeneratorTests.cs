using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.WaveformGeneration;

public class UnitImpulseGeneratorTests
{
    [Fact]
    public void Generate_WithStartIndexZero_PlacesImpulseAtFirstSample()
    {
        var generator = new UnitImpulseGenerator();
        var output = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 4);

        generator.Generate(output, commonParameter, 0);

        Assert.Equal(new double[] { 1.0, 0.0, 0.0, 0.0 }, output);
    }

    [Fact]
    public void Generate_WithNonZeroStartIndex_ProducesNoImpulse()
    {
        var generator = new UnitImpulseGenerator();
        var output = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 8);

        generator.Generate(output, commonParameter, 4);

        Assert.Equal(new double[] { 0.0, 0.0, 0.0, 0.0 }, output);
    }

    [Fact]
    public void Generate_WithEmptyOutput_ThrowsArgumentException()
    {
        var generator = new UnitImpulseGenerator();
        var commonParameter = new CommonParameter(44100.0, 4, 4);

        var exception = Assert.Throws<ArgumentException>(() =>
            generator.Generate(Array.Empty<double>(), commonParameter, 0));

        Assert.Equal("output", exception.ParamName);
    }
}
