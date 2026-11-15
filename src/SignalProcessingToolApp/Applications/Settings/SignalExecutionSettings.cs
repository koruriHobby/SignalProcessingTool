using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Chains;

namespace SignalProcessingToolApp.Applications.Settings;

public record SignalExecutionSettings(
    WaveformType WaveformType,
    SineWaveConfig? SineWaveConfig,
    CommonConfig CommonConfig,
    SignalProcessingConfig ProcessingConfig
);