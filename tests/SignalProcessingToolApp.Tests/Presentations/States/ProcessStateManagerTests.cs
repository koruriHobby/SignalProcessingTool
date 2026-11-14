using SignalProcessingToolApp.Presentations.States;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.States;

public class ProcessStateManagerTests
{
    [Fact]
    public void Initial_StatusIsReadyAndControlsAreEnabled()
    {
        var stateManager = new ProcessStateManager();

        Assert.Equal("準備完了", stateManager.StatusMessage);
        Assert.True(stateManager.IsParametersEnabled);
        Assert.True(stateManager.IsBatchProcessEnabled);
    }

    [Fact]
    public void UpdateFrameState_WhenNotCompleted_DisablesParametersAndBatchProcess()
    {
        var stateManager = new ProcessStateManager();

        stateManager.UpdateFrameState(false);

        Assert.Equal("フレーム処理中", stateManager.StatusMessage);
        Assert.False(stateManager.IsParametersEnabled);
        Assert.False(stateManager.IsBatchProcessEnabled);
    }

    [Fact]
    public void UpdateFrameState_WhenCompleted_SetsCompletedState()
    {
        var stateManager = new ProcessStateManager();
        stateManager.UpdateFrameState(false);

        stateManager.UpdateFrameState(true);

        Assert.Equal("処理完了", stateManager.StatusMessage);
        Assert.True(stateManager.IsParametersEnabled);
        Assert.True(stateManager.IsBatchProcessEnabled);
    }

    [Fact]
    public void CompleteBatch_SetsCompletedState()
    {
        var stateManager = new ProcessStateManager();

        stateManager.CompleteBatch();

        Assert.Equal("処理完了", stateManager.StatusMessage);
    }

    [Fact]
    public void Reset_ReturnsToInitialState()
    {
        var stateManager = new ProcessStateManager();
        stateManager.CompleteBatch();

        stateManager.Reset();

        Assert.Equal("準備完了", stateManager.StatusMessage);
    }
}
