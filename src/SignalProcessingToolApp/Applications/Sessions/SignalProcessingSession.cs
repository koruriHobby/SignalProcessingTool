using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.DTOs;
using SignalProcessingToolApp.Applications.Pipelines;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;

namespace SignalProcessingToolApp.Applications.Sessions;

public class SignalProcessingSession
{
    private readonly SignalProcessingPipeline _pipeline;

    private double[]? _accumulatedInputWaveform;
    private double[]? _accumulatedProcessedWaveform;

    public bool IsInitialized => _accumulatedInputWaveform != null && _accumulatedProcessedWaveform != null;

    public SignalProcessingSession(SignalProcessingPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    /// <summary>
    /// 一括処理を実行し、結果の全データを返す
    /// </summary>
    public (double[] Input, double[] Processed) ExecuteBatch(SignalExecutionRequest request)
    {
        var inputBuffer = new double[request.CommonParameter.DataSize];
        var processedBuffer = new double[request.CommonParameter.DataSize];

        _pipeline.ExecuteBatch(
            inputBuffer,
            processedBuffer,
            request.WaveformType,
            request.SineWaveParameter,
            request.CommonParameter,
            request.ProcessingConfig
        );

        _pipeline.Reset();

        return (inputBuffer, processedBuffer);
    }

    /// <summary>
    /// フレーム処理を1ステップ進め、これまでの累積描画用データを返す
    /// </summary>
    public (double[] DisplayInput, double[] DisplayProcessed, bool IsCompleted) ExecuteNextFrame(SignalExecutionRequest request)
    {
        if (_pipeline.CurrentStartIndex == 0)
        {
            // 初期化
            _accumulatedInputWaveform = new double[request.CommonParameter.DataSize];
            _accumulatedProcessedWaveform = new double[request.CommonParameter.DataSize];
            _pipeline.Reset();
        }

        var accumulatedInput = _accumulatedInputWaveform ?? throw new InvalidOperationException("入力蓄積配列が初期化されていません。");
        var accumulatedProcessed = _accumulatedProcessedWaveform ?? throw new InvalidOperationException("処理後蓄積配列が初期化されていません。");

        int currentIndex = _pipeline.CurrentStartIndex;
        int frameSize = request.CommonParameter.FrameSize;

        Span<double> inputFrameSpan = accumulatedInput.AsSpan(currentIndex, frameSize);
        Span<double> processedFrameSpan = accumulatedProcessed.AsSpan(currentIndex, frameSize);

        _pipeline.ExecuteFrame(
            inputFrameSpan,
            processedFrameSpan,
            request.WaveformType,
            request.SineWaveParameter,
            request.CommonParameter,
            request.ProcessingConfig
        );

        int currentLength = _pipeline.CurrentStartIndex;

        // 途中経過のコピー作成
        double[] displayInput = new double[currentLength];
        double[] displayProcessed = new double[currentLength];

        Array.Copy(accumulatedInput, 0, displayInput, 0, currentLength);
        Array.Copy(accumulatedProcessed, 0, displayProcessed, 0, currentLength);

        bool hasNext = _pipeline.HasNextFrame(request.CommonParameter);
        if (!hasNext)
        {
            _pipeline.Reset();
        }

        return (displayInput, displayProcessed, IsCompleted: !hasNext);
    }

    public void Reset()
    {
        _pipeline.Reset();
        _accumulatedInputWaveform = null;
        _accumulatedProcessedWaveform = null;
    }
}