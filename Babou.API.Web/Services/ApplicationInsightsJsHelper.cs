using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Babou.API.Web.Services
{
    public class ApplicationInsightsJsHelper
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly JavaScriptSnippet _aiJavaScriptSnippet;

        public ApplicationInsightsJsHelper(IHttpContextAccessor httpContext, JavaScriptSnippet aiJavaScriptSnippet)
        {
            _httpContext = httpContext;
            _aiJavaScriptSnippet = aiJavaScriptSnippet;
        }
        public string Script
        {
            get
            {
                var js = _aiJavaScriptSnippet.FullScript;
                const string scriptTagStart = @"<script type=""text/javascript"">";
                var scriptTagStartWithNonce = $"<script type=\"text/javascript\" nonce=\"{_httpContext.HttpContext.Items["csp-nonce"]}\">";
                var script = js.Replace(scriptTagStart, scriptTagStartWithNonce);
                return script;
            }
        }
    }

}
