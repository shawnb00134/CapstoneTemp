CREATE TABLE app_user
(
    app_user_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    username varchar(255) not null,
	password varchar(255) not null,
	firstname varchar(255),
    lastname varchar(255),
	email varchar(255),
	phone char(10),
	avatar varchar(255),
	created_at timestamp DEFAULT NOW() not null,
	updated_at timestamp
);

CREATE TABLE access_role
(
    access_role_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) not null unique
);

CREATE TABLE archetype
(
	archetype_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	name varchar(255) not null unique

);

CREATE TABLE content_role
(
    content_role_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) not null unique,
	archetype_id integer,
	constraint content_role_fk_archetype
	foreign key(archetype_id ) references archetype(archetype_id)
);


CREATE TABLE organization
(
    organization_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) not null,
	created_at timestamp DEFAULT NOW() not null,
	updated_at timestamp,
	is_active   bool default true  not null,
	tags text[]
);

CREATE TABLE published_organization
(
	published_organization_id   integer not null primary key,
	cache  jsonb,
	
	constraint published_organization_fk_organization
	foreign key(published_organization_id ) references organization(organization_id)
);

CREATE TABLE organization_content_role
(
    organization_content_role_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    organization_id integer not null,
	display_name text,
	created_at timestamp DEFAULT NOW() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
		
	constraint organization_content_role_fk_organization
	foreign key (organization_id) references organization(organization_id),
	
	constraint organization_content_role_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint organization_content_role_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)
);


CREATE TABLE organization_user_content_role
(
    organization_user_content_role_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    organization_content_role_id integer not null,
    app_user_id integer not null,

	constraint organization_user_content_role_fk_organization_content_role
	foreign key (organization_content_role_id) references organization_content_role(organization_content_role_id),
	
	constraint organization_user_content_role_fk_app_user
	foreign key (app_user_id) references app_user(app_user_id)
);


CREATE TABLE package
(
    package_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	name varchar(255) not null,
   	is_core bool default false not null,
	created_at timestamp DEFAULT NOW() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	published_at timestamp,
	is_deleted bool default false not null,
	
	constraint package_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint package_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)
);

CREATE TABLE organization_package
(
    organization_id integer NOT NULL,
    package_id integer not null,
	primary key (organization_id, package_id),
	
	constraint organization_package_fk_organization
	foreign key (organization_id) references organization(organization_id),
	
	constraint organization_package_fk_package
	foreign key (package_id) references package(package_id)
);


CREATE TABLE folder_type
(
    folder_type_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) not null
);

CREATE TABLE package_folder
(
    package_folder_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    folder_type_id integer not null,
	display_name varchar(255) not null,
	full_description text,
	short_description varchar(255),
	thumbnail  varchar(255),
	content_role_id  integer,
	package_id   integer not null,
	published   bool default false not null,
	editable   bool default true not null,
	parent_folder_id integer,
	order_in_parent integer not null,
	created_at timestamp DEFAULT NOW() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	published_at timestamp,
	is_deleted bool default false not null,
	
		
	constraint package_folder_fk_folder_type
	foreign key (folder_type_id) references folder_type(folder_type_id),
	
	constraint package_folder_fk_content_role
	foreign key (content_role_id) references content_role(content_role_id),
	
	
	constraint package_folder_fk_package
	foreign key (package_id) references package(package_id),
	
	constraint package_folder_fk_package_folder
	foreign key (parent_folder_id) references package_folder(package_folder_id),
	
	constraint package_folder_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint package_folder_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)
);

CREATE TABLE library_folder
(
    library_folder_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	name varchar(255),
	description text,
	created_at timestamp DEFAULT NOW() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	
	constraint library_folder_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint library_folder_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)

);

CREATE TABLE organization_library_folder
(
    organization_id integer NOT NULL,
    library_folder_id integer not null,
	is_owned  bool default true not null,
	primary key (organization_id, library_folder_id),
	
	constraint organization_library_folder_fk_organization
	foreign key (organization_id) references organization(organization_id),
	constraint organization_library_folder_fk_library_folder
	foreign key (library_folder_id) references library_folder(library_folder_id)
);

CREATE TABLE module
(
    module_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	title varchar(255)  not null, 
	description text,
	survey_start_link text,
	survey_end_link text,
	contact_title  varchar(255),
	contact_number varchar(255),
	thumbnail  varchar(255),
	tags text[],
	display_title varchar(255) not null,
    template_module_id integer,
	is_template bool default false not null,
	library_folder_id  integer not null,
	
	created_at timestamp DEFAULT NOW() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	published_at timestamp,
	is_deleted bool default false not null,
	
	constraint module_fk_module
	foreign key (template_module_id) references module(module_id),
	
	constraint module_fk_library_folder
	foreign key (library_folder_id) references library_folder(library_folder_id),
	
	constraint module_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint module_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)

);

CREATE TABLE open_module
(
 	open_module_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
	organization_id integer,
	module_id integer not null,
	created_at timestamp default now() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	validity_start timestamp,
	validity_end timestamp,
		
	constraint open_module_fk_organization
	foreign key (organization_id) references organization(organization_id),
	
	constraint open_module_fk_module
	foreign key (module_id) references module(module_id),
		
	constraint open_module_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint open_module_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)

);

