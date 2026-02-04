namespace CAMCMSServer.Utils;

public class SqlConstants
{
    #region Data members

    #region content Role

    public const string GetAllContentRoles = "SELECT * FROM cam_cms.content_role ORDER BY content_role_id ASC";

    public const string CreateContentRole = "INSERT INTO content_role (name, archetype_id) VALUES (@Name, @ArchetypeId) RETURNING *";

    public const string GetAllRoleArchetypes = "SELECT archetype_id, name FROM cam_cms.archetype";

    #endregion

    #endregion

    #region Users

    public const string SelectAllUsers = "SELECT * FROM cam_cms.app_user";

    public const string SelectAllTempUsers = "SELECT * FROM cam_cms.temp_user";

    public const string SelectRolesForUser = """
                                             SELECT *
                                             FROM cam_cms.user_access_role as user_role,
                                             	cam_cms.access_role as roles,
                                             	cam_cms.context as context
                                             WHERE user_role.access_role_id = roles.access_role_id
                                             	AND user_role.context_id = context.context_id
                                             	AND user_role.app_user_id =@Id
                                             """;

    public const string SelectUser = "SELECT * FROM cam_cms.app_user WHERE app_user_id = @Id";

    public const string SelectUserByUsername = "SELECT * FROM cam_cms.app_user WHERE username = @Username";

    public const string InsertUser = @"
                                        INSERT INTO app_user (username, firstname, lastname, email, phone, password)
                                        VALUES (@Username, @Firstname, @Lastname, @Email, @Phone, @Password);
                                     ";

    public const string InsertTempUser = @"
                                            INSERT INTO temp_user (app_user_id, invitation_id, username, firstname, lastname, phone, is_deleted, email)
                                            VALUES (@AppUserId, @InvitationId, @Username, @Firstname, @Lastname, @Phone, @IsDeleted, @Email);
                                         ";

    public const string UpdateUser = """
                                        UPDATE app_user
                                        SET
                                            username = @Username,
                                            password = @Password,
                                            email = @Email,
                                            firstname = @Firstname,
                                            lastname = @Lastname,
                                            phone = @Phone
                                        WHERE
                                            app_user_id = @Id
                                     """;

    public const string UpdateTempUser = """
                                            UPDATE temp_user
                                            SET
                                                username = @Username,
                                                email = @Email,
                                                firstname = @Firstname,
                                                lastname = @Lastname,
                                                phone = @Phone,
                                                linked_app_user_id = @LinkedAppUserId,
                                                is_deleted = @IsDeleted
                                            WHERE
                                                temp_user_id = @TempId
                                         """;

    public const string DeleteUser = "DELETE FROM app_user WHERE app_user_id = @Id";

    public const string SoftDeleteUser = "UPDATE app_user SET is_deleted = TRUE WHERE app_user_id = @Id";

    public const string SoftDeleteTempUser = "UPDATE temp_user SET is_deleted = TRUE WHERE temp_user_id = @Id";

    public const string SelectTempUser = "SELECT * FROM temp_user WHERE temp_user_id = @Id";

    #endregion

    #region Authorization

    public const string SelectAllContexts = "SELECT * FROM cam_cms.context_information";

    public const string SelectAllRoles = "SELECT * FROM cam_cms.access_role";

    public const string SelectRolePrivileges = """
                                               SELECT *
                                               FROM cam_cms.access_role_privilege, cam_cms.privilege
                                               WHERE cam_cms.access_role_privilege.privilege_id = cam_cms.privilege.privilege_id
                                               	AND cam_cms.access_role_privilege.access_role_id = @Id
                                               """;

    public const string SelectAllPrivileges = "SELECT * FROM cam_cms.privilege";

    public const string SelectPrivilegeSystem = """
                                                SELECT *
                                                FROM cam_cms.system_privilege
                                                WHERE app_user_id = @UserId AND name = @PrivilegeName
                                                """;

