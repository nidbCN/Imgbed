using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;

namespace Imgbed.Core.Encoders;
internal class WebpEncoder : IEncoder, IDisposable
{
    private readonly ILogger<WebpEncoder> _logger;
    private readonly unsafe AVCodecContext* _encoderCtx;
    private readonly unsafe AVFrame* _frame;

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
    }

    public unsafe void EncodeUnsafe(AVFrame* frame, AVPacket* packet, int width, int height)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));
        if (packet is null)
            throw new ArgumentNullException(nameof(packet));

        var framePixelFormat = (AVPixelFormat)frame->format;

        var scaleCtx = ffmpeg.sws_getContext(
            frame->width, frame->height, framePixelFormat,
            width, height, _encoderCtx->pix_fmt,
            ffmpeg.SWS_LANCZOS, null, null, null);

        _frame->width = width;
        _frame->height = height;
        _frame->format = (int)_encoderCtx->pix_fmt;

        ffmpeg.av_frame_copy_props(_frame, frame);
        ffmpeg.sws_scale_frame(scaleCtx, _frame, frame);

        var sendRet = ffmpeg.avcodec_send_frame(_encoderCtx, _frame);
        ffmpeg.av_frame_unref(_frame);
        if (sendRet != 0)
            return false;

        var encodeResult = ffmpeg.avcodec_receive_packet(_encoderCtx, packet);
        if (encodeResult != 0)
            return false;

        return true;
    }

    public unsafe void Dispose()
    {
        var frame = _frame;
        ffmpeg.av_frame_free(&frame);

        var encoderCtx = _encoderCtx;
        ffmpeg.avcodec_free_context(&encoderCtx);
    }
}
