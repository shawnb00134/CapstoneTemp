using Azure.Core;
using Azure.Identity;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
//using Amazon.RDS.Util;

namespace CAMCMSServer.Database.Context;

public interface IDataContext
{
    #region Methods

    Task<IDbConnectionWrapper> CreateConnection();

    void Init();

    #endregion
}

[ExcludeFromCodeCoverage]
public class LocalPostgreSqlDataContext : IDataContext
{
    #region Data members

    protected readonly IConfiguration Configuration;
    protected NpgsqlDataSource DataSource;

    #endregion

    #region Constructors

    public LocalPostgreSqlDataContext(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    #endregion

    #region Methods

    public async Task<IDbConnectionWrapper> CreateConnection()
    {
        var connection = await this.DataSource.OpenConnectionAsync();
        var connectionWrapper = new NpgsqlConnectionWrapper(connection);
        return connectionWrapper;
    }

    public void Init()
    {
        // Local, password-based connection
        var connectionString =
            this.Configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        // No Azure token provider here – plain password from connection string
        this.DataSource = dataSourceBuilder.Build();
    }

    #endregion
}

[ExcludeFromCodeCoverage]
public class DevPostgreSqlDataContext : IDataContext
{
    #region Data members

    protected readonly IConfiguration Configuration;

    protected NpgsqlDataSource DataSource;

    private readonly DefaultAzureCredential credential;
    private readonly string[] scopes = new[] { "https://ossrdbms-aad.database.windows.net/.default" };

    #endregion

    #region Constructors

    public DevPostgreSqlDataContext(IConfiguration configuration)
    {
        this.Configuration = configuration;
        this.credential = new DefaultAzureCredential();
    }

    #endregion

    #region Methods

    public async Task<IDbConnectionWrapper> CreateConnection()
    {
        var connection = await this.DataSource.OpenConnectionAsync();
        var connectionWrapper = new NpgsqlConnectionWrapper(connection);

        return connectionWrapper;
    }

    public void Init()
    {
        var connectionString =
            this.Configuration.GetConnectionString("WebApiPostgreSQLAzureDevDatabase");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UsePeriodicPasswordProvider(
            async (settings, cancellationToken) =>
            {
                var token = await this.credential.GetTokenAsync(
                    new TokenRequestContext(this.scopes),
                    cancellationToken);
                return token.Token;
            },
            TimeSpan.FromMinutes(10),  // Refresh interval
            TimeSpan.FromSeconds(10)); // Retry interval
        this.DataSource = dataSourceBuilder.Build();
    }

    #endregion
}

[ExcludeFromCodeCoverage]
public class TestPostgreSqlDataContext : IDataContext
{
    #region Data members

    protected readonly IConfiguration Configuration;

    protected NpgsqlDataSource DataSource;

    private readonly DefaultAzureCredential credential;
    private readonly string[] scopes = new[] { "https://ossrdbms-aad.database.windows.net/.default" };

    #endregion

    #region Constructors

    public TestPostgreSqlDataContext(IConfiguration configuration)
    {
        this.Configuration = configuration;
        this.credential = new DefaultAzureCredential();
    }

    #endregion

    #region Methods

    public async Task<IDbConnectionWrapper> CreateConnection()
    {
        var connection = await this.DataSource.OpenConnectionAsync();
        var connectionWrapper = new NpgsqlConnectionWrapper(connection);

        return connectionWrapper;
    }

    public void Init()
    {
        var connectionString =
            this.Configuration.GetConnectionString("WebApiPostgreSQLAzureTestDatabase");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UsePeriodicPasswordProvider(
            async (settings, cancellationToken) =>
            {
                var token = await this.credential.GetTokenAsync(
                    new TokenRequestContext(this.scopes),
                    cancellationToken);
                return token.Token;
            },
            TimeSpan.FromMinutes(10),  // Refresh interval
            TimeSpan.FromSeconds(10)); // Retry interval
        this.DataSource = dataSourceBuilder.Build();
    }

    #endregion
}