    public const string SelectPrivilegeOrganization = """
                                                      SELECT *
                                                      FROM cam_cms.organization_privilege
                                                      WHERE app_user_id = @UserId AND instance = @ContextInstance AND name = @PrivilegeName
                                                      """;

    public const string SelectPrivilegeLibrary = """
                                                 SELECT *
                                                 FROM cam_cms.library_privilege
                                                 WHERE (app_user_id = @UserId AND instance = @ContextInstance AND name = @PrivilegeName AND type::text = 'library folder') 
                                                 OR (library_folder_id = @ContextInstance AND app_user_id = @UserId AND name = @PrivilegeName)
                                                 """;

    public const string SelectPrivilegePackage = """
                                                 SELECT *
                                                 FROM cam_cms.package_privilege
                                                 WHERE (instance = @ContextInstance AND app_user_id = @UserId AND name = @PrivilegeName AND type::text = 'package'::text)
                                                 OR (package_id = @ContextInstance AND app_user_id = @UserId AND name = @PrivilegeName)
                                                 """;

    public const string InsertContext = """
                                        INSERT INTO cam_cms.user_access_role(app_user_id, context_id, access_role_id, created_by)
                                        VALUES (@UserId, @ContextId, @AccessRoleId, @CreatedBy)
                                        """;

    public const string UpdateContext = """
                                        UPDATE cam_cms.user_access_role
                                        SET access_role_id=@AccessRoleId
                                        WHERE app_user_id=@UserId AND context_id=@ContextId
                                        """;

    public const string DeleteContext = """
                                        DELETE FROM cam_cms.user_access_role
                                        WHERE app_user_id=@UserId AND context_id=@ContextId
                                        """;

    public const string GetOrganizationContext = """
                                                    SELECT context_id 
                                                    FROM context 
                                                    WHERE type = 'organization' AND instance = @organizationId;
                                                """;

    public const string SelectRoleById = "SELECT * FROM cam_cms.access_role WHERE access_role_id = @Id";

    public const string SelectUserReadPrivileges =
        "SELECT * FROM cam_cms.privilege_summary WHERE app_user_id = @UserId";

    #endregion

    #region Package

    public const string SelectAllPackages = "SELECT * FROM \"package\" WHERE is_deleted=False";

    public const string SelectAuthorizedPackages = """
                                                   SELECT *
                                                   FROM package
                                                   WHERE is_deleted=False
                                                   AND (package_id in (SELECT instance FROM package_privilege WHERE app_user_id = @userId AND type::text = 'package'::text)
                                                   OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2)
                                                   Or package_id in (SELECT package_id from package_privilege WHERE app_user_id = @userId))
                                                   """;

    public const string SelectPackageById = "SELECT * FROM \"package\" WHERE package_id = @id AND is_deleted=False AND " +
                                            "(package_id in (SELECT instance FROM package_privilege WHERE app_user_id = @userId) OR " +
                                            "@userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2) OR package_id in (SELECT package_id from package_privilege WHERE app_user_id = @userId)) ";
    public const string SelectPackageByName = "SELECT * FROM \"package\" WHERE \"name\" = @name AND is_deleted=False";

    public const string InsertPackage = """
                                            INSERT INTO "package"(name, is_core, created_by)
                                             VALUES (@Name, @IsCore, @CreatedBy);
                                        """;

    public const string DeletePackage = """
                                            UPDATE "package" SET
                                                is_deleted=true
                                            WHERE package_id=@PackageId
                                        """;

    public const string SelectPackageIdFromFolder = "SELECT package_id FROM package_folder WHERE package_folder_id = @PackageFolderId";

    #endregion

    #region PackageFolder

    public const string SelectAllPackageFoldersForPackage =
        """
        SELECT *
        FROM package_folder
        LEFT JOIN content_role ON package_folder.content_role_id = content_role.content_role_id
        WHERE (package_folder.content_role_id IS NOT NULL OR package_folder.content_role_id IS NULL)
        AND package_id = @PackageId
        AND is_deleted = FALSE;
        """;

