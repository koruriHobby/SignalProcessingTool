using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.Chains;

public class SignalProcessingChain
{
    private readonly WaveformGenerator _waveformGenerator;
    private readonly FrameProcessor _frameProcessor;
    private SignalProcessor? _signalProcessor;

    public int CurrentStartIndex => _frameProcessor.CurrentStartIndex;

    public SignalProcessingChain(
        WaveformGenerator waveformGenerator,
        FrameProcessor frameProcessor)
    {
        _waveformGenerator = waveformGenerator ?? throw new ArgumentNullException(nameof(waveformGenerator));
        _frameProcessor = frameProcessor ?? throw new ArgumentNullException(nameof(frameProcessor));
    }

    public void ExecuteBatch(
        Span<double> inputSignal,
        Span<double> processedSignal,
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        GainConfig gainConfig,
        DelayParameter? delayParameter)
    {
        _waveformGenerator.GenerateWaveform(
                inputSignal,
                waveformType,
                sineWaveParameter,
                commonParameter,
                startIndex: 0);

        var signalProcessor = new SignalProcessor(delayParameter);
        signalProcessor.Process(
            inputSignal,
            processedSignal,
            gainConfig);
    }

    public void ExecuteFrame(
        Span<double> inputSignal,
        Span<double> processedSignal,
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        GainConfig gainConfig,
        DelayParameter? delayParameter)
    {
        _waveformGenerator.GenerateWaveform(
            inputSignal,
            waveformType,
            sineWaveParameter,
            commonParameter,
            startIndex: _frameProcessor.CurrentStartIndex);

        _signalProcessor ??= new SignalProcessor(delayParameter);
        _signalProcessor.Process(
            inputSignal,
            processedSignal,
            gainConfig);

        _frameProcessor.AdvanceFrame(commonParameter.FrameSize);
    }

    public bool HasNextFrame(CommonParameter commonParameter)
    {
        return _frameProcessor.HasNextFrame(commonParameter.DataSize);
    }

    public void Reset()
    {
        _signalProcessor = null;
        _frameProcessor.Reset();
    }
}


