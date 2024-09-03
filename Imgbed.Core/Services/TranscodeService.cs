using FFmpeg.AutoGen;
using Imgbed.Core.Decoders;
using Imgbed.Core.Encoders;
using Imgbed.Core.Encoders.Pool;

namespace Imgbed.Core.Services;

public class TranscodeService(EncoderPool encoderPool)
{
    private unsafe AVFrame* _frame;

    public Task CreateTranscodeTask<T>() where T : IEncoder, new()
    {
        var decoder = new AutoDecoder();
        unsafe
        {
            _frame = decoder.Decode();

            // frame -> filter -> frame
        }

        var encoderList = new[]
        {
            ( 3840 , 2560, "_l"),
            ( 1920, 1280, "_m"),
            ( 1080, 720, "_s")
        };

        return Parallel.ForEachAsync(encoderList, async (args, cancellationToken) =>
            {
                var encoder = await encoderPool.GetAsync<T>(cancellationToken);
                var (width, height, size) = args;

                unsafe
                {
                    encoder.EncodeAndSaveUnsafe(_frame, width, height, $"xxx{size}.webp");
                }

                encoderPool.Return(encoder);
            }
        );
    }
}