    public const string InsertPackageFolder = """
                                                  INSERT INTO cam_cms.package_folder(folder_type_id, display_name, full_description, short_description, thumbnail,
                                              								  content_role_id, package_id, editable, parent_folder_id, order_in_parent, created_by)
                                                   VALUES (@FolderTypeId, @DisplayName, @FullDescription, @ShortDescription, @Thumbnail,
                                                             @ContentRoleId, @PackageId, @Editable, @ParentFolderId, @OrderInParent, @CreatedBy);
                                              """;

    // TODO: Make children delete?
    public const string DeletePackageFolder = """
                                                  UPDATE package_folder SET
                                                      is_deleted=true
                                                  WHERE package_folder_id=@PackageFolderId
                                              """;

    public const string UpdatePackageFolder = """
                                              UPDATE cam_cms.package_folder set
                                                folder_type_id = @FolderTypeId,
                                                display_name = @DisplayName,
                                                full_description = @FullDescription,
                                                short_description = @ShortDescription,
                                                thumbnail = @Thumbnail,
                                                content_role_id = @ContentRoleId,
                                                package_id = @PackageId,
                                                published = @Published,
                                                editable = @Editable,
                                                parent_folder_id = @ParentFolderId,
                                                order_in_parent = @OrderInParent
                                              where package_folder_id = @PackageFolderId
                                              """;

    public const string SelectPackageFolderById =
        "SELECT * FROM package_folder WHERE package_folder_id = @PackageFolderId";

    public const string SelectPackageFoldersInTopLevel =
        "select * from cam_cms.package_folder as folder where folder.package_id = @PackageId and folder.is_deleted = false and folder.parent_folder_id is null";

    public const string SelectPackageFoldersInParentFolder =
        "select * from cam_cms.package_folder as folder where folder.package_id = @PackageId and folder.is_deleted = false and folder.parent_folder_id =@ParentFolderId";

    public const string SelectContentRoleIdByFolderId =
        "SELECT content_role_id FROM cam_cms.package_folder WHERE package_folder_id = @folderId AND is_deleted = FALSE";

    public const string SelectAllChildFolders =
        """
        SELECT *
        FROM package_folder LEFT JOIN content_role ON package_folder.content_role_id = content_role.content_role_id
        WHERE parent_folder_id = @PackageFolderId AND is_deleted = FALSE
        """;

    public const string UpdateAllChildFoldersContentRoleId =
        "CALL cam_cms.alter_folder_and_child_folder_content_roles(@PackageFolderId)";

    #endregion

    #region PackageFolderModule

    public const string SelectAllPackageFolderModules =
        "SELECT folder_module.package_folder_module_id, folder_module.package_folder_id,folder_module.published_module_id, folder_module.order_in_folder, folder_module.editable, module.cache  from cam_cms.package_folder_module as folder_module, cam_cms.published_module as module where folder_module.published_module_id = module.published_module_id ";

    public const string CreatePackageFolderModule = """
                                                    INSERT into cam_cms.package_folder_module(package_folder_id, published_module_id, order_in_folder, editable)
                                                    values(@PackageFolderId,@PublishedModuleId,@OrderInFolder,@Editable)
                                                    """;

    public const string SelectAllPackageFolderModulesFromPackage =
        """
                SELECT folder_module.*, module.cache from cam_cms.package_folder as package_folder, cam_cms.package_folder_module as folder_module, cam_cms.published_module as module
        where package_folder.package_folder_id = folder_module.package_folder_id and folder_module.published_module_id = module.published_module_id
        and package_folder.package_id = @PackageId
        """;

    public const string DeletePackageFolderModuleById =
        "DELETE from cam_cms.package_folder_module as module where module.package_folder_module_id = @packageFolderModuleId";

