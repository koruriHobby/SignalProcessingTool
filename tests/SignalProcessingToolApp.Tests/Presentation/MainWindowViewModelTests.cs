#nullable enable

using Xunit;
using SignalProcessingToolApp.Presentation.ViewModel;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;
using SignalProcessingToolApp.Application.Config;

namespace SignalProcessingToolApp.Tests.Presentation;

public class MainWindowViewModelTests
{
    private MainWindowViewModel CreateViewModel() => new MainWindowViewModel();

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultState()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(AppState.Initial, viewModel.CurrentState);
        Assert.False(viewModel.IsGainEnabled);
        Assert.Equal(0, viewModel.SamplingRateIndex);
        Assert.Equal(0, viewModel.FrameSizeIndex);
        Assert.Equal(2, viewModel.DataSizeIndex);
    }

    [Fact]
    public void IsParametersEnabled_WhenStateIsInitial_ShouldBeTrue()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.CurrentState = AppState.Initial;

        // Act
        var result = viewModel.IsParametersEnabled;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsParametersEnabled_WhenStateIsFrameProcessing_ShouldBeFalse()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.CurrentState = AppState.FrameProcessing;

        // Act
        var result = viewModel.IsParametersEnabled;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBatchProcessEnabled_WhenStateIsFrameProcessing_ShouldBeFalse()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.CurrentState = AppState.FrameProcessing;

        // Act
        var result = viewModel.IsBatchProcessEnabled;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBatchProcessEnabled_WhenStateIsInitial_ShouldBeTrue()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.CurrentState = AppState.Initial;

        // Act
        var result = viewModel.IsBatchProcessEnabled;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsWaveformSineWave_WhenWaveformTypeIndexIsZero_ShouldBeTrue()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.WaveformTypeIndex = 0;

        // Act
        var result = viewModel.IsWaveformSineWave;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsWaveformImpulse_WhenWaveformTypeIndexIsOne_ShouldBeTrue()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.WaveformTypeIndex = 1;

        // Act
        var result = viewModel.IsWaveformImpulse;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reset_ShouldResetStateAndClearWaveformData()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.CurrentState = AppState.Completed;
        viewModel.InputWaveformData = new double[] { 1.0, 2.0, 3.0 };
        viewModel.ProcessedWaveformData = new double[] { 1.0, 2.0, 3.0 };

        // Act
        viewModel.ResetCommand.Execute(null);

        // Assert
        Assert.Equal(AppState.Initial, viewModel.CurrentState);
        Assert.Null(viewModel.InputWaveformData);
        Assert.Null(viewModel.ProcessedWaveformData);
    }

    [Fact]
    public void SamplingRates_ShouldContainExpectedValues()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(new[] { 44100, 48000, 96000 }, viewModel.SamplingRates);
    }

    [Fact]
    public void FrameSizes_ShouldContainExpectedValues()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(new[] { 8, 16, 32, 64, 128, 256 }, viewModel.FrameSizes);
    }

    [Fact]
    public void DataSizes_ShouldContainExpectedValues()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(new[] { 8, 16, 32, 64, 128, 256, 512, 1024 }, viewModel.DataSizes);
    }

    [Fact]
    public void GainValues_ShouldContainExpectedValues()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(new[] { 0, -3, -6, -9, -12, -15, -18, -200 }, viewModel.GainValues);
    }
}
