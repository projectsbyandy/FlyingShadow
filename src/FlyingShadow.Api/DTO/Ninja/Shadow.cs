namespace FlyingShadow.Api.DTO.Ninja;

internal record Shadow
{
    public Guid Id { get; set; }
    public required string CodeName { get; set; }
    public required string Clan { get; set; }
    public required string Origin { get; set; }
    public Rank Rank { get; set; }
}