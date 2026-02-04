\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- This migration creates a materialized view that joins the context table with the library_folder and organization tables and any others needed.
CREATE MATERIALIZED VIEW context_information
AS
SELECT context_id, type, instance, 
		CASE 
			WHEN type='library folder' THEN library_folder.name
			WHEN type='organization' THEN organization.name
			ELSE NULL
		END name
	FROM context
	LEFT JOIN library_folder
		ON context.instance=library_folder.library_folder_id
	LEFT JOIN organization
		ON context.instance=organization.organization_id
WITH DATA;

ALTER TABLE IF EXISTS context_information
    OWNER TO CURRENT_USER;

COMMENT ON MATERIALIZED VIEW context_information
    IS 'Joins needed information about a context with the individual context references that object.';

-- Create a trigger function to refresh the materialized view.
CREATE FUNCTION refresh_context_information()
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

-- Create a trigger to refresh the materialized view when the context table is manipulated.
CREATE TRIGGER trigger_context_refresh_context_information
    AFTER INSERT OR DELETE OR UPDATE 
    ON context
    FOR EACH STATEMENT
    EXECUTE FUNCTION refresh_context_information();

REFRESH MATERIALIZED VIEW context_information WITH DATA;
	  
insert into db_migration (migration_id, description, script_name)
values ('0004', 'Create materialized view for storing information about a context and its given instance.', '0004.sql');