namespace CRMBanks.Web.Helper;

public static class DbConnectionStringHelper
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        var section = configuration.GetSection("ConnectionString:PostgresDb");
        
        var host = section["Host"];
        var port = section["Port"];
        var database = section["Database"];
        var user = section["User"];
        var password = section["Password"];
        
        return $"Host={host};Port={port};Database={database};Username={user};Password={password};Pooling=true;";
    }
}