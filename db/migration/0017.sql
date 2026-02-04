\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- Purpose: Update organization_content_role_mapper table to handle multiple archetype IDs.

CREATE TABLE organization_content_role_mapper_v2 (
    organization_content_role_id INTEGER NOT NULL,
    content_role_ids INTEGER[] NOT NULL,
    FOREIGN KEY (organization_content_role_id) REFERENCES organization_content_role(organization_content_role_id)
);

CREATE OR REPLACE PROCEDURE create_organization_content_role_v2(
    in_organization_id INTEGER,
    in_display_name TEXT,
    in_created_by INTEGER,
    in_content_role_ids INTEGER[]
)
LANGUAGE plpgsql
AS $$
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

    INSERT INTO organization_content_role_mapper_v2(
        organization_content_role_id,
        content_role_ids
    ) VALUES (
        role_id,
        in_content_role_ids
    );

END $$;

INSERT INTO db_migration (migration_id, description, script_name)
VALUES ('0017', 'Update organization_content_role_mapper for multiple archetype IDs.', '0017.sql');
