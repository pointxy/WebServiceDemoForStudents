using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace WebDbSupportChecker.App_Code
{
    /// <summary>
    /// מחלקה שבודקת אילו מנועי מסדי נתונים נתמכים על ידי המחשב המקומי:
    /// - Access: JET (.mdb), ACE (.accdb)
    /// - SQL Server: LocalDB (mdf)
    /// </summary>
    public static class AccessSupportChecker
    {
        /// <summary>
        /// מחזירה רשימת מחרוזות שמציינות אילו מנועים נתמכים על ידי המחשב.
        /// </summary>
        public static List<string> GetSupportedEngines()
        {
            var supported = new List<string>();

            // בדיקה אם מותקן ספק OLEDB של Access (ACCDB)
            if (IsOleDbProviderInstalled("Microsoft.ACE.OLEDB.12.0"))
                supported.Add("Access - ACCDB (ACE.OLEDB.12.0)");

            // בדיקה אם מותקן ספק OLEDB של Access (MDB)
            if (IsOleDbProviderInstalled("Microsoft.Jet.OLEDB.4.0"))
                supported.Add("Access - MDB (JET.OLEDB.4.0)");

            // בדיקה אם SQL Server LocalDB מותקן
            string version;
            if (IsSqlLocalDbAvailable(out version))
                supported.Add("SQL Server - LocalDB (Version: " + version + ")");

            // הוספת מידע על ארכיטקטורת התהליך (32 או 64 ביט)
            supported.Add(Environment.Is64BitProcess ? "Architecture: 64-bit" : "Architecture: 32-bit");

            return supported;
        }

        /// <summary>
        /// בודק האם ספק OLEDB מסוים מותקן ברג'יסטרי (Registry).
        /// </summary>
        private static bool IsOleDbProviderInstalled(string providerName)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(providerName);
            bool exists = (key != null);
            if (key != null)
                key.Close();
            return exists;
        }

        /// <summary>
        /// בודק האם SQL Server LocalDB זמין ומחזיר את גירסתו (אם קיימת).
        /// </summary>
        private static bool IsSqlLocalDbAvailable(out string version)
        {
            version = null;

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "sqllocaldb";
                process.StartInfo.Arguments = "i";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();

                if (!string.IsNullOrWhiteSpace(output))
                {
                    string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        version = lines[0].Trim();
                        return true;
                    }
                }
            }
            catch
            {
                // אם sqllocaldb לא קיים או לא נתמך
            }

            return false;
        }
    }
}