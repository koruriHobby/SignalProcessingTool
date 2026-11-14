using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Pipelines;

public class SignalProcessingPipelineTests
{
    private static SignalProcessingPipeline CreatePipeline()
    {
        return new SignalProcessingPipeline(
            new WaveformGenerator(new SineWaveGenerator(), new UnitImpulseGenerator()),
            new FrameProcessor());
    }

    [Fact]
    public void ExecuteBatch_GeneratesInputAndProcessedSignal()
    {
        var pipeline = CreatePipeline();
        var input = new double[4];
        var processed = new double[4];
        var commonParameter = new CommonParameter(44100.0, 4, 4);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));

        pipeline.ExecuteBatch(input, processed, WaveformType.UnitImpulse, null, commonParameter, processingConfig);

        Assert.Equal(new double[] { 1.0, 0.0, 0.0, 0.0 }, input);
        Assert.Equal(input, processed);
    }

    [Fact]
    public void ExecuteFrame_AdvancesStartIndexByFrameSize()
    {
        var pipeline = CreatePipeline();
        var input = new double[2];
        var processed = new double[2];
        var commonParameter = new CommonParameter(44100.0, 2, 4);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));

        pipeline.ExecuteFrame(input, processed, WaveformType.UnitImpulse, null, commonParameter, processingConfig);

        Assert.Equal(2, pipeline.CurrentStartIndex);
        Assert.True(pipeline.HasNextFrame(commonParameter));
    }

    [Fact]
    public void Reset_SetsStartIndexToZero()
    {
        var pipeline = CreatePipeline();
        var input = new double[2];
        var processed = new double[2];
        var commonParameter = new CommonParameter(44100.0, 2, 4);
        var processingConfig = new SignalProcessingConfig(new GainConfig(false, 0.0));
        pipeline.ExecuteFrame(input, processed, WaveformType.UnitImpulse, null, commonParameter, processingConfig);

        pipeline.Reset();

        Assert.Equal(0, pipeline.CurrentStartIndex);
    }
}
