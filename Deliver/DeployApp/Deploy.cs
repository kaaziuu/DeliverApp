using System.Data.SqlClient;

namespace DeployApp;

public static class Deploy
{
    public static void RunSqlScript(string connectionString)
    {
        var currentPath = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);
        var scriptPath = string.Join(
            Path.DirectorySeparatorChar,
            currentPath.Take(currentPath.Length - 1)
        ) + $"{Path.DirectorySeparatorChar}DeployApp";

        var connection = new SqlConnection(connectionString);
        connection.Open();
        var lastExecuteScript = ExecuteScript.GetExecutedScripts(ref connection);
        lastExecuteScript.Folder ??= "0000";
        lastExecuteScript.ScriptNumber ??= -1;
        ExecuteScript.RunScripts(ref connection, scriptPath, lastExecuteScript);
        connection.Close();
    }
}