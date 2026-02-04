-- Update library_privilege materialized view to include some implied privileges for organizations.
DROP MATERIALIZED VIEW IF EXISTS library_privilege CASCADE;
CREATE MATERIALIZED VIEW library_privilege
AS
 SELECT user_access_role.app_user_id,
    context.type,
    context.instance,
    privilege.name, 
	organization.library_folder_id
  FROM user_access_role
       JOIN app_user ON user_access_role.app_user_id = app_user.app_user_id
       JOIN context ON user_access_role.context_id = context.context_id
       JOIN access_role ON user_access_role.access_role_id = access_role.access_role_id
       JOIN access_role_privilege ON access_role.access_role_id = access_role_privilege.access_role_id
       JOIN privilege ON access_role_privilege.privilege_id = privilege.privilege_id 
	   LEFT JOIN organization ON (organization.organization_id = context.instance AND context.type::text = 'organization'::text)
  WHERE context.type::text = 'library folder'::text OR (organization.library_folder_id IS NOT NULL AND 
	   (privilege.name NOT IN ('assign', 'invite', 'post')))
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS library_privilege
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW library_privilege
    IS 'Joins user_access_role, app_user, context, access_role, access_role_privilege, and privilege tables to view users privileges by library folder.';

CREATE MATERIALIZED VIEW privilege_summary
AS
  SELECT 
    user_access_role.app_user_id,
    MIN(user_access_role.access_role_id) AS access_role_id,
    BOOL_OR(library_privilege.name = 'read'::text) AS library_read,
    BOOL_OR(organization_privilege.name = 'read'::text) AS organization_read,
    BOOL_OR(system_privilege.name = 'read'::text) AS system_read,
    BOOL_OR(package_privilege.name = 'read'::text) AS package_read
    FROM user_access_role
    LEFT JOIN library_privilege 
    ON user_access_role.app_user_id = library_privilege.app_user_id
    LEFT JOIN organization_privilege 
    ON user_access_role.app_user_id = organization_privilege.app_user_id
    LEFT JOIN system_privilege 
    ON user_access_role.app_user_id = system_privilege.app_user_id
    LEFT JOIN package_privilege
    ON user_access_role.app_user_id = package_privilege.app_user_id
    GROUP BY 
        user_access_role.app_user_id
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS privilege_summary
    OWNER TO CURRENT_USER;

CREATE OR REPLACE FUNCTION refresh_privilege()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
--     COST NULL
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    REFRESH MATERIALIZED VIEW library_privilege WITH DATA;
    REFRESH MATERIALIZED VIEW organization_privilege WITH DATA;
    REFRESH MATERIALIZED VIEW system_privilege WITH DATA;
    REFRESH MATERIALIZED VIEW package_privilege WITH DATA;
    REFRESH MATERIALIZED VIEW privilege_summary WITH DATA;
    RETURN NULL;
END;
$BODY$;

INSERT INTO db_migration (migration_id, description, script_name)
VALUES ('0018', 'Update library_privilege materialized view to include some implied privileges for organizations.', '0018.sql');