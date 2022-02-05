using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace System.Web
{
    public static class HttpRequestExtensions
    {
        public static bool IsContentPickerRequest(this HttpRequest request) =>
            UriHelper.GetEncodedUrl(request).Contains("/ContentPicker/", StringComparison.InvariantCultureIgnoreCase);
    }
}
