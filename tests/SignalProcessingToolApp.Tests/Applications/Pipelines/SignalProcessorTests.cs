using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.SignalProcessing;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Pipelines;

public class SignalProcessorTests
{
    [Fact]
    public void Process_WhenGainDisabled_CopiesInputToOutput()
    {
        var processor = new SignalProcessor(null);
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(false, -20.0)));

        Assert.Equal(input, output);
    }

    [Fact]
    public void Process_WhenGainEnabled_AppliesGain()
    {
        var processor = new SignalProcessor(null);
        double[] input = { 1.0, 2.0 };
        var output = new double[2];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(true, -20.0)));

        Assert.Equal(0.1, output[0], 10);
        Assert.Equal(0.2, output[1], 10);
    }

    [Fact]
    public void Process_WhenDelayEnabled_DelaysInput()
    {
        var processor = new SignalProcessor(new DelayConfig(true, 4, 2));
        double[] input = { 1.0, 2.0, 3.0, 4.0 };
        var output = new double[4];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(false, 0.0), new DelayConfig(true, 4, 2)));

        Assert.Equal(new double[] { 0.0, 0.0, 1.0, 2.0 }, output);
    }

    [Fact]
    public void Process_WhenDelayAndGainEnabled_AppliesGainBeforeDelay()
    {
        var processor = new SignalProcessor(new DelayConfig(true, 4, 1));
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(true, -20.0), new DelayConfig(true, 4, 1)));

        Assert.Equal(0.0, output[0], 10);
        Assert.Equal(0.1, output[1], 10);
        Assert.Equal(0.2, output[2], 10);
    }
}
