using System.Collections.ObjectModel;
using InterviewApp.Models.Shared.Base;
using InterviewTestApp.Remote.Base.Services;
using InterviewTestApp.ViewModels.Base.Core;

namespace InterviewTestApp.ViewModels.Base;

public abstract class ListViewModel<TItemModel, TFilter, TItemViewModel> : BaseViewModel where TFilter : BaseFilter
{
    private TFilter _filter;
    private ObservableCollection<TItemViewModel> _items;

    protected ListViewModel(ISearchService<TFilter, TItemModel> service)
    {
        Service = service;
    }

    public SimpleAsyncDelegateCommand SearchCommand => GetAsyncCommand(() => SearchCommand, Search);

    public TFilter Filter
    {
        get => _filter;
        set => SetProperty(() => Filter, ref _filter, value);
    }

    public ObservableCollection<TItemViewModel> Items
    {
        get => _items;
        set => SetProperty(() => Items, ref _items, value);
    }

    protected ISearchService<TFilter, TItemModel> Service { get; }

    protected virtual long Take => 50;


    public virtual async Task Search()
    {
        await LoadInternal();
    }

    protected override async Task InitializeInternal()
    {

        await base.InitializeInternal();

        Filter = (TFilter)Activator.CreateInstance(typeof(TFilter));

        Filter.Take = Take;

        await LoadInternal();
    }

    protected abstract void ApplySearchParams();

    protected abstract IList<TItemViewModel> MapSearchResults(IList<TItemModel> items);

    protected virtual async Task LoadInternal()
    {
        ApplySearchParams();

        var result = await Service.Search(Filter);

        var mappedList = MapSearchResults(result).ToList();

        Items = new ObservableCollection<TItemViewModel>(mappedList);
    }
}