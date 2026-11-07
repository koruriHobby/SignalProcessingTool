#nullable enable

namespace SignalProcessingToolApp.Domain;

public record CommonParameter
{
    public double SamplingFrequency { get; }
    public int FrameSize { get; }
    public int DataSize { get; }

    public CommonParameter(
        double samplingFrequency,
        int frameSize,
        int dataSize)
    {
        if (samplingFrequency <= 0)
        {
            throw new ArgumentException(
                $"サンプリング周波数は0より大きい値である必要があります。入力値: {samplingFrequency}",
                nameof(samplingFrequency));
        }

        if (frameSize <= 0)
        {
            throw new ArgumentException(
                $"フレームサイズは0より大きい値である必要があります。入力値: {frameSize}",
                nameof(frameSize));
        }

        if (dataSize <= 0)
        {
            throw new ArgumentException(
                $"データサイズは0より大きい値である必要があります。入力値: {dataSize}",
                nameof(dataSize));
        }

        if (frameSize > dataSize)
        {
            throw new ArgumentException(
                $"フレームサイズはデータサイズ以下である必要があります。フレームサイズ: {frameSize}, データサイズ: {dataSize}",
                nameof(frameSize));
        }

        SamplingFrequency = samplingFrequency;
        FrameSize = frameSize;
        DataSize = dataSize;
    }
}
