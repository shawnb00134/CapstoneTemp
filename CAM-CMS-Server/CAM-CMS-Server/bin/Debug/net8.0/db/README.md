# CAM-CMS Database

This directory contains all database-related scripts and resources for the CAM-CMS project.
The system is built on **PostgreSQL** and supports **local development** as well as **Azure PostgreSQL** deployments.

The database workflow is split into **setup scripts** (run once) and **migration scripts** (run repeatedly as the schema evolves).

---

## Directory Structure

```
db/
├── migration/
│   └── *.sql
├── schema/
│   ├── initdb.sql
│   └── initschema.sql
├── db-local-setup.sh
├── db-team-setup.sh
├── db-team-setup-azure.sh
├── db-run-migration.sh
├── db-run-migration-azure.sh
└── README.md
```

---

## Key Concepts

### Setup vs Migrations

* **Setup scripts**:

  * Create databases
  * Create roles
  * Initialize base schemas and permissions
  * **Run once per database**
* **Migration scripts**:

  * Add or modify schema objects (tables, views, functions, etc.)
  * Track which migrations have already been applied
  * **Can be run multiple times safely**

### Schema Ownership

* All application objects live under the `cam_cms` schema.
* Setup scripts prepare the schema and permissions.
* Migration scripts evolve the schema over time.

---

## Scripts Overview

### 1. `db-local-setup.sh`

Creates a **single local PostgreSQL database** and the required roles for local development.

**Behavior**

* Creates:

  * Database: `<name>`
  * Role: `<name>` (group/owner role, `NOLOGIN`)
  * Role: `<name>_apiuser` (application role, `NOLOGIN`)
* Executes:

  * `schema/initdb.sql` on `postgres`
  * `schema/initschema.sql` on the new database
* Writes a setup log to `logs/db_setup_local_single.log`

---

### 2. `db-run-migration.sh`

Runs SQL migrations against a **local PostgreSQL database**, applying only migrations that have not already been run.

**Behavior**

* Reads migration files from `migration/*.sql`
* Uses a migration history table (`db_migration`)
* Skips migrations that are already recorded
* Optionally executes migrations under a specific role via `SET ROLE`

---

### 3. `db-run-migration-azure.sh`

Same behavior as `db-run-migration.sh`, but intended for **Azure PostgreSQL databases**.

**Behavior**

* Targets Azure-hosted PostgreSQL
* Uses the same migration tracking logic
* Supports role switching (`SET ROLE`) if required

---

### 4. `db-team-setup.sh`

Creates **multiple team databases locally**, each with a **dev** and **test** database.

**Behavior**

* Can create databases using a team count (e.g., 7 teams)
* For each team:

  * Creates `<team>_dev` and `<team>_test` databases
  * Creates required roles
  * Initializes schema and permissions

---

### 5. `db-team-setup-azure.sh`

Azure equivalent of `db-team-setup.sh`.

**Behavior**

* Creates multiple **Azure PostgreSQL databases**
* Each team receives:

  * One dev database
  * One test database
* Initializes roles, schemas, and permissions
* Can create databases using:
  * A team count (e.g., 7 teams)
  * A CSV roster file

**Roster Format**

```
email,team_no
student1@example.edu,1
student2@example.edu,1
student3@example.edu,2
```

---

## Migration and Schema Folders

### `schema/`

Contains **initialization SQL**:

* `initdb.sql`: creates databases and base roles
* `initschema.sql`: initializes the `cam_cms` schema and permissions

These scripts are used **only by setup scripts**.

---

### `migration/`

Contains **incremental migration scripts**:

* Each file represents one schema change
* Filenames are used as migration IDs
* Applied migrations are tracked in the database

These scripts are used by **migration runners** and may be executed multiple times safely.

## Recommended Workflow

### Local Development

1. Run `db-local-setup.sh` **once**
2. Run `db-run-migration.sh` whenever migrations change

### Team / Course Setup

1. Run `db-team-setup.sh` or `db-team-setup-azure.sh` **once**
2. Use migration scripts to evolve schemas over time

---

## Notes

* PostgreSQL client tools (`psql`) must be installed
* Azure PostgreSQL firewall rules must allow the client machine
* Setup scripts should not be re-run on existing databases. If trying to recreate them, ensure the original database is deleted first
