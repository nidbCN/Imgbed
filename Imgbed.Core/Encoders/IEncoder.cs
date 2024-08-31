using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
public interface IEncoder
{
    public unsafe Stream EncodeUnsafe(AVFrame* frame, int width, int height);
}
