#nullable enable

namespace SignalProcessingToolApp.Domains.Parameters;

public record DelayParameter
{
    public int BufferSize { get; }
    public int SampleCount { get; }

    public DelayParameter(
        int bufferSize,
        int sampleCount)
    {
        if (bufferSize <= 0)
        {
            throw new ArgumentException(
                $"ディレイバッファサイズは0より大きい値である必要があります。入力値: {bufferSize}",
                nameof(bufferSize));
        }

        if (sampleCount < 0)
        {
            throw new ArgumentException(
                $"ディレイサンプル数は0以上の値である必要があります。入力値: {sampleCount}",
                nameof(sampleCount));
        }

        if (sampleCount >= bufferSize)
        {
            throw new ArgumentException(
                $"ディレイサンプル数はディレイバッファサイズより小さい必要があります。ディレイバッファサイズ: {bufferSize}, ディレイサンプル数: {sampleCount}",
                nameof(sampleCount));
        }

        BufferSize = bufferSize;
        SampleCount = sampleCount;
    }
}
