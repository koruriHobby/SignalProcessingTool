using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Settings;
using SignalProcessingToolApp.Applications.Factories;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Domains.Filters;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.Services;

public class SignalProcessingService
{
    private readonly SignalProcessingChain _pipeline;
    private readonly ParameterFactory _parameterFactory;

    private double[]? _accumulatedInputWaveform;
    private double[]? _accumulatedProcessedWaveform;

    public bool IsInitialized => _accumulatedInputWaveform != null && _accumulatedProcessedWaveform != null;

    public SignalProcessingService(
        SignalProcessingChain pipeline,
        ParameterFactory parameterFactory)
    {
        _pipeline = pipeline;
        _parameterFactory = parameterFactory ?? throw new ArgumentNullException(nameof(parameterFactory));
    }

    /// <summary>
    /// 一括処理を実行し、結果の全データを返す
    /// </summary>
    public (double[] Input, double[] Processed) ExecuteBatch(SignalExecutionSettings request)
    {
        var commonParameter = _parameterFactory.CreateCommonParameter(request.CommonConfig);
        var sineWaveParameter = _parameterFactory.CreateSineWaveParameter(request.WaveformType, request.SineWaveConfig, commonParameter);
        var delayParameter = _parameterFactory.CreateDelayParameter(request.ProcessingConfig.DelayConfig);
        var iirParameter = _parameterFactory.CreateIirParameter(request.ProcessingConfig.IirConfig, commonParameter);

        var inputBuffer = new double[commonParameter.DataSize];
        var processedBuffer = new double[commonParameter.DataSize];

        _pipeline.ExecuteBatch(
            inputBuffer,
            processedBuffer,
            request.WaveformType,
            sineWaveParameter,
            commonParameter,
            request.ProcessingConfig.GainConfig,
            delayParameter,
            iirParameter
        );

        _pipeline.Reset();

        return (inputBuffer, processedBuffer);
    }

    /// <summary>
    /// フレーム処理を1ステップ進め、これまでの累積描画用データを返す
    /// </summary>
    public (double[] DisplayInput, double[] DisplayProcessed, bool IsCompleted) ExecuteNextFrame(SignalExecutionSettings request)
    {
        var commonParameter = _parameterFactory.CreateCommonParameter(request.CommonConfig);
        var sineWaveParameter = _parameterFactory.CreateSineWaveParameter(request.WaveformType, request.SineWaveConfig, commonParameter);
        var delayParameter = _parameterFactory.CreateDelayParameter(request.ProcessingConfig.DelayConfig);
        var iirParameter = _parameterFactory.CreateIirParameter(request.ProcessingConfig.IirConfig, commonParameter);

        if (_pipeline.CurrentStartIndex == 0)
        {
            // 初期化
            _accumulatedInputWaveform = new double[commonParameter.DataSize];
            _accumulatedProcessedWaveform = new double[commonParameter.DataSize];
            _pipeline.Reset();
        }

        var accumulatedInput = _accumulatedInputWaveform ?? throw new InvalidOperationException("入力蓄積配列が初期化されていません。");
        var accumulatedProcessed = _accumulatedProcessedWaveform ?? throw new InvalidOperationException("処理後蓄積配列が初期化されていません。");

        int currentIndex = _pipeline.CurrentStartIndex;
        int frameSize = commonParameter.FrameSize;

        Span<double> inputFrameSpan = accumulatedInput.AsSpan(currentIndex, frameSize);
        Span<double> processedFrameSpan = accumulatedProcessed.AsSpan(currentIndex, frameSize);

        _pipeline.ExecuteFrame(
            inputFrameSpan,
            processedFrameSpan,
            request.WaveformType,
            sineWaveParameter,
            commonParameter,
            request.ProcessingConfig.GainConfig,
            delayParameter,
            iirParameter
        );

        int currentLength = _pipeline.CurrentStartIndex;

        // 途中経過のコピー作成
        double[] displayInput = new double[currentLength];
        double[] displayProcessed = new double[currentLength];

        Array.Copy(accumulatedInput, 0, displayInput, 0, currentLength);
        Array.Copy(accumulatedProcessed, 0, displayProcessed, 0, currentLength);

        bool hasNext = _pipeline.HasNextFrame(commonParameter);
        if (!hasNext)
        {
            _pipeline.Reset();
        }

        return (displayInput, displayProcessed, IsCompleted: !hasNext);
    }

    public FilterCoefficientsDto CalculateFilterCoefficients(
        FilterType filterType,
        double samplingFrequency,
        double cutoffFrequency)
    {
        return _parameterFactory.CreateFilterCoefficientsDto(filterType, samplingFrequency, cutoffFrequency);
    }

    public void Reset()
    {
        _pipeline.Reset();
        _accumulatedInputWaveform = null;
        _accumulatedProcessedWaveform = null;
    }
}