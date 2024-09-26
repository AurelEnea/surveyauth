using System.ComponentModel.DataAnnotations;

namespace SurveyAuth.Models
{
    public class Survey
    {
    //[Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "Survey name is required")]
    public string Name { get; set; }
    public string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    }
}