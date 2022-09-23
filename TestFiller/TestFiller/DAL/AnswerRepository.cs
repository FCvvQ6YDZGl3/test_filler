using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using TestFiller.Models;
using Dapper;

namespace TestFiller.DAL
{
    public class AnswerRepository
    {
        private string connectionString = "Data Source=DESKTOP-V2UKGVT;Initial Catalog=educational;Integrated Security=True";
        public List<Answer> GetAnswers()
        {
            List<Answer> Answers = new List<Answer>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Answers = db.Query<Answer>("SELECT * FROM test.Answer").ToList();
            }
            return Answers;
        }
        public Answer Get(int id)
        {
            Answer Answer = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Answer = db.Query<Answer>("SELECT * FROM test.Answer WHERE Id = @id",
                    new
                    {
                        id
                    }).FirstOrDefault();
                return Answer;
            }
        }
        public Answer Create(Answer Answer)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO test.Answer (text) VALUES (@text);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                int? AnswerId = db.Query<int>(sqlQuery, Answer).FirstOrDefault();
                Answer.id = (int)AnswerId;
            }
            return Answer;
        }

        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM test.Answer WHERE Id = @id";
                db.Execute(sqlQuery, new { id });
            }
        }
    }
}
