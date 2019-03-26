﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Jal.HttpClient.Model
{
    public class HttpRequest : IDisposable
    {
        public System.Net.Http.HttpRequestMessage Message { get; set; }

        public System.Net.Http.HttpContent Content
        {
            get
            {
                return Message?.Content;
            }
            set
            {
                Message.Content = value;
            }
        }

        public System.Net.Http.HttpMethod Method
        {
            get
            {
                return Message?.Method;
            }
        }

        public System.Net.Http.Headers.HttpRequestHeaders Headers
        {
            get
            {
                return Message?.Headers;
            }
        }

        public Uri Uri
        {
            get
            {
                return Message?.RequestUri;
            }
            set
            {
                Message.RequestUri = value;
            }
        }

        public System.Net.Http.HttpClient HttpClient { get; internal set; }

        public bool DisposeClient { get; }

        public List<Type> MiddlewareTypes { get; }

        public Dictionary<string,object> Context { get; }

        public List<HttpQueryParameter> QueryParameters { get; set; }

        public int Timeout {
            get
            {
                return HttpClient.Timeout.Milliseconds;
            }
            set
            {
                if(value>0)

                HttpClient.Timeout = TimeSpan.FromMilliseconds(value);
            }

        }

        public HttpIdentity Identity { get; set; }



        public HttpRequest(string uri, System.Net.Http.HttpMethod httpMethod):
        this(uri, httpMethod, new System.Net.Http.HttpClient())
        {
            DisposeClient = true;
        }

        public HttpRequest(string uri, System.Net.Http.HttpMethod httpMethod, Func<System.Net.Http.HttpClient> factory) :
        this(uri, httpMethod, factory())
        {

        }

        public HttpRequest(string uri, System.Net.Http.HttpMethod httpMethod, System.Net.Http.HttpClient httpclient)
        {
            Message = new System.Net.Http.HttpRequestMessage(httpMethod, uri);

            HttpClient = httpclient;
            
            QueryParameters = new List<HttpQueryParameter>();

            MiddlewareTypes = new List<Type>();

            Identity = new HttpIdentity(Guid.NewGuid().ToString());

            Context = new Dictionary<string, object>();

            DisposeClient = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Message != null)
                {
                    Message.Dispose();
                }

                Message = null;

                if (HttpClient != null && DisposeClient)
                {
                    HttpClient.Dispose(); 
                }

                HttpClient = null;
            }
        }
    }
}