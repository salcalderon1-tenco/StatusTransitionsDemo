# StatusTransitionsDemo
Project for implement StatusTransitions

## How to Run
Run the project directly in the repository root:

```sh
dotnet run --project Simone.Common.StatusTransitions.Web.Test --launch-profile https
```

## How add migrations

To add a new migration, run the following command in the folder containing the project:

```sh
dotnet ef migrations add MyMigration --context AppDbContext -v
```