    public const string UpdatePackageFolderModule = """
                                                    UPDATE cam_cms.package_folder_module as module set
                                                    package_folder_id = @PackageFolderId,
                                                    published_module_id = @PublishedModuleId,
                                                    order_in_folder = @OrderInFolder,
                                                    editable = @Editable
                                                    where module.package_folder_module_id = @PackageFolderModuleId


                                                    """;

    public const string SelectAllPackageFolderModulesForPackageFolder = @"
    SELECT 
        pfm.package_folder_module_id, 
        pfm.package_folder_id, 
        pfm.published_module_id, 
        pfm.order_in_folder, 
        pfm.editable, 
        pm.cache 
    FROM 
        package_folder_module pfm 
    JOIN 
        published_module pm 
    ON 
        pfm.published_module_id = pm.published_module_id 
    WHERE 
        pfm.package_folder_id = @PackageFolderId
    ";

    #endregion

    #region Module

    public const string SelectAllModules = "SELECT * FROM module WHERE is_deleted=False";
    public const string SelectModuleById = "SELECT * FROM module WHERE module_id = @id AND is_deleted=False";
    public const string SelectModuleByTitle = "SELECT * FROM module WHERE title = @title AND is_deleted=False";

    public const string SelectAllModulesByLibraryFolderId =
        "SELECT * FROM module WHERE library_folder_id = @id AND is_deleted=False";

    public const string InsertModule = """
                                           INSERT INTO Module(title, description, survey_start_link, survey_end_link, contact_title, contact_number, thumbnail, tags, display_title, template_module_id, is_template, library_folder_id, created_by)
                                           VALUES (@Title,@Description,@SurveyStartLink,@SurveyEndLink,@ContactTitle,@ContactNumber,@Thumbnail,@Tags,@DisplayTitle,@TemplateModuleId,@IsTemplate, @LibraryFolderId, @CreatedBy)
                                       """;

    public const string UpdateModule = """
                                           UPDATE module SET
                                               title=@Title,
                                               description=@Description,
                                               survey_start_link=@SurveyStartLink,
                                               survey_end_link=@SurveyEndLink,
                                               contact_title=@ContactTitle,
                                               contact_number=@ContactNumber,
                                               thumbnail=@Thumbnail,
                                               tags=@Tags,
                                               display_title=@DisplayTitle,
                                               template_module_id=@TemplateModuleId,
                                               is_template=@IsTemplate,
                                               library_folder_id=@LibraryFolderId,
                                           WHERE module_id=@ModuleId
                                       """;

    public const string UpdateModuleLibraryId = """
                                                    UPDATE module SET
                                                        library_folder_id=@LibraryFolderId
                                                    WHERE module_id=@ModuleId
                                                """;

    /** TODO: Don't delete, just update is_deleted column, figure out how to handle this with library folder
     * UPDATE module SET
     * is_deleted=true
     * WHERE module_id=@ModuleId
     */
    public const string DeleteModule = """
                                           DELETE FROM module WHERE module_id = @ModuleId
                                       """;

    public const string HasPublishedModule = "SELECT * FROM published_module WHERE published_module_id = @ModuleId";

    public const string SelectModuleByElementLocation = """
                                                        SELECT module.*
                                                            FROM cam_cms.module, cam_cms.element_set, cam_cms.set_location
                                                            WHERE cam_cms.module.module_id = cam_cms.element_set.module_id
                                                                AND cam_cms.element_set.element_set_id = cam_cms.set_location.element_set_id
                                                                AND cam_cms.set_location.element_id = @ElementId
                                                                AND cam_cms.set_location.element_set_id = @SetLocationId
                                                        """;

    #endregion

    #region PublishedModule

    public const string InsertPublishedModule = """
                                                INSERT INTO published_module(published_module_id,cache)
                                                VALUES(@Id,CAST(@Cache AS json))
                                                """;

