namespace SurveyAuth.Models
{
    public class Question
    {
        public int Id {get; set;}
        public int SurveyId {get; set;}
        public string Text {get; set;}
        public string Description {get; set;}
        public Survey Survey {get; set;}
    }
}