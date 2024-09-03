using FFmpeg.AutoGen;
using Imgbed.Core.Extensions.FFMpeg;

namespace Imgbed.Core.Encoders;
internal class JpegEncoder : IEncoder, IDisposable
{
    private readonly unsafe AVCodecContext* _encoderCtx;
    private readonly unsafe AVFrame* _frame;
    private readonly unsafe AVPacket* _packet;

    public JpegEncoder()
    {
        
    }

    private unsafe void UnRef()
    {
        ffmpeg.av_frame_unref(_frame);
        ffmpeg.av_packet_unref(_packet);
    }

    public unsafe void EncodeAndSaveUnsafe(AVFrame* frame, int width, int height, string path)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));

        var framePixelFormat = (AVPixelFormat)frame->format;

        var scaleCtx = ffmpeg.sws_getContext(
            frame->width, frame->height, framePixelFormat,
            width, height, _encoderCtx->pix_fmt,
            ffmpeg.SWS_LANCZOS, null, null, null);

        _frame->width = width;
        _frame->height = height;
        _frame->format = (int)_encoderCtx->pix_fmt;

        ffmpeg.av_frame_copy_props(_frame, frame)
            .ThrowExceptionIfError(_ => UnRef());

        ffmpeg.sws_scale_frame(scaleCtx, _frame, frame)
            .ThrowExceptionIfError(_ => UnRef());

        ffmpeg.avcodec_send_frame(_encoderCtx, _frame)
            .ThrowExceptionIfError(_ => UnRef());

        var stream = new MemoryStream();

        var ret = 0;
        while (ret == 0)
        {
            ret = ffmpeg.avcodec_receive_packet(_encoderCtx, _packet);

            using var bufStream = new UnmanagedMemoryStream(_packet->data, _packet->size);
            bufStream.CopyTo(stream);
        }

        if (ret != ffmpeg.AVERROR(ffmpeg.EAGAIN) && ret != ffmpeg.AVERROR_EOF)
        {
            ret.ThrowExceptionIfError(_ => UnRef());
        }

        UnRef();
    }

    public void Dispose()
    {

    }

}
