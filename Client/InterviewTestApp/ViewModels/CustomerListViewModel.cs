using InterviewApp.Models.Shared;
using InterviewTestApp.Remote.Services;
using InterviewTestApp.ViewModels.Base;

namespace InterviewTestApp.ViewModels;

public class CustomerListViewModel : ListViewModel<CustomerDto, CustomerFilter, CustomerDto>
{
    private string _name;
    private string _phone;
    private string _email;

    public CustomerListViewModel(CustomerService service)
        : base(service)
    {
    }

    public string Name
    {
        get => _name;
        set => SetProperty(() => Name, ref _name, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(() => Phone, ref _phone, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(() => Email, ref _email, value);
    }

    protected override void ApplySearchParams()
    {
        Filter.Name = Name;
        Filter.Phone = Phone;
        Filter.Email = Email;
    }

    protected override IList<CustomerDto> MapSearchResults(IList<CustomerDto> items)
    {
        return items;
    }
}