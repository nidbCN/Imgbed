using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;

public interface IEncoder : IDisposable
{
    public unsafe void EncodeAndSaveUnsafe(AVFrame* frame, int width, int height, string path);

    public void Reset();
}
