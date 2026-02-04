\echo Applying 0002 privilege materialized views to schema cam_cms
-- Purpose: Creates Triggers for package level contexts and a materialized view to quickly view package level privileges.
CREATE OR REPLACE FUNCTION insert_package_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    INSERT INTO context(type, instance)
        VALUES ('package', NEW.package_id);
    RETURN NULL;
END;
$BODY$;

ALTER FUNCTION insert_package_context()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION insert_package_context()
    IS 'Adds a new package context with new id as instance.';

CREATE OR REPLACE FUNCTION delete_package_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    DELETE FROM context
        WHERE type='package' AND instance=OLD.package_id;
    RETURN NULL;
END;
$BODY$;

CREATE OR REPLACE TRIGGER trigger_package_insert_context
    AFTER INSERT
    ON package
    FOR EACH ROW
    EXECUTE FUNCTION insert_package_context();

COMMENT ON TRIGGER trigger_package_insert_context ON package
    IS 'Adds a new package context for a new package.';

CREATE OR REPLACE TRIGGER trigger_package_delete_context
    AFTER UPDATE OF is_deleted
    ON package
    FOR EACH ROW
    EXECUTE FUNCTION delete_package_context();

COMMENT ON TRIGGER trigger_package_delete_context ON package
    IS 'Removes old package context for a deleted package.';

-- Update context_information materialized view to include package level contexts.
DROP MATERIALIZED VIEW IF EXISTS context_information CASCADE;
CREATE MATERIALIZED VIEW context_information
AS
SELECT context_id, type, instance,
        CASE
            WHEN type = 'library folder' THEN library_folder.name
            WHEN type = 'organization' THEN organization.name
            WHEN type = 'package' THEN package.name
            ELSE NULL
        END name
    FROM context
    LEFT JOIN library_folder
        ON context.instance = library_folder.library_folder_id
    LEFT JOIN organization
        ON context.instance = organization.organization_id
    LEFT JOIN package
        ON context.instance = package.package_id
WITH DATA;

ALTER TABLE IF EXISTS context_information
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW context_information
    IS 'Joins needed information about a context with the individual context references that object.';

CREATE OR REPLACE FUNCTION refresh_context_information()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
	REFRESH MATERIALIZED VIEW context_information WITH DATA;
	RETURN NULL;
END;
$BODY$;

ALTER FUNCTION refresh_context_information()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION refresh_context_information()
    IS 'Refreshes the context_information few with new data.';

CREATE MATERIALIZED VIEW package_privilege AS
 SELECT
	user_access_role.app_user_id,
    context.type,
    context.instance,
    privilege.name, 
	organization_package.package_id, 
	organization_package.is_owned
  FROM user_access_role
       JOIN app_user ON user_access_role.app_user_id = app_user.app_user_id
       JOIN context ON user_access_role.context_id = context.context_id
       JOIN access_role ON user_access_role.access_role_id = access_role.access_role_id
       JOIN access_role_privilege ON access_role.access_role_id = access_role_privilege.access_role_id
       JOIN privilege ON access_role_privilege.privilege_id = privilege.privilege_id
	   JOIN context_information ON context.context_id = context_information.context_id
	   LEFT JOIN organization_package ON (organization_package.organization_id = context.instance AND context.type::text = 'organization')
  WHERE context.type::text = 'package'::text OR (organization_package.package_id IS NOT NULL AND (
      (organization_package.is_owned = true AND privilege.name NOT IN ('assign', 'invite', 'post', 'delete'))
      OR
      (organization_package.is_owned = false AND privilege.name NOT IN ('assign', 'invite', 'post', 'delete', 'update', 'create'))
  ))
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS package_privilege
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW package_privilege
    IS 'Joins user_access_role, app_user, context, access_role, access_role_privilege and privilege tables to allow viewing of privileges by user on the package level.';

REFRESH MATERIALIZED VIEW package_privilege WITH DATA;

-- Create a trigger to refresh the materialized view when the context table is manipulated.
CREATE OR REPLACE TRIGGER trigger_context_refresh_context_information
    AFTER INSERT OR DELETE OR UPDATE 
    ON context
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_context_information();

REFRESH MATERIALIZED VIEW context_information WITH DATA;

insert into db_migration (migration_id, description, script_name)
values ('0015', 'Creates Triggers for package level contexts and a materialized view to quickly view package level privileges.', '0015.sql');