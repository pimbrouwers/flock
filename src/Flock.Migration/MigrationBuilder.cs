using System;
using System.Text;

namespace Flock
{
  public class MigrationBuilder<TMigration> where TMigration : IMigration, new()
  {
    private string connectionString;
    private string migrationTable;
    private string scriptsDirectory;
    private string statementSeparator;

    public void Build()
    {
      var migration = new TMigration();

      if (!string.IsNullOrWhiteSpace(connectionString))
      {
        migration.ConnectionString = connectionString;
      }

      if (!string.IsNullOrWhiteSpace(migrationTable))
      {
        migration.MigrationTable = migrationTable;
      }

      if (!string.IsNullOrWhiteSpace(scriptsDirectory))
      {
        migration.ScriptsDirectory = scriptsDirectory;
      }

      if (!string.IsNullOrWhiteSpace(statementSeparator))
      {
        migration.StatementSeparator = statementSeparator;
      }

      RenderEmblem();
      migration.Process();
    }

    public MigrationBuilder<TMigration> ConnectionString(string connectionString)
    {
      this.connectionString = connectionString;
      return this;
    }

    public MigrationBuilder<TMigration> MigrationTable(string migrationTable)
    {
      this.migrationTable = migrationTable;
      return this;
    }

    public MigrationBuilder<TMigration> ScriptsDirectory(string scriptsDirectory)
    {
      this.scriptsDirectory = scriptsDirectory;
      return this;
    }

    public MigrationBuilder<TMigration> StatementSeparator(string statementSeparator)
    {
      this.statementSeparator = statementSeparator;
      return this;
    }

    private void RenderEmblem()
    {
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.AppendLine(@" ///,        ////");
      sb.AppendLine(@" \  /,      /  >.");
      sb.AppendLine(@"  \  /,   _/  /.");
      sb.AppendLine(@"   \_  /_/   /./");
      sb.AppendLine(@"    \__/_   <");
      sb.AppendLine(@"    /<<< \_\");
      sb.AppendLine(@"   /,)^>>_._ \");
      sb.AppendLine(@"   (/   \\ /\\\");
      sb.AppendLine(@"        // ````");
      sb.AppendLine(@"=======((`=======");
      sb.AppendLine(@"===== FLOCK =====");
      sb.AppendLine(@"=================");

      Console.WriteLine(sb.ToString());
    }
  }
}