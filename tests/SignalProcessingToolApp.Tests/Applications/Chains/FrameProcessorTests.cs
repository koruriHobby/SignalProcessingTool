using SignalProcessingToolApp.Applications.Pipelines;
using Xunit;

namespace SignalProcessingToolApp.Tests.Applications.Pipelines;

public class FrameProcessorTests
{
    [Fact]
    public void Constructor_InitializesStartIndexToZero()
    {
        var processor = new FrameProcessor();

        Assert.Equal(0, processor.CurrentStartIndex);
    }

    [Fact]
    public void AdvanceFrame_IncreasesStartIndexByFrameSize()
    {
        var processor = new FrameProcessor();

        processor.AdvanceFrame(8);

        Assert.Equal(8, processor.CurrentStartIndex);
    }

    [Fact]
    public void HasNextFrame_ReturnsFalse_WhenStartIndexReachesDataSize()
    {
        var processor = new FrameProcessor();
        processor.AdvanceFrame(8);

        Assert.True(processor.HasNextFrame(16));
        Assert.False(processor.HasNextFrame(8));
    }

    [Fact]
    public void Reset_SetsStartIndexToZero()
    {
        var processor = new FrameProcessor();
        processor.AdvanceFrame(8);

        processor.Reset();

        Assert.Equal(0, processor.CurrentStartIndex);
    }
}
