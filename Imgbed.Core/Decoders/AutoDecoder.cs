using FFmpeg.AutoGen;
using Imgbed.Core.Extensions;

namespace Imgbed.Core.Decoders;
public class AutoDecoder
{
    public AutoDecoder()
    {

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

        var packet = ffmpeg.av_packet_alloc();

        while (ffmpeg.av_read_frame(formatContext, packet) >= 0)
        {
            if (packet->stream_index == streamIndex)
                break;
        }

        ffmpeg.avcodec_send_packet(decoderContext, packet)
            .ThrowExceptionIfError();

        var frame = ffmpeg.av_frame_alloc();
        var ret = 0;
        while (ret >= 0)
        {
            ret = ffmpeg.avcodec_receive_frame(decoderContext, frame);
        }

        if (ret != ffmpeg.AVERROR_EOF
        && ret != ffmpeg.AVERROR(ffmpeg.EAGAIN))
        {
            ret.ThrowExceptionIfError();

        }

        return frame;
    }
}
