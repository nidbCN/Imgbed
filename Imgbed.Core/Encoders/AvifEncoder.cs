using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
internal class AvifEncoder : IEncoder, IDisposable
{
    public unsafe void EncodeAndSaveUnsafe(AVFrame* frame, int width, int height, string path)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
