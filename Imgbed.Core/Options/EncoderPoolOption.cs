namespace Imgbed.Core.Options;

public record EncoderPoolOption
{
    public ICollection<string> NamespaceList { get; set; } = new HashSet<string>()
    {
        nameof(Encoders)
    };

    public uint MaxTotalCount { get; init; } = 16;
    public uint MaxPreEncoderCount { get; init; } = 4;
}
