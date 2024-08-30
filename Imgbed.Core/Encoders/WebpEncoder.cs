using FFmpeg.AutoGen;
using Imgbed.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Imgbed.Core.Encoders;
internal class WebpEncoder : IEncoder, IDisposable
{
    private readonly ILogger<WebpEncoder> _logger;

    private readonly unsafe AVCodecContext* _encoderCtx;
    private readonly unsafe AVFrame* _frame;
    private readonly unsafe AVPacket* _packet;

    public unsafe WebpEncoder(ILogger<WebpEncoder> logger)
    {
        _logger = logger;

        var codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_WEBP);
        _encoderCtx = ffmpeg.avcodec_alloc_context3(codec);

        _encoderCtx->time_base = new() { den = 1, num = 1000 }; // 设置时间基准
        _encoderCtx->framerate = new() { num = 25, den = 1 };
        _encoderCtx->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
        _encoderCtx->flags |= ffmpeg.AV_CODEC_FLAG_COPY_OPAQUE;

        ffmpeg.avcodec_open2(_encoderCtx, codec, null);

        _frame = ffmpeg.av_frame_alloc();
        _packet = ffmpeg.av_packet_alloc();
    }

    private unsafe void UnRef()
    {
        ffmpeg.av_frame_unref(_frame);
        ffmpeg.av_packet_unref(_packet);
    }

    public unsafe Stream EncodeUnsafe(AVFrame* frame, int width, int height)
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

        return stream;
    }

    public unsafe void Dispose()
    {
        var frame = _frame;
        ffmpeg.av_frame_free(&frame);
        var packet = _packet;
        ffmpeg.av_packet_free(&packet);

        var encoderCtx = _encoderCtx;
        ffmpeg.avcodec_free_context(&encoderCtx);
    }
}
