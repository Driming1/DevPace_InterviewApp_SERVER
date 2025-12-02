using Flurl;
using Flurl.Http;
using InterviewApp.Models.Shared.ApiResponse;

namespace InterviewTestApp.Remote.Base;

public class HttpApiClient
{
    private const string BaseUrl = "https://localhost:7299/";

    private object[] _segments;

    public void SetRootSegments(params object[] segments)
    {
        _segments = segments.ToArray();
    }

    public async Task<TResponse> Get<TResponse>(string action)
    {
        var result = await RunRequest<object, ApiResponseModel<TResponse>>(action, null, RequestType.Get);
        return result != null ? result.Model : default;
    }

    public async Task<ListResponseModel<TResponse>> GetList<TResponse>(string action)
    {
        var result = await RunRequest<object, ListResponseModel<TResponse>>(action, null, RequestType.Get);

        if (result == null)
        {
            result = new ListResponseModel<TResponse>(new List<TResponse>());
        }

        return result;
    }

    public async Task<TResponse> Get<TModel, TResponse>(string action, TModel model)
    {
        var result = await RunRequest<TModel, ApiResponseModel<TResponse>>(action, model, RequestType.Get);
        return result != null ? result.Model : default;
    }

    public async Task<ListResponseModel<TResponse>> GetList<TModel, TResponse>(string action, TModel model)
    {
        var result = await RunRequest<TModel, ListResponseModel<TResponse>>(action, model, RequestType.Get);

        if (result == null)
        {
            result = new ListResponseModel<TResponse>(new List<TResponse>());
        }

        return result;
    }

    public async Task<TResponse> Post<TResponse>(string action)
    {
        var result = await RunRequest<object, ApiResponseModel<TResponse>>(action, null, RequestType.Post);
        return result != null ? result.Model : default;
    }

    public async Task<ListResponseModel<TResponse>> PostList<TResponse>(string action)
    {
        var result =  await RunRequest<object, ListResponseModel<TResponse>>(action, null, RequestType.Post);
        if (result == null)
        {
            result = new ListResponseModel<TResponse>(new List<TResponse>());
        }

        return result;
    }

    public async Task<TResponse> Post<TModel, TResponse>(string action, TModel model)
    {
        var result = await RunRequest<TModel, ApiResponseModel<TResponse>>(action, model, RequestType.Post);
        return result != null ? result.Model : default;
    }

    public async Task<ListResponseModel<TResponse>> PostList<TModel, TResponse>(string action, TModel model)
    {
        var result = await RunRequest<TModel, ListResponseModel<TResponse>>(action, model, RequestType.Post);
        if (result == null)
        {
            result = new ListResponseModel<TResponse>(new List<TResponse>());
        }

        return result;
    }

    private async Task<TResponse> RunRequest<TModel, TResponse>(
        string action,
        TModel model,
        RequestType requestType)
    {
        Task<IFlurlResponse> task = null;

        var url = BuildUrl(action);
        //Это можно переписать на switch expression в C# 8
        if (requestType == RequestType.Get)
        {
            if (!EqualityComparer<TModel>.Default.Equals(model, default(TModel)))
            {
                url.SetQueryParams(model);
            }

            task = url.GetAsync();
        }
        else if (requestType == RequestType.Post)
        {
            task = !EqualityComparer<TModel>.Default.Equals(model, default(TModel)) ? url.PostJsonAsync(model) : url.PostAsync();
        }

        return await task.ReceiveJson<TResponse>();
    }

    private Url BuildUrl(string action)
    {
        return BaseUrl
            .AppendPathSegments(_segments)
            .AppendPathSegment(action);
    }
}

public enum RequestType
{
    Get = 0,
    Post = 1
}
