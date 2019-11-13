// Web Request with helper functions
// David Piao

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PCKLIB
{
    public class WRequest
    {

        public CookieContainer cookies = new CookieContainer();
        public HttpClientHandler handler;
        public readonly HttpClient client;
        public HttpResponseMessage response;
        public IEnumerable<Cookie> responseCookies;
        HttpRequestMessage request;
        public WRequest()
        {
            handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookies
            };
            client = new HttpClient(handler);
        }

        public async System.Threading.Tasks.Task<string> post_response(string end_point, Object post_data, Dictionary<string, string> header = null, string data_type = "json")
        {
            try
            {
                request = new HttpRequestMessage(HttpMethod.Post, end_point);
                if(header != null)
                {
                    foreach(var pair in header)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
                if(post_data != null)
                {
                    if (data_type.ToLower() == "json")
                    {
                        request.Content = new StringContent(post_data.ToString(), Encoding.UTF8, "application/json");//CONTENT-TYPE header
                    }
                    else
                    {
                        request.Content = new FormUrlEncodedContent((Dictionary<string, string>)post_data);
                    }
                }
                
                response = await client.SendAsync(request);
                responseCookies = cookies.GetCookies(new Uri(end_point)).Cast<Cookie>();
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WRequest: {end_point} \n {post_data.ToString()} \n {data_type}\n" + ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }
        public async System.Threading.Tasks.Task<string> get_response(string end_point, Dictionary<string, string> header = null, string scheme = "Bearer", string param = "")
        {
            try
            {
                request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(end_point),
                };
                if (header != null)
                {
                    foreach (var pair in header)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
                if (param != "")
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", param);
                }
                response = await client.SendAsync(request);
                responseCookies = cookies.GetCookies(new Uri(end_point)).Cast<Cookie>();
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WRequest: {end_point} \n Token: {param}\n" + ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }

        public async System.Threading.Tasks.Task<bool> download(string url, string filePath)
        {
            try
            {
                request = new HttpRequestMessage(HttpMethod.Get, url);
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var httpStream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = File.Create(filePath))
                using (var reader = new StreamReader(httpStream))
                {
                    await httpStream.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Download Error!" + ex.Message);
            }
            return false;
        }
    }
}
