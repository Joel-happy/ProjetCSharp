namespace ApiWebApp.Models
{
    public class ApiResult<T>
    {
        // T ---> placeholder for type (string, int, etc)
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        
        // If ErorrMessage == null ---> IsSuccess == true
        public bool IsSuccess => ErrorMessage == null;
    }
}