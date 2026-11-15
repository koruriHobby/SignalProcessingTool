using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.Factories;

public class ParameterFactory
{
    public CommonParameter CreateCommonParameter(CommonConfig commonConfig)
    {
        return new CommonParameter(commonConfig.SamplingFrequency, commonConfig.FrameSize, commonConfig.DataSize);
    }

    public SineWaveParameter? CreateSineWaveParameter(
        WaveformType waveformType,
        SineWaveConfig? sineWaveConfig,
        CommonParameter commonParameter)
    {
        if (waveformType != WaveformType.SineWave || sineWaveConfig is null)
        {
            return null;
        }

        return new SineWaveParameter(sineWaveConfig.Frequency, commonParameter.SamplingFrequency);
    }

    public DelayParameter? CreateDelayParameter(DelayConfig? delayConfig)
    {
        if (delayConfig is null || !delayConfig.IsEnabled)
        {
            return null;
        }

        return new DelayParameter(delayConfig.BufferSize, delayConfig.SampleCount);
    }
}
