#nullable enable

using Xunit;
using SignalProcessingToolApp.Application;
using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;
using SignalProcessingToolApp.Domain.SignalProcessing.Gain;

namespace SignalProcessingToolApp.Tests.Application;

public class GenerateSignalUsecaseTests
{
    private GenerateSignalUsecase CreateUsecase()
    {
        var sineWaveGenerator = new SineWaveGenerator();
        var unitImpulseGenerator = new UnitImpulseGenerator();
        var gainProcessor = new GainProcessor();

        return new GenerateSignalUsecase(sineWaveGenerator, unitImpulseGenerator, gainProcessor);
    }

    [Fact]
    public void ExecuteBatch_WithSineWave_ShouldGenerateCompleteWaveform()
    {
        // Arrange
        var usecase = CreateUsecase();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var gainConfig = new GainConfig(isEnabled: false, gainDb: 0.0);
        var processingConfig = new SignalProcessingConfig(gainConfig);

        // Act
        var processed = usecase.ExecuteBatch(
            WaveformType.SineWave,
            sineParameter,
            commonParameter,
            processingConfig);

        // Assert
        Assert.NotNull(processed);
        Assert.Equal(1024, processed.Length);
    }

    [Fact]
    public void ExecuteBatch_WithUnitImpulse_ShouldGenerateImpulseWaveform()
    {
        // Arrange
        var usecase = CreateUsecase();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);
        var gainConfig = new GainConfig(isEnabled: false, gainDb: 0.0);
        var processingConfig = new SignalProcessingConfig(gainConfig);

        // Act
        var processed = usecase.ExecuteBatch(
            WaveformType.UnitImpulse,
            null,
            commonParameter,
            processingConfig);

        // Assert
        Assert.NotNull(processed);
        Assert.Equal(1024, processed.Length);
    }

    [Fact]
    public void ExecuteBatch_WithGainEnabled_ShouldApplyGain()
    {
        // Arrange
        var usecase = CreateUsecase();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var gainConfig = new GainConfig(isEnabled: true, gainDb: -6.0);
        var processingConfig = new SignalProcessingConfig(gainConfig);
        var noGainConfig = new GainConfig(isEnabled: false, gainDb: 0.0);
        var noGainProcessingConfig = new SignalProcessingConfig(noGainConfig);

        // Act
        var input = usecase.ExecuteBatch(
            WaveformType.SineWave,
            sineParameter,
            commonParameter,
            noGainProcessingConfig);
        var processed = usecase.ExecuteBatch(
            WaveformType.SineWave,
            sineParameter,
            commonParameter,
            processingConfig);

        // Assert
        Assert.NotNull(input);
        Assert.NotNull(processed);
        // Check that processed waveform has reduced amplitude compared to input
        var inputMax = input.Max(x => Math.Abs(x));
        var processedMax = processed.Max(x => Math.Abs(x));
        Assert.True(processedMax < inputMax);
    }

    [Fact]
    public void ExecuteFrame_WithMultipleFrames_ShouldIterateCorrectly()
    {
        // Arrange
        var usecase = CreateUsecase();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var gainConfig = new GainConfig(isEnabled: false, gainDb: 0.0);
        var processingConfig = new SignalProcessingConfig(gainConfig);

        // Act
        var frames = new List<double[]>();
        while (usecase.HasNextFrame(commonParameter))
        {
            var processed = usecase.ExecuteFrame(
                WaveformType.SineWave,
                sineParameter,
                commonParameter,
                processingConfig);
            frames.Add(processed);
        }

        // Assert
        Assert.NotEmpty(frames);
        Assert.All(frames, frame =>
        {
            Assert.Equal(256, frame.Length);
        });
    }

    [Fact]
    public void Reset_ShouldResetFrameIndex()
    {
        // Arrange
        var usecase = CreateUsecase();
        var commonParameter = new CommonParameter(44100.0, 256, 1024);
        var sineParameter = new SineWaveParameter(1000.0, 44100.0);
        var gainConfig = new GainConfig(isEnabled: false, gainDb: 0.0);
        var processingConfig = new SignalProcessingConfig(gainConfig);

        // Act
        usecase.ExecuteFrame(
            WaveformType.SineWave,
            sineParameter,
            commonParameter,
            processingConfig);
        usecase.Reset();

        // Assert
        Assert.True(usecase.HasNextFrame(commonParameter));
    }
}
