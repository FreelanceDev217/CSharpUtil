// Used for proxy data expression
// David Piao

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace PCKLIB
{
    public class ProxyData
    {
        public string ip;
        public string port;
        public string login;
        public string password;

        public WebProxy Proxy { get; set; }

        public ProxyData(string host, int port)
        {
            this.Proxy = new WebProxy(host, port);
        }

        public ProxyData(string host, int port, string username, string password)
        {
            this.Proxy = new WebProxy(new Uri(string.Format("http://{0}:{1}", (object)host, (object)port)), true, (string[])null, (ICredentials)new NetworkCredential(username, password));
        }

        public ProxyData(WebProxy proxy)
        {
            this.Proxy = proxy;
            this.ip = proxy.Address.Host;
            this.port = proxy.Address.Port.ToString();
            this.login = proxy.Credentials.GetCredential(new Uri(string.Format("http://{0}:{1}", (object)this.ip, (object)this.port)), "").UserName;
            this.password = proxy.Credentials.GetCredential(new Uri(string.Format("http://{0}:{1}", (object)this.ip, (object)this.port)), "").Password;
        }

        public static List<ProxyData> LoadProxyDataFromFile(string path)
        {
            List<ProxyData> proxyDataList = new List<ProxyData>();
            foreach (string readAllLine in System.IO.File.ReadAllLines(path))
            {
                int length = readAllLine.Split(':').Length;
                if (length == 2)
                {
                    Regex regex = new Regex("(.*):(.*)");
                    string host = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[1].Value;
                    string s = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[2].Value;
                    if (host != "" && host != null && s != "" && s != null)
                    {
                        ProxyData proxyData = new ProxyData(host, int.Parse(s));
                        proxyDataList.Add(proxyData);
                    }
                }
                if (length == 4)
                {
                    Regex regex = new Regex("(.*):(.*):(.*):(.*)");
                    string host = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[1].Value;
                    string s = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[2].Value;
                    string username = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[3].Value;
                    string password = regex.Match(readAllLine.Replace(" ", string.Empty)).Groups[4].Value;
                    if (host != "" && host != null && s != "" && s != null)
                    {
                        ProxyData proxyData = new ProxyData(host, int.Parse(s), username, password);
                        proxyDataList.Add(proxyData);
                    }
                }
            }
            return proxyDataList;
        }

        public static List<List<ProxyData>> DivideProxyDataForThreads(List<ProxyData> inputProxyData, int threadsCount)
        {
            if (inputProxyData.Count < threadsCount)
                throw new Exception("inputProxyData.Count < threadsCount! NEED MORE inputProxyData!");
            List<List<ProxyData>> proxyDataListList = new List<List<ProxyData>>();
            for (int index = 0; index < threadsCount; ++index)
                proxyDataListList.Add(new List<ProxyData>());
            int index1 = 0;
            while (index1 != inputProxyData.Count)
            {
                foreach (List<ProxyData> proxyDataList in proxyDataListList)
                {
                    if (index1 < inputProxyData.Count)
                    {
                        proxyDataList.Add(inputProxyData[index1]);
                        ++index1;
                    }
                }
            }
            return proxyDataListList;
        }

        public static ProxyData GetProxyFromString(string strCurrentProxy)
        {
            int length = strCurrentProxy.Split(':').Length;
            if (length == 2)
            {
                Regex regex = new Regex("(.*):(.*)");
                string host = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[1].Value;
                string s = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[2].Value;
                if (host != "" && host != null && s != "" && s != null)
                    return new ProxyData(host, int.Parse(s))
                    {
                        ip = host,
                        port = s
                    };
            }
            if (length == 4)
            {
                Regex regex = new Regex("(.*):(.*):(.*):(.*)");
                string host = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[1].Value;
                string s = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[2].Value;
                string username = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[3].Value;
                string password = regex.Match(strCurrentProxy.Replace(" ", string.Empty)).Groups[4].Value;
                if (host != "" && host != null && s != "" && s != null)
                    return new ProxyData(host, int.Parse(s), username, password)
                    {
                        ip = host,
                        port = s,
                        login = username,
                        password = password
                    };
            }
            return (ProxyData)null;
        }
    }
}
