using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Settings;
using SignalProcessingToolApp.Applications.Chains;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Settings;

public class SignalExecutionSettingsTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sineWaveConfig = new SineWaveConfig(1000.0);
        var commonConfig = new CommonConfig(44100.0, 8, 16);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));

        var request = new SignalExecutionSettings(
            WaveformType.SineWave,
            sineWaveConfig,
            commonConfig,
            processingConfig);

        Assert.Equal(WaveformType.SineWave, request.WaveformType);
        Assert.Same(sineWaveConfig, request.SineWaveConfig);
        Assert.Same(commonConfig, request.CommonConfig);
        Assert.Same(processingConfig, request.ProcessingConfig);
    }
}
