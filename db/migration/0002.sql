\echo Applying 0002 privilege materialized views to schema :"cam_cms"
SET search_path TO cam_cms, public;

-- Materialized view to store privileges for users at the library level.
CREATE MATERIALIZED VIEW library_privilege
AS
 SELECT user_access_role.app_user_id,
    context.type,
    context.instance,
    privilege.name
   FROM user_access_role,
    app_user,
    context,
    access_role,
    access_role_privilege,
    privilege
  WHERE user_access_role.app_user_id = app_user.app_user_id 
  AND user_access_role.context_id = context.context_id 
  AND user_access_role.access_role_id = access_role.access_role_id 
  AND access_role.access_role_id = access_role_privilege.access_role_id 
  AND access_role_privilege.privilege_id = privilege.privilege_id 
  AND context.type::text = 'library folder'::text
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS library_privilege
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW library_privilege
    IS 'Joins user_access_role, app_user, context, access_role, access_role_privilege, and privilege tables to view users privileges by library folder.';

-- Materialized view to store privileges for users at the organization level.
CREATE MATERIALIZED VIEW organization_privilege
AS
 SELECT user_access_role.app_user_id,
    context.type,
    context.instance,
    privilege.name
   FROM user_access_role,
    app_user,
    context,
    access_role,
    access_role_privilege,
    privilege
  WHERE user_access_role.app_user_id = app_user.app_user_id 
  AND user_access_role.context_id = context.context_id 
  AND user_access_role.access_role_id = access_role.access_role_id 
  AND access_role.access_role_id = access_role_privilege.access_role_id 
  AND access_role_privilege.privilege_id = privilege.privilege_id 
  AND context.type::text = 'organization'::text
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS organization_privilege
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW organization_privilege
    IS 'Joins user_access_role, app_user, context, access_role, access_role_privilege, and privilege tables to view users privileges by organization.';

-- Materialized view to store privileges for users at the system level.
CREATE MATERIALIZED VIEW system_privilege
AS
 SELECT user_access_role.app_user_id,
    context.type,
    context.instance,
    privilege.name
   FROM user_access_role,
    app_user,
    context,
    access_role,
    access_role_privilege,
    privilege
  WHERE user_access_role.app_user_id = app_user.app_user_id 
  AND user_access_role.context_id = context.context_id 
  AND user_access_role.access_role_id = access_role.access_role_id 
  AND access_role.access_role_id = access_role_privilege.access_role_id 
  AND access_role_privilege.privilege_id = privilege.privilege_id 
  AND context.type::text = 'system'::text
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS system_privilege
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW system_privilege
    IS 'Joins user_access_role, app_user, context, access_role, access_role_privilege and privilege tables to allow viewing of privileges by user on the system level.';

-- Create a trigger function to refresh the materialized views that stores privileges for users at a given level.
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
    RETURN NULL;
END;
$BODY$;

ALTER FUNCTION refresh_privilege()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION refresh_privilege()
    IS 'Refreshes materialized views that store privileges for users at a given level.';

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the access_role table.
CREATE OR REPLACE TRIGGER trigger_access_role_refresh_privilege
    AFTER INSERT OR DELETE OR UPDATE 
    ON access_role
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the access_role_privilege table.
CREATE OR REPLACE TRIGGER trigger_access_role_privilege_refresh_privilege
    AFTER INSERT OR DELETE OR UPDATE 
    ON access_role_privilege 
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the app_user table.
CREATE OR REPLACE TRIGGER trigger_app_user_refresh_privilege
    AFTER DELETE
    ON app_user
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the context table.
CREATE OR REPLACE TRIGGER trigger_context_refresh_privilege
    AFTER INSERT OR DELETE OR UPDATE 
    ON context
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the privilege table.
CREATE OR REPLACE TRIGGER trigger_privilege_refresh_privilege
    AFTER INSERT OR DELETE OR UPDATE 
    ON privilege
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();

-- Create a trigger to refresh the materialized views on INSERT, DELETE, or UPDATE commands to the user_access_role table.
CREATE OR REPLACE TRIGGER trigger_user_access_role_refresh_privilege
    AFTER INSERT OR DELETE OR UPDATE 
    ON user_access_role
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_privilege();


REFRESH MATERIALIZED VIEW library_privilege WITH DATA;
REFRESH MATERIALIZED VIEW organization_privilege WITH DATA;
REFRESH MATERIALIZED VIEW system_privilege WITH DATA;
	  
insert into db_migration (migration_id, description, script_name)
values ('0002', 'create materialized views on privilege and created triggers to refresh the views', '0002.sql');
