namespace InterviewApp.Models.Shared.ApiResponse
{
    public class ApiResponseModel
    {
        private bool? _status;
        public bool Status
        {
            get => _status ?? Errors == null || Errors.Count == 0;
            set => _status = value;
        }

        public string Message { get; set; }

        public Dictionary<string, string> Errors { get; set; }

        public static T CreateError<T>(string errorType, string errorMessage) where T : ApiResponseModel, new()
        {
            return new T
            {
                Errors = new Dictionary<string, string>()
                {
                    { errorType, errorMessage }
                }
            };
        }
    }

    public class ApiResponseModel<TModel> : ApiResponseModel
    {
        public ApiResponseModel()
        {
        }

        public ApiResponseModel(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; set; }
    }
}