using FFmpeg.AutoGen;
using Imgbed.Core.Extensions;

namespace Imgbed.Core.Decoders;
public class AutoDecoder : IDisposable
{
    private readonly unsafe AVFrame* _frame;
    private readonly unsafe AVPacket* _packet;

    public class DecoderWrapper : IDisposable
    {
        public unsafe DecoderWrapper(AVCodec* codec)
        {
            _codecContext = ffmpeg.avcodec_alloc_context3(codec);
        }

        public int Count { get; private set; }

        private readonly unsafe AVCodecContext* _codecContext;

        public unsafe AVCodecContext* CodecContext
        {
            get
            {
                Count++;
                return _codecContext;
            }
        }

        public unsafe void Dispose()
        {
            var codecCtx = _codecContext;
            ffmpeg.avcodec_free_context(&codecCtx);
        }
    }

    public unsafe AutoDecoder()
    {
        _frame = ffmpeg.av_frame_alloc();
        _packet = ffmpeg.av_packet_alloc();
    }

    public unsafe void Dispose()
    {
        var frame = _frame;
        ffmpeg.av_frame_free(&frame);

        var packet = _packet;
        ffmpeg.av_packet_free(&packet);
    }

    public unsafe AVFrame* Decode()
    {
        AVFormatContext* formatContext;
        ffmpeg.avformat_open_input(&formatContext, "", null, null)
            .ThrowExceptionIfError();

        ffmpeg.avformat_find_stream_info(formatContext, null)
            .ThrowExceptionIfError();

        var streamIndex = ffmpeg.av_find_best_stream(
            formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO,
            -1, -1, null, 0)
            .ThrowExceptionIfError();

        var stream = formatContext->streams[streamIndex];
        var decoder = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);

        if (decoder is null)
        {
            throw new NotSupportedException("Can not found codec for .");
        }

        var decoderContext = ffmpeg.avcodec_alloc_context3(decoder);

        ffmpeg.avcodec_parameters_to_context(decoderContext, stream->codecpar)
            .ThrowExceptionIfError();
        ffmpeg.avcodec_open2(decoderContext, decoder, null)
            .ThrowExceptionIfError();


        while (ffmpeg.av_read_frame(formatContext, _packet) >= 0)
        {
            if (_packet->stream_index == streamIndex)
                break;
        }

        ffmpeg.avcodec_send_packet(decoderContext, _packet)
            .ThrowExceptionIfError();

        var ret = 0;
        while (ret >= 0)
        {
            ret = ffmpeg.avcodec_receive_frame(decoderContext, _frame);
        }

        if (ret != ffmpeg.AVERROR_EOF
        && ret != ffmpeg.AVERROR(ffmpeg.EAGAIN))
        {
            ret.ThrowExceptionIfError();

        }

        return _frame;
    }
}
