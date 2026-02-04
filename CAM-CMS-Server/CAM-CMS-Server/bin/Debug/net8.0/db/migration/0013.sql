ALTER TABLE temp_user
ADD COLUMN email VARCHAR(255),
ADD COLUMN firstname VARCHAR(255),
ADD COLUMN lastname VARCHAR(255),
ADD COLUMN phone VARCHAR(20),
ADD COLUMN is_deleted BOOLEAN DEFAULT FALSE,
ADD COLUMN username VARCHAR(255),
ADD COLUMN linked_app_user_id integer;


ALTER TABLE app_user

ADD COLUMN is_deleted BOOLEAN DEFAULT FALSE;


insert into db_migration (migration_id, description, script_name)
values ('0013', 'update temp_user and app_user tables', '0013.sql');
