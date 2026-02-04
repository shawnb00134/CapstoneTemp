using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                AppUsers = GetAppUsers(),
                Organizations = GetOrganizations()
            };
            return View(model);
        }

        private List<AppUserInfo> GetAppUsers()
        {
            var users = new List<AppUserInfo>();
            var cs = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using var con = new NpgsqlConnection(cs);
            con.Open();

            string query = @"SELECT * FROM app_user ORDER BY username;";

            using var cmd = new NpgsqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new AppUserInfo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("app_user_id")),
                    Username = reader.IsDBNull(reader.GetOrdinal("username")) ? null : reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                    //FirstName = reader.IsDBNull(reader.GetOrdinal("first_name")) ? null : reader.GetString(reader.GetOrdinal("first_name")),
                    //LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString(reader.GetOrdinal("last_name"))
                    // Add other columns as needed
                });
            }

            return users;
        }

        private List<string> GetOrganizations()
        {
            var orgs = new List<string>();
            var cs = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using var con = new NpgsqlConnection(cs);
            con.Open();

            string query = @"SELECT name FROM organization ORDER BY name;";

            using var cmd = new NpgsqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                orgs.Add(reader.GetString(0));

            return orgs;
        }

        //temp_user
        /*
        public IActionResult Index()
        {
            var users = GetTempUsers();
            return View(users);
        }
        */


        /*
        //No Data Found
        private List<string> GetTempUsers()
        {
            var users = new List<string>();
            var cs = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using var con = new NpgsqlConnection(cs);
            con.Open();

            string query = @"SELECT username FROM cam_cms.temp_user ORDER BY username;";

            using var cmd = new NpgsqlCommand(query, con);
            using var r = cmd.ExecuteReader();

            while (r.Read())
                users.Add(r.GetString(0));

            return users;
        }
        */

        /*
        //0 rows found
        private List<string> GetTempUsers()
        {
            var users = new List<string>();
            var cs = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using var con = new NpgsqlConnection(cs);
            con.Open();

            string query = @"SELECT COUNT(*) FROM cam_cms.temp_user;";

            using var cmd = new NpgsqlCommand(query, con);

            var count = (long)cmd.ExecuteScalar();

            users.Add($"Rows found: {count}");

            return users;
        }
        */


        /*
        //Connected to database
        private List<string> GetTempUsers()
        {
            var users = new List<string>();
            var cs = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using var con = new NpgsqlConnection(cs);
            con.Open();

            using var cmd = new NpgsqlCommand("SELECT current_database();", con);

            var db = cmd.ExecuteScalar().ToString();

            users.Add($"Connected DB: {db}");

            return users;
        }
        */
    }
}

/*
namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var organizations = _context.Organizations.ToList();
            return View(organizations);
        }
    }
}
*/
/*
namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var tableNames = GetTableNames();
            return View(tableNames);
        }

        private List<string> GetTableNames()
        {
            var tables = new List<string>();
            var connectionString = _configuration.GetConnectionString("WebApiPostgreSQLLocalDatabase");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Query to get all table names from the cam_cms schema
                string query = @"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = 'cam_cms' 
                    AND table_type = 'BASE TABLE'
                    ORDER BY table_name;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return tables;
        }
    }
}
*/