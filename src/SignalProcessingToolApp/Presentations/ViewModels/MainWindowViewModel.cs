#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalProcessingToolApp.Application;
using SignalProcessingToolApp.Application.Config;
using SignalProcessingToolApp.Domain;
using SignalProcessingToolApp.Domain.SignalProcessing;
using SignalProcessingToolApp.Domain.WaveformGeneration;
using System.Windows.Input;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public enum AppState
{
    Initial,
    FrameProcessing,
    Completed
}

public partial class MainWindowViewModel : ObservableObject
{
    private readonly SignalProcessingPipeline _useCase;

    public CommonParameterViewModel CommonParamVm { get; } = new();
    public InputSignalViewModel WaveformVm { get; } = new();
    public SignalProcessingViewModel SignalConfigVm { get; } = new();
    public PlotViewModel PlotVm { get; } = new();

    private double[]? _accumulatedInputWaveform;
    private double[]? _accumulatedProcessedWaveform;

    public event Action<string>? ErrorOccurred;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsParametersEnabled))]
    [NotifyPropertyChangedFor(nameof(IsBatchProcessEnabled))]
    [NotifyCanExecuteChangedFor(nameof(BatchProcessCommand))]
    [NotifyCanExecuteChangedFor(nameof(FrameProcessCommand))]
    [NotifyPropertyChangedFor(nameof(StatusMessage))]
    private AppState _currentState = AppState.Initial;

    public string StatusMessage => CurrentState switch
    {
        AppState.Initial => "準備完了",
        AppState.Completed => "処理完了",
        AppState.FrameProcessing => "フレーム処理中",
        _ => "準備完了"
    };

    public bool IsParametersEnabled => CurrentState == AppState.Initial || CurrentState == AppState.Completed;
    public bool IsBatchProcessEnabled => CurrentState != AppState.FrameProcessing;

    public MainWindowViewModel()
    {
        _useCase = new SignalProcessingPipeline(
            new SineWaveGenerator(),
            new UnitImpulseGenerator(),
            new GainProcessor());
    }

    [RelayCommand]
    private void Reset()
    {
        try
        {
            _useCase.Reset();
            CurrentState = AppState.Initial;
            PlotVm.Clear();
            _accumulatedInputWaveform = null;
            _accumulatedProcessedWaveform = null;
        }
        catch
        {
            CurrentState = AppState.Initial;
        }
    }

    [RelayCommand(CanExecute = nameof(IsBatchProcessEnabled))]
    private void BatchProcess()
    {
        try
        {
            //パラメータ生成
            var commonParam = CommonParamVm.CreateParameter();
            var sineParam = WaveformVm.CreateSineWaveParameter(commonParam.SamplingFrequency);
            var processingConfig = SignalConfigVm.CreateConfig();

            //配列生成
            var inputBuffer = new double[commonParam.DataSize];
            var processedBuffer = new double[commonParam.DataSize];

            //一括処理実行
            _useCase.ExecuteBatch(
                inputBuffer,
                processedBuffer,
                WaveformVm.GetWaveformType(),
                sineParam,
                commonParam,
                processingConfig);

            //プロット用の配列に格納
            _accumulatedInputWaveform = inputBuffer;
            _accumulatedProcessedWaveform = processedBuffer;

            //プロット更新
            PlotVm.UpdateData(_accumulatedInputWaveform, _accumulatedProcessedWaveform);

            //状態更新
            CurrentState = AppState.Completed;
            _useCase.Reset();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    [RelayCommand]
    private void FrameProcess()
    {
        try
        {
            //パラメータ生成
            var commonParam = CommonParamVm.CreateParameter();
            var sineParam = WaveformVm.CreateSineWaveParameter(commonParam.SamplingFrequency);
            var processingConfig = SignalConfigVm.CreateConfig();

            if (CurrentState == AppState.Initial || CurrentState == AppState.Completed)
            {
                //フレーム処理開始時の初期化
                CurrentState = AppState.FrameProcessing;
                _accumulatedInputWaveform = new double[commonParam.DataSize];
                _accumulatedProcessedWaveform = new double[commonParam.DataSize];
                _useCase.Reset();
            }

            //Span生成
            Span<double> inputFrameSpan = _accumulatedInputWaveform.AsSpan(_useCase.CurrentStartIndex, commonParam.FrameSize);
            Span<double> processedFrameSpan = _accumulatedProcessedWaveform.AsSpan(_useCase.CurrentStartIndex, commonParam.FrameSize);

            //フレーム処理実行
            _useCase.ExecuteFrame(
                inputFrameSpan,
                processedFrameSpan,
                WaveformVm.GetWaveformType(),
                sineParam,
                commonParam,
                processingConfig);

            if (_accumulatedInputWaveform != null && _accumulatedProcessedWaveform != null && _useCase.CurrentStartIndex > 0)
            {
                //プロット用の配列に格納
                double[] displayInput = new double[_useCase.CurrentStartIndex];
                double[] displayProcessed = new double[_useCase.CurrentStartIndex];

                Array.Copy(_accumulatedInputWaveform, 0, displayInput, 0, _useCase.CurrentStartIndex);
                Array.Copy(_accumulatedProcessedWaveform, 0, displayProcessed, 0, _useCase.CurrentStartIndex);

                //プロット更新
                PlotVm.UpdateData(displayInput, displayProcessed);
            }

            //フレーム処理完了判定
            if (!_useCase.HasNextFrame(commonParam))
            {
                CurrentState = AppState.Completed;
                _useCase.Reset();
            }
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private void HandleError(Exception ex)
    {
        PlotVm.Clear();
        _accumulatedInputWaveform = null;
        _accumulatedProcessedWaveform = null;
        CurrentState = AppState.Initial;
        ErrorOccurred?.Invoke($"入力値のエラー: {ex.Message}");
        _useCase.Reset();
    }
}