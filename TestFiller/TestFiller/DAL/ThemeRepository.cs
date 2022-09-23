using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TestFiller.Models;


namespace TestFiller.DAL
{
    public class ThemeRepository
    {
        private string connectionString = "Data Source=DESKTOP-V2UKGVT;Initial Catalog=educational;Integrated Security=True";
        public List<Theme> GetThemes()
        {

            List<Theme> themes = new List<Theme>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                themes = db.Query<Theme>("SELECT * FROM test.theme").ToList();
            }
            return themes;
        }
        public Theme Get(int id)
        {
            Theme theme = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                theme = db.Query<Theme>("SELECT * FROM test.theme WHERE Id = @id",
                    new
                    {
                        id
                    }).FirstOrDefault();
                return theme;
            }
        }
        public Theme Create(Theme theme)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO test.theme (title) VALUES (@title);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                int? themeId = db.Query<int>(sqlQuery, theme).FirstOrDefault();
                theme.id = (int)themeId;
            }
            return theme;
        }

        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM test.theme WHERE Id = @id";
                db.Execute(sqlQuery, new { id });
            }
        }
    }
}
