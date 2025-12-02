namespace InterviewApp.Models.Shared
{
    public class CustomerDto
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
