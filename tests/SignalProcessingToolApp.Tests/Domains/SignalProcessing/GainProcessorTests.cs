using SignalProcessingToolApp.Domains.SignalProcessing;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.SignalProcessing;

public class GainProcessorTests
{
    [Fact]
    public void Apply_WithZeroDecibel_KeepsInputUnchanged()
    {
        var processor = new GainProcessor();
        double[] input = { 1.0, -2.0, 3.0 };
        var output = new double[3];

        processor.Apply(input, output, 0.0);

        Assert.Equal(input, output);
    }

    [Fact]
    public void Apply_WithMinusTwentyDecibel_ScalesByTenth()
    {
        var processor = new GainProcessor();
        double[] input = { 1.0, 2.0 };
        var output = new double[2];

        processor.Apply(input, output, -20.0);

        Assert.Equal(0.1, output[0], 10);
        Assert.Equal(0.2, output[1], 10);
    }

    [Fact]
    public void Apply_WithMinusInfinityDecibel_ProducesZeros()
    {
        var processor = new GainProcessor();
        double[] input = { 1.0, 2.0 };
        var output = new double[2];

        processor.Apply(input, output, -200.0);

        Assert.Equal(new double[] { 0.0, 0.0 }, output);
    }

    [Fact]
    public void Apply_WithEmptyInput_ThrowsArgumentException()
    {
        var processor = new GainProcessor();

        var exception = Assert.Throws<ArgumentException>(() => processor.Apply(Array.Empty<double>(), new double[2], 0.0));

        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public void Apply_WithEmptyOutput_ThrowsArgumentException()
    {
        var processor = new GainProcessor();

        var exception = Assert.Throws<ArgumentException>(() => processor.Apply(new double[2], Array.Empty<double>(), 0.0));

        Assert.Equal("output", exception.ParamName);
    }

    [Fact]
    public void Apply_WhenOutputShorterThanInput_ThrowsArgumentException()
    {
        var processor = new GainProcessor();

        var exception = Assert.Throws<ArgumentException>(() => processor.Apply(new double[4], new double[2], 0.0));

        Assert.Equal("output", exception.ParamName);
    }
}
