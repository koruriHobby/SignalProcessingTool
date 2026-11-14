using SignalProcessingToolApp.Domains.Parameters;
using Xunit;

namespace SignalProcessingToolApp.Tests.Domains.Parameters;

public class DelayParameterTests
{
    [Fact]
    public void Constructor_WithValidValues_SetsProperties()
    {
        var parameter = new DelayParameter(8, 3);

        Assert.Equal(8, parameter.BufferSize);
        Assert.Equal(3, parameter.SampleCount);
    }

    [Fact]
    public void Constructor_WhenBufferSizeIsNotPositive_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DelayParameter(0, 0));
    }

    [Fact]
    public void Constructor_WhenSampleCountIsNegative_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DelayParameter(8, -1));
    }

    [Fact]
    public void Constructor_WhenSampleCountIsNotLessThanBufferSize_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DelayParameter(8, 8));
    }
}
