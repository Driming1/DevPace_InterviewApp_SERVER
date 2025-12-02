using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.ApiResponse;
using InterviewApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("Search")]
        [HttpPost]
        public ListResponseModel<CustomerDto> Search(CustomerFilter filterModel)
        {
            var result = _customerService.Search(filterModel);
            return new ListResponseModel<CustomerDto>(result);
        }
    }
}
