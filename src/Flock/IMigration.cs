using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Flock
{
  public interface IMigration
  {
    /// <summary>
    /// Database connection string
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// Name of table where migration history is stored
    /// </summary>
    string MigrationTable { get; set; }

    /// <summary>
    /// Directory containing scripts to try/execute
    /// </summary>
    string ScriptsDirectory { get; set; }

    /// <summary>
    /// SQL batch separator
    /// </summary>
    string StatementSeparator { get; set; }

    /// <summary>
    /// Enumerate files in ScriptsDirectory, check process status, execute statements and log
    /// </summary>
    void Process();
  }
}