using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Settings;
using SignalProcessingToolApp.Applications.Factories;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.Services;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Services;

public class SignalProcessingServiceTests
{
    private static SignalProcessingService CreateSession()
    {
        var pipeline = new SignalProcessingChain(
            new WaveformGenerator(new SineWaveGenerator(), new UnitImpulseGenerator()),
            new FrameProcessor());

        return new SignalProcessingService(pipeline, new ParameterFactory());
    }

    private static SignalExecutionSettings CreateRequest(int frameSize, int dataSize)
    {
        return new SignalExecutionSettings(
            WaveformType.UnitImpulse,
            null,
            new CommonConfig(44100.0, frameSize, dataSize),
            new SignalProcessingConfig(new GainConfig(false, 0.0)));
    }

    [Fact]
    public void ExecuteBatch_ReturnsFullDataSize()
    {
        var session = CreateSession();

        var (input, processed) = session.ExecuteBatch(CreateRequest(4, 8));

        Assert.Equal(8, input.Length);
        Assert.Equal(8, processed.Length);
        Assert.Equal(1.0, input[0]);
    }

    [Fact]
    public void ExecuteNextFrame_ReturnsAccumulatedData()
    {
        var session = CreateSession();
        var request = CreateRequest(4, 8);

        var (displayInput, displayProcessed, isCompleted) = session.ExecuteNextFrame(request);

        Assert.Equal(4, displayInput.Length);
        Assert.Equal(4, displayProcessed.Length);
        Assert.False(isCompleted);
        Assert.True(session.IsInitialized);
    }

    [Fact]
    public void ExecuteNextFrame_ReturnsCompleted_WhenAllFramesProcessed()
    {
        var session = CreateSession();
        var request = CreateRequest(4, 8);
        session.ExecuteNextFrame(request);

        var (displayInput, _, isCompleted) = session.ExecuteNextFrame(request);

        Assert.Equal(8, displayInput.Length);
        Assert.True(isCompleted);
    }

    [Fact]
    public void Reset_ClearsInitializedState()
    {
        var session = CreateSession();
        session.ExecuteNextFrame(CreateRequest(4, 8));

        session.Reset();

        Assert.False(session.IsInitialized);
    }
}
