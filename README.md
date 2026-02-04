# Getting Started with the CAM-CMS Project

The **CAM-CMS project** is a content management application consisting of:

* An **ASP.NET Core backend**
* A **React frontend**
* A **PostgreSQL database**

> [!Note]
> `project-setup.ps1` is **deprecated and does not work**.
> This project is intended to be set up using **Bash scripts only**.

---

## Prerequisites

Before beginning, ensure the following are installed and available in your PATH:

* **PostgreSQL client (`psql`)**
* **Node.js + npm**
* **.NET SDK**
* A **Bash-compatible terminal**
  (Linux, macOS, or Windows via WSL2 / Git Bash)

---

## Setup Instructions

Follow the steps below **in order** to correctly set up and run the project locally.

---

## 1. Set up HTTPS certificates and frontend dependencies

The React frontend runs over HTTPS and requires trusted development certificates.

Run the following command:

```bash
dotnet dev-certs https --trust
```

You will be prompted to trust the certificate — **accept the prompt**.

Next, install frontend dependencies:

```bash
npm install
```

> This only needs to be run **once per machine**, unless dependencies are removed or the project is cloned again.

---

## 2. Set up the local database structure

Before running the application, the PostgreSQL database must be created and initialized.

### 2.1 Create the local database and schema

Run `db-local-setup.sh`, located in the `db/` directory:

```bash
chmod +x db-local-setup.sh
./db-local-setup.sh --pgUser <user> --pgPwd <password> --name <db>
```

**Example:**

```bash
chmod +x db-local-setup.sh
./db-local-setup.sh --pgPwd gowest --name cam_cms
```

This script:

* Creates the database
* Creates required roles
* Initializes the `cam_cms` schema

**Notes:**

* `--pgUser` defaults to `postgres`
* `--pgHost` and `--pgPort` are optional and default to `localhost:5432`

---

### 2.2 Run database migrations

Once the database exists, apply schema migrations using `db-run-migration.sh`:

```bash
chmod +x db-run-migration.sh
./db-run-migration.sh --pgUser postgres --pgPwd gowest --db-name cam_cms
```

This script is **idempotent**, meaning:

* It can be run multiple times safely
* Only new migrations are applied
* Existing migrations are skipped automatically

Run this script whenever new migration files are added.

---

## 3. Verify backend connection strings

The backend must be configured to point at the database you just created.

1. Navigate to the backend project:

   * `CAM-CMS-Server/`
2. Open the solution in **Visual Studio**
3. Locate `appsettings.json` at the root of the backend project
4. Verify the connection string under:

   ```
   ConnectionStrings → WebApiPostgreSQLLocalDatabase
   ```
5. Ensure it matches:

   * Database name
   * Host
   * Port
   * Credentials used during setup

---

## 4. Run the application

The backend **must be running before** the frontend is started.

### 4.1 Start the backend server

Using Bash:

```bash
chmod +x launch-server.sh
./launch-server.sh
```

Alternatively, the backend can be run directly from **Visual Studio**.

Confirm the server is running before continuing.

---

### 4.2 Start the frontend (React app)

In a separate terminal:

```bash
chmod +x launch-web.sh
./launch-web.sh
```

Once running, the React application will be available in your browser.
