namespace SignalProcessingToolApp.Application;

public class FrameProcessor
{
    private int _currentStartIndex;

    public int CurrentStartIndex => _currentStartIndex;

    public FrameProcessor()
    {
        _currentStartIndex = 0;
    }

    public void AdvanceFrame(int frameSize)
    {
        _currentStartIndex += frameSize;
    }

    public bool HasNextFrame(int dataSize)
    {
        return _currentStartIndex < dataSize;
    }

    public void Reset()
    {
        _currentStartIndex = 0;
    }
}