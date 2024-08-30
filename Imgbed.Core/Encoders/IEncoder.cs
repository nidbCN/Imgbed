using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;
internal interface IEncoder
{
    public unsafe bool TryEncodeUnsafe(AVFrame* frame, AVPacket* packet, int width, int height);
}
