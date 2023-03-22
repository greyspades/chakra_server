namespace Resume.Models;

public class ResumeDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string CollectionName { get; set; } = null!;
}

public class ResumeModel {
    public dynamic? Cv { get; set; }
    public dynamic? Name { get; set; }
    public dynamic? Other { get; set; }
}