-- Purpose: Create a materialized view to store user read privileges.
DROP MATERIALIZED VIEW IF EXISTS privilege_summary CASCADE;
CREATE MATERIALIZED VIEW privilege_summary
AS
  SELECT 
    user_access_role.app_user_id,
    MIN(user_access_role.access_role_id) AS access_role_id,
    BOOL_OR(library_privilege.name = 'read'::text) AS library_read,
    BOOL_OR(organization_privilege.name = 'read'::text) AS organization_read,
    BOOL_OR(system_privilege.name = 'read'::text) AS system_read
    FROM user_access_role
    LEFT JOIN library_privilege 
    ON user_access_role.app_user_id = library_privilege.app_user_id
    LEFT JOIN organization_privilege 
    ON user_access_role.app_user_id = organization_privilege.app_user_id
    LEFT JOIN system_privilege 
    ON user_access_role.app_user_id = system_privilege.app_user_id
    GROUP BY 
        user_access_role.app_user_id
WITH NO DATA;

ALTER MATERIALIZED VIEW IF EXISTS privilege_summary
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW privilege_summary
    IS 'Joins library_privilege, organization_privilege, and system_privilege to view users read privileges throughout the system.';

-- Update the refresh_privilege function to include the privilege_summary materialized view.
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
    REFRESH MATERIALIZED VIEW privilege_summary WITH DATA;
    RETURN NULL;
END;
$BODY$;

REFRESH MATERIALIZED VIEW privilege_summary WITH DATA;

insert into db_migration (migration_id, description, script_name)
values ('0012', 'Create a materialized view to store user read privileges.', '0012.sql')