ALTER TABLE element_set DROP CONSTRAINT element_set_module_id_order_in_module_key;


insert into db_migration (migration_id, description, script_name)
values ('0008', 'drop the unique constraint in element_set', '0008.sql');