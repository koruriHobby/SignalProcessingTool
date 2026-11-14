using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.ViewModels.SubViewModels;

public class InputSignalViewModelTests
{
    [Fact]
    public void Initial_IsSineWaveSelected()
    {
        var viewModel = new InputSignalViewModel();

        Assert.True(viewModel.IsWaveformSineWave);
        Assert.False(viewModel.IsWaveformImpulse);
        Assert.Equal(1000, viewModel.SineWaveFrequencies[viewModel.FrequencyIndex]);
    }

    [Fact]
    public void WaveformTypeIndex_WhenImpulse_SelectsImpulse()
    {
        var viewModel = new InputSignalViewModel();

        viewModel.WaveformTypeIndex = 1;

        Assert.False(viewModel.IsWaveformSineWave);
        Assert.True(viewModel.IsWaveformImpulse);
    }

    [Fact]
    public void Initial_ContainsSelectableValues()
    {
        var viewModel = new InputSignalViewModel();

        Assert.Equal(new List<string> { "正弦波", "インパルス" }, viewModel.WaveformTypes);
        Assert.Equal(new List<int> { 100, 200, 500, 1000, 2000, 5000, 10000, 20000 }, viewModel.SineWaveFrequencies);
    }
}
