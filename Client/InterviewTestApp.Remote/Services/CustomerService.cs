using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.ApiResponse;
using InterviewTestApp.Remote.Base;
using InterviewTestApp.Remote.Base.Services;

namespace InterviewTestApp.Remote.Services;

public class CustomerService : ISearchService<CustomerFilter, CustomerDto>
{
    private readonly HttpApiClient _httpClient;

    public CustomerService(HttpApiClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.SetRootSegments("Customer");
    }

    public async Task<IList<CustomerDto>> Search(CustomerFilter filter)
    {
        var result = await _httpClient.PostList<CustomerFilter, CustomerDto>("Search", filter);
        return result.List;
    }
    public async Task<CustomerDto> SaveAsync(CustomerDto dto)
    {
        return await _httpClient.Post<CustomerDto, CustomerDto>("Save", dto);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, long? excludeId = null)
    {
        var body = new EmailCheckRequest { Email = email , Id = excludeId };
        var response = await _httpClient.PostApiResponse<EmailCheckRequest, bool>(
       "IsEmailUnique",
       body);

        return response.Model;
    }
    public async Task<CustomerDto> GetByIdAsync(long id)
    {
        return await _httpClient.Get<CustomerDto>(id.ToString());
    }
}