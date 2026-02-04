\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- PROCEDURE create_organization_content_role(integer, text, integer, integer)
-- Purpose:  Create a new content role for an organization and will attempt to create a organization_content_role_mapper entry
-- 			 will rollback if the organization_content_role_mapper entry fails

--DROP PROCEDURE IF EXISTS create_organization_content_role(integer, text, integer, integer)

CREATE OR REPLACE PROCEDURE create_organization_content_role(
    IN in_organization_id INTEGER,
    IN in_display_name TEXT,
    IN in_created_by INTEGER,
    IN in_content_role_id INTEGER
)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    role_id INTEGER;
BEGIN
    INSERT INTO organization_content_role(
        organization_id,
        display_name,
        created_by
    ) VALUES (
        in_organization_id,
        in_display_name,
        in_created_by
    ) RETURNING organization_content_role_id INTO role_id;

    IF role_id IS NULL THEN
        RAISE EXCEPTION 'Failed to create organization_content_role';
    END IF;

    INSERT INTO organization_content_role_mapper(
        organization_content_role_id,
        content_role_id
    ) VALUES (
       	role_id,
        in_content_role_id
    );
    
END;
$BODY$;

ALTER PROCEDURE create_organization_content_role(integer, text, integer, integer)
OWNER TO CURRENT_USER;



-- PROCEDURE delete_organization_content_role(integer) 
-- Purpose:  Delete a content role for an organization content role and delete the organization_content_role_mapper entry

--DROP PROCEDURE IF EXISTS delete_organization_content_role(integer)

CREATE OR REPLACE PROCEDURE delete_organization_content_role(
    IN in_organization_content_role_id INTEGER
)
LANGUAGE plpgsql
AS $BODY$
BEGIN
    DELETE FROM organization_content_role_mapper
    WHERE organization_content_role_id = in_organization_content_role_id;

    DELETE FROM organization_content_role
    WHERE organization_content_role_id = in_organization_content_role_id;
END;
$BODY$;

ALTER PROCEDURE delete_organization_content_role(integer)
OWNER TO CURRENT_USER;



insert into db_migration (migration_id, description, script_name)
values ('0011', 'create procedure for creating a new content role for an organization', '0011.sql');







	

