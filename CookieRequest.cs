    // Web client which can be initialized using predefined cookie container
    class CookieMonster : ICookieVisitor
    {
        readonly List<Cookie> cookies = new List<Cookie>();
        readonly ManualResetEvent gotAllCookies = new ManualResetEvent(false);

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            cookies.Add(cookie);

            if (count == total - 1)
                gotAllCookies.Set();

            return true;
        }

        public void WaitForAllCookies()
        {
            gotAllCookies.WaitOne();
        }

        public void Dispose()
        {

        }

        public List<CefSharp.Cookie> AllCookies
        {
            get { return cookies; }
        }
    }

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
            request.Method = "GET";
            request.CookieContainer = _cookies;

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();

            //When you get the response from the website, the cookies will be stored
            //automatically in "_cookies".

            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
        }
    }