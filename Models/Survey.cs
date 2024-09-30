using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyAuth.Models
{
    public class Survey
    {
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [DisplayName("Name")]
    public string Name { get; set; }
    public string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    }
}