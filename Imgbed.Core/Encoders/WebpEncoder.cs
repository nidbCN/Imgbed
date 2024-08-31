using FFmpeg.AutoGen;
using Imgbed.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Imgbed.Core.Encoders;
internal class WebpEncoder : IEncoder, IDisposable
{
    private readonly ILogger<WebpEncoder> _logger;

    private readonly unsafe AVFormatContext* _formatCtx;
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

        var fmt = ffmpeg.av_guess_format("webp", null, null);
        _formatCtx = ffmpeg.avformat_alloc_context();
        _formatCtx->oformat = fmt;

        _frame = ffmpeg.av_frame_alloc();
        _packet = ffmpeg.av_packet_alloc();
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

        ffmpeg.avio_open(&_formatCtx->pb, path, ffmpeg.AVIO_FLAG_READ_WRITE)
            .ThrowExceptionIfError();

        // 此处参数 c 未使用(ffmpeg 6.1)
        // stream 在执行完后需要销毁
        var stream = ffmpeg.avformat_new_stream(_formatCtx, null);

        stream->id = 0;
        stream->index = 0;
        stream->codecpar->codec_id = _encoderCtx->codec_id;
        stream->codecpar->codec_type = _encoderCtx->codec_type;
        stream->codecpar->format = (int)_encoderCtx->pix_fmt;
        stream->codecpar->width = width;
        stream->codecpar->height = height;
        stream->time_base = new() { den = 1, num = 1000 };

        _formatCtx->streams[0] = stream;

        // scaleCtx 执行完后需要销毁
        var scaleCtx = ffmpeg.sws_getContext(
            frame->width, frame->height, framePixelFormat,
            width, height, _encoderCtx->pix_fmt,
            ffmpeg.SWS_LANCZOS, null, null, null);

        _frame->width = width;
        _frame->height = height;
        _frame->format = (int)_encoderCtx->pix_fmt;

        ffmpeg.avformat_write_header(_formatCtx, null)
            .ThrowExceptionIfError();

        // TODO: unref 用法、安全性存疑，需要查阅文档。
        ffmpeg.av_frame_copy_props(_frame, frame)
            .ThrowExceptionIfError(_ => ffmpeg.av_frame_unref(_frame));

        ffmpeg.sws_scale_frame(scaleCtx, _frame, frame)
            .ThrowExceptionIfError(_ => ffmpeg.av_frame_unref(_frame));

        // 销毁 scaleCtx
        ffmpeg.sws_freeContext(scaleCtx);

        ffmpeg.avcodec_send_frame(_encoderCtx, _frame)
            .ThrowExceptionIfError(_ => ffmpeg.av_frame_unref(_frame));

        ffmpeg.av_frame_unref(_frame);

        var ret = ffmpeg.avcodec_receive_packet(_encoderCtx, _packet);

        if (ret != ffmpeg.AVERROR(ffmpeg.EAGAIN) && ret != ffmpeg.AVERROR_EOF)
        {
            ret.ThrowExceptionIfError(catchInvoke:
                _ => ffmpeg.av_packet_unref(_packet));
        }

        _packet->stream_index = 0;

        ffmpeg.av_write_frame(_formatCtx, _packet);
        ffmpeg.av_write_trailer(_formatCtx);
        ffmpeg.av_packet_unref(_packet);
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
