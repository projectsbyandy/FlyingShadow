using System.ComponentModel.DataAnnotations;

namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record MockDataOptions
{
    [Required(AllowEmptyStrings = false)]
    public string FakeJwtPath { get; init; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public string FakeLoginDetailsListPath { get; init; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public string FakeUsersPath { get; init; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public string FakeShadowsPath { get; init; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public string FakeStealthMetricsPath { get; init; } = string.Empty;
}