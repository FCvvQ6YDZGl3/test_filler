using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestFiller.Models
{
    public class QuestionAndAnswersInput
    {
        public List<Theme> themes { get; set; }
        public int themeId { get; set; }
        public string questionText { get; set; }
        public string answerText1 { get; set; }
        public string answerText2 { get; set; }
        public string answerText3 { get; set; }
        public string answerText4 { get; set; }
        public string answerText5 { get; set; }
    }
}