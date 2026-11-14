using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.ViewModels.SubViewModels;

public class CommonParameterViewModelTests
{
    [Fact]
    public void Initial_HasDefaultIndexes()
    {
        var viewModel = new CommonParameterViewModel();

        Assert.Equal(0, viewModel.SamplingFrequencyIndex);
        Assert.Equal(0, viewModel.FrameSizeIndex);
        Assert.Equal(2, viewModel.DataSizeIndex);
    }

    [Fact]
    public void Initial_ContainsSelectableValues()
    {
        var viewModel = new CommonParameterViewModel();

        Assert.Equal(new List<int> { 44100, 48000, 96000 }, viewModel.SamplingFrequencies);
        Assert.Equal(new List<int> { 8, 16, 32, 64, 128, 256 }, viewModel.FrameSizes);
        Assert.Equal(new List<int> { 8, 16, 32, 64, 128, 256, 512, 1024 }, viewModel.DataSizes);
    }
}
