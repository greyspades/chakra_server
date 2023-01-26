namespace Roles.Models;
public class RoleModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public int? Experience { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Unit { get; set; }
}