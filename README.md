# flock
[flock](https://github.com/pimbrouwers/flock) is .NET Standard compliant SQL Migration Library.

## Supported Platforms (current plugins)
  
### SQL Server

The following demonstrates how to use against SQL Server via [`SqlServerMigration`](https://www.nuget.org/packages/Flock.Migration.SqlServer).

```c#
using System;
using Flock;

namespace Flock.TestSqlServerMigration
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var migration = new MigrationBuilder<SqlServerMigration>()
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