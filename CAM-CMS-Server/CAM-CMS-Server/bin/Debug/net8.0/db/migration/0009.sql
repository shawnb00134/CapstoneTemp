CREATE TABLE invitation
(
    invitation_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    summary text,
    organization_id integer,
    start_datetime timestamp,
    end_datetime timestamp,
    module_view_limit int,
    created_at timestamp DEFAULT NOW() not null,
    updated_at timestamp,
    
    constraint invitation_fk_organization
    foreign key(organization_id ) references organization(organization_id)
);

CREATE TABLE temp_user
(
    temp_user_id integer NOT NULL GENERATED ALWAYS AS IDENTITY primary key,
    app_user_id integer not null,
    module_view_count bigint,
    invitation_id integer not null,
    created_at timestamp DEFAULT NOW() not null,
    updated_at timestamp,
    
    constraint temp_user_fk_app_user
    foreign key(app_user_id ) references app_user(app_user_id),
    constraint temp_user_fk_invitation
    foreign key(invitation_id ) references invitation(invitation_id)
);

insert into db_migration (migration_id, description, script_name)
values ('0009', 'create temp_user and invitation tables', '0009.sql');
