using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace SignalProcessingToolApp.Applications.Configs;

public record DelayConfig
{
    public bool IsEnabled { get; }
    public int BufferSize { get; }
    public int SampleCount { get; }

    public DelayConfig(
        bool isEnabled,
        int bufferSize,
        int sampleCount)
    {
        IsEnabled = isEnabled;
        BufferSize = bufferSize;
        SampleCount = sampleCount;
    }
}
