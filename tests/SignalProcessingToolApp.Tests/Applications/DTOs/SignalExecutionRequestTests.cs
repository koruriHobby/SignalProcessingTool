using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.DTOs;

public class SignalExecutionRequestTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sineWaveConfig = new SineWaveConfig(1000.0);
        var commonConfig = new CommonConfig(44100.0, 8, 16);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));

        var request = new SignalExecutionRequest(
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
