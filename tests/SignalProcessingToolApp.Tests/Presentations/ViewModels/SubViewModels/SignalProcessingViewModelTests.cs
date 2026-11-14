using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.ViewModels.SubViewModels;

public class SignalProcessingViewModelTests
{
    [Fact]
    public void Initial_GainIsDisabledWithFirstValue()
    {
        var viewModel = new SignalProcessingViewModel();

        Assert.False(viewModel.IsGainEnabled);
        Assert.Equal(0, viewModel.GainIndex);
    }

    [Fact]
    public void Initial_ContainsSelectableGainValues()
    {
        var viewModel = new SignalProcessingViewModel();

        Assert.Equal(new List<int> { 0, -3, -6, -9, -12, -15, -18, -200 }, viewModel.GainValues);
    }
}
