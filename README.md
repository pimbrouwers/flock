# flock
[flock](https://github.com/pimbrouwers/flock) is .NET Standard compliant SQL Migration Library.

## Support Platforms
  - SQL Server

## `SqlMigration` example

The following demonstrates how to use against SQL Server via `SqlMigration`.

```c#
using System;
using Flock;

namespace Flock.TestSqlMigration
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var migration = new MigrationBuilder<SqlMigration>()
        .ConnectionString(@"Data Source=SomeServer;Initial Catalog=SomeDatabase;Integrated Security=True")
        .MigrationTable("_Migration") // optional
        .ScriptsDirectory(@"C:\Scrips")
        .StatementSeparator("GO") // optional
        .Build();

      migration.Process();
    }
  }
}
```