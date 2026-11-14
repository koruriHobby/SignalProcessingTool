using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Presentations.Factories;
using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.Factories;

public class SignalExecutionRequestFactoryTests
{
    [Fact]
    public void Create_WithDefaultSineWaveViewModels_CreatesSineWaveRequest()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel();
        var signalProcessingVm = new SignalProcessingViewModel();

        var request = factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm);

        Assert.Equal(WaveformType.SineWave, request.WaveformType);
        Assert.Equal(44100.0, request.CommonParameter.SamplingFrequency);
        Assert.Equal(8, request.CommonParameter.FrameSize);
        Assert.Equal(32, request.CommonParameter.DataSize);
        Assert.NotNull(request.SineWaveParameter);
        Assert.Equal(1000.0, request.SineWaveParameter!.Frequency);
        Assert.False(request.ProcessingConfig.GainConfig.IsEnabled);
    }

    [Fact]
    public void Create_WithImpulseViewModel_CreatesUnitImpulseRequestWithoutSineWaveParameter()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel { WaveformTypeIndex = 1 };
        var signalProcessingVm = new SignalProcessingViewModel();

        var request = factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm);

        Assert.Equal(WaveformType.UnitImpulse, request.WaveformType);
        Assert.Null(request.SineWaveParameter);
    }

    [Fact]
    public void Create_WithGainEnabled_CreatesGainConfigFromSelectedValue()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel();
        var signalProcessingVm = new SignalProcessingViewModel { IsGainEnabled = true, GainIndex = 2 };

        var request = factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm);

        Assert.True(request.ProcessingConfig.GainConfig.IsEnabled);
        Assert.Equal(-6.0, request.ProcessingConfig.GainConfig.GainDb);
    }

    [Fact]
    public void Create_WithDelayDisabled_CreatesDisabledDelayConfig()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel();
        var signalProcessingVm = new SignalProcessingViewModel();

        var request = factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm);

        Assert.NotNull(request.ProcessingConfig.DelayConfig);
        Assert.False(request.ProcessingConfig.DelayConfig!.IsEnabled);
    }

    [Fact]
    public void Create_WithDelayEnabled_CreatesDelayConfigFromInputText()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel();
        var signalProcessingVm = new SignalProcessingViewModel
        {
            IsDelayEnabled = true,
            DelayBufferSizeText = "16",
            DelaySampleCountText = "4"
        };

        var request = factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm);

        Assert.True(request.ProcessingConfig.DelayConfig!.IsEnabled);
        Assert.Equal(16, request.ProcessingConfig.DelayConfig.BufferSize);
        Assert.Equal(4, request.ProcessingConfig.DelayConfig.SampleCount);
    }

    [Fact]
    public void Create_WithDelayEnabledAndNonIntegerText_ThrowsFormatException()
    {
        var factory = new SignalExecutionRequestFactory();
        var commonParameterVm = new CommonParameterViewModel();
        var inputSignalVm = new InputSignalViewModel();
        var signalProcessingVm = new SignalProcessingViewModel
        {
            IsDelayEnabled = true,
            DelayBufferSizeText = "abc",
            DelaySampleCountText = "4"
        };

        Assert.Throws<FormatException>(() => factory.Create(commonParameterVm, inputSignalVm, signalProcessingVm));
    }
}
