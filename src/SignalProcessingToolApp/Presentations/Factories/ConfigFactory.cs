using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp.Presentations.Factories;

public class ConfigFactory
{
    private const int DisabledDelaySize = 0;

    public CommonConfig CreateCommonConfig(
        CommonParameterViewModel commonParameterViewModel)
    {
        double samplingFrequency = commonParameterViewModel.SamplingFrequencies[commonParameterViewModel.SamplingFrequencyIndex];
        int frameSize = commonParameterViewModel.FrameSizes[commonParameterViewModel.FrameSizeIndex];
        int dataSize = commonParameterViewModel.DataSizes[commonParameterViewModel.DataSizeIndex];

        return new CommonConfig(samplingFrequency, frameSize, dataSize);
    }

    public SineWaveConfig CreateSineWaveConfig(
        InputSignalViewModel inputSignalViewModel)
    {
        double frequency = inputSignalViewModel.SineWaveFrequencies[inputSignalViewModel.FrequencyIndex];
        return new SineWaveConfig(frequency);
    }

    public SignalProcessingConfig CreateSignalProcessingConfig(
        SignalProcessingViewModel signalProcessingViewModel)
    {
        int gainDb = signalProcessingViewModel.GainValues[signalProcessingViewModel.GainIndex];
        var gainConfig = new GainConfig(signalProcessingViewModel.IsGainEnabled, gainDb);
        var delayConfig = CreateDelayConfig(signalProcessingViewModel);

        return new SignalProcessingConfig(gainConfig, delayConfig);
    }

    private static DelayConfig CreateDelayConfig(
        SignalProcessingViewModel signalProcessingViewModel)
    {
        if (!signalProcessingViewModel.IsDelayEnabled)
        {
            return new DelayConfig(false, DisabledDelaySize, DisabledDelaySize);
        }

        return new DelayConfig(
            true,
            signalProcessingViewModel.ParseDelayBufferSize(),
            signalProcessingViewModel.ParseDelaySampleCount());
    }
}
