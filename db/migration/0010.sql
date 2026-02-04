\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- PROCEDURE alter_folder_and_child_folder_content_roles(integer)
-- Purpose:  Alter the current package folder's content roles and all child folder's content roles

--DROP PROCEDURE IF EXISTS alter_folder_and_child_folder_content_roles(integer);


CREATE OR REPLACE PROCEDURE alter_folder_and_child_folder_content_roles(
    IN in_folder_id INTEGER
)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    folders_count INTEGER;
    folder RECORD;
	new_content_role_id INTEGER;
BEGIN
    -- Checks in_folder_id for any child folders
    SELECT COUNT(package_folder_id) INTO folders_count
    FROM package_folder
    WHERE parent_folder_id = in_folder_id AND 
          is_deleted = FALSE;

    IF folders_count < 1 THEN
        RAISE NOTICE 'No folders found';
        RETURN;  -- Exit the procedure if no folders are found
    END IF;
	-- retrieves the content role id of the current folder
	SELECT  content_role_id INTO new_content_role_id 
	FROM package_folder WHERE package_folder_id = in_folder_id AND is_deleted = FALSE;
	
	RAISE NOTICE 'Content_Role_ID: %', new_content_role_id;
    -- Loop through the folders
    FOR folder IN
        SELECT package_folder_id
        FROM package_folder
        WHERE parent_folder_id = in_folder_id AND 
              is_deleted = FALSE
    LOOP
        RAISE NOTICE 'Folder being updated: %', folder.package_folder_id;
		-- updates the content role id of the current folder in the loop
		UPDATE package_folder SET content_role_id = new_content_role_id WHERE package_folder_id = folder.package_folder_id AND
		is_deleted = FALSE;
        -- Recursively call the procedure to update the child folders
		CALL alter_folder_and_child_folder_content_roles(folder.package_folder_id);
    END LOOP;

END;
$BODY$;

ALTER PROCEDURE alter_folder_and_child_folder_content_roles(integer) 
OWNER TO CURRENT_USER;

insert into db_migration (migration_id, description, script_name)
values ('0010', 'create procedure for changing the content_role_ids ', '0010.sql');
