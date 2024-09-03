using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
internal class AvifEncoder : IEncoder
{
    public unsafe void EncodeAndSaveUnsafe(AVFrame* frame, int width, int height, string path)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
