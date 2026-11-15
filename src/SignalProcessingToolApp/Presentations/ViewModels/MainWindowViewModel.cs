#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalProcessingToolApp.Applications.Configs;
using SignalProcessingToolApp.Applications.Settings;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.Services;
using SignalProcessingToolApp.Domains.Parameters;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using SignalProcessingToolApp.Presentations.Factories;
using SignalProcessingToolApp.Presentations.States;
using System.ComponentModel;
using System.Windows.Input;

namespace SignalProcessingToolApp.Presentations.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly SignalProcessingService _session;
    private readonly ProcessStateManager _stateManager = new();
    private readonly SignalExecutionSettingsFactory _requestFactory;

    public CommonParameterViewModel CommonParamVm { get; } = new();
    public InputSignalViewModel WaveformVm { get; } = new();
    public SignalProcessingViewModel SignalConfigVm { get; } = new();
    public PlotViewModel PlotVm { get; } = new();

    public event Action<string>? ErrorOccurred;

    public int CurrentDataSize => CommonParamVm.DataSizes[CommonParamVm.DataSizeIndex];

    public string StatusMessage => _stateManager.StatusMessage;
    public bool IsParametersEnabled => _stateManager.IsParametersEnabled;
    public bool IsBatchProcessEnabled => _stateManager.IsBatchProcessEnabled;

    public MainWindowViewModel(
        SignalProcessingService session,
        SignalExecutionSettingsFactory requestFactory)
    {
        _session = session;
        _requestFactory = requestFactory;
        _stateManager.PropertyChanged += OnStateManagerPropertyChanged;
        SignalConfigVm.PropertyChanged += OnSignalConfigPropertyChanged;
        CommonParamVm.PropertyChanged += OnCommonParameterPropertyChanged;
    }

    [RelayCommand]
    private void Reset()
    {
        ResetAll();
    }

    [RelayCommand(CanExecute = nameof(IsBatchProcessEnabled))]
    private void BatchProcess()
    {
        try
        {
            var (input, processed) = _session.ExecuteBatch(_requestFactory.Create(CommonParamVm, WaveformVm, SignalConfigVm));
            PlotVm.UpdateData(input, processed);
            _stateManager.CompleteBatch();
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    [RelayCommand]
    private void FrameProcess()
    {
        try
        {
            var (displayInput, displayProcessed, isCompleted) = _session.ExecuteNextFrame(_requestFactory.Create(CommonParamVm, WaveformVm, SignalConfigVm));
            PlotVm.UpdateData(displayInput, displayProcessed);
            _stateManager.UpdateFrameState(isCompleted);
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    private void OnStateManagerPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        OnPropertyChanged(propertyChangedEventArgs.PropertyName);
        BatchProcessCommand.NotifyCanExecuteChanged();
        FrameProcessCommand.NotifyCanExecuteChanged();
    }

    private void OnSignalConfigPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName is nameof(SignalProcessingViewModel.FilterTypeIndex)
            or nameof(SignalProcessingViewModel.CutoffFrequencyText))
        {
            UpdateCoefficientDisplay();
        }
    }

    private void OnCommonParameterPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(CommonParameterViewModel.SamplingFrequencyIndex))
        {
            UpdateCoefficientDisplay();
        }
    }

    private void UpdateCoefficientDisplay()
    {
        if (SignalConfigVm.FilterTypeIndex < 0
            || !double.TryParse(SignalConfigVm.CutoffFrequencyText, out double cutoffFrequency))
        {
            SignalConfigVm.ClearCoefficientTexts();
            return;
        }

        try
        {
            double samplingFrequency = CommonParamVm.SamplingFrequencies[CommonParamVm.SamplingFrequencyIndex];
            var coefficients = _session.CalculateFilterCoefficients(
                SignalConfigVm.GetFilterType(),
                samplingFrequency,
                cutoffFrequency);
            SignalConfigVm.UpdateCoefficientTexts(coefficients);
        }
        catch (ArgumentException exception)
        {
            SignalConfigVm.ClearCoefficientTexts();
            HandleError(exception);
        }
    }

    private void HandleError(Exception exception)
    {
        ResetAll();
        ErrorOccurred?.Invoke($"入力値のエラー: {exception.Message}");
    }

    private void ResetAll()
    {
        _session.Reset();
        PlotVm.Clear();
        _stateManager.Reset();
    }
}