CREATE TABLE element_set
(
 	element_set_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	module_id integer not null,
	order_in_module integer not null,
	editable   bool default true not null,
 	set_attribute  json,  -- may contain styling info and is_appendix indicating if it's documents.
    unique (module_id, order_in_module),
	
	constraint element_set_fk_module
	foreign key (module_id) references module(module_id)

);

CREATE TABLE element_type
(
    element_type_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) unique not null
);

CREATE TABLE element
(
 	element_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	title varchar(255) not null,
	description text,
	element_type_id integer not null,
	citation text,
	content json,   -- may contain actual content, or external_source like url, or assessment question
	tags text[],
	library_folder_id integer not null,
	created_at timestamp default now() not null,
	created_by   int not null,
	updated_at timestamp,
	updated_by   int,
	is_deleted bool default false not null,
	
	constraint element_fk_element_type
	foreign key (element_type_id) references element_type(element_type_id),

	constraint element_fk_library_folder
	foreign key (library_folder_id) references library_folder(library_folder_id),
	
	constraint element_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id),
	
	constraint element_fk_app_user_updated_by
	foreign key (updated_by) references app_user(app_user_id)

);

CREATE TABLE set_location
(
 	element_set_id integer NOT NULL,
	element_id integer not null,
	order_in_set integer not null,
	editable   bool default true not null,

	location_attribute json,  -- contains element styling info.

   	primary key (element_set_id,element_id),
	unique (element_set_id ,order_in_set ),
	
	constraint set_location_fk_element_set
	foreign key (element_set_id) references element_set(element_set_id),
	
	constraint set_location_fk_element
	foreign key (element_id) references element(element_id)

);


CREATE TABLE published_module
(
	published_module_id   integer not null primary key,
	cache  jsonb,
	
	constraint published_module_fk_module
	foreign key(published_module_id ) references module(module_id)
);


CREATE TABLE package_folder_module
(
 	package_folder_module_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	package_folder_id integer not null,
	published_module_id   integer not null,
	order_in_folder integer not null,
	editable   bool default true not null,
 	
    unique (package_folder_id, published_module_id),
	
	constraint package_folder_module_fk_package_folder
	foreign key (package_folder_id) references package_folder(package_folder_id),
	
	constraint package_folder_module_fk_published_module
	foreign key (published_module_id) references published_module(published_module_id)
);

CREATE TABLE privilege
(
    privilege_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    name varchar(255) not null
);

CREATE TABLE access_role_privilege
(
 	access_role_id integer NOT NULL,
	privilege_id integer not null,
	
	primary key (access_role_id,privilege_id),
	
	constraint access_role_privilege_fk_access_role
	foreign key (access_role_id) references access_role(access_role_id),
	constraint access_role_privilege_fk_privilege
	foreign key (privilege_id) references privilege(privilege_id)

);


CREATE TABLE context
(
    context_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    type varchar(255) not null,
	instance int
);

CREATE TABLE user_access_role
(
    user_access_role_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
	app_user_id integer not null,
	context_id integer,
	access_role_id integer not null,
	created_at timestamp default now() not null,
	created_by   int not null,
	
	constraint user_access_role_fk_app_user
	foreign key (app_user_id) references app_user(app_user_id),
	
	constraint user_access_role_fk_access_role
	foreign key (access_role_id) references access_role(access_role_id),
	
	constraint user_access_role_fk_context
	foreign key (context_id) references context(context_id),
	
	constraint user_access_role_fk_app_user_created_by
	foreign key (created_by) references app_user(app_user_id)
	
);

create table db_migration(
	migration_id  char(4) primary key,
	datetime   timestamp  DEFAULT NOW() not null,
	description text  not null,
	script_name  text  not null
);

-- delete from access_role;

-- ALTER TABLE access_role ALTER COLUMN access_role_id RESTART WITH 1;

-- set role to apiuser;

insert into privilege(name) 
values 
	   ('create'),
	   ('read'),
	   ('update'),
	   ('delete'),
	   ('assign'),
	   ('invite'),
	   ('post')
	   ;

insert into access_role(name) 
values ('user'),
	   ('cam admin'),
	   ('group leader'),
	   ('brand admin'),
	   ('org admin'),
	   ('content creator'),
	   ('content viewer');

insert into access_role_privilege(access_role_id, privilege_id) 
values 
	   (2,1),
	   (2,2),
	   (2,3),
	   (2,4),
	   (2,5),
	   (2,6),
	   (2,7),
	   (4,1),
	   (4,2),
	   (4,3),
	   (4,4),
	   (4,5),
	   (4,6),
	   (4,7),
	   (5,1),
	   (5,2),
	   (5,3),
	   (5,4),
	   (5,5),
	   (5,6),
	   (5,7),
	   (6,1),
	   (6,2),
	   (6,3),
	   (6,4),
	   (7,2)
	   ;
	   
   
insert into folder_type(name) 
values ('category'),
	   ('track')
	   ;	

insert into archetype(name) 
values 	('patient'),
		('provider'),
		('educator'),
		('caregiver');
		
insert into element_type(name) 
values ('text'),
	   ('image'),
	   ('audio'),
	   ('video'),
	   ('pdf'),
	   ('question'),
       ('anchor')
	   ;

insert into context(type)
values
		('system')
		;

insert into db_migration (migration_id, description, script_name)
values ('0001', 'initial script', '0001.sql');