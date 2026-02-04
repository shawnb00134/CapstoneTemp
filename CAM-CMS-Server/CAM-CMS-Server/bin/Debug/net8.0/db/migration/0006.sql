CREATE TABLE organization_content_role_mapper
(
	organization_content_role_id integer not null,
	content_role_id integer not null,
	primary key (organization_content_role_id,content_role_id),
	
	constraint organization_content_role_mapper_fk_organization_content_role
	foreign key(organization_content_role_id ) references organization_content_role(organization_content_role_id),
	
	constraint organization_content_role_mapper_fk_content_role
	foreign key(content_role_id ) references content_role(content_role_id)
);


insert into db_migration (migration_id, description, script_name)
values ('0006', 'create organization_content_role_mapper table', '0006.sql');