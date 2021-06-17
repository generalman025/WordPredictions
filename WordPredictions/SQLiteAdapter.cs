using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPredictions
{
    public class SQLiteAdapter
    {
        private string path;
        public SQLiteAdapter()
        {
            path = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), @"customDB\Dictionary.db");
        }

        public List<String> getWords(string word)
        {
            try
            {
                using (var con = new SQLiteConnection(path))
                {
                    con.Open();

                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = "SELECT Value FROM Main.Words WHERE Value LIKE @search LIMIT 10";
                    cmd.Parameters.AddWithValue("@search", word + "%");
                    cmd.Prepare();

                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    var result = new List<String>();
                    while (rdr.Read())
                    {
                        result.Add(rdr.GetString(0));
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
