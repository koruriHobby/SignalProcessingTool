using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp.Presentations.Factories;

public class SignalExecutionRequestFactory
{
    private const int SineWaveIndex = 0;

    private readonly ConfigFactory _configFactory;

    public SignalExecutionRequestFactory(ConfigFactory configFactory)
    {
        _configFactory = configFactory ?? throw new ArgumentNullException(nameof(configFactory));
    }

    public SignalExecutionRequest Create(
        CommonParameterViewModel commonParameterViewModel,
        InputSignalViewModel inputSignalViewModel,
        SignalProcessingViewModel signalProcessingViewModel)
    {
        var commonConfig = _configFactory.CreateCommonConfig(commonParameterViewModel);
        var waveformType = CreateWaveformType(inputSignalViewModel);
        var sineWaveConfig = waveformType == WaveformType.SineWave
            ? _configFactory.CreateSineWaveConfig(inputSignalViewModel)
            : null;
        var processingConfig = _configFactory.CreateSignalProcessingConfig(signalProcessingViewModel);

        return new SignalExecutionRequest(
            waveformType,
            sineWaveConfig,
            commonConfig,
            processingConfig);
    }

    private static WaveformType CreateWaveformType(InputSignalViewModel inputSignalViewModel)
    {
        return inputSignalViewModel.WaveformTypeIndex == SineWaveIndex
            ? WaveformType.SineWave
            : WaveformType.UnitImpulse;
    }
}
