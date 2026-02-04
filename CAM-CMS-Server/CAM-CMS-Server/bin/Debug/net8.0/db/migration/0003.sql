\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

CREATE OR REPLACE FUNCTION insert_organization_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    INSERT INTO context(type, instance)
        VALUES ('organization', NEW.organization_id);
	RETURN NULL;
END;
$BODY$;

ALTER FUNCTION insert_organization_context()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION insert_organization_context()
    IS 'Adds a new organization context with new id as instance.';

CREATE OR REPLACE FUNCTION delete_organization_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    DELETE FROM context
        WHERE type='organization' AND instance=OLD.organization_id;
	RETURN NULL;
END;
$BODY$;

ALTER FUNCTION delete_organization_context()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION delete_organization_context()
    IS 'Removes old organization context with old id as instance.';

CREATE OR REPLACE FUNCTION insert_library_folder_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    INSERT INTO context(type, instance)
        VALUES ('library folder', NEW.library_folder_id);
	RETURN NULL;
END;
$BODY$;

ALTER FUNCTION insert_library_folder_context()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION insert_library_folder_context()
    IS 'Adds a new library folder context with new id as instance.';

CREATE OR REPLACE FUNCTION delete_library_folder_context()
    RETURNS trigger
    SECURITY DEFINER
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    DELETE FROM context
        WHERE type='library folder' AND instance=OLD.library_folder_id;
	RETURN NULL;
END;
$BODY$;

ALTER FUNCTION delete_library_folder_context()
    OWNER TO CURRENT_USER;

COMMENT ON FUNCTION delete_library_folder_context()
    IS 'Removes old library folder context with old id as instance.';

CREATE OR REPLACE TRIGGER trigger_organization_insert_context
    AFTER INSERT
    ON organization
    FOR EACH ROW
   	EXECUTE FUNCTION insert_organization_context();

COMMENT ON TRIGGER trigger_organization_insert_context ON organization
    IS 'Adds a new context for the newly added organization.';

CREATE OR REPLACE TRIGGER trigger_organization_delete_context
    AFTER DELETE
    ON organization
    FOR EACH ROW
   	EXECUTE FUNCTION delete_organization_context();

COMMENT ON TRIGGER trigger_organization_delete_context ON organization
    IS 'Removes old context for the deleted organization.';

CREATE OR REPLACE TRIGGER trigger_library_folder_insert_context
    AFTER INSERT
    ON library_folder
    FOR EACH ROW
   	EXECUTE FUNCTION insert_library_folder_context();

COMMENT ON TRIGGER trigger_library_folder_insert_context ON library_folder
    IS 'Adds a new context for the newly added library folder.';

CREATE OR REPLACE TRIGGER trigger_library_folder_delete_context
    AFTER DELETE
    ON library_folder
    FOR EACH ROW
   	EXECUTE FUNCTION delete_library_folder_context();

COMMENT ON TRIGGER trigger_library_folder_delete_context ON library_folder
    IS 'Removes old context for the deleted library folder.';

insert into db_migration (migration_id, description, script_name)
values ('0003', 'create triggers and functions to manage contexts', '0003.sql');