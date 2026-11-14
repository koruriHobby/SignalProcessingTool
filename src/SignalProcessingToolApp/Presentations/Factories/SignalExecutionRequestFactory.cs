using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp.Presentations.Factories;

public class SignalExecutionRequestFactory
{
    private const int SineWaveIndex = 0;

    public SignalExecutionRequest Create(
        CommonParameterViewModel commonParameterVm,
        InputSignalViewModel inputSignalVm,
        SignalProcessingViewModel signalProcessingVm)
    {
        var commonParameter = CreateCommonParameter(commonParameterVm);
        var waveformType = CreateWaveformType(inputSignalVm);
        var sineWaveParameter = CreateSineWaveParameter(inputSignalVm, commonParameter);
        var processingConfig = CreateProcessingConfig(signalProcessingVm);

        return new SignalExecutionRequest(
            waveformType,
            sineWaveParameter,
            commonParameter,
            processingConfig);
    }

    private static CommonParameter CreateCommonParameter(CommonParameterViewModel viewModel)
    {
        double samplingFrequency = viewModel.SamplingFrequencies[viewModel.SamplingFrequencyIndex];
        int frameSize = viewModel.FrameSizes[viewModel.FrameSizeIndex];
        int dataSize = viewModel.DataSizes[viewModel.DataSizeIndex];

        return new CommonParameter(samplingFrequency, frameSize, dataSize);
    }

    private static WaveformType CreateWaveformType(InputSignalViewModel viewModel)
    {
        return viewModel.WaveformTypeIndex == SineWaveIndex ? WaveformType.SineWave : WaveformType.UnitImpulse;
    }

    private static SineWaveParameter? CreateSineWaveParameter(
        InputSignalViewModel viewModel,
        CommonParameter commonParameter)
    {
        if (CreateWaveformType(viewModel) != WaveformType.SineWave)
        {
            return null;
        }

        double frequency = viewModel.SineWaveFrequencies[viewModel.FrequencyIndex];
        return new SineWaveParameter(frequency, commonParameter.SamplingFrequency);
    }

    private static SignalProcessingConfig CreateProcessingConfig(SignalProcessingViewModel viewModel)
    {
        int gainDb = viewModel.GainValues[viewModel.GainIndex];
        var gainConfig = new GainConfig(viewModel.IsGainEnabled, gainDb);

        return new SignalProcessingConfig(gainConfig);
    }
}
