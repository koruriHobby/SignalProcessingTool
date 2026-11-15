using SignalProcessingToolApp.Domains.Filters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.SignalProcessing;

public class FilterDesignerTests
{
    [Fact]
    public void Design_WhenB0Check_ReturnsPassThroughCoefficients()
    {
        var parameter = FilterDesigner.Design(48000.0, 100.0, FilterType.B0Check);

        Assert.Equal(1.0, parameter.B0, 10);
        Assert.Equal(0.0, parameter.B1, 10);
        Assert.Equal(0.0, parameter.B2, 10);
        Assert.Equal(0.0, parameter.A1, 10);
        Assert.Equal(0.0, parameter.A2, 10);
    }

    [Fact]
    public void Design_WhenLpf_ReturnsSymmetricNumeratorAndDcGainOfOne()
    {
        var parameter = FilterDesigner.Design(48000.0, 1000.0, FilterType.Lpf);

        Assert.Equal(parameter.B0, parameter.B2, 10);
        double dcGain = (parameter.B0 + parameter.B1 + parameter.B2) / (1.0 - parameter.A1 - parameter.A2);
        Assert.Equal(1.0, dcGain, 6);
    }

    [Fact]
    public void Design_WhenHpf_ReturnsNumeratorWithZeroDcGain()
    {
        var parameter = FilterDesigner.Design(48000.0, 1000.0, FilterType.Hpf);

        double dcGain = (parameter.B0 + parameter.B1 + parameter.B2) / (1.0 - parameter.A1 - parameter.A2);
        Assert.Equal(0.0, dcGain, 10);
    }

    [Fact]
    public void Design_WhenLpfCutoffIsNegative_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FilterDesigner.Design(48000.0, -1.0, FilterType.Lpf));
    }

    [Fact]
    public void Design_WhenHpfCutoffIsNyquistOrHigher_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FilterDesigner.Design(48000.0, 24000.0, FilterType.Hpf));
    }
}
