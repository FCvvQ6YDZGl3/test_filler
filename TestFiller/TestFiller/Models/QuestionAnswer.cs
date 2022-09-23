using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestFiller.Models
{
    public class QuestionAnswer
    {
        public int questionId { get; set; }
        public int answerId {get; set;}
        public bool isCorrect { get; set; }
    }
}
