using FFmpeg.AutoGen;
using Imgbed.Core.Encoders;

namespace Imgbed.Core;

public class EncoderContext
{
    public IEncoder? Encoder { get; set; }

    public EncoderContext() { }

    public EncoderContext(IEncoder encoder)
        => Encoder = encoder;

    public unsafe Task EncodeAndSave(AVFrame* frame, int width, int height)
    {
        if (Encoder is null)
            throw new Exception(nameof(Encoder));

        return Task.Run(() => Encoder.EncodeAndSaveUnsafe(frame, width, height, "x.jpg"));
    }
}
