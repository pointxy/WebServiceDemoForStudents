using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web;

/// <summary>
/// מחלקת עזר לעבודה עם מסדי נתונים מסוג Access (.mdb / .accdb)
/// </summary>
public static class DbHelper_Access
{
    private static readonly string dbFileName = "projectDb.accdb";

    /// <summary>
    /// מחזיר את מחרוזת ההתחברות למסד הנתונים
    /// </summary>
    private static string ConnectionString
    {
        get
        {
            string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/" + dbFileName);
            string ext = Path.GetExtension(dbPath).ToLower();

            string provider = ext == ".accdb"
                ? "Microsoft.ACE.OLEDB.12.0"
                : "Microsoft.Jet.OLEDB.4.0";

            return "Provider=" + provider + ";Data Source=" + dbPath + ";Persist Security Info=False;";
        }
    }

    public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
        {
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
                }
            }

            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
    {
        using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
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

            // אם מדובר בפקודת Insert - החזר את ה-ID האחרון
            if (sql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = "SELECT @@IDENTITY";
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
        List<string> tables = new List<string>();
        using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        {
            conn.Open();
            DataTable schema = conn.GetSchema("Tables");
            foreach (DataRow row in schema.Rows)
            {
                string type = row["TABLE_TYPE"].ToString();
                if (type == "TABLE")
                    tables.Add(row["TABLE_NAME"].ToString());
            }
        }
        return tables;
    }

    public static DataTable DescribeTable(string tableName)
    {
        using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        {
            string sql = "SELECT * FROM [" + tableName + "] WHERE 1=0";
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
            {
                DataTable schemaTable = new DataTable();
                adapter.FillSchema(schemaTable, SchemaType.Source);
                return schemaTable;
            }
        }
    }
}
