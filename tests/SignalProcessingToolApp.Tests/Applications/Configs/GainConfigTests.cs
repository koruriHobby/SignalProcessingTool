using SignalProcessingToolApp.Applications.Configs;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Configs;

public class GainConfigTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var config = new GainConfig(true, -6.0);

        Assert.True(config.IsEnabled);
        Assert.Equal(-6.0, config.GainDb);
    }
}
