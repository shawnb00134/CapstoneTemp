-- Purpose: Update privilege_summary materialized view and refresh_privilege function to include package_privilege.

DROP MATERIALIZED VIEW IF EXISTS privilege_summary CASCADE;
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

COMMENT ON MATERIALIZED VIEW privilege_summary
    IS 'Joins library_privilege, organization_privilege, and system_privilege to view users read privileges throughout the system.';

-- Update the refresh_privilege function to include the package_privilege view.
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

REFRESH MATERIALIZED VIEW privilege_summary WITH DATA;

CREATE OR REPLACE TRIGGER trigger_organization_package_changed
    AFTER INSERT OR UPDATE OR DELETE
    ON organization_package
    FOR EACH ROW
    EXECUTE FUNCTION refresh_privilege();

insert into db_migration (migration_id, description, script_name)
values ('0016', 'Update privilege_summary materialized view and refresh_privilege function to include package_privilege.', '0016.sql')