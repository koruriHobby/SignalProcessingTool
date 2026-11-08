#nullable enable

using SignalProcessingToolApp;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Application;

public enum WaveformType
{
    SineWave,
    UnitImpulse
}

public class WaveformGenerator
{
    private readonly SineWaveGenerator _sineWaveGenerator;
    private readonly UnitImpulseGenerator _unitImpulseGenerator;

    public WaveformGenerator(
        SineWaveGenerator sineWaveGenerator,
        UnitImpulseGenerator unitImpulseGenerator)
    {
        _sineWaveGenerator = sineWaveGenerator ?? throw new ArgumentNullException(nameof(sineWaveGenerator));
        _unitImpulseGenerator = unitImpulseGenerator ?? throw new ArgumentNullException(nameof(unitImpulseGenerator));
    }

    public void GenerateWaveform(
        Span<double> output,
        WaveformType waveformType,
        SineWaveParameter? sineWaveParameter,
        CommonParameter commonParameter,
        int startIndex)
    {
        if (waveformType == WaveformType.SineWave && sineWaveParameter == null)
        {
            throw new ArgumentNullException(
                nameof(sineWaveParameter),
                "正弦波が選択されている場合、sineWaveParameter は null ではいけません。");
        }

        switch (waveformType)
        {
            case WaveformType.SineWave:
                _sineWaveGenerator.Generate(
                    output,
                    sineWaveParameter!,
                    commonParameter,
                    startIndex);
                break;
            case WaveformType.UnitImpulse:
                _unitImpulseGenerator.Generate(
                    output,
                    commonParameter,
                    startIndex);
                break;
            default:
                throw new ArgumentException(
                    $"不正な波形種別です。入力値: {waveformType}",
                    nameof(waveformType));
        }
    }
}