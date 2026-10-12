# Simone.Common.StatusTransitions.Web.Test

Web **test project** (sample host) to verify how the
[`Simone.Common.StatusTransitions`](../../src/Simone.Common.StatusTransitions) package works.
It is not part of the published package: it is an ASP.NET Core MVC application that consumes the
package in a realistic way so you can manually test the status-transition lifecycle in a browser.

## What does it test?

It spins up a small **Subcontracts** application that exercises the main pieces of the package:

- Package registration with `AddStatusTransitions<AppDbContext>` and `UseStatusTransitions()`.
- An example **process/workflow** (`SubcontractProcess`) that is seeded automatically at startup:

  ```
  (initial) ─Create─▶ Draft ─Send─▶ Submitted ─Approve─▶ Approved
                                      Submitted ─Reject─▶ Rejected ─Edit─▶ Draft
  ```

- **Notification types** seeded automatically at startup:
  - `JustAuthor` sends notifications to the author through Email, Android and iOS.
  - `JustSupervisor` sends notifications to the supervisor through Email, Android and iOS, and
    requires the `TechService-TicketEdition` permission.

  The seeder creates either notification type only when it is missing. It does not overwrite an
  existing type, so you can update its values directly in the database when needed; those changes
  are retained on subsequent startups.

- Creating transitions (`TryCreateTransition`) from `SubcontractsController`.
- A **transition executable** (`SubcontractTransitionExecutable`) that hooks the business logic
  before (`ValidateChange`) and after (`Changed`) each status change.
- The package **tag helpers** (`<transition-badge>`, transition buttons) in the views.
- **Notifications** over RabbitMQ when a transition is completed.
- Real Simon-e authentication/authorization through **OIDC** against the Identity Server.

The project uses **SQL Server** (EF Core, with migrations) and **Serilog** for logging
(console + daily rolling file at `logs/log-*.txt`).

## Structure

| Folder | Contents |
| --- | --- |
| `Controllers/` | `SubcontractsController` (CRUD + transitions), `HomeController` |
| `Data/` | `AppDbContext` and the factory for design-time migrations |
| `Migrations/` | EF Core migrations (applied automatically at startup) |
| `Models/` | `Subcontract` entity, `SubcontractProcess` definition, view models |
| `Services/` | `SubcontractProcessSeeder` (seeds the workflow), `SubcontractTransitionExecutable` |
| `Views/` | MVC views (including the package tag helpers) |

---

## Configuration (`appsettings.json`)

Before running, review three sections: `ConnectionStrings`, `IdentityOptions` and `RabbitMQ`.

> Replace the placeholder values with those of your environment.

### 1. `ConnectionStrings`

SQL Server connection strings.

```json
"ConnectionStrings": {
  "Database": "Data Source=localhost,1433;Initial Catalog=StatusTransitions.Test;USER ID=<user>;Password=<Password>;MultipleActiveResultSets=True;TrustServerCertificate=True",
  "Tickets":  "Data Source=localhost,1433;Initial Catalog=Simone.Tickets;USER ID=<user>;Password=<Password>;MultipleActiveResultSets=True;TrustServerCertificate=True"
}
```

- **`Database`** — database for this test host. This is where the package tables live
  (processes, statuses, relations, transitions) along with the `Subcontracts` table. If the
  database does not exist, it is created automatically at startup because migrations are applied
  on launch.
- **`Tickets`** — Simon-e tickets/security database. It is used by the identity module
  (referenced from `IdentityOptions.TicketConnectionStringName`).

> `MultipleActiveResultSets=True` and `TrustServerCertificate=True` are required: the first because
> of how the package queries EF Core, the second to accept the dev SQL Server certificate.

### 2. `IdentityOptions`

Configures **OIDC** sign-in against the Simon-e Identity Server. This is what makes `[Authorize]`
and `RequireAuthenticatedUser()` work and what provides a real authenticated user (whose GUID is
used as the author of the transitions).

```json
"IdentityOptions": {
  "ApplicationName": "Simon-e Tech Service",
  "AuthorityUrl": "https://simone.tenco.dev",
  "ApplicationUrl": "https://localhost:7148",
  "PostLogoutRedirectUrl": "https://localhost:7148/signin-oidc",
  "SecurityUrl": "https://simone.tenco.dev",
  "Username": "<username>",
  "Password": "<Password>",
  "TicketConnectionStringName": "Tickets",
  "RequiredScopes": [ "Simone" ]
}
```

