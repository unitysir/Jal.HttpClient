﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Jal.HttpClient.Interface;
using Jal.HttpClient.Model;

namespace Jal.HttpClient.Impl
{
    public class HttpRequestToWebRequestConverter : IHttpRequestToWebRequestConverter
    {
        private readonly IHttpContentTypeBuilder _httpContentTypeBuilder;

        private readonly IHttpMethodMapper _httpMethodMapper;

        public HttpRequestToWebRequestConverter(IHttpContentTypeBuilder httpContentTypeBuilder, IHttpMethodMapper httpMethodMapper)
        {
            _httpContentTypeBuilder = httpContentTypeBuilder;

            _httpMethodMapper = httpMethodMapper;
        }

        public WebRequest Convert(HttpRequest httpRequest, int timeout)
        {
            var url = httpRequest.Url;

            if (httpRequest.QueryParameters.Count > 0)
            {
                url = new UriBuilder(url) { Query = BuildQueryParameters(httpRequest.QueryParameters) }.Uri.ToString();
            }

            var request = WebRequest.Create(new Uri(url));

            request.Method = _httpMethodMapper.Map(httpRequest.HttpMethod);

            request.Timeout = httpRequest.Timeout < 0 ? timeout : httpRequest.Timeout;

            if (!string.IsNullOrEmpty(httpRequest.HttpContentType))
            {
                request.ContentType = _httpContentTypeBuilder.Build(httpRequest.HttpContentType, httpRequest.HttpCharacterSet);
            }
           
            if (httpRequest.Headers.Count > 0)
            {
                WriteHeaders(httpRequest, request);
            }

            if (!string.IsNullOrWhiteSpace(httpRequest.Body))
            {
                WriteContent(httpRequest, request);
            }

            return request;

        }

        private string BuildQueryParameters(List<HttpQueryParameter> httpQueryParameters)
        {
            var builder = new StringBuilder();

            foreach (var httpQueryParameter in httpQueryParameters.Where(httpQueryParameter => !string.IsNullOrWhiteSpace(httpQueryParameter.Value)))
            {
                builder.AppendFormat("{0}={1}&", httpQueryParameter.Name, httpQueryParameter.Value);
            }

            var parameter = builder.ToString();

            if (!string.IsNullOrWhiteSpace(parameter))
            {
                parameter = parameter.Substring(0, parameter.Length - 1);
            }
            return parameter;
        }


        private void WriteHeaders(HttpRequest httpRequest, WebRequest webRequest)
        {
            foreach (var header in httpRequest.Headers)
            {
                if (webRequest.Headers.AllKeys.Contains(header.Name))
                {
                    webRequest.Headers.Remove(header.Name);
                }
                webRequest.Headers.Add(header.Name, header.Value);
            }
        }

        private void WriteContent(HttpRequest webHandlerRequest, WebRequest request)
        {
            request.ContentLength = Encoding.UTF8.GetByteCount(webHandlerRequest.Body);

            using (var writeStream = request.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(webHandlerRequest.Body);

                writeStream.Write(bytes, 0, bytes.Length);

                writeStream.Flush();
            }
        }
    }
}
