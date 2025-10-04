using System.Data;

public interface IDbConnectionSingleton
{
    IDbConnection CreateConnection();
} 