namespace System.Web
{
    public static class HttpRequestExtensions
    {
        public static bool IsContentPickerRequest(this HttpRequestBase request) =>
            request.RawUrl.Contains("/ContentPicker/");
    }
}