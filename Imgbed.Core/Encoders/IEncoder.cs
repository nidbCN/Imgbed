using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
internal interface IEncoder
{
    public unsafe Stream EncodeUnsafe(AVFrame* frame, int width, int height);
}
