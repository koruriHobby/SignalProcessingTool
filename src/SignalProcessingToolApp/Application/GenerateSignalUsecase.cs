#nullable enable

using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.SignalProcessing.Gain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Application;

public class GenerateSignalUsecase
{
    private readonly SineWaveGenerator _sineWaveGenerator;
    private readonly UnitImpulseGenerator _unitImpulseGenerator;
    private readonly GainProcessor _gainProcessor;
    private int _currentStartIndex;
    private double[]? _lastProcessedWaveform;

    /// <summary>
    /// 現在のフレーム処理の開始インデックス
    /// </summary>
    public int CurrentStartIndex => _currentStartIndex;

    public GenerateSignalUsecase(
        SineWaveGenerator sineWaveGenerator,
        UnitImpulseGenerator unitImpulseGenerator,
        GainProcessor gainProcessor)
    {
        _sineWaveGenerator = sineWaveGenerator ?? throw new ArgumentNullException(nameof(sineWaveGenerator));
        _unitImpulseGenerator = unitImpulseGenerator ?? throw new ArgumentNullException(nameof(unitImpulseGenerator));
        _gainProcessor = gainProcessor ?? throw new ArgumentNullException(nameof(gainProcessor));
        _currentStartIndex = 0;
        _lastProcessedWaveform = null;
    }

    public double[] ExecuteBatch(
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        SignalProcessingConfig processingConfig)
    {
        double[] waveform = GenerateWaveform(
            waveformType,
            sineWaveParameter,
            commonParameter,
            startIndex: 0);

        return ApplySignalProcessing(
            waveform,
            processingConfig);
    }

    public double[] ExecuteFrame(
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        SignalProcessingConfig processingConfig)
    {
        double[] waveform = GenerateWaveform(
            waveformType,
            sineWaveParameter,
            commonParameter,
            startIndex: _currentStartIndex);

        _lastProcessedWaveform = ApplySignalProcessing(
            waveform,
            processingConfig);

        _currentStartIndex += commonParameter.FrameSize;

        return _lastProcessedWaveform;
    }

    public bool HasNextFrame(CommonParameter commonParameter)
    {
        return _currentStartIndex < commonParameter.DataSize;
    }

    public void Reset()
    {
        _currentStartIndex = 0;
        _lastProcessedWaveform = null;
    }

    private double[] GenerateWaveform(
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        int startIndex)
    {
        return waveformType switch
        {
            WaveformType.SineWave => GenerateSineWave(
                sineWaveParameter,
                commonParameter,
                startIndex),
            WaveformType.UnitImpulse => _unitImpulseGenerator.Generate(
                commonParameter,
                startIndex),
            _ => throw new ArgumentException(
                $"不正な波形種別です。入力値: {waveformType}",
                nameof(waveformType))
        };
    }

    private double[] GenerateSineWave(
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        int startIndex)
    {
        if (sineWaveParameter == null)
        {
            throw new ArgumentNullException(
                nameof(sineWaveParameter),
                "正弦波が選択されている場合、sineWaveParameter は null ではいけません。");
        }

        return _sineWaveGenerator.Generate(
            sineWaveParameter,
            commonParameter,
            startIndex);
    }

    private double[] ApplySignalProcessing(
        double[] waveform,
        SignalProcessingConfig processingConfig)
    {
        double[] result = waveform;

        if (processingConfig.GainConfig.IsEnabled)
        {
            result = _gainProcessor.Apply(
                result,
                processingConfig.GainConfig.GainDb);
        }

        return result;
    }
}

public enum WaveformType
{
    SineWave,
    UnitImpulse
}
