namespace FirstTests.Config;

/// <summary>Lớp ánh xạ với appsettings.json. Tên thuộc tính PHẢI khớp tên khóa.</summary>
public class TestConfig
{
    public string BaseUrl        { get; set; } = "";
    public string Browser        { get; set; } = "chrome";
    public int    TimeoutSeconds { get; set; } = 30;
    public bool   Headless       { get; set; }
    public int    WindowWidth    { get; set; } = 1920;
    public int    WindowHeight   { get; set; } = 1080;
}