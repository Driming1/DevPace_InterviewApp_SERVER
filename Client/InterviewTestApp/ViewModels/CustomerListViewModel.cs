using System.Collections.ObjectModel;
using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using InterviewTestApp.Remote.Services;
using InterviewTestApp.ViewModels.Base;
using InterviewTestApp.ViewModels.Base.Core;

namespace InterviewTestApp.ViewModels;

public class CustomerListViewModel : ListViewModel<CustomerDto, CustomerFilter, CustomerDto>
{
    private const int PageSize = 100;

    private string _orderBy = "Id";
    private string _name;
    private string _phone;
    private string _email;

    private ActiveState _activeState = ActiveState.Active;
    private OrderDirection _orderDirection = OrderDirection.Ascending;
    private long _currentSkip;
    private bool _isLoading;
    private bool _hasMore = true;

    private readonly SimpleAsyncDelegateCommand _sortByNameCommand;
    private readonly SimpleAsyncDelegateCommand _sortByEmailCommand;
    private readonly SimpleAsyncDelegateCommand _sortByPhoneCommand;
    private readonly SimpleAsyncDelegateCommand _sortByActiveStateCommand;
    private readonly SimpleAsyncDelegateCommand _loadMoreCommand;

    public CustomerListViewModel(CustomerService service)
        : base(service)
    {
        _sortByNameCommand = new SimpleAsyncDelegateCommand(SortByNameAsync);
        _sortByEmailCommand = new SimpleAsyncDelegateCommand(SortByEmailAsync);
        _sortByPhoneCommand = new SimpleAsyncDelegateCommand(SortByPhoneAsync);
        _sortByActiveStateCommand = new SimpleAsyncDelegateCommand(SortByActiveStateAsync);
        _loadMoreCommand = new SimpleAsyncDelegateCommand(LoadMoreAsync);
    }

    #region Commands

    public SimpleAsyncDelegateCommand LoadMoreCommand => _loadMoreCommand;
    public SimpleAsyncDelegateCommand SortByNameCommand => _sortByNameCommand;
    public SimpleAsyncDelegateCommand SortByEmailCommand => _sortByEmailCommand;
    public SimpleAsyncDelegateCommand SortByPhoneCommand => _sortByPhoneCommand;
    public SimpleAsyncDelegateCommand SortByActiveStateCommand => _sortByActiveStateCommand;

    #endregion

    #region Properties

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

    public IEnumerable<ActiveState> ActiveStateValues => new[]
    {
        ActiveState.Active,
        ActiveState.InActive
    };

    public ActiveState ActiveState
    {
        get => _activeState;
        set => SetProperty(() => ActiveState, ref _activeState, value);
    }

    public string OrderBy
    {
        get => _orderBy;
        set => SetProperty(() => OrderBy, ref _orderBy, value);
    }

    public OrderDirection OrderDirection
    {
        get => _orderDirection;
        set => SetProperty(() => OrderDirection, ref _orderDirection, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(() => IsLoading, ref _isLoading, value);
    }

    public bool HasMore
    {
        get => _hasMore;
        private set => SetProperty(() => HasMore, ref _hasMore, value);
    }
    #endregion

    #region Methods
    protected override void ApplySearchParams()
    {
        Filter.OrderBy = OrderBy;
        Filter.OrderDirection = OrderDirection;

        Filter.Name = Name;
        Filter.Phone = Phone;
        Filter.Email = Email;
        Filter.ActiveState = ActiveState;
    }

    protected override async Task InitializeInternal()
    {
        Filter = new CustomerFilter
        {
            Skip = 0,
            Take = PageSize,
            OrderBy = OrderBy,
            OrderDirection = OrderDirection,
            ActiveState = ActiveState
        };

        Items = new ObservableCollection<CustomerDto>();

        _currentSkip = 0;
        HasMore = true;

        await LoadPageAsync(reset: true);
    }


    public override async Task Search()
    {
        _currentSkip = 0;
        HasMore = true;

        if (Items == null)
            Items = new ObservableCollection<CustomerDto>();
        else
            Items.Clear();

        await LoadPageAsync(reset: true);
    }

    private async Task<bool> LoadMoreAsync()
    {
        return await LoadPageAsync(reset: false);
    }

    private async Task<bool> LoadPageAsync(bool reset)
    {
        if (IsLoading || !HasMore)
            return false;

        IsLoading = true;

        try
        {
            ApplySearchParams();

            Filter.Skip = _currentSkip;
            Filter.Take = PageSize;

            var result = await Service.Search(Filter);
            var list = MapSearchResults(result).ToList();

            if (reset)
            {
                Items = new ObservableCollection<CustomerDto>(list);
            }
            else
            {
                foreach (var item in list)
                    Items.Add(item);
            }

            _currentSkip += list.Count;
            HasMore = list.Count == PageSize;

            return true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<bool> ApplySortAndReloadAsync(string column)
    {
        if (OrderBy == column)
        {
            OrderDirection = OrderDirection == OrderDirection.Ascending
                ? OrderDirection.Descending
                : OrderDirection.Ascending;
        }
        else
        {
            OrderBy = column;
            OrderDirection = OrderDirection.Ascending;
        }

        await Search();
        return true;
    }

    private Task<bool> SortByNameAsync() => ApplySortAndReloadAsync("Name");
    private Task<bool> SortByEmailAsync() => ApplySortAndReloadAsync("Email");
    private Task<bool> SortByPhoneAsync() => ApplySortAndReloadAsync("Phone");
    private Task<bool> SortByActiveStateAsync() => ApplySortAndReloadAsync("ActiveState");

    protected override IList<CustomerDto> MapSearchResults(IList<CustomerDto> items)
    {
        return items;
    }

    #endregion
}