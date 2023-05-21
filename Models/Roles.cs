using System.ComponentModel.DataAnnotations;

namespace Roles.Models;
public class JobRoleModel
{
    public string? Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Status { get; set; }
    [Required]
    public string? Description { get; set; }
    public int? Experience { get; set; }
    public string? Salary { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Unit { get; set; }
    [Required]
    public string? Code { get; set; }
}

public class Job {
    public string? Item { get; set; }
    public string? Code { get; set; }
}