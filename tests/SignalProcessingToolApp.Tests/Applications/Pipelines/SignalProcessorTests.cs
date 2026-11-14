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
        var processor = new SignalProcessor(new GainProcessor());
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(false, -20.0)));

        Assert.Equal(input, output);
    }

    [Fact]
    public void Process_WhenGainEnabled_AppliesGain()
    {
        var processor = new SignalProcessor(new GainProcessor());
        double[] input = { 1.0, 2.0 };
        var output = new double[2];

        processor.Process(input, output, new SignalProcessingConfig(new GainConfig(true, -20.0)));

        Assert.Equal(0.1, output[0], 10);
        Assert.Equal(0.2, output[1], 10);
    }

    [Fact]
    public void Constructor_WithNullGainProcessor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SignalProcessor(null!));
    }
}
