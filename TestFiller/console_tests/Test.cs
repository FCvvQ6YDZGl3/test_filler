using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperApplication.Models
{
    public class Theme
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect  { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class QuestionRepository
    {
        string connectionString = ConfigurationManager.ConnectionStrings["educational"].ConnectionString;
        public List<Theme> GetThemes()
        {
            List<Theme> themes = new List<Theme>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                themes = db.Query<Theme>("SELECT * FROM test.theme").ToList();
            }
            return themes;
        }
        public List<Question> GetQuestions(int idTheme)
        {
            List<Question> questions = new List<Question>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                questions = db.Query<Question>("SELECT * FROM test.Question q left join test.themeQuestion tq ON tq.questionId = q.id where tq.themeId = @idTheme", new { idTheme }).ToList();
            }
            return questions;
        }

        public List<Answer> GetAnswersOnQuestionId(int idQuestion)
        {
            List<Answer> answers = new List<Answer>();
            string textQuery = "SELECT a.*, qa.isCorrect FROM test.Answer a "
                                + "INNER JOIN test.QuestionAnswer qa ON qa.answerId = a.id AND qa.questionId = @idQuestion";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                answers = db.Query<Answer>(textQuery, new { idQuestion }).ToList();
            }
            return answers;
        }

        public User Get(int id)
        {
            User user = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                user = db.Query<User>("SELECT * FROM Users WHERE Id = @id", new { id }).FirstOrDefault();
            }
            return user;
        }

        public User Create(User user)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "INSERT INTO Users (Name, Age) VALUES(@Name, @Age); SELECT CAST(SCOPE_IDENTITY() as int)";
                int? userId = db.Query<int>(sqlQuery, user).FirstOrDefault();
                user.Id = (int)userId;
                }
            return user;
        }

        public void Update(User user)
            {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
            var sqlQuery = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id";
            db.Execute(sqlQuery, user);
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
                {
                var sqlQuery = "DELETE FROM Users WHERE Id = @id";
                db.Execute(sqlQuery, new { id });
                }
        }
    }


}

class Test
{
    DapperApplication.Models.QuestionRepository repository;

    private question[] questions;
    private byte numberOfCorrectAnswers;
    private theme[] themes;
    private byte theme_id;
    private void initTheme()
    {
        int i = 0;
        var ts = repository.GetThemes();
        i = 0;
        themes = new theme[ts.Count];
        foreach (DapperApplication.Models.Theme theme in ts)
        {
            themes[i] = new theme(theme);
            i++;
        }
    }

    private void initQuestion()
    {
        var qs = repository.GetQuestions(theme_id);
        int i = 0;
        questions = new question[qs.Count];
        foreach (DapperApplication.Models.Question q in qs)
        {
            questions[i] = new question(q.Text, repository.GetAnswersOnQuestionId(q.Id).ToArray());
            i++;
        }
    }


    private void Initialize()
    {
        numberOfCorrectAnswers = 0;
        repository = new DapperApplication.Models.QuestionRepository();
    }

    public bool offerToBeTested()
    {
        const string welcome = "Добро пожаловать на тестирование по языку C# 4.0 !";
        string userResponse;
        Console.WriteLine(welcome);

        initTheme();
        suggestATopic();
        userResponse = Console.ReadLine();
        theme_id = (byte)themes[Convert.ToByte(userResponse) - 1].id;

        initQuestion();
        Console.WriteLine("Введите \"start\" для начала теста:");
        userResponse = Console.ReadLine();
        return (userResponse == "start");
    }

