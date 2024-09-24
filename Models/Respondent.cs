namespace SurveyAuth.Models
{
    // No longer needed, since we're using AspNetUsers instead
    public class Respondent
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Email {get; set;}
        public DateTime CreatedAt {get; set;}
    }
}