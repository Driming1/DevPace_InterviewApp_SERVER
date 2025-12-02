using System.ComponentModel.DataAnnotations;
using InterviewApp.Models.Shared.Enums;

namespace InterviewApp.Models.Shared.Base;

public class BaseFilter
{
    [Required]
    public long Skip { get; set; }

    [Required]
    public long Take { get; set; }

    public string OrderBy { get; set; }

    public OrderDirection OrderDirection { get; set; }
}