using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

/// <summary>
/// מחלקת עזר לעבודה עם SQL Server (כולל קבצי MDF)
/// </summary>
public static class DbHelper_MsSql
{
    private static readonly string dbFileName = "CommentsDb.mdf";

    private static string ConnectionString
    {
        get
        {
            string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/" + dbFileName);
            return "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + dbPath + ";Integrated Security=True;";
        }
    }

    public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
            }

            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
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
                cmd.CommandText = "SELECT SCOPE_IDENTITY()";
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
        DataTable dt = ExecuteQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");
        List<string> tables = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            tables.Add(row["TABLE_NAME"].ToString());
        }
        return tables;
    }

    public static DataTable DescribeTable(string tableName)
    {
        string sql = @"
            SELECT 
                COLUMN_NAME AS Field,
                DATA_TYPE AS Type,
                CHARACTER_MAXIMUM_LENGTH AS Length,
                IS_NULLABLE AS Nullable
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @table";

        return ExecuteQuery(sql, new Dictionary<string, object>
        {
            { "@table", tableName }
        });
    }
}
