-- Add Basic Users for development purposes

INSERT INTO app_user (username, password)
values ('admin123', 'AQAAAAIAAYagAAAAEHp/inpVUblMJESVAi+TDzpP4ttwxjffcsHjc2l9/uMEdMDInrW9W+5r1T+UpfqTSA=='), 
('TestNoPrivileges', 'AQAAAAIAAYagAAAAEMCRDC2sjBFunQspCeLg1sB8Bd5G7LLSPZ/JaYm0pwZI6EffbHyOg6HfFT9Pqx5rXQ==');

INSERT INTO user_access_role (app_user_id, context_id, access_role_id, created_by)
values (1, 1, 2, 1);

INSERT INTO db_migration (migration_id, description, script_name)
VALUES ('0019', 'Add Basic Users for development purposes', '0019.sql')