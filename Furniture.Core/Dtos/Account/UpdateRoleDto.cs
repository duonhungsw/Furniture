namespace Furniture.Core;

public class UpdateRoleDto
{
    public Guid Id { get; set; } 
    public string? Name {  get; set; }
    public string RoleName { get; set; } = string.Empty;
}
