using InterviewApp.Models.Shared;

namespace InterviewApp.Data
{
    public interface ICustomerRepository
    {
        IList<CustomerDto> Search(CustomerFilter filterModel);
    }
}
