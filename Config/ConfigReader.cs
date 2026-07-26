using Microsoft.Extensions.Configuration;

namespace FirstTests.Config;

public static class ConfigReader
{
    private static TestConfig? _cache;

    public static TestConfig Load()
    {
        if (_cache is not null) return _cache;          // đọc file 1 lần cho cả phiên

        var env = Environment.GetEnvironmentVariable("TEST_ENV");

        var builder = new ConfigurationBuilder()
            // AppContext.BaseDirectory = bin/Debug/net8.0/ — luôn đúng.
            // KHÔNG dùng Directory.GetCurrentDirectory(): nó đổi tuỳ cách chạy.
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false);

        if (!string.IsNullOrWhiteSpace(env))
            builder.AddJsonFile($"Config/appsettings.{env}.json", optional: true);

        // Cho phép ghi đè từ dòng lệnh / CI:  TEST_Browser=firefox dotnet test
        builder.AddEnvironmentVariables("TEST_");

        _cache = builder.Build().Get<TestConfig>()
                 ?? throw new InvalidOperationException(
                        "Không đọc được appsettings.json — kiểm tra CopyToOutputDirectory.");
        return _cache;
    }
}