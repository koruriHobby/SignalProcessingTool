using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Applications.Sessions;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Sessions;

public class SignalProcessingSessionTests
{
    private static SignalProcessingSession CreateSession()
    {
        var pipeline = new SignalProcessingPipeline(
            new WaveformGenerator(new SineWaveGenerator(), new UnitImpulseGenerator()),
            new SignalProcessor(new GainProcessor()),
            new FrameProcessor());

        return new SignalProcessingSession(pipeline);
    }

    private static SignalExecutionRequest CreateRequest(int frameSize, int dataSize)
    {
        return new SignalExecutionRequest(
            WaveformType.UnitImpulse,
            null,
            new CommonParameter(44100.0, frameSize, dataSize),
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
