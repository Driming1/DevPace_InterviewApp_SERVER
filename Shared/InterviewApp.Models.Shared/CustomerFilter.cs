using InterviewApp.Models.Shared.Base;

namespace InterviewApp.Models.Shared
{
    public class CustomerFilter : BaseFilter
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
