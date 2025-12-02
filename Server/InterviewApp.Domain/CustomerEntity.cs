using InterviewApp.Domain.Base;

namespace InterviewApp.Domain
{
    public class CustomerEntity : EntityBase
    {
        public virtual string Name { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }
    }
}