    public const string DeletePublishedModule = """
                                                DELETE FROM published_module WHERE published_module_id = @Id
                                                """;

    public const string SelectAllPublishedModule = """
                                                    SELECT * FROM published_module
                                                   """;

    public const string SelectPublishedModuleById = """
                                                    SELECT * FROM published_module WHERE published_module_id = @Id
                                                    """;

    public const string UpdatePublishedModule = """
                                                UPDATE published_module SET
                                                    cache = CAST(@Cache AS json)
                                                    WHERE published_module_id=@Id
                                                """;

    public const string SelectLibraryFolderIdFromPublishedModuleId = """
                                                                     SELECT library_folder_id
                                                                     FROM module
                                                                     WHERE module_id = @publishedModuleId
                                                                     """;

    #endregion

    #region Sets

    public const string SelectElementSetsByModule = "SELECT * FROM element_set WHERE module_id = @ModuleId";
    public const string SelectElementSetById = "SELECT * FROM element_set WHERE element_set_id = @id";

    public const string InsertElementSet = """
                                               INSERT INTO element_set (module_id,order_in_module,editable,set_attribute)
                                               VALUES (@ModuleId,@Place,@IsEditable,CAST(@StylingJson AS json))
                                           """;

    public const string UpdateElementSet = """
                                               UPDATE element_set SET
                                                   order_in_module=@Place,
                                                   editable=@IsEditable,
                                                   set_attribute=CAST(@StylingJson AS json)
                                               WHERE element_set_id=@SetLocationId
                                           """;

    public const string DeleteElementSet = "DELETE FROM element_set WHERE element_set_id=@SetLocationId";

    public const string SelectLibraryFolderIdFromElementSet = """
                                                              SELECT library_folder_id
                                                              FROM module
                                                              WHERE module_id = (
                                                                  SELECT module_id
                                                                  FROM element_set
                                                                  WHERE element_set_id = @SetLocationId
                                                              )
                                                              """;

    #endregion

    #region ElementLocations

    public const string SelectElementLocationsBySet =
        "SELECT * FROM set_location WHERE element_set_id = @SetLocationId";

    public const string SelectElementLocations = "SELECT * FROM set_location WHERE element_id = @ElementId;";

    public const string SelectLocationsByModule = """
                                                  SELECT cam_cms.set_location.*
                                                  FROM cam_cms.element_set, cam_cms.set_location
                                                  WHERE cam_cms.set_location.element_set_id = cam_cms.element_set.element_set_id
                                                      AND cam_cms.element_set.module_id = @ModuleId
                                                  """;

    public const string InsertElementLocation = """
                                                    INSERT INTO set_location (element_set_id,element_id,order_in_set,editable,location_attribute)
                                                    VALUES (@SetLocationId,@ElementId,@Place,@IsEditable,CAST(@LocationAttributeJson AS json));
                                                """;

    public const string UpdateElementLocation = """
                                                    UPDATE set_location SET
                                                        order_in_set=@Place,
                                                        editable=@IsEditable,
                                                        location_attribute=CAST(@LocationAttributeJson AS json)
                                                    WHERE element_set_id=@SetLocationId AND element_id=@ElementId;
                                                """;

    public const string UpdateLocationElement = """
                                                    UPDATE set_location SET
                                                        element_id=@ElementId,
                                                        editable=@IsEditable,
                                                        location_attribute=CAST(@LocationAttributeJson AS json)
                                                    WHERE element_set_id=@SetLocationId AND order_in_set=@Place;
                                                """;

    public const string UpdateLocationSet = """
                                                UPDATE set_location SET element_set_id=@NewId, order_in_set=@Place
                                                WHERE element_set_id=@CurrentId AND element_id=@ElementId;
                                            """;

    public const string UpdateElementLocationAttributes =
        """UPDATE set_location SET  location_attribute=CAST(@LocationAttributeJson AS json) WHERE element_set_id=@SetLocationId AND element_id=@ElementId;""";

