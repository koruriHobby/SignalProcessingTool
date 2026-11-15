using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Pipelines;

public class WaveformGeneratorTests
{
    private static WaveformGenerator CreateGenerator()
    {
        return new WaveformGenerator(new SineWaveGenerator(), new UnitImpulseGenerator());
    }

    [Fact]
    public void GenerateWaveform_UnitImpulse_GeneratesImpulseAtStart()
    {
        var generator = CreateGenerator();
        var output = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 4);

        generator.GenerateWaveform(output, WaveformType.UnitImpulse, null, commonParameter, 0);

        Assert.Equal(new double[] { 1.0, 0.0, 0.0, 0.0 }, output);
    }

    [Fact]
    public void GenerateWaveform_SineWave_GeneratesSineWave()
    {
        var generator = CreateGenerator();
        var output = new double[4];
        var sineWaveParameter = new SineWaveParameter(1000.0, 8000.0);
        var commonParameter = new CommonParameter(8000.0, 4, 4);

        generator.GenerateWaveform(output, WaveformType.SineWave, sineWaveParameter, commonParameter, 0);

        for (int index = 0; index < output.Length; index++)
        {
            Assert.Equal(Math.Sin(index * Math.PI / 4), output[index], 10);
        }
    }

    [Fact]
    public void GenerateWaveform_SineWaveWithNullParameter_ThrowsArgumentNullException()
    {
        var generator = CreateGenerator();
        var output = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 4);

        Assert.Throws<ArgumentNullException>(() =>
            generator.GenerateWaveform(output, WaveformType.SineWave, null, commonParameter, 0));
    }

    [Fact]
    public void GenerateWaveform_InvalidWaveformType_ThrowsArgumentException()
    {
        var generator = CreateGenerator();
        var output = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 4);

        Assert.Throws<ArgumentException>(() =>
            generator.GenerateWaveform(output, (WaveformType)int.MaxValue, null, commonParameter, 0));
    }
}
