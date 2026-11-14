using SignalProcessingToolApp.Applications.Configs;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Configs;

public class SignalProcessingConfigTests
{
    [Fact]
    public void Constructor_SetsGainConfig()
    {
        var gainConfig = new GainConfig(false, 0.0);

        var config = new SignalProcessingConfig(gainConfig);

        Assert.Same(gainConfig, config.GainConfig);
    }

    [Fact]
    public void Constructor_WithNullGainConfig_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SignalProcessingConfig(null!));
    }
}
