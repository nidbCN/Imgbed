namespace Imgbed.WebApi.Entities;

public record Group
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public IList<string> Encoder { get; set; }
}
