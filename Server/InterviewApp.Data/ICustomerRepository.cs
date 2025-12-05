using InterviewApp.Domain.Customers;
using InterviewApp.Models.Shared;

namespace InterviewApp.Data
{
    public interface ICustomerRepository
    {
        IList<CustomerDto> Search(CustomerFilter filterModel);
        CustomerEntity GetById(long id);
        void Save(CustomerEntity entity);
        bool IsEmailUnique(string email, long? id);
    }
}
