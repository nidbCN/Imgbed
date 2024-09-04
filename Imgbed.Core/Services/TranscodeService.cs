using FFmpeg.AutoGen;
using Imgbed.Core.Decoders;
using Imgbed.Core.Encoders;

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

        var encodeArgList = new[]
        {
            ( -1, 2880, "_l"),
            ( -1, 1440, "_m"),
            ( -1, 720, "_s")
        };

        return Parallel.ForEachAsync(encodeArgList, async (args, cancellationToken) =>
            {
                var encoder = await encoderPool.GetAsync<T>(cancellationToken);
                var (width, height, size) = args;

                unsafe
                {
                    if (_frame->width < _frame->height)
                        (width, height) = (height, width);

                    encoder.EncodeAndSaveUnsafe(_frame, width, height, $"xxx{size}.webp");
                }

                encoderPool.Return(encoder);
            }
        );
    }
}