    private void suggestATopic()
    {
        string numberedThemes;
        byte leftPadding;
        numberedThemes = "";
        leftPadding = (byte)themes.Length.ToString().Length;
        for (int i = 0; i < themes.Length; i++)
        {
            numberedThemes += String.Format("{0} - {1}\n", ((i + 1).ToString().PadLeft(leftPadding,' ')), themes[i].title);
        }
        Console.WriteLine("Выберите тему тестирования (введите номер):");
        Console.WriteLine(numberedThemes);
    }
    private void notifyNonExistentResponse(char answer)
    {
        Console.WriteLine("Введенной вами буквы {0} нет в предложенных ответах! Нажимте любую клавишу для продолжения.", answer);
    }
    public void run()
    {
        Initialize();

        if (!offerToBeTested())
        {
            Environment.Exit(exitCode: 0);
        }
        Console.Clear();
        

        question q;
        char answer;
        bool answerIsIncluded;
        try
        {
            for (byte i = 0; i < questions.Length; i++)
            {
                q = questions[i];
                q.askQuestion();
                q.shuffleAnswers();
                q.suggestAnswerOptions();
                answer = q.getAnAnswer();

                answerIsIncluded = q.checkOccurrenceOfUserResponse(answer);
                if (!answerIsIncluded)
                {
                    notifyNonExistentResponse(answer);
                    Console.ReadKey();
                    i--;
                    Console.Clear();
                    continue;
                }

                if (q.checkTheAnswer(answer))
                {
                    numberOfCorrectAnswers++;
                }

                Console.Clear();
            }
        }
        catch (IndexOutOfRangeException exception)
        {
            Console.WriteLine("Выход за границы массива в строке {0}", exception.StackTrace);
        }

        repotTheEndOfTheTest();
    }
    public void repotTheEndOfTheTest()
    {
        Console.WriteLine("Тестирование завершено.");
        Console.WriteLine(String.Format("Успешно: {0} %", calcThePercentageOfSuccess()));
    }
    private byte calcThePercentageOfSuccess()
    {
        double percentageOfSuccess = 0.0;
        try
        {
            percentageOfSuccess = (Convert.ToDouble(numberOfCorrectAnswers) / Convert.ToDouble(numberOfQuestions())) * 100;
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Количество вопросов в тесте равно нулю {0}!", numberOfQuestions());
        }
        return (byte)(percentageOfSuccess);
    }

    private byte numberOfQuestions()
    {
        return (byte)questions.Length;
    }
}

class theme
{
    public int id { get; }
    public string title { get; }
    public theme(DapperApplication.Models.Theme _theme)
    {
        id = _theme.Id;
        title = _theme.Title;
    }
}
class question
{
    public const byte startingIndexOfAnswerNumbering = 97;
    public string questionText;
    public DapperApplication.Models.Answer[] answerOptions;

    public question(string _questionText, DapperApplication.Models.Answer[] _answerOptions)
    {
        questionText = _questionText;
        answerOptions = _answerOptions;
    }
    public void askQuestion()
    {
        Console.WriteLine(questionText);
    }
    public void suggestAnswerOptions()
    {
        Console.WriteLine();
        for (byte i = 0; i < answerOptions.Length; i++)
        {
            Console.WriteLine((char)(startingIndexOfAnswerNumbering + i) + ")" + answerOptions[i].Text + "\t");
        }
        Console.WriteLine();
        Console.WriteLine("Введите правильную букву ответа:");
    }

    public void shuffleAnswers()
    {
        Random rnd = new Random();
        answerOptions = answerOptions.OrderBy(x => rnd.Next()).ToArray();
    }

    public char getAnAnswer()
    {
        char selectedAnswer;
        selectedAnswer = (char)Console.ReadLine()[0];
        return selectedAnswer;
    }
    public bool checkOccurrenceOfUserResponse(char userResponse)
    {
        bool included;
        included = startingIndexOfAnswerNumbering <= userResponse
            && userResponse < startingIndexOfAnswerNumbering + answerOptions.Length;
        return included;
    }
    public bool checkTheAnswer(char answer)
    {
        bool isCorrect = true;
        isCorrect = isCorrect && answerOptions[(int)(answer) - startingIndexOfAnswerNumbering].IsCorrect;
        return isCorrect;
    }
}