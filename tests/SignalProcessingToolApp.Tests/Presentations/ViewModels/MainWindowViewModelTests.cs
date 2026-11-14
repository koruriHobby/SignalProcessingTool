using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Applications.Sessions;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using SignalProcessingToolApp.Presentations.Factories;
using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.ViewModels;

public class MainWindowViewModelTests
{
    private static MainWindowViewModel CreateViewModel()
    {
        var pipeline = new SignalProcessingPipeline(
            new WaveformGenerator(new SineWaveGenerator(), new UnitImpulseGenerator()),
            new SignalProcessor(new GainProcessor()),
            new FrameProcessor());

        return new MainWindowViewModel(new SignalProcessingSession(pipeline), new SignalExecutionRequestFactory());
    }

    [Fact]
    public void Initial_StatusIsReadyAndDataSizeIsDefault()
    {
        var viewModel = CreateViewModel();

        Assert.Equal("準備完了", viewModel.StatusMessage);
        Assert.Equal(32, viewModel.CurrentDataSize);
        Assert.True(viewModel.IsParametersEnabled);
    }

    [Fact]
    public void BatchProcessCommand_UpdatesPlotAndCompletesStatus()
    {
        var viewModel = CreateViewModel();

        viewModel.BatchProcessCommand.Execute(null);

        Assert.Equal("処理完了", viewModel.StatusMessage);
        Assert.Equal(32, viewModel.PlotVm.InputWaveformData!.Length);
        Assert.Equal(32, viewModel.PlotVm.ProcessedWaveformData!.Length);
    }

    [Fact]
    public void FrameProcessCommand_UpdatesPlotAndFrameProcessingStatus()
    {
        var viewModel = CreateViewModel();

        viewModel.FrameProcessCommand.Execute(null);

        Assert.Equal("フレーム処理中", viewModel.StatusMessage);
        Assert.Equal(8, viewModel.PlotVm.InputWaveformData!.Length);
        Assert.False(viewModel.IsBatchProcessEnabled);
    }

    [Fact]
    public void ResetCommand_ClearsPlotAndReturnsToInitialStatus()
    {
        var viewModel = CreateViewModel();
        viewModel.FrameProcessCommand.Execute(null);

        viewModel.ResetCommand.Execute(null);

        Assert.Equal("準備完了", viewModel.StatusMessage);
        Assert.Null(viewModel.PlotVm.InputWaveformData);
        Assert.Null(viewModel.PlotVm.ProcessedWaveformData);
    }

    [Fact]
    public void FrameProcessCommand_WhenParametersInvalid_RaisesErrorAndResets()
    {
        var viewModel = CreateViewModel();
        viewModel.CommonParamVm.FrameSizeIndex = 5;
        viewModel.CommonParamVm.DataSizeIndex = 0;
        string? errorMessage = null;
        viewModel.ErrorOccurred += message => errorMessage = message;

        viewModel.FrameProcessCommand.Execute(null);

        Assert.NotNull(errorMessage);
        Assert.StartsWith("入力値のエラー", errorMessage);
        Assert.Equal("準備完了", viewModel.StatusMessage);
    }
}
