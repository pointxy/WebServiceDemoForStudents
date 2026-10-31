// need to add asembly to vs
/*
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// מחלקת עזר לעבודה עם MySQL
/// </summary>
public static class DbHelper_MySql
{
    private static readonly string connectionString = "Server=localhost;Database=mydb;Uid=root;Pwd=password;";

    public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
            }

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
            }

            conn.Open();
            int affected = cmd.ExecuteNonQuery();

            if (sql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                cmd.Parameters.Clear();
                object result = cmd.ExecuteScalar();
                int newId;
                if (result != null && int.TryParse(result.ToString(), out newId))
                    return newId;
            }

            return affected;
        }
    }

    public static List<string> ListTables()
    {
        DataTable dt = ExecuteQuery("SHOW TABLES");
        List<string> tables = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            tables.Add(row[0].ToString());
        }
        return tables;
    }

    public static DataTable DescribeTable(string tableName)
    {
        return ExecuteQuery("DESCRIBE `" + tableName + "`");
    }
}
*/