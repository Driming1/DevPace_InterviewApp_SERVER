namespace InterviewApp.Models.Shared.ApiResponse
{
    public class ListResponseModel<T> : ApiResponseModel
    {
        public ListResponseModel()
        {

        }

        public ListResponseModel(IList<T> list)
        {
            List = list;
            RowsCount = list?.Count ?? 0;
        }

        public ListResponseModel(IList<T> list, long rowsCount)
        {
            List = list;
            RowsCount = rowsCount;
        }

        public IList<T> List { get; set; }

        public long RowsCount { get; set; }
    }
}