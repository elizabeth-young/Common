using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Common.Helpers
{
    public class Http
    {
        private Uri _baseUrl;
        private CookieContainer _cookies;

        public Http()
        {
            _cookies = new CookieContainer();
        }
        public Http(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
            _cookies = new CookieContainer();
        }

        public HttpWebResponse PostNameValues(string url, object data)
        {
            return Post(data.ToNameValuePairs(), "application/x-www-form-urlencoded", url);
        }

        public HttpWebResponse PostNameValueString(string url, string nameValues)
        {
            return Post(nameValues, "application/x-www-form-urlencoded", url);
        }

        public HttpWebResponse PostJson(string url, string json, int timeout = 100000)
        {
            return Post(json, "application/json", url, timeout);
        }

        public HttpWebResponse Get(string url, object data)
        {
            return Get(string.Format("{0}?{1}", url, data.ToNameValuePairs()));
        }

        public HttpWebResponse Get(string url)
        {
            return Get(url, 100000);
        }

        public HttpWebResponse Get(string url, int timeout)
        {
            HttpWebRequest request = GetWebRequest(url);
            request.Method = "GET";
            request.CookieContainer = _cookies;

            WebResponse webResponse = request.GetResponse();
            var responseReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
            try
            {
                return SaveCookies((HttpWebResponse)request.GetResponse());
            }
            catch (WebException ex)
            {
                var exReader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                throw new ApplicationException(exReader.ReadToEnd(), ex);
            }
        }

        private HttpWebResponse Post(string body, string contentType, string url, int timeout = 100000)
        {
            byte[] requestContent = Encoding.UTF8.GetBytes(body);

            HttpWebRequest request = GetWebRequest(url);
            request.Method = "POST";
            request.CookieContainer = _cookies;
            request.ContentType = contentType;
            request.ContentLength = requestContent.Length;
            request.Timeout = timeout;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestContent, 0, requestContent.Length);
            requestStream.Close();

            try
            {
                return SaveCookies((HttpWebResponse)request.GetResponse());
            }
            catch (WebException ex)
            {
                var responseReader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                throw new ApplicationException(responseReader.ReadToEnd(), ex);
            }
        }

        private HttpWebRequest GetWebRequest(string url)
        {
            Uri uri = _baseUrl != null ? new Uri(_baseUrl, url) : new Uri(url);

            return (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
        }

        private HttpWebResponse SaveCookies(HttpWebResponse response)
        {
            _cookies.Add(response.Cookies);
            return response;
        }
    }
}
