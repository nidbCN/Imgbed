using FFmpeg.AutoGen;
using Imgbed.Core.Encoders;
using Microsoft.Extensions.ObjectPool;

namespace Imgbed.Core.Services;
internal class DecodeEncodeService
{
    private readonly unsafe AVFrame* _frame;

    public DecodeEncodeService(
        ObjectPool<AvifEncoder> avifEncoderPool,
        ObjectPool<JpegEncoder> jpegEncoderPool,
        ObjectPool<WebpEncoder> webpEncoderPool
        )
    {

    }
}
