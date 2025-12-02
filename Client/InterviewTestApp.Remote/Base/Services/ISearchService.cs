namespace InterviewTestApp.Remote.Base.Services;

public interface ISearchService<TFilter, TModel>
{
    Task<IList<TModel>> Search(TFilter filter);
}