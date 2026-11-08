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
    private AppState _currentState = AppState.Initial;

    [ObservableProperty]
    private string _statusMessage = "準備完了";

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
            StatusMessage = "準備完了";
        }
        catch
        {
            StatusMessage = "準備完了";
            CurrentState = AppState.Initial;
        }
    }

    [RelayCommand(CanExecute = nameof(IsBatchProcessEnabled))]
    private void BatchProcess()
    {
        try
        {
            var commonParam = CommonParamVm.CreateParameter();
            var sineParam = WaveformVm.CreateSineWaveParameter(commonParam.SamplingFrequency);
            var processingConfig = SignalConfigVm.CreateConfig();

            int dataSize = commonParam.DataSize;
            var inputBuffer = new double[dataSize];
            var processedBuffer = new double[dataSize];

            _useCase.ExecuteBatch(
                inputBuffer,
                processedBuffer,
                WaveformVm.GetWaveformType(),
                sineParam,
                commonParam,
                processingConfig);

            _accumulatedInputWaveform = inputBuffer;
            _accumulatedProcessedWaveform = processedBuffer;

            PlotVm.UpdateData(_accumulatedInputWaveform, _accumulatedProcessedWaveform);

            CurrentState = AppState.Completed;
            StatusMessage = "一括処理が完了しました";
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
            var commonParam = CommonParamVm.CreateParameter();

            if (CurrentState == AppState.Initial || CurrentState == AppState.Completed)
            {
                CurrentState = AppState.FrameProcessing;
                _accumulatedInputWaveform = new double[commonParam.DataSize];
                _accumulatedProcessedWaveform = new double[commonParam.DataSize];
                _useCase.Reset();
            }

            int frameSize = commonParam.FrameSize;
            int startIndex = _useCase.CurrentStartIndex;

            Span<double> inputFrameSpan = _accumulatedInputWaveform.AsSpan(startIndex, frameSize);
            Span<double> processedFrameSpan = _accumulatedProcessedWaveform.AsSpan(startIndex, frameSize);

            var sineParam = WaveformVm.CreateSineWaveParameter(commonParam.SamplingFrequency);
            var processingConfig = SignalConfigVm.CreateConfig();

            _useCase.ExecuteFrame(
                inputFrameSpan,
                processedFrameSpan,
                WaveformVm.GetWaveformType(),
                sineParam,
                commonParam,
                processingConfig);

            int accumulatedCount = _useCase.CurrentStartIndex;

            if (_accumulatedInputWaveform != null && _accumulatedProcessedWaveform != null && accumulatedCount > 0)
            {
                double[] displayInput = new double[accumulatedCount];
                double[] displayProcessed = new double[accumulatedCount];

                Array.Copy(_accumulatedInputWaveform, 0, displayInput, 0, accumulatedCount);
                Array.Copy(_accumulatedProcessedWaveform, 0, displayProcessed, 0, accumulatedCount);

                PlotVm.UpdateData(displayInput, displayProcessed);
            }

            if (!_useCase.HasNextFrame(commonParam))
            {
                CurrentState = AppState.Completed;
                StatusMessage = "フレーム処理が完了しました";
                _useCase.Reset();
            }
            else
            {
                StatusMessage = $"フレーム処理中... ({_useCase.CurrentStartIndex} / {commonParam.DataSize} サンプル処理済み)";
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
        StatusMessage = "準備完了";
        CurrentState = AppState.Initial;
        ErrorOccurred?.Invoke($"入力値のエラー: {ex.Message}");
        _useCase.Reset();
    }
}