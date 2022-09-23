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
    public class QuestionAnswerRepository
    {
        private string connectionString = "Data Source=DESKTOP-V2UKGVT;Initial Catalog=educational;Integrated Security=True";
        public void Create(QuestionAnswer questionAnswer)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO test.questionAnswer (questionId, answerId, isCorrect) 
                                    VALUES (@questionId, @answerId, @isCorrect);";
                db.Execute(sqlQuery, questionAnswer);
            }
            return;
        }
    }
}
