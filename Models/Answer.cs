using SurveyAuth.Data;

namespace SurveyAuth.Models
{
    public class Answer
    {
        public int Id {get; set;}
        public string ApplicationUserId {get; set;}
        public int QuestionId {get; set;}
        public string Text {get; set;}
        public DateTime LastModified {get; set;} = DateTime.Now;
        public ApplicationUser Respondent {get; set;}
        public Question Question {get; set;}
    }
}