| Field | Description |
| --- | --- |
| `ApplicationName` | Application name as recognized by the Identity Server. |
| `AuthorityUrl` | Identity Server URL (OIDC token issuer). |
| `ApplicationUrl` | **Local** URL of this application. It must match the port you run it on (see `launchSettings.json`). |
| `PostLogoutRedirectUrl` | OIDC return URL (`/signin-oidc`), based on `ApplicationUrl`. |
| `SecurityUrl` | URL of the Simon-e security/permissions service. |
| `Username` / `Password` | Service credentials used to resolve permissions. |
| `TicketConnectionStringName` | Name of the connection string (in `ConnectionStrings`) used for tickets. |
| `RequiredScopes` | OIDC scopes the application must request. |

> ⚠️ **Important:** `ApplicationUrl` and `PostLogoutRedirectUrl` must point to the same port you
> start the project with (by default `https://localhost:7148`). In addition, that `localhost` must
> be registered as a valid client/redirect in the Identity Server
> (see [Registering localhost in the Identity Server](#registering-localhost-in-the-identity-server-infra)).

### 3. `RabbitMQ`

Configures the messaging broker the package uses to **publish notifications** when a transition is
completed. It **must be registered before StatusTransitions** (`Program.cs` already does this),
because the notification publisher depends on it.

```json
"RabbitMQ": {
  "Host": "172.100.0.88",
  "Username": "simone_admin",
  "Password": "Tenco1234",
  "Exchanges": [ "Simone-Notifications" ],
  "Errors": { "Name": "Errors", "Exchange": "Simone-Errors" },
  "NotificationsExchange": "Simone-Notifications"
}
```

| Field | Description |
| --- | --- |
| `Host` | RabbitMQ server host. |
| `Username` / `Password` | Connection credentials. |
| `Exchanges` | Exchanges declared on connect. |
| `Errors` | Queue/exchange for error messages. |
| `NotificationsExchange` | Exchange where transition notifications are published. |

---

## How to run it on localhost

### Prerequisites

- **.NET SDK 10.0** (the project targets `net10.0`).
- Access to a **SQL Server** (the dev one or a local one) with the accounts in `ConnectionStrings`.
- Access to the **RabbitMQ** server defined in `RabbitMQ`.
- Network access to the **Identity Server** (`AuthorityUrl`) and `localhost` registered in it
  (see the next section).

### Steps

1. Adjust `appsettings.json` (the sections above) to your environment.

2. From the repository root, restore and build:

   ```sh
   dotnet restore
   dotnet build tests/Simone.Common.StatusTransitions.Web.Test
   ```

3. Run the project:

   ```sh
   dotnet run --project tests/Simone.Common.StatusTransitions.Web.Test --launch-profile https
   ```

   - You do not need to create the database by hand: at startup migrations are applied
     (`MigrateAsync`) and the subcontract workflow is seeded (`SubcontractProcessSeeder`).
   - Ports are defined in
     [`Properties/launchSettings.json`](Properties/launchSettings.json):
     - **https** profile: `https://localhost:7148` (and `http://localhost:5288`).
     - **http** profile: `http://localhost:5288`.

4. Open the browser at **https://localhost:7148**. You will be redirected to the Identity Server to
   sign in; after authenticating, the app takes you to the subcontracts list.

> If you change the port, also update `ApplicationUrl` and `PostLogoutRedirectUrl` in
> `IdentityOptions` **and** the `localhost` registration in the Identity Server.

### Logs

Serilog writes to the console and to `logs/log-<date>.txt` (daily rolling file, created next to the
executable). There you can see startup, migration, seed and per-transition events.

---

## Registering `localhost` in the Identity Server (infra)

Because sign-in is **real OIDC**, the Identity Server only accepts redirects to URLs it has
registered. So, to be able to sign in from your machine, you must **modify the identity Docker
configuration in the infrastructure repository** and add this application's `localhost` as a valid
client/redirect.

The following URLs need to be registered (adjust the port to the one you use):

- **Redirect URI:** `https://localhost:7148/signin-oidc`
- **Post logout redirect URI:** `https://localhost:7148/signout-callback-oidc`
- **Allowed origin / CORS:** `https://localhost:7148`

These values must match `ApplicationUrl` / `PostLogoutRedirectUrl` in `IdentityOptions`.

General steps:

1. In the **infra** repository, locate the Identity Server configuration (the `docker` / compose or
   the client configuration file that brings up the identity container).
2. Add `https://localhost:7148` (and its `signin-oidc` / `signout-callback-oidc`) to the client's
   list of allowed redirects/origins for Simon-e.
3. Recreate/restart the identity container so it picks up the new configuration.
4. Run this project again and sign in.

> Without this step, when you try to sign in the Identity Server will reject the redirect to
> `localhost` (typically an *invalid redirect_uri* error or an unauthorized client).
