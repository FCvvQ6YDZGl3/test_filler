using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestFiller.Models;

namespace TestFiller.Controllers
{
    public class HomeController : Controller
    {
        private DAL.ThemeRepository themeRepository = new DAL.ThemeRepository();
        private DAL.QuestionRepository questionRepository = new DAL.QuestionRepository();
        private DAL.QuestionThemeRepository questionThemeRepository = new DAL.QuestionThemeRepository();
        private DAL.AnswerRepository answerRepository = new DAL.AnswerRepository();
        private DAL.QuestionAnswerRepository questionAnswerRepository = new DAL.QuestionAnswerRepository();
        public IActionResult Index()
        {
            return View("Welcome");
        }

        public ViewResult ListThemes()
        {
            var themes = themeRepository.GetThemes();
            return View(themes);
        }
        [HttpGet]
        public ViewResult AddTheme()
        {
            return View();
        }
        [HttpPost]
        public ViewResult AddTheme(Theme theme)
        {
            themeRepository.Create(theme);
            var themes = themeRepository.GetThemes();
            return View("ListThemes", themes);
        }

        [HttpGet]
        public ViewResult DeleteTheme(int themeId)
        {
            themeRepository.Delete(themeId);
            var themes = themeRepository.GetThemes();
            return View("ListThemes", themes);
        }
        public ViewResult Question()
        {
            var questions = questionRepository.GetQuestions();
            return View(questions);
        }
        [HttpGet]
        public ViewResult QuesAndAnswForm()
        {
            QuestionAndAnswersInput questionAndAnswersInput = new QuestionAndAnswersInput();
            var themes = themeRepository.GetThemes();
            questionAndAnswersInput.themes = themes;
            return View(questionAndAnswersInput);
        }
        [HttpPost]
        public ViewResult QuesAndAnswForm(QuestionAndAnswersInput questionAndAnswersInput)
        {
            if (ModelState.IsValid)
            {
                Question question = new Question();
                question.text = questionAndAnswersInput.questionText;
                questionRepository.Create(question);

                ThemeQuestion themeQuestion = new ThemeQuestion();
                themeQuestion.questionId = question.id;
                themeQuestion.themeId = questionAndAnswersInput.themeId;
                questionThemeRepository.Create(themeQuestion);

                QuestionAnswer questionAnswer = new QuestionAnswer();
                questionAnswer.isCorrect = true;
                questionAnswer.questionId = question.id;

                Answer answer = new Answer();
                answer.text = questionAndAnswersInput.answerText1;
                answerRepository.Create(answer);
                questionAnswer.answerId = answer.id;
                questionAnswerRepository.Create(questionAnswer);

                questionAnswer.isCorrect = false;

                answer.text = questionAndAnswersInput.answerText2;
                answerRepository.Create(answer);
                questionAnswer.answerId = answer.id;
                questionAnswerRepository.Create(questionAnswer);

                answer.text = questionAndAnswersInput.answerText3;
                answerRepository.Create(answer);
                questionAnswer.answerId = answer.id;
                questionAnswerRepository.Create(questionAnswer);

                answer.text = questionAndAnswersInput.answerText4;
                answerRepository.Create(answer);
                questionAnswer.answerId = answer.id;
                questionAnswerRepository.Create(questionAnswer);

                answer.text = questionAndAnswersInput.answerText5;
                answerRepository.Create(answer);
                questionAnswer.answerId = answer.id;
                questionAnswerRepository.Create(questionAnswer);

                var questions = questionRepository.GetQuestions();
                return View("Question", questions);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ViewResult DeleteQuestion(int questionId)
        {
            questionRepository.Delete(questionId);
            var questions = questionRepository.GetQuestions();
            return View("Question", questions);
        }
    }
}