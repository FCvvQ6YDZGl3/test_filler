using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestFiller.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace TestFiller.DAL
{
    public class QuestionRepository
    {
        private string connectionString = "Data Source=DESKTOP-V2UKGVT;Initial Catalog=educational;Integrated Security=True";
        public List<Question> GetQuestions()
        {
            List<Question> Questions = new List<Question>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Questions = db.Query<Question>("SELECT * FROM test.Question").ToList();
            }
            return Questions;
        }
        public Question Get(int id)
        {
            Question Question = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Question = db.Query<Question>("SELECT * FROM test.Question WHERE Id = @id",
                    new
                    {
                        id
                    }).FirstOrDefault();
                return Question;
            }
        }
        public Question Create(Question Question)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO test.Question (text) VALUES (@text);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                int? QuestionId = db.Query<int>(sqlQuery, Question).FirstOrDefault();
                Question.id = (int)QuestionId;
            }
            return Question;
        }

        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM test.Question WHERE Id = @id";
                db.Execute(sqlQuery, new { id });
            }
        }
    }
}
