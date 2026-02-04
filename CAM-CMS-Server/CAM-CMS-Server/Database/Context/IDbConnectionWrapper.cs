using System.Data;
using System.Diagnostics.CodeAnalysis;
using CAMCMSServer.Model;
using CAMCMSServer.Model.Requests;
using CAMCMSServer.Utils;
using Dapper;

#pragma warning disable CS8767

namespace CAMCMSServer.Database.Context;

public interface IDbConnectionWrapper : IDbConnection
{
    #region Methods

    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);

    Task<int> ExecuteAsync(string sql, object? param = null);

    #endregion
}

[ExcludeFromCodeCoverage]
public class NpgsqlConnectionWrapper : IDbConnectionWrapper
{
    #region Data members

    private readonly IDbConnection connection;

    #endregion

    #region Properties

    public string ConnectionString
    {
        get => this.connection.ConnectionString;
        set => this.connection.ConnectionString = value;
    }

    public int ConnectionTimeout => this.connection.ConnectionTimeout;
    public string Database => this.connection.Database;
    public ConnectionState State => this.connection.State;

    #endregion

    #region Constructors

    public NpgsqlConnectionWrapper(IDbConnection connection)
    {
        this.connection = connection;
    }

    #endregion

    #region Methods

    public IDbCommand CreateCommand()
    {
        return this.connection.CreateCommand();
    }

    public IDbTransaction BeginTransaction()
    {
        return this.connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return this.connection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        this.connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        this.connection.Close();
    }

    public void Open()
    {
        this.connection.Open();
    }

    public void Dispose()
    {
        this.connection.Close();
        this.connection.Dispose();
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        return this.connection.QueryAsync<T>(sql, param);
    }

    public Task<int> ExecuteAsync(string sql, object? param = null)
    {
        return this.connection.ExecuteAsync(sql, param);
    }

    #endregion
}

[ExcludeFromCodeCoverage]
public class MockConnectionWrapper : IDbConnectionWrapper
{
    #region Properties

    public static List<LibraryFolder> LibraryFolders { get; private set; } = null!;

    public static List<Element> Elements { get; private set; } = null!;
    public static List<ElementLocation> Locations { get; private set; } = null!;
    public static List<ElementSet> Sets { get; private set; } = null!;
    public static List<Module> Modules { get; private set; } = null!;

    public static List<PublishedModule> PublishedModules { get; private set; } = null!;
    public static List<User> Users { get; private set; } = null!;

    private static List<AuthorizationRequest> AuthorizationRequests { get; set; } = null!;

    public static List<Model.Context> Contexts { get; private set; } = null!;

    public static List<AccessRole> Roles { get; private set; } = null!;

    public static List<Privilege> Privileges { get; private set; } = null!;

    public static List<Package> Packages { get; private set; } = null!;

    public static List<PackageFolder> PackageFolders { get; private set; } = null!;

    public static List<PackageFolderModule> PackageFoldersModule { get; private set; } = null!;

    public static List<Organization> Organizations { get; private set; } = null!;

    public static List<OrganizationPackage> OrganizationPackages { get; private set; } = null!;

    public static List<ContentRole> ContentRoles { get; private set; } = null!;

    public static List<OrganizationContentRole> OrganizationContentRoles { get; private set; } = null!;

    // Implement IDbConnection methods as a mock

    public string ConnectionString { get; set; }

    public int ConnectionTimeout => 30;
    public string Database => "MockDatabase";
    public ConnectionState State => ConnectionState.Open;

    #endregion

    #region Constructors

    public MockConnectionWrapper(IDbConnection connection)
    {
        this.ConnectionString = connection.ConnectionString;
    }

    #endregion

    #region Methods

    public IDbCommand CreateCommand()
    {
        throw new NotSupportedException("Mock implementation: CreateCommand is not supported.");
    }

    public IDbTransaction BeginTransaction()
    {
        throw new NotSupportedException("Mock implementation: BeginTransaction is not supported.");
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        throw new NotSupportedException("Mock implementation: BeginTransaction is not supported.");
    }

    public void ChangeDatabase(string databaseName)
    {
        throw new NotSupportedException("Mock implementation: ChangeDatabase is not supported.");
    }

