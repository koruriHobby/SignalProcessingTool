#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalProcessingToolApp.Application;
using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.SignalProcessing.Gain;
using SignalProcessingToolApp.Domain.WaveformGeneration;

namespace SignalProcessingToolApp.Presentation.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private const double DefaultSamplingFrequency = 44100.0;
    private const int DefaultFrameSize = 256;
    private const int DefaultDataSize = 1024;

    private readonly GenerateSignalUsecase _useCase;
    private CommonParameter? _commonParameter;
    private SineWaveParameter? _sineWaveParameter;
    private double[]? _accumulatedInputWaveform;
    private double[]? _accumulatedProcessedWaveform;
    private int _accumulatedSampleCount = 0;

    public event Action<string>? ErrorOccurred;

    [ObservableProperty]
    private AppState currentState = AppState.Initial;

    partial void OnCurrentStateChanged(AppState value)
    {
        OnPropertyChanged(nameof(IsParametersEnabled));
        OnPropertyChanged(nameof(IsBatchProcessEnabled));
    }

    [ObservableProperty]
    private int samplingRateIndex = 0;

    [ObservableProperty]
    private int frameSizeIndex = 0;

    [ObservableProperty]
    private int dataSizeIndex = 2;

    [ObservableProperty]
    private int waveformTypeIndex = 0;

    partial void OnWaveformTypeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsWaveformSineWave));
        OnPropertyChanged(nameof(IsWaveformImpulse));
        OnPropertyChanged(nameof(SineWaveFrequencyVisibility));

        // ラジオボタン変更時にパラメータを初期化するが、入力波形は生成しない
        // （仕様により、ボタン押下時のみグラフが更新される）
        if (_commonParameter == null)
        {
            // パラメータが未初期化の場合は初期値で初期化
            double samplingFrequency = SamplingRates[SamplingRateIndex];
            int frameSize = FrameSizes[FrameSizeIndex];
            int dataSize = DataSizes[DataSizeIndex];
            _commonParameter = new CommonParameter(samplingFrequency, frameSize, dataSize);

            // 正弦波の場合のみパラメータを設定
            if (IsWaveformSineWave)
            {
                double frequency = SineWaveFrequencies[FrequencyIndex];
                _sineWaveParameter = new SineWaveParameter(frequency, samplingFrequency);
            }
        }

        // グラフは自動更新しない（仕様により初期状態ではプロットされない）
    }

    [ObservableProperty]
    private int frequencyIndex = 3;  // デフォルト: 1000 Hz（インデックス3）

    partial void OnFrequencyIndexChanged(int value)
    {
        // 周波数が変更されたとき、_sineWaveParameterのみを更新
        // グラフは自動更新しない（仕様により、ボタン押下時のみ更新される）
        if (IsWaveformSineWave && _commonParameter != null)
        {
            double frequency = SineWaveFrequencies[FrequencyIndex];
            double samplingFrequency = SamplingRates[SamplingRateIndex];
            _sineWaveParameter = new SineWaveParameter(frequency, samplingFrequency);
        }
    }

    [ObservableProperty]
    private bool isGainEnabled = false;

    [ObservableProperty]
    private int gainIndex = 0;

    [ObservableProperty]
    private string statusMessage = "準備完了";

    [ObservableProperty]
    private double[]? inputWaveformData;

    [ObservableProperty]
    private double[]? processedWaveformData;

    public List<int> SamplingRates { get; } = new() { 44100, 48000, 96000 };
    public List<int> FrameSizes { get; } = new() { 8, 16, 32, 64, 128, 256 };
    public List<int> DataSizes { get; } = new() { 8, 16, 32, 64, 128, 256, 512, 1024 };
    public List<string> WaveformTypes { get; } = new() { "正弦波", "インパルス" };
    public List<int> GainValues { get; } = new() { 0, -3, -6, -9, -12, -15, -18, -200 };
    public List<int> SineWaveFrequencies { get; } = new() { 100, 200, 500, 1000, 2000, 5000, 10000, 20000 };

    public bool IsParametersEnabled => CurrentState == AppState.Initial || CurrentState == AppState.Completed;
    public bool IsBatchProcessEnabled => CurrentState != AppState.FrameProcessing;
    public bool IsWaveformSineWave => WaveformTypeIndex == 0;
    public bool IsWaveformImpulse => WaveformTypeIndex == 1;
    public System.Windows.Visibility SineWaveFrequencyVisibility => IsWaveformSineWave ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    public int CurrentDataSize => DataSizeIndex < DataSizes.Count ? DataSizes[DataSizeIndex] : DefaultDataSize;


    public MainWindowViewModel()
    {
        var sineWaveGenerator = new SineWaveGenerator();
        var unitImpulseGenerator = new UnitImpulseGenerator();
        var gainProcessor = new GainProcessor();

        _useCase = new GenerateSignalUsecase(
            sineWaveGenerator,
            unitImpulseGenerator,
            gainProcessor);
    }

    [RelayCommand]
    private void Reset()
    {
        try
        {
            _useCase.Reset();
            CurrentState = AppState.Initial;
            InputWaveformData = null;
            ProcessedWaveformData = null;
            _accumulatedInputWaveform = null;
            _accumulatedProcessedWaveform = null;
            _accumulatedSampleCount = 0;
            StatusMessage = "準備完了";
        }
        catch
        {
            StatusMessage = "準備完了";
            CurrentState = AppState.Initial;
        }
    }

    [RelayCommand]
    private void BatchProcess()
    {
        try
        {
            ValidateAndUpdateParameters();

            if (_commonParameter == null)
            {
                throw new InvalidOperationException("共通パラメータが設定されていません");
            }

            var processingConfig = CreateSignalProcessingConfig();

            var signalWaveform = _useCase.ExecuteBatch(
                GetWaveformType(),
                GetWaveformParameter(),
                _commonParameter,
                processingConfig);

            // 一括処理時は蓄積データをリセット
            _accumulatedInputWaveform = GenerateInputWaveform();
            _accumulatedProcessedWaveform = signalWaveform;

            InputWaveformData = _accumulatedInputWaveform;
            ProcessedWaveformData = _accumulatedProcessedWaveform;

            CurrentState = AppState.Completed;
            StatusMessage = "一括処理が完了しました";
            _useCase.Reset();
        }
        catch (Exception ex)
        {
            InputWaveformData = null;
            ProcessedWaveformData = null;
            _accumulatedInputWaveform = null;
            _accumulatedProcessedWaveform = null;
            _commonParameter = null;
            StatusMessage = "準備完了";
            CurrentState = AppState.Initial;
            ErrorOccurred?.Invoke($"入力値のエラー: {ex.Message}");
            _useCase.Reset();
        }
    }

    [RelayCommand]
    private void FrameProcess()
    {
        try
        {
            if (CurrentState == AppState.Initial || CurrentState == AppState.Completed)
            {
                ValidateAndUpdateParameters();

                if (_commonParameter == null)
                {
                    throw new InvalidOperationException("共通パラメータが設定されていません");
                }

                CurrentState = AppState.FrameProcessing;
                // フレーム処理開始時に蓄積データを初期化
                _accumulatedInputWaveform = new double[_commonParameter.DataSize];
                _accumulatedProcessedWaveform = new double[_commonParameter.DataSize];
                _accumulatedSampleCount = 0;
                _useCase.Reset();
            }

            if (_commonParameter == null)
            {
                throw new InvalidOperationException("共通パラメータが設定されていません");
            }

            var processingConfig = CreateSignalProcessingConfig();

            // ExecuteFrame 前のインデックスをキャプチャ（フレーム開始位置）
            int frameStartIndex = _useCase.CurrentStartIndex;

            var signalWaveform = _useCase.ExecuteFrame(
                GetWaveformType(),
                GetWaveformParameter(),
                _commonParameter,
                processingConfig);

            // フレーム処理時は、最後のフレームサイズ分のデータを蓄積データに追加
            int frameLength = _commonParameter.FrameSize;

            // 入力信号のフレーム分を蓄積データにコピー
            if (_accumulatedInputWaveform != null)
            {
                var frameInputWaveform = GenerateInputWaveformWithStartIndex(frameStartIndex);
                Array.Copy(frameInputWaveform, 0, _accumulatedInputWaveform, frameStartIndex, frameLength);
            }

            // 処理後の信号のフレーム分を蓄積データにコピー
            if (_accumulatedProcessedWaveform != null)
            {
                // signalWaveform は全体のデータサイズ長の配列であり、
                // フレーム開始位置にあたるデータはsignalWaveform[0]から始まる
                Array.Copy(signalWaveform, 0, _accumulatedProcessedWaveform, frameStartIndex, frameLength);
            }

            // 蓄積済みサンプル数を更新
            _accumulatedSampleCount = _useCase.CurrentStartIndex;

            // グラフには蓄積済みデータのみを表示（蓄積済みサンプル部分だけを切り出す）
            if (_accumulatedInputWaveform != null && _accumulatedProcessedWaveform != null && _accumulatedSampleCount > 0)
            {
                // プロット用に蓄積済み部分だけ抽出
                double[] displayInputWaveform = new double[_accumulatedSampleCount];
                double[] displayProcessedWaveform = new double[_accumulatedSampleCount];

                Array.Copy(_accumulatedInputWaveform, 0, displayInputWaveform, 0, _accumulatedSampleCount);
                Array.Copy(_accumulatedProcessedWaveform, 0, displayProcessedWaveform, 0, _accumulatedSampleCount);

                InputWaveformData = displayInputWaveform;
                ProcessedWaveformData = displayProcessedWaveform;
            }

            if (!_useCase.HasNextFrame(_commonParameter))
            {
                CurrentState = AppState.Completed;
                StatusMessage = "フレーム処理が完了しました";
                _useCase.Reset();
            }
            else
            {
                StatusMessage = $"フレーム処理中... ({_useCase.CurrentStartIndex} / {_commonParameter.DataSize} サンプル処理済み)";
            }
        }
        catch (Exception ex)
        {
            InputWaveformData = null;
            ProcessedWaveformData = null;
            _accumulatedInputWaveform = null;
            _accumulatedProcessedWaveform = null;
            _accumulatedSampleCount = 0;
            _commonParameter = null;
            StatusMessage = "準備完了";
            CurrentState = AppState.Initial;
            ErrorOccurred?.Invoke($"入力値のエラー: {ex.Message}");
            _useCase.Reset();
        }
    }

    private void ValidateAndUpdateParameters()
    {
        double samplingFrequency = SamplingRates[SamplingRateIndex];
        int frameSize = FrameSizes[FrameSizeIndex];
        int dataSize = DataSizes[DataSizeIndex];

        _commonParameter = new CommonParameter(
            samplingFrequency,
            frameSize,
            dataSize);

        if (GetWaveformType() == WaveformType.SineWave)
        {
            double frequency = SineWaveFrequencies[FrequencyIndex];
            _sineWaveParameter = new SineWaveParameter(
                frequency,
                samplingFrequency);
        }
    }

    private SignalProcessingConfig CreateSignalProcessingConfig()
    {
        int gainDb = GainValues[GainIndex];
        var gainConfig = new GainConfig(
            IsGainEnabled,
            gainDb);

        return new SignalProcessingConfig(gainConfig);
    }

    private WaveformType GetWaveformType()
    {
        return WaveformTypeIndex == 0 ? WaveformType.SineWave : WaveformType.UnitImpulse;
    }

    private SineWaveParameter? GetWaveformParameter()
    {
        return GetWaveformType() == WaveformType.SineWave ? _sineWaveParameter : null;
    }

    private double[] GenerateInputWaveform()
    {
        if (_commonParameter == null)
        {
            return Array.Empty<double>();
        }

        if (GetWaveformType() == WaveformType.SineWave)
        {
            if (_sineWaveParameter == null)
            {
                return Array.Empty<double>();
            }

            var generator = new SineWaveGenerator();
            return generator.Generate(
                _sineWaveParameter,
                _commonParameter,
                startIndex: 0);
        }
        else
        {
            var generator = new UnitImpulseGenerator();
            return generator.Generate(
                _commonParameter,
                startIndex: 0);
        }
    }

    private double[] GenerateInputWaveformWithStartIndex(int startIndex)
    {
        if (_commonParameter == null)
        {
            return Array.Empty<double>();
        }

        if (GetWaveformType() == WaveformType.SineWave)
        {
            if (_sineWaveParameter == null)
            {
                return Array.Empty<double>();
            }

            var generator = new SineWaveGenerator();
            return generator.Generate(
                _sineWaveParameter,
                _commonParameter,
                startIndex: startIndex);
        }
        else
        {
            var generator = new UnitImpulseGenerator();
            return generator.Generate(
                _commonParameter,
                startIndex: startIndex);
        }
    }
}
