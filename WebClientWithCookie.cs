// Web client to be used with predefined cookie
// David Piao

using System.IO;
using System.Net;

public class WebClientWithCookie
{
    //The cookies will be here.
    private System.Net.CookieContainer _cookies = new System.Net.CookieContainer();

    public WebClientWithCookie(System.Net.CookieContainer cont)
    {
        _cookies = cont;
    }
    public void ClearCookies()
    {
        _cookies = new System.Net.CookieContainer();
    }

    public HtmlAgilityPack.HtmlDocument GetPage(string url)
    {
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
        request.Timeout = 20000;
        request.Method = "GET";
        request.CookieContainer = _cookies;

        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        var stream = response.GetResponseStream();

        //When you get the response from the website, the cookies will be stored
        //automatically in "_cookies".

        using (var reader = new StreamReader(stream))
        {
            string html = reader.ReadToEnd();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(WebUtility.HtmlDecode(html));
            return doc;
        }
    }
}