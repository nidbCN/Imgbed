using FFmpeg.AutoGen;
using Imgbed.Core.Decoders;
using Imgbed.Core.Encoders;
using Microsoft.Extensions.ObjectPool;

namespace Imgbed.Core.Services;
internal class DecodeEncodeService
{
    private readonly unsafe AVFrame* _frame;
    private readonly ObjectPool<AvifEncoder> _avifEncoderPool;
    private readonly ObjectPool<JpegEncoder> _jpegEncoderPool;
    private readonly ObjectPool<WebpEncoder> _webpEncoderPool;
    private readonly ObjectPool<AutoDecoder> _autoDecoderPool;

    public DecodeEncodeService(
        ObjectPool<AvifEncoder> avifEncoderPool,
        ObjectPool<JpegEncoder> jpegEncoderPool,
        ObjectPool<WebpEncoder> webpEncoderPool,
        ObjectPool<AutoDecoder> autoDecoderPool)
    {
        _avifEncoderPool = avifEncoderPool;
        _jpegEncoderPool = jpegEncoderPool;
        _webpEncoderPool = webpEncoderPool;
        _autoDecoderPool = autoDecoderPool;
    }

    public Task Transcode<T>() where T : IEncoder
    {
        var decoder = _autoDecoderPool.Get();
        unsafe
        {
            var frame = decoder.Decode();

            // frame -> filter -> frame

            IEncoder encoder = null!;

            if (_avifEncoderPool.GetType().GenericTypeArguments[0] == typeof(T))
            {
                encoder = _avifEncoderPool.Get();
            }
            else if (_jpegEncoderPool.GetType().GenericTypeArguments[0] == typeof(T))
            {
                encoder = _jpegEncoderPool.Get();
            }
            else if (_webpEncoderPool.GetType().GenericTypeArguments[0] == typeof(T))
            {
                encoder = _webpEncoderPool.Get();
            }

            Task.WaitAll([
                 Task.Run(() => encoder.EncodeAndSaveUnsafe(frame, 6000, 4000, "l.webp")),
                Task.Run(() => encoder.EncodeAndSaveUnsafe(frame, 2880, 1920, "m.webp")),
                Task.Run(() => encoder.EncodeAndSaveUnsafe(frame, 1080, 720, "s.webp")),
            ]);

            _webpEncoderPool.Return(encoder as WebpEncoder);
        }
    }
}
