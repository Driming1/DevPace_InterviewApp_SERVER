using InterviewApp.Data;
using InterviewApp.Models.Shared;

namespace InterviewApp.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IList<CustomerDto> Search(CustomerFilter filterModel)
        {
            return _customerRepository.Search(filterModel);
        }
    }
}
