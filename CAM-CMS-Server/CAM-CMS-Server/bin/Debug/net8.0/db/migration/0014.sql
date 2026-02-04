drop table organization_library_folder;

alter table organization_package add  is_owned bool default true not null;

alter table organization add  library_folder_id int;
alter table organization add connect_button_module_id int;

alter table organization
add constraint organization_fk_library_folder
foreign key (library_folder_id) references library_folder(library_folder_id);

alter table organization
add constraint organization_fk_published_module
foreign key (connect_button_module_id) references published_module(published_module_id);

insert into db_migration (migration_id, description, script_name)
values ('0014', 'drop organization_library_folder and change organization table', '0014.sql');