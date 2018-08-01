using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Flock
{
  public class MigrationBuilder<TMigration> where TMigration : IMigration, new()
  {
    private string connectionString;
    private string migrationTable;
    private string scriptsDirectory;
    private string statementSeparator;

    public IMigration Build()
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

      return migration;
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
  }
}