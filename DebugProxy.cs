using System;
using System.Net;
using System.Net.Http;

namespace laui
{
    public class DebugProxy : IWebProxy
    {
        public Uri ProxyUri { get; set; }
        public ICredentials Credentials { get; set; }

        public DebugProxy(string uri) 
            : this (new Uri(uri))
        {
        }

        public DebugProxy(Uri uri)
        {
            this.ProxyUri = uri;
        }

        public Uri GetProxy(Uri destination)
        {
            return ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            // All requests flow through the proxy
            return false;
        }
    }
}