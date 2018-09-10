using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Flock
{
  /// <summary>
  /// SQL Server IMigration implementation
  /// </summary>
  public class SqlServerMigration : IMigration
  {
    private SqlConnection connection;

    public string ConnectionString { get; set; }
    public string MigrationTable { get; set; } = "_Migration";
    public string ScriptsDirectory { get; set; }
    public string StatementSeparator { get; set; } = "GO";

    public void Process()
    {
      if (!Directory.Exists(ScriptsDirectory))
      {
        throw new ArgumentException(nameof(ScriptsDirectory));
      }

      using (connection = CreateOpenConnection())
      {
        //has _migration table been created?
        CreateMigrationTableIfNotExists();

        //loop script files
        foreach (var scriptPath in Directory.EnumerateFiles(ScriptsDirectory, "*.sql", SearchOption.TopDirectoryOnly))
        {
          var fileInfo = new FileInfo(scriptPath);
          //has script been executed?
          if (!HasMigrated(fileInfo.Name))
          {
            try
            {
              ExecuteStatements(fileInfo);
              Console.WriteLine("{0} was successfully applied.", fileInfo.Name);
            }
            catch (Exception ex)
            {
              Console.WriteLine("{0} could not be applied.", fileInfo.Name);
              Console.WriteLine("\nERROR:\n{0}\n", ex.Message);

              //output reason for failure and stop processing
              break;
            }
          }
          else
          {
            Console.WriteLine("{0} has already been applied.", fileInfo.Name);
          }
        }
      }
    }

    /// <summary>
    /// Assemble open connection to database
    /// </summary>
    /// <returns></returns>
    public SqlConnection CreateOpenConnection()
    {
      if (string.IsNullOrWhiteSpace(ConnectionString))
      {
        throw new ArgumentNullException(nameof(ConnectionString));
      }

      var connection = new SqlConnection(ConnectionString);
      connection.Open();

      return connection;
    }

    /// <summary>
    /// Create log table for migration scripts if it doesn't exist
    /// </summary>
    public void CreateMigrationTableIfNotExists()
    {
      var cmdText = $@"
        if object_id('{MigrationTable}') is null
          begin
            create table {MigrationTable}
            (
              Id int identity not null primary key
              ,Script varchar(512)
              ,ExecutionDate datetime default (getutcdate())
              ,Checksum nvarchar(max)
              ,Text nvarchar(max)
            );
          end;
      ";
      var cmd = new SqlCommand(cmdText);
      cmd.Connection = connection;

      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Buffers script contents, splits into statements and executes within transaction
    /// </summary>
    /// <param name="scriptPath"></param>
    /// <returns>True/False if succeeded</returns>
    public void ExecuteStatements(FileInfo fileInfo)
    {
      string scriptText = File.ReadAllText(fileInfo.FullName);

      if (string.IsNullOrWhiteSpace(scriptText))
      {
        return;
      }

      IEnumerable<string> statements = ParseScript(scriptText);

      if (statements != null && statements.Any())
      {
        var transaction = connection.BeginTransaction();

        foreach (var statement in statements)
        {
          var cmd = new SqlCommand(statement);
          cmd.Connection = connection;
          cmd.Transaction = transaction;

          try
          {
            cmd.ExecuteNonQuery();
          }
          catch
          {
            transaction.Rollback();
            throw;
          }
        }

        LogMigration(transaction, fileInfo.Name, scriptText);
        transaction.Commit();
      }
    }

    /// <summary>
    /// Queries migration log to determine if script has been applied
    /// </summary>
    /// <param name="scriptName"></param>
    /// <returns></returns>
    public bool HasMigrated(string scriptName)
    {
      bool hasMigrated = false;

      var cmd = new SqlCommand($"select count(*) from {MigrationTable} where Script = @scriptName");
      cmd.Connection = connection;

      cmd.Parameters.AddWithValue("scriptName", scriptName);

      int recordCount = (int)cmd.ExecuteScalar();

      if (recordCount == 1)
      {
        hasMigrated = true;
      }

      return hasMigrated;
    }

    /// <summary>
    /// Inserts record into migration log
    /// </summary>
    /// <param name="scriptName"></param>
    public void LogMigration(SqlTransaction transaction, string scriptName, string scriptText)
    {
      var cmd = new SqlCommand($"insert into {MigrationTable} (Script, Checksum, Text) values (@scriptName, @checksum, @scriptText)");
      cmd.Connection = connection;
      cmd.Transaction = transaction;

      cmd.Parameters.AddWithValue("scriptName", scriptName);
      cmd.Parameters.AddWithValue("checksum", ScriptChecksum(scriptText));
      cmd.Parameters.AddWithValue("scriptText", scriptText);

      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Generate an MD5 Checksum from the script text
    /// </summary>
    /// <param name="scriptText"></param>
    /// <returns></returns>
    private string ScriptChecksum(string scriptText)
    {
      using (var md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] scriptTextBytes = Encoding.ASCII.GetBytes(scriptText);
        byte[] hashBytes = md5.ComputeHash(scriptTextBytes);

        var sb = new StringBuilder();

        for (int i = 0; i < hashBytes.Length; i++)
        {
          sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString();
      }
    }

    /// <summary>
    /// Parse script
    /// </summary>
    /// <param name="scriptPath"></param>
    /// <returns></returns>
    private IEnumerable<string> ParseScript(string scriptText)
    {
      return Regex.Split(
        scriptText,
        $@"^\s*{StatementSeparator}\s*$",
        RegexOptions.Multiline | RegexOptions.IgnoreCase
      )
      .Where(s => !string.IsNullOrWhiteSpace(s.Trim()));
    }
  }
}