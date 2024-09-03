using FFmpeg.AutoGen;
using Imgbed.Core.Encoders;
using Microsoft.Extensions.ObjectPool;

namespace Imgbed.Core.Services;
internal class DecodeEncodeService
{
    private readonly unsafe AVFrame* _frame;
    private readonly ObjectPool<AvifEncoder> _avifEncoderPool;
    private readonly ObjectPool<JpegEncoder> _jpegEncoderPool;
    private readonly ObjectPool<WebpEncoder> _webpEncoderPool;

    public DecodeEncodeService(
        ObjectPool<AvifEncoder> avifEncoderPool,
        ObjectPool<JpegEncoder> jpegEncoderPool,
        ObjectPool<WebpEncoder> webpEncoderPool
        )
    {
        _avifEncoderPool = avifEncoderPool;
        _jpegEncoderPool = jpegEncoderPool;
        _webpEncoderPool = webpEncoderPool;
    }
}