    public const string DeleteElementLocation =
        "CALL cam_cms.delete_set_location(@SetLocationId, @ElementId)";

    public const string DeleteElementLocations = "DELETE FROM set_location WHERE element_id = @ElementId";

    public const string SelectParentModuleLibraryFolderIdFromElementLocation = """
                                                                               SELECT library_folder_id
                                                                               FROM module
                                                                               WHERE module_id = (
                                                                                   SELECT module_id
                                                                                   FROM element_set
                                                                                   WHERE element_set_id = @SetLocationId
                                                                               )
                                                                               """;

    #endregion

    #region Elements

    public const string SelectAllElements = "SELECT * FROM Element";
    public const string SelectElementById = "SELECT * FROM Element WHERE element_id = @id";
    public const string SelectElementsByLibraryId = "SELECT * FROM Element WHERE library_folder_id = @id";

    public const string SelectElementsByModule = """
                                                 SELECT *
                                                 FROM cam_cms.element
                                                 WHERE cam_cms.element.element_id IN (
                                                      SELECT element_id
                                                      FROM cam_cms.element_set, cam_cms.set_location
                                                      WHERE cam_cms.set_location.element_set_id = cam_cms.element_set.element_set_id
                                                 	        AND cam_cms.element_set.module_id = @ModuleId);
                                                 """;

    public const string SelectElementsByLibraryIdDisplayInfoOnly =
        "SELECT element_id,title,library_folder_id,element_type_id FROM Element WHERE library_folder_id = @id";

    public const string SelectAddedElement = "SELECT * FROM cam_cms.element ORDER BY element_id DESC LIMIT 1";

    public const string InsertElement = """
                                            INSERT INTO cam_cms.element(title,description,element_type_id,citation,tags,content,library_folder_id, created_by)
                                            VALUES (@Title,@Description,@TypeId,@Citation,@Tags,CAST(@content AS json),@LibraryFolderId, @CreatedBy)
                                        """;

    public const string UpdateElement = """
                                            UPDATE Element SET
                                                 title=@Title,
                                                 description=@Description,
                                                 element_type_id=@TypeId,
                                                 citation=@Citation,
                                                 tags=@Tags,
                                                 updated_at=NOW(),
                                                 updated_by=@UpdatedBy,
                                                 content=CAST(@content AS json),
                                                 library_folder_id=@LibraryFolderId
                                            WHERE element_id=@ElementId
                                        """;

    public const string UpdateElementTitle = """
                                             UPDATE Element SET
                                                title=@Title
                                             WHERE element_id=@ElementID
                                             """;

    public const string UpdateElementLibraryFolderId =
        "UPDATE Element SET library_folder_id=@LibraryFolderId WHERE element_id=@ElementId";

    public const string
        DeleteElement =
            "DELETE FROM Element WHERE element_id = @ElementId"; // TODO: Don't delete, just update is_deleted column

    #endregion

    #region LibraryFolder

    public const string SelectAllLibraryFolders = "SELECT * FROM library_folder";

    public const string InsertLibraryFolder = """
                                              INSERT INTO cam_cms.library_folder(name,description,created_by)
                                              VALUES (@Name,@Description,@CreatedBy)
                                              """;

    public const string DeleteLibraryFolderById =
        "DELETE FROM library_folder WHERE library_folder_id = @LibraryFolderId";

    public const string SelectAuthorizedLibraryFolders = """
                                                         SELECT *
                                                         FROM cam_cms.library_folder
                                                         WHERE library_folder_id in (SELECT instance FROM cam_cms.library_privilege WHERE app_user_id = @userId AND type::text = 'library folder')
                                                         OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2)
                                                         OR library_folder_id in (SELECT library_folder_id from cam_cms.library_privilege WHERE app_user_id = @userId)
                                                         """;

    #endregion

    #region Organization

