using System.Threading.Tasks;
using Jal.HttpClient.Interface.Fluent;
using Jal.HttpClient.Model;

namespace Jal.HttpClient.Impl.Fluent
{
    public class HttpContentDescriptor : IHttpContentTypeDescriptor
    {
        private readonly System.Net.Http.HttpContent _httpcontent;

        private readonly HttpDescriptorContext _httpcontext;

        public HttpContentDescriptor(System.Net.Http.HttpContent httpcontent, HttpDescriptorContext httpcontext)
        {
            _httpcontent = httpcontent;
            _httpcontext = httpcontext;
        }

        public IHttpContentTypeDescriptor WithEncoding(string encoding)
        {
            _httpcontent.Headers.ContentEncoding.Add(encoding);
            return this;
        }

        public IHttpContentTypeDescriptor WithContentType(string contenttype)
        {
            _httpcontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contenttype);
            return this;
        }

        public HttpResponse Send()
        {
            if (_httpcontext.QueryParemeterDescriptorAction != null)
            {
                var queryParemeterDescriptor = new HttpQueryParameterDescriptor(_httpcontext.HttpRequest);
                _httpcontext.QueryParemeterDescriptorAction(queryParemeterDescriptor);
            }

            if (_httpcontext.MiddlewareDescriptorAction != null)
            {
                var middlewareParemeterDescriptor = new HttpMiddlewareDescriptor(_httpcontext.HttpRequest);
                _httpcontext.MiddlewareDescriptorAction(middlewareParemeterDescriptor);
            }

            if (_httpcontext.HeaderDescriptorAction != null)
            {
                var headerDescriptor = new HttpHeaderDescriptor(_httpcontext.HttpRequest);
                _httpcontext.HeaderDescriptorAction(headerDescriptor);
            }

            return _httpcontext.HttpHandler.Send(_httpcontext.HttpRequest);
        }

        public async Task<HttpResponse> SendAsync()
        {
            if (_httpcontext.QueryParemeterDescriptorAction != null)
            {
                var queryParemeterDescriptor = new HttpQueryParameterDescriptor(_httpcontext.HttpRequest);
                _httpcontext.QueryParemeterDescriptorAction(queryParemeterDescriptor);
            }

            if (_httpcontext.MiddlewareDescriptorAction != null)
            {
                var middlewareParemeterDescriptor = new HttpMiddlewareDescriptor(_httpcontext.HttpRequest);
                _httpcontext.MiddlewareDescriptorAction(middlewareParemeterDescriptor);
            }

            if (_httpcontext.HeaderDescriptorAction != null)
            {
                var headerDescriptor = new HttpHeaderDescriptor(_httpcontext.HttpRequest);
                _httpcontext.HeaderDescriptorAction(headerDescriptor);
            }

            return await _httpcontext.HttpHandler.SendAsync(_httpcontext.HttpRequest);
        }

    }
}