-- Run as postgres to create a new DB

CREATE DATABASE :newdb;
\c :newdb

REVOKE ALL ON DATABASE :newdb FROM PUBLIC;

-- per-team apiuser, passed in as -v apiuser=<teamNN_apiuser>
GRANT CONNECT ON DATABASE :newdb TO :apiuser;

-- team group role for human users / group-level perms
GRANT CONNECT ON DATABASE :newdb TO :teamrole;

ALTER DATABASE :newdb SET search_path = 'cam_cms';