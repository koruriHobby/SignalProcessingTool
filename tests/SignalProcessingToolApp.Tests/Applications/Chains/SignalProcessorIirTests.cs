using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Domains.Parameters;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Chains;

public class SignalProcessorIirTests
{
    [Fact]
    public void Process_WhenIirEnabled_AppliesIirAfterDelay()
    {
        var processor = new SignalProcessor(
            new DelayParameter(4, 1),
            new IirParameter(0.0, 1.0, 0.0, 0.0, 0.0));
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.Process(input, output, new GainConfig(false, 0.0));

        Assert.Equal(new double[] { 0.0, 0.0, 1.0 }, output);
    }

    [Fact]
    public void Process_WhenIirIsNull_DoesNotChangeOutput()
    {
        var processor = new SignalProcessor(null, null);
        double[] input = { 1.0, 2.0, 3.0 };
        var output = new double[3];

        processor.Process(input, output, new GainConfig(false, 0.0));

        Assert.Equal(input, output);
    }
}
