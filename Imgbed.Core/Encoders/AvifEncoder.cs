using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
internal class AvifEncoder : IEncoder, IDisposable
{
    public unsafe Stream EncodeUnsafe(AVFrame* frame, int width, int height) => throw new NotImplementedException();
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
