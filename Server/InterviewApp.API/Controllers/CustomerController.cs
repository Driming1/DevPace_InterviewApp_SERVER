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

        [HttpGet("{id:long}")]
        public ActionResult<ApiResponseModel<CustomerDto>> Get(long id)
        {
            var dto = _customerService.GetById(id);
            if (dto == null)
            {
                return NotFound(ApiResponseModel.CreateError<ApiResponseModel<CustomerDto>>("NotFound", "Customer not found"));
            }

            return Ok(new ApiResponseModel<CustomerDto>(dto));
        }

        [Route("Save")]
        [HttpPost]
        public ActionResult<CustomerDto> Save(CustomerDto dto)
        {
            var saved = _customerService.Save(dto);
            return Ok(saved);
        }

        [Route("IsEmailUnique")]
        [HttpPost]
        public ApiResponseModel<bool> IsEmailUnique(EmailCheckRequest dto)
        {
            var result = _customerService.IsEmailUnique(dto.Email, dto.Id);
            return new ApiResponseModel<bool>(result);
        }
    }
}
