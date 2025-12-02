using InterviewApp.Models.Shared;
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
}