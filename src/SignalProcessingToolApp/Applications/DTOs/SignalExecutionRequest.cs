using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.DTOs;

public record SignalExecutionRequest(
    WaveformType WaveformType,
    SineWaveParameter? SineWaveParameter,
    CommonParameter CommonParameter,
    SignalProcessingConfig ProcessingConfig
);