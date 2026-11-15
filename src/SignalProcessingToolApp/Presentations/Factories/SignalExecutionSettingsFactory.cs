using SignalProcessingToolApp.Applications.Settings;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp.Presentations.Factories;

public class SignalExecutionSettingsFactory
{
    private const int SineWaveIndex = 0;

    private readonly ConfigFactory _configFactory;

    public SignalExecutionSettingsFactory(ConfigFactory configFactory)
    {
        _configFactory = configFactory ?? throw new ArgumentNullException(nameof(configFactory));
    }

    public SignalExecutionSettings Create(
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

        return new SignalExecutionSettings(
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
