using System.ComponentModel.DataAnnotations;

namespace FlyingShadow.Core.DTO.Battle;

public record BattleRequest
{
    [Required]
    public string ShadowOneCodeName { get; init; } = string.Empty;
    
    [Required]
    public string ShadowTwoCodeName { get; init; } = string.Empty;
}