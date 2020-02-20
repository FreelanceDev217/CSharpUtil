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
using System.Threading.Tasks;

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

        /*var data = new Dictionary<string, string> // sample
                        {
                            { "username", MainApp.g_setting.username },
                            { "password", pwd }
                        };
                        */
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

        
        // Creates HTTP POST request & uploads database to server. 
        private void UploadFilesToServer(Uri uri, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = streamReader.ReadToEnd();
                                    //responseString depends upon your web service.
                                    if (responseString == "Success")
                                    {
                                        System.Diagnostics.Debug.WriteLine("Stored successfully on server.");
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("Error occurred while uploading backup on server.");
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }, null);
                }
                catch (Exception)
                {

                }
            }, httpWebRequest);
        }


        
        /// Writes multi part HTTP POST request. 
        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        
        /// Writes string to stream. 
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        
        /// Writes byte array to stream. 
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }
    }
}
