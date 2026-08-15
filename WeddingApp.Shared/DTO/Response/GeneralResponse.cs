
namespace WeddingApp.Shared.DTO.Response
{
    public class GeneralResponse<T>
    {
        public bool IsSuccess { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
