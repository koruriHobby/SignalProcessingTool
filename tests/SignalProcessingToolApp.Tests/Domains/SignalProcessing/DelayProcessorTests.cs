using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.SignalProcessing;

public class DelayProcessorTests
{
    [Fact]
    public void ProcessFrame_WhenSampleCountIsZero_CopiesInputToOutput()
    {
        var processor = new DelayProcessor(new DelayParameter(4, 0));
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.ProcessFrame(input, output);

        Assert.Equal(input, output);
    }

    [Fact]
    public void ProcessFrame_DelaysSamplesBySampleCount()
    {
        var processor = new DelayProcessor(new DelayParameter(4, 2));
        double[] input = { 1.0, 2.0, 3.0, 4.0 };
        var output = new double[4];

        processor.ProcessFrame(input, output);

        Assert.Equal(new double[] { 0.0, 0.0, 1.0, 2.0 }, output);
    }

    [Fact]
    public void ProcessFrame_KeepsStateAcrossFrames()
    {
        var processor = new DelayProcessor(new DelayParameter(4, 2));
        double[] firstInput = { 1.0, 2.0 };
        double[] secondInput = { 3.0, 4.0 };
        var output = new double[2];

        processor.ProcessFrame(firstInput, output);
        Assert.Equal(new double[] { 0.0, 0.0 }, output);

        processor.ProcessFrame(secondInput, output);
        Assert.Equal(new double[] { 1.0, 2.0 }, output);
    }

    [Fact]
    public void ProcessFrame_WithEmptyInput_ThrowsArgumentException()
    {
        var processor = new DelayProcessor(new DelayParameter(4, 2));

        Assert.Throws<ArgumentException>(() => processor.ProcessFrame(ReadOnlySpan<double>.Empty, new double[1]));
    }

    [Fact]
    public void ProcessFrame_WhenOutputShorterThanInput_ThrowsArgumentException()
    {
        var processor = new DelayProcessor(new DelayParameter(4, 2));
        double[] input = { 1.0, 2.0 };

        Assert.Throws<ArgumentException>(() => processor.ProcessFrame(input, new double[1]));
    }
}
