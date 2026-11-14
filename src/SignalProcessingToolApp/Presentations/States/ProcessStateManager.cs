using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalProcessingToolApp.Presentations.States;

public partial class ProcessStateManager : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusMessage))]
    [NotifyPropertyChangedFor(nameof(IsParametersEnabled))]
    [NotifyPropertyChangedFor(nameof(IsBatchProcessEnabled))]
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

    public void UpdateFrameState(bool isCompleted)
    {
        CurrentState = isCompleted ? AppState.Completed : AppState.FrameProcessing;
    }

    public void CompleteBatch()
    {
        CurrentState = AppState.Completed;
    }

    public void Reset()
    {
        CurrentState = AppState.Initial;
    }
}