    public void Open()
    {
        LibraryFolders = new List<LibraryFolder>
        {
            new()
            {
                LibraryFolderId = 1,
                Description = "Testing Library Folder 1",
                Name = "Test Library Folder 1"
            },
            new()
            {
                LibraryFolderId = 2,
                Description = "Testing Library Folder 2",
                Name = "Test Library Folder 2"
            },
            new()
            {
                LibraryFolderId = 3,
                Description = "Testing Library Folder 3",
                Name = "Test Library Folder 3"
            },
            new()
            {
                LibraryFolderId = 4,
                Description = "Testing Library Folder 3",
                Name = "Test Library Folder 3"
            },
            new()
            {
                LibraryFolderId = 5,
                Description = "Testing Library Folder 3",
                Name = "Test Library Folder 3"
            }
        };

        Elements = new List<Element>
        {
            new()
            {
                ElementId = 1,
                Title = "Testing Element One",
                Description = "Testing Element One",
                TypeId = 1,
                Citation = "Testing Element One",
                Content = "{ \"test\": \"test\" }",
                ExternalSource = "SOURCE One",
                Tags = Array.Empty<string>(),
                LibraryFolderId = 1
            },
            new()
            {
                ElementId = 2,
                Title = "Testing Element One",
                Description = "Testing Element One",
                TypeId = 1,
                Citation = "Testing Element One",
                Content = "{ \"test\": \"test\" }",
                ExternalSource = "SOURCE One",
                Tags = Array.Empty<string>(),
                LibraryFolderId = 4
            },
            new()
            {
                ElementId = 3,
                Title = "Testing Element One",
                Description = "Testing Element One",
                TypeId = 1,
                Citation = "Testing Element One",
                Content = "{ \"test\": \"test\" }",
                ExternalSource = "SOURCE One",
                Tags = Array.Empty<string>(),
                LibraryFolderId = 1
            },
            new()
            {
                ElementId = 4,
                Title = "Test Anchor",
                Description = "",
                TypeId = 7,
                Citation = "",
                Content = "",
                ExternalSource = "",
                Tags = Array.Empty<string>(),
                LibraryFolderId = 1
            }
        };

        Locations = new List<ElementLocation>
        {
            new()
            {
                SetLocationId = 1,
                ElementId = 2,
                Place = 0,
                IsEditable = false,
                Attributes = new SetLocationAttributes
                {
                    Width = "auto",
                    Height = "auto",
                    Alignment = "left",
                    HeadingLevel = 0
                }
            },
            new()
            {
                SetLocationId = 1,
                ElementId = 1,
                Place = 1,
                IsEditable = false,
                Attributes = new SetLocationAttributes
                {
                    Width = "400",
                    Height = "auto",
                    Alignment = "center",
                    HeadingLevel = 0
                }
            }
        };

        Sets = new List<ElementSet>
        {
            new()
            {
                SetLocationId = 1,
                ModuleId = 1,
                Place = 0,
                IsEditable = false,
                Styling = new SetStyling
                {
                    is_appendix = false,
                    is_horizontal = false,
                    has_page_break = "false"
                }
            },
            new()
            {
                SetLocationId = 2,
                ModuleId = 1,
                Place = 1,
                IsEditable = false,
                Styling = new SetStyling
                {
                    is_appendix = false,
                    is_horizontal = false,
                    has_page_break = "false"
                }
            },
            new()
            {
                SetLocationId = 3,
                ModuleId = 2,
                Place = 1,
                IsEditable = false,
                Styling = new SetStyling
                {
                    is_appendix = false,
                    is_horizontal = false,
                    has_page_break = "false"
                }
            }
        };

        Modules = new List<Module>
        {
            new()
            {
                ModuleId = 1,
                Title = "Testing Module One",
                Description = "Testing Module One",
                SurveyStartLink = "Testing Module One",
                SurveyEndLink = "Testing Module One",
                ContactTitle = "Testing Module One",
                ContactNumber = "Testing Module One",
                Thumbnail = "Testing Module One",
                Tags = Array.Empty<string>(),
                DisplayTitle = "Testing Module One",
                TemplateModuleId = 0,
                IsTemplate = false,
                PublishedTime = "PUBLISHED One",
                LibraryFolderId = 1
            },
            new()
            {
                ModuleId = 2,
                Title = "Testing Module Two",
                Description = "Testing Module Two",
                SurveyStartLink = "Testing Module Two",
                SurveyEndLink = "Testing Module Two",
                ContactTitle = "Testing Module Two",
                ContactNumber = "Testing Module Two",
                Thumbnail = "Testing Module Two",
                Tags = Array.Empty<string>(),
                DisplayTitle = "Testing Module Two",
                TemplateModuleId = 0,
                IsTemplate = false,
                PublishedTime = "PUBLISHED Two",
                LibraryFolderId = 1
            },
            new()
            {
                ModuleId = 3,
                Title = "Testing Module Three",
                Description = "Testing Module Three",
                SurveyStartLink = "Testing Module Three",
                SurveyEndLink = "Testing Module Three",
                ContactTitle = "Testing Module Three",
                ContactNumber = "Testing Module Three",
                Thumbnail = "Testing Module Three",
                Tags = Array.Empty<string>(),
                DisplayTitle = "Testing Module Three",
                TemplateModuleId = 0,
                IsTemplate = false,
                PublishedTime = "PUBLISHED Three",
                LibraryFolderId = 5
            }
        };
        PublishedModules = new List<PublishedModule>
        {
            new()
            {
                Id = 1,
                Cache = ""
            },
            new()
            {
                Id = 2,
                Cache = ""
            },
            new()
            {
                Id = 3,
                Cache = ""
            }
        };
        Users = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "Testing",
                Password = "Test"
            },
            new()
            {
                Id = 2,
                Username = "Tester",
                Password = "Test"
            }
        };

        AuthorizationRequests = new List<AuthorizationRequest>
        {
            new()
            {
                UserId = 1,
                ContextType = "library folder",
                ContextInstance = 3,
                PrivilegeName = "create"
            },
            new()
            {
                UserId = 1,
                ContextType = "library folder",
                ContextInstance = 3,
                PrivilegeName = "update"
            },
            new()
            {
                UserId = 1,
                ContextType = "library folder",
                ContextInstance = 3,
                PrivilegeName = "delete"
            },
            new()
            {
                UserId = 1,
                ContextType = "library folder",
                ContextInstance = 3,
                PrivilegeName = "read"
            },
            new()
            {
                UserId = 1,
                ContextType = "organization",
                ContextInstance = 3,
                PrivilegeName = "create"
            },
            new()
            {
                UserId = 1,
                ContextType = "organization",
                ContextInstance = 3,
                PrivilegeName = "update"
            },
            new()
            {
                UserId = 1,
                ContextType = "organization",
                ContextInstance = 3,
                PrivilegeName = "delete"
            },
            new()
            {
                UserId = 1,
                ContextType = "organization",
                ContextInstance = 3,
                PrivilegeName = "read"
            },
            new()
            {
                UserId = 1,
                ContextType = "system",
                PrivilegeName = "create"
            },
            new()
            {
                UserId = 1,
                ContextType = "system",
                PrivilegeName = "update"
            },
            new()
            {
                UserId = 1,
                ContextType = "system",
                PrivilegeName = "delete"
            },
            new()
            {
                UserId = 1,
                ContextType = "system",
                PrivilegeName = "read"
            }
        };

        Contexts = new List<Model.Context>
        {
            new()
            {
                Id = 1,
                Type = "system"
            },
            new()
            {
                Id = 2,
                Type = "library folder",
                Instance = 1,
                InstanceName = "Test Library Folder 1"
            },
            new()
            {
                Id = 3,
                Type = "library folder",
                Instance = 2,
                InstanceName = "Test Library Folder 2"
            },
            new()
            {
                Id = 4,
                Type = "library folder",
                Instance = 3,
                InstanceName = "Test Library Folder 3"
            },
            new()
            {
                Id = 5,
                Type = "library folder",
                Instance = 4,
                InstanceName = "Test Library Folder 4"
            },
            new()
            {
                Id = 6,
                Type = "library folder",
                Instance = 5,
                InstanceName = "Test Library Folder 5"
            }
        };

        Roles = new List<AccessRole>
        {
            new()
            {
                Id = 1,
                Name = "user",
                Privileges = new List<Privilege>()
            },
            new()
            {
                Id = 2,
                Name = "CAMCMS admin",
                Privileges = new List<Privilege>()
            },
            new()
            {
                Id = 3,
                Name = "staff",
                Privileges = new List<Privilege>()
            },
            new()
            {
                Id = 4,
                Name = "brand admin",
                Privileges = new List<Privilege>()
            },
            new()
            {
                Id = 5,
                Name = "admin",
                Privileges = new List<Privilege>()
            },
            new()
            {
                Id = 6,
                Name = "facilitator",
                Privileges = new List<Privilege>()
            }
        };

        Privileges = new List<Privilege>
        {
            new()
            {
                Id = 1,
                Name = "create"
            },
            new()
            {
                Id = 2,
                Name = "read"
            },
            new()
            {
                Id = 3,
                Name = "update"
            },
            new()
            {
                Id = 4,
                Name = "delete"
            },
            new()
            {
                Id = 5,
                Name = "assign"
            },
            new()
            {
                Id = 6,
                Name = "invite"
            },
            new()
            {
                Id = 7,
                Name = "post"
            }
        };
        Packages = new List<Package>
        {
            new()
            {
                CreatedAt = null,
                CreatedBy = 2,
                IsCore = true,
                Name = "Package 1",
                PackageFolders = new List<PackageFolder>(),
                PackageId = 1,
                PackageTypeId = 1,
                PublishedAt = null,
                UpdatedAt = null,
                UpdatedBy = null
            },
            new()
            {
                CreatedAt = null,
                CreatedBy = 2,
                IsCore = true,
                Name = "Package 2",
                PackageFolders = new List<PackageFolder>(),
                PackageId = 2,
                PackageTypeId = 1,
                PublishedAt = null,
                UpdatedAt = null,
                UpdatedBy = null
            }
        };
        PackageFolders = new List<PackageFolder>
        {
            new()
            {
                ContentRoleId = 1,
                CreatedBy = 2,
                CreatedAt = null,
                DisplayName = "packageFolder1",
                Editable = true,
                FolderTypeId = 1,
                FullDescription = "packageFolder1",
                OrderInParent = 0,
                PackageFolderId = 1,
                PackageFolders = new List<PackageFolder?>(),
                PackageId = 1,
                PackageFoldersModule = new List<PackageFolderModule>(),
                ShortDescription = "package f 1",
                PublishedAt = null,
                Published = false,
                ParentFolderId = null,
                UpdatedAt = null,
                UpdatedBy = null,
                Thumbnail = null
            },
            new()
            {
                ContentRoleId = 1,
                CreatedBy = 2,
                CreatedAt = null,
                DisplayName = "packageFolder2",
                Editable = true,
                FolderTypeId = 1,
                FullDescription = "packageFolder2",
                OrderInParent = 1,
                PackageFolderId = 2,
                PackageFolders = new List<PackageFolder?>(),
                PackageId = 1,
                PackageFoldersModule = new List<PackageFolderModule>(),
                ShortDescription = "package f 2",
                PublishedAt = null,
                Published = false,
                ParentFolderId = 1,
                UpdatedAt = null,
                UpdatedBy = null,
                Thumbnail = null
            },
            new()
            {
                ContentRoleId = 1,
                CreatedBy = 2,
                CreatedAt = null,
                DisplayName = "packageFolder3",
                Editable = true,
                FolderTypeId = 1,
                FullDescription = "packageFolder3",
                OrderInParent = 0,
                PackageFolderId = 3,
                PackageFolders = new List<PackageFolder?>(),
                PackageId = 2,
                PackageFoldersModule = new List<PackageFolderModule>(),
                ShortDescription = "package f 3",
                PublishedAt = null,
                Published = false,
                ParentFolderId = null,
                UpdatedAt = null,
                UpdatedBy = null,
                Thumbnail = null
            }
        };
        PackageFoldersModule = new List<PackageFolderModule>
        {
            new()
            {
                PackageFolderId = 1,
                Cache = "",
                Editable = true,
                OrderInFolder = 0,
                PackageFolderModuleId = 1,
                PublishedModuleId = 1
            },
            new()
            {
                PackageFolderId = 2,
                Cache = "",
                Editable = true,
                OrderInFolder = 0,
                PackageFolderModuleId = 2,
                PublishedModuleId = 2
            }
        };
        Organizations = new List<Organization>
        {
            new()
            {
                OrganizationId = 1,
                CreatedAt = null,
                IsActive = false,
                Name = "Org 1",
                Tags = new[] { "" },
                UpdatedAt = null
            },
            new()
            {
                CreatedAt = null,
                IsActive = true,
                Name = "Org 2",
                OrganizationId = 2,
                Tags = new[] { "" },
                UpdatedAt = null
            }
        };
        OrganizationPackages = new List<OrganizationPackage>
        {
            new()
            {
                OrganizationId = 1,
                PackageId = 1
            },
            new()
            {
                OrganizationId = 2,
                PackageId = 1
            },
            new()
            {
                OrganizationId = 1,
                PackageId = 3
            }
        };
        ContentRoles = new List<ContentRole>
        {
            new()
            {
                ContentRoleId = 1,
                Name = "content role 1"
            },
            new()
            {
                ContentRoleId = 2,
                Name = "content role 2"
            }
        };
        OrganizationContentRoles = new List<OrganizationContentRole>
        {
            new()
            {
                ContentRoleId = 1,
                CreatedAt = "",
                CreatedBy = 1,
                DisplayName = "Org content 1",
                OrganizationContentRoleId = 1,
                OrganizationId = 1,
                UpdatedAt = "",
                UpdatedBy = 1
            },
            new()
            {
                ContentRoleId = 2,
                CreatedAt = "",
                CreatedBy = 1,
                DisplayName = "Org content 2",
                OrganizationContentRoleId = 2,
                OrganizationId = 2,
                UpdatedAt = "",
                UpdatedBy = 1
            }
        };
    }

    public void Close()
    {
        throw new NotSupportedException("Mock implementation: Close is not supported.");
    }

    public void Dispose()
    {
    }

    public Task<int> ExecuteAsync(string sql, object? param = null)
    {
        if (param?.GetType() == typeof(Element) || param?.GetType() == typeof(Element))
        {
            if (sql.Contains("INSERT"))
            {
                Elements.Add((Element)param);
            }
            else if (sql.Contains("DELETE"))
            {
                Elements.Remove(Elements.Find(x => x.Equals((Element)param)) ??
                                new Element { ElementId = 0 });
            }
            else if (sql.Contains("UPDATE"))
            {
                var element = (Element)param;
                if (element.TypeId != 7)
                {
                    Elements.ForEach(x =>
                    {
                        if (x.Equals(element))
                        {
                            x.Title = element.Title;
                            x.Description = element.Description;
                            x.TypeId = element.TypeId;
                            x.Citation = element.Citation;
                            x.Content = element.Content;
                            x.ExternalSource = element.ExternalSource;
                            x.Tags = element.Tags;
                            x.CreatedAt = element.CreatedAt;
                            x.LibraryFolderId = element.LibraryFolderId;
                        }
                    });
                }
                else
                {
                    Elements.ForEach(x =>
                    {
                        if (x.Equals(element))
                        {
                            x.Title = element.Title;
                        }
                    });
                }
            }
        }
        else if (param?.GetType() == typeof(ElementLocation))
        {
            var location = (ElementLocation)param;
            if (sql.Contains("INSERT"))
            {
                Locations.Add(location);
            }
            else if (sql.Contains("DELETE") || sql.Contains("delete_set_location"))
            {
                Locations.Remove(Locations.Find(x => x.Equals(location)) ??
                                 new ElementLocation { SetLocationId = 0, ElementId = 0 });
            }
            else if (sql.Contains("UPDATE"))
            {
                Locations.ForEach(x =>
                {
                    if (x.Equals(location))
                    {
                        x.Place = location.Place;
                        x.IsEditable = location.IsEditable;
                        x.Attributes = location.Attributes;
                    }
                });
            }
            else if (sql.Contains(""))
            {
            }
        }
        else if (param?.GetType() == typeof(ElementSet))
        {
            var set = (ElementSet)param;
            if (sql.Contains("INSERT"))
            {
                Sets.Add((ElementSet)param);
            }
            else if (sql.Contains("DELETE") || sql.Contains("delete_element_set"))
            {
                Sets.Remove(Sets.Find(x => x.Equals(set)) ?? new ElementSet { SetLocationId = 0 });
            }
            else if (sql.Contains("UPDATE"))
            {
                Sets.ForEach(x =>
                {
                    if (x.Equals(set))
                    {
                        x.ModuleId = set.ModuleId;
                        x.Place = set.Place;
                        x.IsEditable = set.IsEditable;
                        x.Styling!.is_appendix = set.Styling!.is_appendix;
                        x.Styling.is_horizontal = set.Styling.is_horizontal;
                        x.Styling.has_page_break = set.Styling.has_page_break;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(Module))
        {
            var module = (Module)param;
            if (sql.Contains("INSERT"))
            {
                Modules.Add(module);
            }
            else if (sql.Contains("DELETE"))
            {
                Modules.Remove(Modules.Find(x => x.Equals(module)) ?? new Module { ModuleId = 0 });
            }
            else if (sql.Contains("UPDATE"))
            {
                if (module.CreatedAt == "delete")
                {
                    Modules.Remove(Modules.Find(x => x.Equals(module)) ?? new Module { ModuleId = 0 });
                }
                else
                {
                    Modules.ForEach(x =>
                    {
                        if (x.Equals(module))
                        {
                            x.Title = module.Title;
                            x.Description = module.Description;
                            x.SurveyStartLink = module.SurveyStartLink;
                            x.SurveyEndLink = module.SurveyEndLink;
                            x.ContactTitle = module.ContactTitle;
                            x.ContactNumber = module.ContactNumber;
                            x.Thumbnail = module.Thumbnail;
                            x.Tags = module.Tags;
                            x.DisplayTitle = module.DisplayTitle;
                            x.TemplateModuleId = module.TemplateModuleId;
                            x.IsTemplate = module.IsTemplate;
                            x.PublishedTime = module.PublishedTime;
                            x.CreatedAt = module.CreatedAt;
                            x.LibraryFolderId = module.LibraryFolderId;
                        }
                    });
                }
            }
        }
        else if (param?.GetType() == typeof(LibraryFolder))
        {
            var folder = (LibraryFolder)param;
            if (sql.Contains("INSERT"))
            {
                LibraryFolders.Add(folder);
            }
            else if (sql.Contains("DELETE"))
            {
                LibraryFolders.Remove(folder);
            }
            else if (sql.Contains("UPDATE"))
            {
                LibraryFolders.ForEach(x =>
                {
                    if (x.Equals(folder))
                    {
                        x.LibraryFolderId = folder.LibraryFolderId;
                        x.Description = folder.Description;
                        x.Name = folder.Name;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(User))
        {
            var user = (User)param;
            if (sql.Contains("INSERT"))
            {
                Users.Add(user);
            }
            else if (sql.Contains("DELETE"))
            {
                Users.Remove(Users.Find(x => x.Equals(user)) ?? new User { Id = 0 });
            }
            else if (sql.Contains("UPDATE"))
            {
                Users.ForEach(x =>
                    {
                        if (x.Equals(user))
                        {
                            x.Id = user.Id;
                            x.Username = user.Username;
                            x.Password = user.Password;
                            x.Firstname = user.Firstname;
                            x.Lastname = user.Lastname;
                            x.Email = user.Email;
                            x.Phone = user.Phone;
                        }
                    }
                );
            }
        }
        else if (param?.GetType() == typeof(LibraryFolder))
        {
            var folder = (LibraryFolder)param;
            if (sql.Contains("INSERT"))
            {
                LibraryFolders.Add(folder);
            }
            else if (sql.Contains("DELETE"))
            {
                LibraryFolders.Remove(folder);
            }
            else if (sql.Contains("UPDATE"))
            {
                LibraryFolders.ForEach(x =>
                {
                    if (x.Equals(folder))
                    {
                        x.LibraryFolderId = folder.LibraryFolderId;
                        x.Description = folder.Description;
                        x.Name = folder.Name;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(UserAccessRoleRequest))
        {
            var role = (UserAccessRoleRequest)param;
            if (Users.Find(x => x.Id == role.UserId) == null)
            {
                return Task.FromResult(0);
            }

            var rolesIEnumerable = Users.Find(x => x.Id == role.UserId)?.Roles ?? Array.Empty<UserAccessRoleRequest>();
            var roles = rolesIEnumerable.ToList();
            if (sql.Contains("INSERT"))
            {
                roles.Add(role);
            }
            else if (sql.Contains("DELETE"))
            {
                roles.RemoveAll(x => x.ContextId == role.ContextId);
            }
            else if (sql.Contains("UPDATE"))
            {
                roles.ForEach(x =>
                {
                    if (x.ContextId == role.ContextId)
                    {
                        x.AccessRoleId = role.AccessRoleId;
                    }
                });
            }

            Users.Find(x => x.Id == role.UserId).Roles = roles;
        }
        else if (param?.GetType() == typeof(PublishedModule))
        {
            var publishedModule = (PublishedModule)param;
            if (sql.Contains("INSERT"))
            {
                PublishedModules.Add(publishedModule);
            }
            else if (sql.Contains("DELETE"))
            {
                PublishedModules.RemoveAll(x => x.Id == publishedModule.Id);
            }
            else if (sql.Contains("UPDATE"))
            {
                PublishedModules.ForEach(x =>
                {
                    if (x.Id == publishedModule.Id)
                    {
                        x.Cache = publishedModule.Cache;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(Package))
        {
            var package = (Package)param;

            if (sql.Contains("INSERT"))
            {
                Packages.Add(package);
            }
            else if (sql.Contains("DELETE"))
            {
                Packages.RemoveAll(x => x.PackageId == package.PackageId);
            }
            else if (sql.Contains("UPDATE"))
            {
                if ((bool)package.IsDeleted!)
                {
                    Packages.RemoveAll(x => x.PackageId == package.PackageId);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        else if (param?.GetType() == typeof(PackageFolder))
        {
            var packageFolder = (PackageFolder)param;
            if (sql.Contains("INSERT"))
            {
                PackageFolders.Add(packageFolder);
            }
            else if (sql.Contains("DELETE"))
            {
                PackageFolders.RemoveAll(x => x.PackageFolderId == packageFolder.PackageFolderId);
            }
            else if (sql.Contains("UPDATE"))
            {
                if (packageFolder.IsDeleted == true)
                {
                    PackageFolders.RemoveAll(x => x.PackageFolderId == packageFolder.PackageFolderId);
                }
                else
                {
                    PackageFolders.ForEach(x =>
                    {
                        if (x.PackageFolderId == packageFolder.PackageFolderId)
                        {
                            x.ContentRoleId = packageFolder.ContentRoleId;
                            x.DisplayName = packageFolder.DisplayName;
                            x.Editable = packageFolder.Editable;
                            x.FolderTypeId = packageFolder.FolderTypeId;
                            x.FullDescription = packageFolder.FullDescription;
                            x.OrderInParent = packageFolder.OrderInParent;
                            x.PackageId = packageFolder.PackageId;
                            x.ParentFolderId = packageFolder.ParentFolderId;
                            x.ShortDescription = packageFolder.ShortDescription;
                            x.Thumbnail = packageFolder.Thumbnail;
                            x.UpdatedAt = packageFolder.UpdatedAt;
                            x.UpdatedBy = packageFolder.UpdatedBy;
                        }
                    });
                }
            }
            else if (sql.Contains("CALL"))
            {
                PackageFolders.ForEach(x =>
                {
                    if (x.ParentFolderId == packageFolder.PackageFolderId)
                    {
                        x.ContentRoleId = packageFolder.ContentRoleId;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(PackageFolderModule))
        {
            var packageFolderModule = (PackageFolderModule)param;
            if (sql.Contains("INSERT"))
            {
                PackageFoldersModule.Add(packageFolderModule);
            }
            else if (sql.Contains("DELETE"))
            {
                PackageFoldersModule.RemoveAll(
                    x => x.PackageFolderModuleId == packageFolderModule.PackageFolderModuleId);
            }
            else if (sql.Contains("UPDATE"))
            {
                PackageFoldersModule.ForEach(x =>
                {
                    if (x.PackageFolderModuleId == packageFolderModule.PackageFolderModuleId)
                    {
                        x.Cache = packageFolderModule.Cache;
                        x.Editable = packageFolderModule.Editable;
                        x.OrderInFolder = packageFolderModule.OrderInFolder;
                        x.PackageFolderId = packageFolderModule.PackageFolderModuleId;
                        x.PublishedModuleId = packageFolderModule.PublishedModuleId;
                    }
                });
            }
        }
        else if (param?.GetType() == typeof(Organization))
        {
            var organization = (Organization)param;
            if (sql.Contains("INSERT"))
            {
                Organizations.Add(organization);
            }
        }
        else if (param?.GetType() == typeof(OrganizationPackage))
        {
        }
        else if (param?.GetType() == typeof(OrganizationContentRole))
        {
            var organizationContentRole = (OrganizationContentRole)param;

            if (sql.Contains(SqlConstants.CreateOrganizationContentRole))
            {
                OrganizationContentRoles.Add(organizationContentRole);
            }
        }
        else
        {
            if (sql.Contains(SqlConstants.CreateOrganizationPackage))
            {
                var organizationPropertyInfo = param.GetType().GetProperty("organizationId").GetValue(param, null);
                var packagePropertyInfo = param.GetType().GetProperty("packageId").GetValue(param, null);
                var organizationId = (int)organizationPropertyInfo;
                var packageId = (int)packagePropertyInfo;
                var organizationPackage = new OrganizationPackage
                {
                    OrganizationId = organizationId,
                    PackageId = packageId
                };
                OrganizationPackages.Add(organizationPackage);
            }
            else if (sql.Contains(SqlConstants.DeleteOrganizationPackage))
            {
                var organizationPropertyInfo = param.GetType().GetProperty("organizationId").GetValue(param, null);
                var packagePropertyInfo = param.GetType().GetProperty("packageId").GetValue(param, null);
                var organizationId = (int)organizationPropertyInfo;
                var packageId = (int)packagePropertyInfo;
                var organizationPackage = new OrganizationPackage
                {
                    OrganizationId = organizationId,
                    PackageId = packageId
                };
                OrganizationPackages.Remove(organizationPackage);
            }
            else if (sql.Contains(SqlConstants.DeleteOrganizationContentRole))
            {
                var organizationContentPropertyInfo =
                    param.GetType().GetProperty("organizationContentRoleId").GetValue(param, null);
                var organizationContentId = (int)organizationContentPropertyInfo;
                var contentRole =
                    OrganizationContentRoles.Where(x => x.OrganizationContentRoleId == organizationContentId);

                OrganizationContentRoles.Remove(contentRole.ElementAt(0));
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        return Task.FromResult(0);
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        IEnumerable<T> list = null;

        if (typeof(T) == typeof(Element))
        {
            list = (IEnumerable<T>)processElementQuery(sql, param);
        }
        else if (typeof(T) == typeof(ElementLocation))
        {
            switch (sql)
            {
                case SqlConstants.SelectElementLocationsBySet:
                    var set = (ElementSet)(param ?? throw new ArgumentNullException(nameof(param)));
                    list = (IEnumerable<T>)Locations
                        .Where(x => x.SetLocationId == set.SetLocationId)
                        .ToList();
                    break;
                case SqlConstants.SelectElementLocations:
                    var element = (Element)(param ?? throw new ArgumentNullException(nameof(param)));
                    list = (IEnumerable<T>)Locations
                        .Where(x => x.ElementId == element.ElementId)
                        .ToList();
                    break;
                case SqlConstants.SelectLocationsByModule:
                    var moduleId = param?.GetType().GetProperty("ModuleId")?.GetValue(param, null);
                    var sets = Sets
                        .Where(x => x.ModuleId == (int)(moduleId ?? throw new ArgumentNullException(nameof(moduleId))))
                        .ToList();
                    list = (IEnumerable<T>)Locations
                        .Where(x => sets.Any(y => y.SetLocationId == x.SetLocationId))
                        .ToList();
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(ElementSet))
        {
            list = (IEnumerable<T>)processSetQuery(sql, param);
        }
        else if (typeof(T) == typeof(Module))
        {
            list = sql switch
            {
                SqlConstants.SelectAllModules => (IEnumerable<T>)Modules.ToList(),
                SqlConstants.SelectModuleById => (IEnumerable<T>)Modules.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("id")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return id == x.ModuleId;
                    })
                    .ToList(),
                SqlConstants.SelectAllModulesByLibraryFolderId => (IEnumerable<T>)Modules.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("id")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return id == x.LibraryFolderId;
                    })
                    .ToList(),
                SqlConstants.SelectModuleByTitle => (IEnumerable<T>)Modules.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("title")?.GetValue(param, null);
                        var title = (string)(paramId ?? throw new ArgumentNullException());
                        return title == x.Title;
                    })
                    .ToList(),
                _ => throw new InvalidCastException()
            };
        }
        else if (typeof(T) == typeof(User))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllUsers:
                    list = (IEnumerable<T>)Users;
                    break;
                case SqlConstants.SelectUser:
                    var paramId = param?.GetType().GetProperty("Id")?.GetValue(param, null);
                    var id = (int)(paramId ?? throw new ArgumentNullException());
                    var users = Users.Where(x => x.Id == id);
                    list = (IEnumerable<T>)users;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(AuthorizationRequest))
        {
            switch (sql)
            {
                case SqlConstants.SelectPrivilegeOrganization:
                case SqlConstants.SelectPrivilegeLibrary:
                case SqlConstants.SelectPrivilegeSystem:
                    var paramRequest = (AuthorizationRequest)param!;
                    var privileges = AuthorizationRequests.Where(x =>
                    {
                        var containsUser = x.UserId == paramRequest.UserId;
                        var containsType = x.ContextType!.Equals(paramRequest.ContextType);
                        var containsPrivilege = x.PrivilegeName!.Equals(paramRequest.PrivilegeName);
                        var containsInstance = true;
                        if (!paramRequest.ContextType!.Equals("system"))
                        {
                            containsInstance = x.ContextInstance == paramRequest.ContextInstance;
                        }

                        return containsUser && containsType && containsPrivilege && containsInstance;
                    });
                    list = (IEnumerable<T>)privileges;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(LibraryFolder))
        {
            list = (IEnumerable<T>)LibraryFolders.ToList();
        }
        else if (typeof(T) == typeof(UserAccessRoleRequest))
        {
            var userId = (int)(param?.GetType().GetProperty("Id")?.GetValue(param, null) ?? 0);
            list = (IEnumerable<T>)Users.Find(x => x.Id == userId)?.Roles;
        }
        else if (typeof(T) == typeof(Model.Context))
        {
            list = (IEnumerable<T>)Contexts;
        }
        else if (typeof(T) == typeof(AccessRole))
        {
            list = (IEnumerable<T>)Roles;
        }
        else if (typeof(T) == typeof(Privilege))
        {
            if (sql.Equals(SqlConstants.SelectAllPrivileges))
            {
                list = (IEnumerable<T>)Privileges;
            }
            else if (sql.Equals(SqlConstants.SelectRolePrivileges))
            {
                var roleId = (int)(param?.GetType().GetProperty("Id")?.GetValue(param, null) ?? 0);
                var rolePrivileges = Roles.Find(x => x.Id == roleId)?.Privileges;
                list = (IEnumerable<T>)rolePrivileges ?? new List<T>();
            }
        }
        else if (typeof(T) == typeof(PublishedModule))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllPublishedModule:
                    list = (IEnumerable<T>)PublishedModules;
                    break;
                case SqlConstants.SelectPublishedModuleById:
                    var publishedModules = PublishedModules.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("id")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return x.Id == id;
                    });
                    list = (IEnumerable<T>)publishedModules;
                    break;
                case SqlConstants.HasPublishedModule:
                    var modules = PublishedModules.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("ModuleId")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return id == x.Id;
                    }).ToList();
                    list = (IEnumerable<T>)modules;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(Package))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllPackages:
                    list = (IEnumerable<T>)Packages;
                    break;
                case SqlConstants.SelectPackageById:
                    var packages = Packages.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("id")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return x.PackageId == id;
                    });
                    list = (IEnumerable<T>)packages;
                    break;
                case SqlConstants.SelectPackageByName:
                    var package = Packages.Where(x =>
                    {
                        var paramName = param?.GetType().GetProperty("name")?.GetValue(param, null);
                        var name = (string)paramName ?? throw new ArgumentNullException();
                        return x.Name == name;
                    });
                    list = (IEnumerable<T>)package;
                    break;
                case SqlConstants.SelectAllPackageByOrganizationId:
                    var paramOrgId = param?.GetType().GetProperty("organizationId")?.GetValue(param, null);
                    var orgId = (int)(paramOrgId ?? throw new ArgumentNullException());
                    var packagesFromOrgPackage = OrganizationPackages.Where(x => x.OrganizationId == orgId)
                        .Select(x => x.PackageId);
                    var packagesToReturn = new List<Package>();
                    foreach (var currentPackage in Packages)
                    {
                        foreach (var id in packagesFromOrgPackage)
                        {
                            if (currentPackage.PackageId == id)
                            {
                                packagesToReturn.Add(currentPackage);
                                break;
                            }
                        }

                        packagesFromOrgPackage.ToList().Remove(currentPackage.PackageId);
                    }

                    list = (IEnumerable<T>)packagesToReturn;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(PackageFolder))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllPackageFoldersForPackage:
                    var folders = PackageFolders.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("PackageId")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return x.PackageId == id;
                    });
                    list = (IEnumerable<T>)folders;
                    break;
                case SqlConstants.SelectAllChildFolders:
                    var childFolders = PackageFolders.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("PackageFolderId")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return x.ParentFolderId == id;
                    });
                    list = (IEnumerable<T>)childFolders;
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else if (typeof(T) == typeof(PackageFolderModule))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllPackageFolderModules:
                    list = (IEnumerable<T>)PackageFoldersModule;
                    break;
                case SqlConstants.SelectAllPackageFolderModulesFromPackage:

                    var modules = PackageFoldersModule.Where(x =>
                    {
                        var paramId = param?.GetType().GetProperty("packageId")?.GetValue(param, null);
                        var id = (int)(paramId ?? throw new ArgumentNullException());
                        return x.PackageFolderId == id;
                    });
                    list = (IEnumerable<T>)modules;

                    break;
            }
        }
        else if (typeof(T) == typeof(Organization))
        {
            switch (sql)
            {
                case SqlConstants.SelectAllOrganizations:
                    list = (IEnumerable<T>)Organizations;
                    break;
                case SqlConstants.SelectOrganizationByOrganizationId:
                    var id = param?.GetType().GetProperty("id")?.GetValue(param, null);
                    list = (IEnumerable<T>)Organizations
                        .Where(x => x.OrganizationId == (int)(id ?? throw new ArgumentNullException(nameof(id))))
                        .ToList();
                    break;
            }
        }
        else if (typeof(T) == typeof(OrganizationPackage))
        {
            list = (IEnumerable<T>)OrganizationPackages;
        }
        else if (typeof(T) == typeof(ContentRole))
        {
            list = (IEnumerable<T>)ContentRoles;
        }
        else if (typeof(T) == typeof(OrganizationContentRole))
        {
            switch (sql)
            {
                case SqlConstants.SelectOrganizationContentRolesForOrganization:
                    list = new List<T>();
                    var organizationId = param?.GetType().GetProperty("organizationId")?.GetValue(param, null);
                    var orgIds = OrganizationContentRoles.Where(x => x.OrganizationId == (int)organizationId)
                        .ToList();
                    list.AsList().AddRange(orgIds as IEnumerable<T>);
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        else
        {
            switch (sql)
            {
                case SqlConstants.SelectOrganizationIdsByPackageId:
                    list = (IEnumerable<T>)new List<int>();
                    var packageId = param?.GetType().GetProperty("packageId")?.GetValue(param, null);
                    var orgIds = OrganizationPackages.Where(x => x.PackageId == (int)packageId)
                        .Select(x => x.OrganizationId).ToList();
                    list.AsList().AddRange(orgIds as IEnumerable<T>);
                    break;
                case SqlConstants.SelectContentRoleIdByFolderId:
                    list = (IEnumerable<T>)new List<int?>();
                    var packageFolderId = param?.GetType().GetProperty("folderId")?.GetValue(param, null);
                    var contentRoleId = PackageFolders.Where(x => x.PackageFolderId == (int?)packageFolderId)
                        .Select(x => x.ContentRoleId).ToList();
                    list.AsList().AddRange(contentRoleId as IEnumerable<T>);
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        return Task.FromResult(list);
    }

    private static IEnumerable<Element> processElementQuery(string query, object? param = null)
    {
        switch (query)
        {
            case SqlConstants.SelectAllElements:
                return Elements.ToList();
            case SqlConstants.SelectElementById:
                var id = param?.GetType().GetProperty("id")?.GetValue(param, null);
                return Elements
                    .Where(x => x.ElementId == (int)(id ?? throw new ArgumentNullException(nameof(id))))
                    .ToList();
            case SqlConstants.SelectAddedElement:
                return Elements.Where((x, i) => i == Elements.Count - 1).ToList();
            case SqlConstants.SelectElementsByLibraryId:
            case SqlConstants.SelectElementsByLibraryIdDisplayInfoOnly:
                id = param?.GetType().GetProperty("id")?.GetValue(param, null);
                return Elements
                    .Where(x => x.LibraryFolderId ==
                                (int)(id ?? throw new ArgumentNullException(nameof(id)))).ToList();
            case SqlConstants.SelectElementsByModule:
                var moduleId = param?.GetType().GetProperty("ModuleId")?.GetValue(param, null);
                return Elements.Where(x =>
                {
                    var sets = Sets
                        .Where(x => x.ModuleId == (int)(moduleId ?? throw new ArgumentNullException(nameof(moduleId))))
                        .ToList();

                    var locations = Locations
                        .Where(x => sets.Any(y => y.SetLocationId == x.SetLocationId))
                        .ToList();

                    return locations.Any(y => y.ElementId == x.ElementId);
                });
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static IEnumerable<ElementSet> processSetQuery(string query, object? param = null)
    {
        switch (query)
        {
            case SqlConstants.SelectElementSetsByModule:
                var moduleId = param?.GetType().GetProperty("ModuleId")?.GetValue(param, null);
                return Sets
                    .Where(x => x.ModuleId == (int)(moduleId ?? throw new ArgumentNullException(nameof(moduleId))))
                    .ToList();
            case SqlConstants.SelectElementSetById:
                var setId = param?.GetType().GetProperty("id")?.GetValue(param, null);
                return Sets.Where(x =>
                    x.SetLocationId == (int)(setId ?? throw new ArgumentNullException(nameof(setId)))).ToList();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}