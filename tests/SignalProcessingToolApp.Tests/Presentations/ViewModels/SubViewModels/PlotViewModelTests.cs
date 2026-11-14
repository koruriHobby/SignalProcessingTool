using SignalProcessingToolApp.Presentations.ViewModels;
using Xunit;

namespace SignalProcessingToolApp.Tests.Presentations.ViewModels.SubViewModels;

public class PlotViewModelTests
{
    [Fact]
    public void Initial_HasNoData()
    {
        var viewModel = new PlotViewModel();

        Assert.Null(viewModel.InputWaveformData);
        Assert.Null(viewModel.ProcessedWaveformData);
    }

    [Fact]
    public void UpdateData_SetsInputAndProcessedData()
    {
        var viewModel = new PlotViewModel();
        double[] input = { 1.0, 0.0 };
        double[] processed = { 0.5, 0.0 };

        viewModel.UpdateData(input, processed);

        Assert.Same(input, viewModel.InputWaveformData);
        Assert.Same(processed, viewModel.ProcessedWaveformData);
    }

    [Fact]
    public void Clear_RemovesData()
    {
        var viewModel = new PlotViewModel();
        viewModel.UpdateData(new double[] { 1.0 }, new double[] { 1.0 });

        viewModel.Clear();

        Assert.Null(viewModel.InputWaveformData);
        Assert.Null(viewModel.ProcessedWaveformData);
    }
}
