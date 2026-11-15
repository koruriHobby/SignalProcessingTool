using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;

namespace SignalProcessingToolApp.Applications.DTOs;

public record SignalExecutionRequest(
    WaveformType WaveformType,
    SineWaveConfig? SineWaveConfig,
    CommonConfig CommonConfig,
    SignalProcessingConfig ProcessingConfig
);