    public const string CreateOrganization =
        "INSERT INTO cam_cms.Organization(name, is_active, tags) VALUES (@Name, @IsActive, @Tags) RETURNING organization_id";

    public const string GetOrganizationByName = """
                                                SELECT organization_id 
                                                FROM cam_cms.Organization 
                                                WHERE name = @Name 
                                                ORDER BY organization_id DESC 
                                                LIMIT 1;
                                                """;

    public const string UserCanCreateForOrganization =
        "SELECT true FROM cam_cms.organization_privilege WHERE (instance = @id AND app_user_id = @userId AND name = 'create') OR userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2)";

    public const string SelectAllOrganizations = "SELECT * FROM cam_cms.Organization";

    public const string SelectOrganizationByOrganizationId =
        "SELECT * FROM cam_cms.Organization where Organization.organization_id = @id AND " +
        "(organization_id in (SELECT instance FROM cam_cms.organization_privilege WHERE app_user_id = @userId and name = 'read') " +
        "OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2))";

    public const string SelectAuthorizedOrganizationsByUserId =
        "SELECT * FROM cam_cms.Organization WHERE (organization_id in (SELECT instance FROM cam_cms.organization_privilege WHERE app_user_id = @userId) " +
        "OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2))";

    public const string DeleteUserAccessRole = """
                                                DELETE FROM user_access_role
                                                WHERE context_id IN (
                                                    SELECT context_id
                                                    FROM context
                                                    WHERE type = 'organization'
                                                    AND instance = @OrganizationId
                                                );
                                                """;

    public const string DeleteOrgContext = """
                                        DELETE FROM context
                                        WHERE type = 'organization'
                                        AND instance = @OrganizationId;
                                        """;

    public const string DeleteOrgContentRoleMapper = """
                                                     DELETE FROM organization_content_role_mapper_v2
                                                     WHERE organization_content_role_id IN (
                                                         SELECT organization_content_role_id
                                                         FROM organization_content_role
                                                         WHERE organization_id = @OrganizationId
                                                     );
                                                     """;

    public const string DeleteOrganizationContentRoles = """
                                                         DELETE FROM organization_content_role
                                                         WHERE organization_id = @OrganizationId;
                                                         """;

    public const string UnpublishOrganizationPackages = """
                                                        DELETE FROM organization_package
                                                        WHERE organization_id = @OrganizationId;
                                                        """;

    public const string DeleteOrganization = @"
        DELETE FROM cam_cms.organization 
        WHERE organization_id = @OrganizationId;
    ";
    public const string CreateOrgLibraryFolder = @"
        INSERT INTO cam_cms.library_folder(name, description, created_by)
        VALUES (@Name, @Description, @CreatedBy)
        RETURNING library_folder_id";

    public const string GetLibraryFolderByName = @"
        SELECT library_folder_id
        FROM cam_cms.library_folder
        WHERE name = @Name
        ORDER BY library_folder_id DESC
        LIMIT 1";

    public const string CreateOrgPackage = @"
        INSERT INTO cam_cms.package(name, is_core, created_by, is_deleted)
        VALUES (@Name, @IsCore, @CreatedBy,  @IsDeleted)
        RETURNING package_id";

    public const string GetPackageByName = @"
        SELECT package_id
        FROM cam_cms.package
        WHERE name = @Name
        ORDER BY package_id DESC
        LIMIT 1";

    public const string InsertOrganizationPackage = @"
        INSERT INTO cam_cms.organization_package(organization_id, package_id, is_owned)
        VALUES (@OrganizationId, @PackageId, @IsOwned)";

    public const string UpdateOrganizationLibraryFolder = @"
        UPDATE cam_cms.organization
        SET library_folder_id = @LibraryFolderId
        WHERE organization_id = @OrganizationId";



    #endregion

    #region OrganizationPackage

