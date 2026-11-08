using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.SignalProcessing;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Application;

public class SignalProcessingPipeline
{
    private readonly WaveformGenerator _waveformGenerator;
    private readonly SignalProcessor _signalProcessor;
    private readonly FrameProcessor _frameProcessor;

    public int CurrentStartIndex => _frameProcessor.CurrentStartIndex;

    public SignalProcessingPipeline(
        SineWaveGenerator sineWaveGenerator,
        UnitImpulseGenerator unitImpulseGenerator,
        GainProcessor gainProcessor)
    {
        _waveformGenerator = new WaveformGenerator(sineWaveGenerator, unitImpulseGenerator);
        _signalProcessor = new SignalProcessor(gainProcessor);
        _frameProcessor = new FrameProcessor();
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


