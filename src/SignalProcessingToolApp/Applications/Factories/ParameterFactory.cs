using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Domains.Filters;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
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

    public IirParameter? CreateIirParameter(
        IirConfig? iirConfig,
        CommonParameter commonParameter)
    {
        if (iirConfig is null || !iirConfig.IsEnabled)
        {
            return null;
        }

        return FilterDesigner.Design(
            commonParameter.SamplingFrequency,
            iirConfig.CutoffFrequency,
            iirConfig.FilterType);
    }

    public FilterCoefficientsDto CreateFilterCoefficientsDto(
        FilterType filterType,
        double samplingFrequency,
        double cutoffFrequency)
    {
        var iirParameter = FilterDesigner.Design(
            samplingFrequency,
            cutoffFrequency,
            filterType);

        return new FilterCoefficientsDto(
            iirParameter.B0,
            iirParameter.B1,
            iirParameter.B2,
            iirParameter.A1,
            iirParameter.A2);
    }
}