    public const string SelectOrganizationIdsByPackageId =
        "SELECT organization_id FROM cam_cms.organization_package WHERE package_id = @packageId AND (organization_id in (SELECT instance FROM cam_cms.organization_privilege WHERE app_user_id = @userId)) OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2) ";

    public const string CreateOrganizationPackage =
        "INSERT INTO cam_cms.organization_package(organization_id,package_id) VALUES (@organizationId,@packageId)";

    public const string DeleteOrganizationPackage =
        "DELETE FROM cam_cms.organization_package WHERE organization_id=@organizationId AND package_id = @packageId";

    public const string SelectAllPackageByOrganizationId =
        """
        SELECT package.*  FROM cam_cms.package AS package join cam_cms.organization_package AS package_org ON package.package_id = package_org.package_id WHERE package_org.organization_id = @organizationId
        AND ((organization_id in (SELECT instance FROM cam_cms.organization_privilege WHERE app_user_id = @userId)) OR @userId in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2))
        """;

    #endregion

    #region OrganizationContentRole
    
    public const string CreateOrganizationContentRole =
        "CALL cam_cms.create_organization_content_role_v2(@OrganizationId, @DisplayName, @CreatedBy, @ArchetypeIds)";

    public const string SelectOrganizationContentRolesForOrganization =
        "SELECT cam_cms.organization_content_role.*, organization_content_role_mapper_v2.content_role_ids " +
        "FROM cam_cms.organization_content_role " +
        "INNER JOIN cam_cms.organization_content_role_mapper_v2 " +
        "ON cam_cms.organization_content_role.organization_content_role_id = cam_cms.organization_content_role_mapper_v2.organization_content_role_id " +
        "WHERE cam_cms.organization_content_role.organization_id = @organizationId;";

    public const string DeleteOrganizationContentRole = """
        DELETE FROM cam_cms.organization_content_role_mapper_v2
        WHERE organization_content_role_id = @OrganizationContentRoleId;

        DELETE FROM cam_cms.organization_content_role
        WHERE organization_content_role_id = @OrganizationContentRoleId;
    """;

    public const string SelectArchetypeIdsForContentRole =
        "SELECT content_role_ids " +
        "FROM cam_cms.organization_content_role_mapper_v2 " +
        "WHERE organization_content_role_id = @organizationContentRoleId;";

    public const string UserCanCreateOrganizationContentRole =
        "SELECT true FROM cam_cms.organization_privilege WHERE (instance = @OrganizationId AND app_user_id = @CreatedBy AND name = 'create') OR @CreatedBy in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2)";

    public const string UserCanDeleteOrganizationContentRole =
        "SELECT true FROM cam_cms.organization_privilege WHERE (instance = @OrganizationId AND app_user_id = @CreatedBy AND name = 'delete') OR @CreatedBy in (SELECT app_user_id from cam_cms.user_access_role where access_role_id = 2)";

    #endregion

    #region Invitations

    public const string InsertInvitation = @"
        INSERT INTO invitation (summary, organization_id, start_datetime, end_datetime, module_view_limit, created_at, updated_at)
        VALUES (@Summary, @OrganizationId, @StartDatetime, @EndDatetime, @ModuleViewLimit, @CreatedAt, @UpdatedAt)";

    public const string SelectInvitationById = @"
        SELECT * FROM invitation WHERE invitation_id = @Id";

    public const string SelectAllInvitations = @"
        SELECT * FROM invitation";

    public const string UpdateInvitation = @"
        UPDATE invitation
        SET summary = @Summary,
            organization_id = @OrganizationId,
            start_datetime = @StartDatetime,
            end_datetime = @EndDatetime,
            module_view_limit = @ModuleViewLimit,
            created_at = @CreatedAt,
            updated_at = @UpdatedAt
        WHERE invitation_id = @InvitationId";

    public const string DeleteInvitation = @"
        DELETE FROM invitation WHERE invitation_id = @Id";

    #endregion
}