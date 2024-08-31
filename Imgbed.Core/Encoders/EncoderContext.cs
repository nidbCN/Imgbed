using FFmpeg.AutoGen;

namespace Imgbed.Core.Encoders;

public class EncoderContext
{
    public IEncoder? Encoder { get; set; }

    public EncoderContext() { }

    public EncoderContext(IEncoder encoder) 
        => Encoder = encoder;

    public unsafe Task<Stream> Encode(AVFrame* frame, int width, int height)
    {
        if (Encoder is null)
            throw new Exception(nameof(Encoder));

        return Task.Run(() => Encoder.EncodeUnsafe(frame, width, height));
    }
}
