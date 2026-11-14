using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.Pipelines;

public class SignalProcessingPipeline
{
    private readonly WaveformGenerator _waveformGenerator;
    private readonly SignalProcessor _signalProcessor;
    private readonly FrameProcessor _frameProcessor;

    public int CurrentStartIndex => _frameProcessor.CurrentStartIndex;

    public SignalProcessingPipeline(
        WaveformGenerator waveformGenerator,
        SignalProcessor signalProcessor,
        FrameProcessor frameProcessor)
    {
        _waveformGenerator = waveformGenerator ?? throw new ArgumentNullException(nameof(waveformGenerator));
        _signalProcessor = signalProcessor ?? throw new ArgumentNullException(nameof(signalProcessor));
        _frameProcessor = frameProcessor ?? throw new ArgumentNullException(nameof(frameProcessor));
    }

    public void ExecuteBatch(
        Span<double> inputSignal,
        Span<double> processedSignal,
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        SignalProcessingConfig processingConfig)
    {
        _waveformGenerator.GenerateWaveform(
                inputSignal,
                waveformType,
                sineWaveParameter,
                commonParameter,
                startIndex: 0);

        _signalProcessor.Process(
            inputSignal,
            processedSignal,
            processingConfig);
    }

    public void ExecuteFrame(
        Span<double> inputSignal,
        Span<double> processedSignal,
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        SignalProcessingConfig processingConfig)
    {
        _waveformGenerator.GenerateWaveform(
            inputSignal,
            waveformType,
            sineWaveParameter,
            commonParameter,
            startIndex: _frameProcessor.CurrentStartIndex);

        _signalProcessor.Process(
            inputSignal,
            processedSignal,
            processingConfig);

        _frameProcessor.AdvanceFrame(commonParameter.FrameSize);
    }

    public bool HasNextFrame(CommonParameter commonParameter)
    {
        return _frameProcessor.HasNextFrame(commonParameter.DataSize);
    }

    public void Reset()
    {
        _frameProcessor.Reset();
    }
}


