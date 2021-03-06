﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Jal.ChainOfResponsability;
using System.Linq;

namespace Jal.HttpClient.Serilog
{
    public class SerilogMiddelware : IAsyncMiddleware<HttpContext>
    {
        private async Task BuildResponseLog(HttpResponse response, HttpRequest request)
        {
            var log = Log.Logger.ForContext("Type", "client-response");

            if (response.Message!=null && response.Message.Content != null && IsString(response.Message.Content))
            {
                try
                {
                    log = log.ForContext("Content", await response.Message.Content.ReadAsStringAsync().ConfigureAwait(false), true);
                }
                catch (Exception)
                {
                    log = log.ForContext("Content", "error to log", true);
                }
            }

            if (response.Message != null && response.Message.Headers != null)
            {
                try
                {
                    log = log.ForContext("Headers", response.Message.Headers.ToDictionary(x => x.Key, x => x.Value), true);
                }
                catch (Exception)
                {
                    log = log.ForContext("Headers", "error to log", true);
                }
            }

            if (response.Message!=null)
            {
                if (response.Exception != null)
                {
                    log.Error(response.Exception, "RequestId: {RequestId}, RequestUri: {RequestUri}, Duration: {Duration} ms, StatusCode: {StatusCode}, ReasonPhrase: {ReasonPhrase}, Version: {Version}", request.Tracing.RequestId, request.Message.RequestUri.ToString(), response.Duration, response.Message.StatusCode, response.Message.ReasonPhrase.ToString(), response.Message.Version.ToString());
                }
                else
                {
                    log.Debug("RequestId: {RequestId}, RequestUri: {RequestUri}, Duration: {Duration} ms, StatusCode: {StatusCode}, ReasonPhrase: {ReasonPhrase}, Version: {Version}", request.Tracing.RequestId, request.Message.RequestUri.ToString(), response.Duration, response.Message.StatusCode, response.Message.ReasonPhrase.ToString(), response.Message.Version.ToString());
                }
            }
            else
            {
                if (response.Exception != null)
                {
                    log.Error(response.Exception, "RequestId: {RequestId}, RequestUri: {RequestUri}, Duration: {Duration} ms", request.Tracing.RequestId, request.Message.RequestUri.ToString(), response.Duration);
                }
                else
                {
                    log.Debug("RequestId: {RequestId}, RequestUri: {RequestUri}, Duration: {Duration} ms", request.Tracing.RequestId, request.Message.RequestUri.ToString(), response.Duration);
                }
            }
        }

        public bool IsString(HttpContent content)
        {
            var contenttype = content?.Headers?.ContentType?.MediaType;

            return !string.IsNullOrWhiteSpace(contenttype) && (contenttype.Contains("text") || contenttype.Contains("xml") || contenttype.Contains("json") || contenttype.Contains("html"));
        }

        private async Task BuildRequestLog(HttpRequest request)
        {
            var log = Log.Logger.ForContext("Type", "client-request");

            if (request.Message.Content != null && request.Message.Content is StringContent)
            {
                try
                {
                    log = log.ForContext("Content", await request.Message.Content.ReadAsStringAsync().ConfigureAwait(false), true);
                }
                catch (Exception)
                {
                    log = log.ForContext("Content", "error to log", true);
                }
                
            }

            if(request.Message.Headers!=null)
            {
                try
                {
                    log = log.ForContext("Headers", request.Message.Headers.ToDictionary(x => x.Key, x => x.Value), true);
                }
                catch (Exception)
                {
                    log = log.ForContext("Headers", "error to log", true);
                }
                
            }

            log.Debug("RequestId: {RequestId}, Method: {Method}, RequestUri: {RequestUri}, Version: {Version}", request.Tracing.RequestId, request.Message.Method, request.Message.RequestUri.ToString(), request.Message.Version.ToString());
        }

        public async Task ExecuteAsync(AsyncContext<HttpContext> context, Func<AsyncContext<HttpContext>, Task> next)
        {
            await BuildRequestLog(context.Data.Request);

            await next(context);

            await BuildResponseLog(context.Data.Response, context.Data.Request);
        }
    }
}
