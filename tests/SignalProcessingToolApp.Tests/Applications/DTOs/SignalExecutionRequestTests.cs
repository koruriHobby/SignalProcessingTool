using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.DTOs;

public class SignalExecutionRequestTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sineWaveParameter = new SineWaveParameter(1000.0, 44100.0);
        var commonParameter = new CommonParameter(44100.0, 8, 16);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));

        var request = new SignalExecutionRequest(
            WaveformType.SineWave,
            sineWaveParameter,
            commonParameter,
            processingConfig);

        Assert.Equal(WaveformType.SineWave, request.WaveformType);
        Assert.Same(sineWaveParameter, request.SineWaveParameter);
        Assert.Same(commonParameter, request.CommonParameter);
        Assert.Same(processingConfig, request.ProcessingConfig);
    }
}
