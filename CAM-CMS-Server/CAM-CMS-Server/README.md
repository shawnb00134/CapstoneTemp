# Getting Started with ASP.Net Core Web API

The CAM-CMS project was created using the [Microsoft ASP.Net Core React template](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-5.0&tabs=visual-studio).

The Web API uses [ASP.Net](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0) framework in [C#](https://learn.microsoft.com/en-us/dotnet/csharp/).

Server requires .NET 8 be installed.

## Setup and Launching
- Setup: Run ..\project-setup.ps1
- Launch: Run ..\launch-server.ps1

## Available Scripts

In the this directory, you can run:

### `ASPNETCORE_URLS="<urls>" ASPNETCORE_ENVIRONMENT="<enviornment>" dotnet run`
**Note: `ASPNETCORE_ENVIRONMENT` is set to `dev` by default**

Runs the app in the development mode.

`HTTPS` is enabled by default and can be found on port `7079`.\
`HTTP` is always enabled and processes into `HTTPS` from port `5141`.

`ASPNETCORE_URLS` can be set to `https://hostname:7079;http://hostname:5141` to define the ports to run on (this is the value that is used currently).\
(**optional**) `ASPNETCORE_ENVIRONMENT` can be set to `dev` or `review` currently to define the mode and db to connect to.

Open [https://localhost:7079](https://localhost:7079) to view it in your browser if ran local.

More information on running can be found at [Microsoft dotnet run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run).

### `dotnet publish`
Compiles the application, reads through its dependencies specified in the project file, and publishes the resulting set of files to a directory.

More information on publishing can be found at [Microsoft dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish).

### `dotnet build`

Builds the project and its dependencies into a set of binaries.\
The binaries include the project's code in Intermediate Language (IL) files with a .dll extension.\
Depending on the project type and settings, other files may be included.

More information on building can be found at [Microsoft dotnet build](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build).

### `dotnet test`

Runs the tests in the project.\
Tests can be found in the `Tests` directory which mirrors the structure of the `webapi` project structure.

More information on testing can be found at [Microsoft dotnet test](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test).

### More Commands
More commands can be found at [Microsoft dotnet commands](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet).

## Learn More

You can learn more in the [.Net documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/).

To learn more about other libraries used in this project, take a look at the following sections.

## Third Party Libraries

### [Dapper](https://dapper-tutorial.net/dapper)

Dapper is used to connect to and query databases from ASP.Net.

More information on Dapper can be found at [Dapper](https://dapper-tutorial.net/dapper).

### [Npgsql](https://www.npgsql.org/doc/index.html)

Npgsql is used to connect ASP.Net to PostgreSQL databases.

More information on Npgsql can be found at [Npgsql](https://www.npgsql.org/doc/index.html).

### [Moq](https://documentation.help/Moq/)

Moq is used to create mock objects for testing in ASP.Net.

More information on Moq can be found at [Moq](https://documentation.help/Moq/)

### [NUnit](https://docs.nunit.org/)

NUnit is used to run and implement tests for ASP.Net.

More information on NUnit can be found at [NUnit](https://docs.nunit.org/).

### [JWT](https://jwt.io/)
JWT is used to create and validate JSON Web Tokens.

More information on JWT can be found at [JWT](https://jwt.io/).