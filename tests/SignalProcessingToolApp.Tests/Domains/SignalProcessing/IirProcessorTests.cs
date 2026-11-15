using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.SignalProcessing;

public class IirProcessorTests
{
    [Fact]
    public void ProcessFrame_WithThroughCoefficients_CopiesInputToOutput()
    {
        var processor = new IirProcessor(new IirParameter(1.0, 0.0, 0.0, 0.0, 0.0));
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.ProcessFrame(input, output);

        Assert.Equal(input, output);
    }

    [Fact]
    public void ProcessFrame_WithOneSampleDelayCoefficients_DelaysByOneSample()
    {
        var processor = new IirProcessor(new IirParameter(0.0, 1.0, 0.0, 0.0, 0.0));
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.ProcessFrame(input, output);

        Assert.Equal(new double[] { 0.0, 1.0, 2.0 }, output);
    }

    [Fact]
    public void ProcessFrame_KeepsStateAcrossFrames()
    {
        var processor = new IirProcessor(new IirParameter(0.0, 0.0, 1.0, 0.0, 0.0));
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
        var processor = new IirProcessor(new IirParameter(1.0, 0.0, 0.0, 0.0, 0.0));

        Assert.Throws<ArgumentException>(() => processor.ProcessFrame(ReadOnlySpan<double>.Empty, new double[1]));
    }

    [Fact]
    public void ProcessFrame_WhenOutputShorterThanInput_ThrowsArgumentException()
    {
        var processor = new IirProcessor(new IirParameter(1.0, 0.0, 0.0, 0.0, 0.0));
        double[] input = { 1.0, 2.0 };

        Assert.Throws<ArgumentException>(() => processor.ProcessFrame(input, new double[1]));
    }
}
