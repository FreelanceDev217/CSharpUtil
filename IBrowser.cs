// Web automation abstract class - Selenium Chrome
// David Piao

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cookie = System.Net.Cookie;
using Size = System.Drawing.Size;
using System.Net.Http;
using Microsoft.Win32;

namespace PCKLIB
{
    public class IBrowser
    {
        public object m_locker = new object();

        public string m_ID;
        public Guid m_guid;

        public object m_chr_data_dir = new object();
        public string m_chr_user_data_dir = "";
        public string m_chr_extension_dir = Environment.CurrentDirectory + "\\ChromeExtension";

        public ChromeDriver browser;
        public IJavaScriptExecutor m_js;
        public CookieContainer m_cookies;
        public ProxyData m_proxy;

        public System.Drawing.Point m_location = new System.Drawing.Point(0, 0);
        public System.Drawing.Size m_size = new System.Drawing.Size(0, 0);

        public IEnumerable<int> PID = null;

        public string m_err_str = "##$$##$$";
        public bool m_incognito = false;
        public bool m_dis_webrtc = false;
        public bool m_dis_cache = false;
        public bool m_dis_js = false;
        public bool m_headless = true;

        public string m_dl_folder = "downloads";
        public void delete_current_chrome_data()
        {
            try
            {
                Directory.Delete(m_chr_user_data_dir, true);
                return;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"#{m_ID} - Deleting chrome data dir failed. {ex.Message}");
            }
        }

        public async Task<bool> navigate(string target)
        {
            try
            {
                string url = browser.Url;
                browser.Navigate().GoToUrl(target);
                return await wait_url_change(url);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> navigate_back()
        {
            try
            {
                string url = browser.Url;
                m_js.ExecuteScript("window.history.go(-1)");
                return await wait_url_change(url);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void new_tab(string tabUrl)
        {
            lock (m_locker)
            {
                string newTabScript = "var d=document,a=d.createElement('a');"
                                + "a.target='_blank';a.href='{0}';"
                                + "a.innerHTML='new tab';"
                                + "d.body.appendChild(a);"
                                + "a.click();"
                                + "a.parentNode.removeChild(a);";
                if (String.IsNullOrEmpty(tabUrl))
                    tabUrl = "about:blank";

                m_js.ExecuteScript(String.Format(newTabScript, tabUrl));
            }
        }
        public async Task<bool> start()
        {
            try
            {
                //lock (m_chr_data_dir)
                //{
                //    m_guid = Guid.NewGuid();
                //    m_chr_user_data_dir = $"\\ChromeData\\selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString();
                //    Directory.CreateDirectory(m_chr_user_data_dir);
                //}

                System.Diagnostics.Debug.WriteLine($"#{m_ID} - Start...");
                try
                {
                    ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
                    defaultService.HideCommandPromptWindow = true;

                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (m_incognito)
                    {
                        chromeOptions.AddArguments("--incognito");
                    }

                    if (m_proxy != null)
                    {
                        var proxy = new Proxy();
                        proxy.HttpProxy = $"{m_proxy.ip}:{m_proxy.port}";
                        proxy.SslProxy = $"{m_proxy.ip}:{m_proxy.port}";
                        proxy.SocksProxy = $"{m_proxy.ip}:{m_proxy.port}";
                        proxy.SocksUserName = m_proxy.login;
                        proxy.SocksPassword = m_proxy.password;
                        chromeOptions.Proxy = proxy;
                    }

                    if (m_headless)
                    {
                        chromeOptions.AddArgument("--headless");
                        chromeOptions.AddArgument("--disable-gpu");
                        chromeOptions.AddArgument("--disable-extensions");
                        chromeOptions.AddArgument("--window-size=1920,1080");
                    }

                    chromeOptions.AddArguments("--no-sandbox");
                    chromeOptions.AddArgument("--start-maximized");

                    chromeOptions.AddArgument("--log-level=3");

                    //chromeOptions.AddArguments("--disk-cache-size=0");
                    //chromeOptions.AddArgument("--load-extension=" + m_chr_extension_dir + "\\proxy helper");
                    //m_chr_user_data_dir = @"C:\Users\Piao David\AppData\Local\Google\Chrome\User Data\";
                    //chromeOptions.AddArgument("--user-data-dir=\"" + m_chr_user_data_dir + "\"");

                    //m_dl_folder = Directory.GetCurrentDirectory() + "\\downloads";
                    //if (Directory.Exists(m_dl_folder) == false)
                    //{
                    //    Directory.CreateDirectory(m_dl_folder);
                    //}
                    //chromeOptions.AddUserProfilePreference("download.default_directory", m_dl_folder);
                    //chromeOptions.AddUserProfilePreference("download.prompt_for_download", "false");
                    //chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

                    //chromeOptions.AddArgument("--disable-features=RendererCodeIntegrity");

                    //string randomUserAgent = get_random_agent();
                    //chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)randomUserAgent));

                    chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Severe);
                    
                    //string chr_path = "";

                    //string reg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                    //RegistryKey registryKey;
                    //using (registryKey = Registry.LocalMachine.OpenSubKey(reg))
                    //{
                    //    if (registryKey != null)
                    //        chr_path = registryKey.GetValue("Path").ToString() + @"\chrome.exe";
                    //}
                    //if (chr_path == "")
                    //{
                    //    if (Environment.Is64BitOperatingSystem)
                    //        chr_path = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                    //    else
                    //        chr_path = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

                    //    if (!System.IO.File.Exists(chr_path))
                    //    {
                    //        chr_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                    //    }
                    //}

                    //if (!System.IO.File.Exists(chr_path))
                    //{
                    //    System.Diagnostics.Debug.WriteLine($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                    //    return false;
                    //}
                    //chromeOptions.BinaryLocation = chr_path;

                    try
                    {
                        browser = new ChromeDriver(defaultService, chromeOptions, TimeSpan.FromSeconds(130));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"#{m_ID} - Fail to start chrome.exe. Please make sure any other chrome windows are not opened.");
                        return false;
                    }
                    browser.Manage().Window.Size = m_size;
                    browser.Manage().Window.Position = m_location;

                    m_js = (IJavaScriptExecutor)browser;
                    //if(m_dis_cache)
                    //{
                    //    await Navigate("chrome-extension://kkmknnnjliniefekpicbaaobdnjjikfp/options.html");

                    //}
                    //if (m_proxy != null && !m_incognito) // regular proxy setting
                    //{
                    //    string ip = m_proxy.ip;
                    //    string port = m_proxy.port;
                    //    string login = m_proxy.login;
                    //    string password = m_proxy.password;
                    //    await Navigate("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/options.html");
                    //    m_js.ExecuteScript("$('#http-host').val(\"" + ip + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#http-port').val(\"" + port + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#https-host').val(\"" + ip + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#https-port').val(\"" + port + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#socks-host').val(\"" + ip + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#socks-port').val(\"" + port + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#username').val(\"" + login + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#password').val(\"" + password + "\")", Array.Empty<object>());
                    //    m_js.ExecuteScript("var a = document.getElementById(\"socks4\"); a.click();", Array.Empty<object>());
                    //    m_js.ExecuteScript("$('#proxy-rule').val(\"singleProxy\");", Array.Empty<object>());
                    //    m_js.ExecuteScript("save();", Array.Empty<object>());
                    //    Driver.Navigate().GoToUrl("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/popup.html");
                    //    m_js.ExecuteScript("httpProxy();", Array.Empty<object>());
                    //if (!m_incognito && m_dis_js) // regular proxy setting
                    //{
                    //    await Navigate("chrome-extension://jfpdlihdedhlmhlbgooailmfhahieoem/options.html");
                    //}
                    System.Diagnostics.Debug.WriteLine($"#{m_ID} - Successfully started.");

                    //Driver.Manage().Window.Size = m_size;
                    //Driver.Manage().Window.Position = m_location;
                    System.Diagnostics.Debug.WriteLine($"#{m_ID} - Cache and cookies are cleared. ");

                    if (m_incognito == false)
                        await remove_all_cookies(); //<- not necessary in incogneto mode
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"#{m_ID} - Failed to start. Exception:{ex.Message}\n{ex.StackTrace}");
                    try
                    {
                        browser.Quit();
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{ex.Message}");
            }
            return false;
        }

        public async Task<bool> remove_all_cookies()
        {
            try
            {
                browser.Manage().Cookies.DeleteAllCookies();
                return true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"Removing history failed. {ex.Message}");
            }
            return false;
        }
        public async Task<bool> wait_url_change(string url, double timeout = 7)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                while (wt.ElapsedMilliseconds < timeout * 1000)
                {
                    if (browser.Url != url)
                        return true;
                    await delay(0.1);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"#{m_ID} - Failed to wait for url change. Exception:{ex.Message}");
            }
            return false;
        }

        public CookieContainer convert_selenium_cookie_to_container(ICookieJar seleniumCookie)
        {
            CookieContainer cookieContainer = new CookieContainer();
            using (IEnumerator<OpenQA.Selenium.Cookie> enumerator = seleniumCookie.AllCookies.GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OpenQA.Selenium.Cookie current = enumerator.Current;
                    cookieContainer.Add(new Cookie(current.Name, current.Value, current.Path, current.Domain));
                }
            }
            return cookieContainer;
        }

        public bool is_element_present(By by)
        {
            try
            {
                browser.FindElement(by);
                return true;
            }
            catch (NoSuchElementException ex)
            {
                return false;
            }
        }

        public async Task<bool> wait_visible(string xpath, double timeout = 1)
        {
            return await wait_visible(By.XPath(xpath), timeout);
        }
        public async Task<bool> wait_visible(By by, double timeout = 1)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < timeout * 1000)
            {
                if (await is_element_visible(by))
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }

        public async Task<bool> wait_unvisible(string xpath, double timeout = 1)
        {
            return await wait_unvisible(By.XPath(xpath), timeout);
        }
        public async Task<bool> wait_unvisible(By by, double timeout = 1)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                while (wt.ElapsedMilliseconds < timeout * 1000)
                {
                    try
                    {
                        if (!await is_element_visible(by))
                            return true;
                    }
                    catch
                    {
                        return false;
                    }
                    await delay(0.1);
                }
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        public void drop_file(IWebElement target, string filePath, double offsetX = 0, double offsetY = 0)
        {
            const string JS_DROP_FILE = "for(var b=arguments[0],k=arguments[1],l=arguments[2],c=b.ownerDocument,m=0;;){var e=b.getBoundingClientRect(),g=e.left+(k||e.width/2),h=e.top+(l||e.height/2),f=c.elementFromPoint(g,h);if(f&&b.contains(f))break;if(1<++m)throw b=Error('Element not interractable'),b.code=15,b;b.scrollIntoView({behavior:'instant',block:'center',inline:'center'})}var a=c.createElement('INPUT');a.setAttribute('type','file');a.setAttribute('style','position:fixed;z-index:2147483647;left:0;top:0;');a.onchange=function(){var b={effectAllowed:'all',dropEffect:'none',types:['Files'],files:this.files,setData:function(){},getData:function(){},clearData:function(){},setDragImage:function(){}};window.DataTransferItemList&&(b.items=Object.setPrototypeOf([Object.setPrototypeOf({kind:'file',type:this.files[0].type,file:this.files[0],getAsFile:function(){return this.file},getAsString:function(b){var a=new FileReader;a.onload=function(a){b(a.target.result)};a.readAsText(this.file)}},DataTransferItem.prototype)],DataTransferItemList.prototype));Object.setPrototypeOf(b,DataTransfer.prototype);['dragenter','dragover','drop'].forEach(function(a){var d=c.createEvent('DragEvent');d.initMouseEvent(a,!0,!0,c.defaultView,0,0,0,g,h,!1,!1,!1,!1,0,null);Object.setPrototypeOf(d,null);d.dataTransfer=b;Object.setPrototypeOf(d,DragEvent.prototype);f.dispatchEvent(d)});a.parentElement.removeChild(a)};c.documentElement.appendChild(a);a.getBoundingClientRect();return a;";

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IWebDriver driver = ((RemoteWebElement)target).WrappedDriver;
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;

            IWebElement input = (IWebElement)jse.ExecuteScript(JS_DROP_FILE, target, offsetX, offsetY);
            input.SendKeys(filePath);
        }
        public async Task<bool> node_present(string xpath, double timeout = 5)
        {
            try
            {
                int time = 0;
                object node = null;
                string script = "(function()" +
                                    "{" +
                                        "node = document.evaluate('" + xpath + "', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;" +
                                        "if (node==null) return null;" +
                                        "return node.id;" +
                                "})()";
                while (time < timeout * 1000)
                {
                    node = m_js.ExecuteScript(script);
                    if (node != null)
                    {
                        await delay(1);
                        return true;
                    }
                    int part = (new Random()).Next(1, 3) * 100;
                    await delay(part);
                    time += part;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> wait_present(string xpath, double timeout = 0.8)
        {
            return await wait_present(By.XPath(xpath), timeout);
        }
        public async Task<bool> wait_present(By by, double timeout = 5)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                do
                {
                    if (is_element_present(by))
                        return true;
                    await Task.Delay(100);
                }
                while (wt.ElapsedMilliseconds < timeout * 1000);
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> wait_attr_change(string xpath, string attribute, string rex, double timeout = 5)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                do
                {
                    string val = browser.FindElementByXPath(xpath).GetAttribute(attribute);
                    if (rex == "")
                    {
                        if (val == "" || val == null)
                            return true;
                    }
                    else if (Regex.IsMatch(val, rex))
                        return true;
                    await Task.Delay(100);
                }
                while (wt.ElapsedMilliseconds < timeout * 1000);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<By> wait_present(List<By> by, double timeout = 1)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                while (wt.ElapsedMilliseconds < timeout * 1000)
                {
                    using (List<By>.Enumerator enumerator = by.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            By current = enumerator.Current;
                            if (is_element_present(current))
                                return current;
                        }
                    }
                    await Task.Delay(100);
                }
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        public IntPtr get_handle()
        {
            string title = String.Format("{0} - Mozilla Firefox", browser.Title);
            var process = Process.GetProcesses()
                .FirstOrDefault(x => x.MainWindowTitle == title);
            if (process != null)
            {
                return process.MainWindowHandle;
            }
            return IntPtr.Zero;
        }
        public async Task<bool> click_element(string xpath)
        {
            try
            {
                if (await click_element(xpath, 0))
                    return true;
                if (await click_element(xpath, 1))
                    return true;
                if (await click_element(xpath, 2))
                    return true;
                bool ret = false;
                try
                {
                    m_js.ExecuteAsyncScript($"document.evaluate('{xpath}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                    ret = true;
                }
                catch (Exception ex)
                {
                    ret = false;
                }

                return ret;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
        public async Task<bool> click_element(string xpath, int mode = 0, double delay_time = 0.1)
        {
            try
            {
                await click_element(By.XPath(xpath), mode);
                await delay(delay_time);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public bool trigger_change(string xpath)
        {
            try
            {
                string js = @" 
                    if('createEvent' in document) {
                        var evt = document.createEvent('HTMLEvents');
                        evt.initEvent('change', false, true);
                        arguments[0].dispatchEvent(evt);
                    }
                    else
                        arguments[0].fireEvent('onchange');";
                browser.ExecuteScript(js, ((RemoteWebDriver)browser).FindElementByXPath(xpath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> click_element(By by, int mode = 0)
        {
            try
            {
                if (mode == 0)
                {
                    browser.ExecuteScript("arguments[0].click('');", ((RemoteWebDriver)browser).FindElement(by));
                }
                else if (mode == 1)
                {
                    browser.FindElement(by).Click();
                }
                else if (mode == 2)
                {
                    Actions action = new Actions(browser);
                    action.MoveToElement(browser.FindElement(by)).Perform();
                    action.Click(browser.FindElement(by)).Perform();
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<bool> enter_text(string xpath, string textToEnter, bool manualyEnter = false, string atributeToEdit = "value")
        {
            return await enter_text(By.XPath(xpath), textToEnter, manualyEnter, atributeToEdit);
        }
        public async Task<bool> enter_text(By by, string textToEnter, bool manualyEnter = false, string atributeToEdit = "value")
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            try
            {
                if (is_element_present(by) && await is_element_visible(by))
                {
                    browser.FindElement(by).SendKeys((string)Keys.Control + "a");
                    if (manualyEnter)
                        browser.FindElement(by).SendKeys(textToEnter);
                    else
                        browser.ExecuteScript($"arguments[0].value = '{textToEnter}';", ((RemoteWebDriver)browser).FindElement(by));
                    return true;
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"#{m_ID} - Failed to enter text. Exception:{ex.Message}");
                return false;
            }
            return false;
        }
        public async Task<string> set_value(string xpath, string val, string field = "value")
        {
            try
            {
                Object node = null;
                string script = "(function()" +
                                    "{" +
                                        "node = document.evaluate(\"" + xpath + "\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;" +
                                        "if (node==null) return '" + m_err_str + "';" +
                                        "node." + field + "=\"" + val + "\";" +
                                        "return 'ok';" +
                                "})()";
                node = m_js.ExecuteScript(script);
                if (node != null)
                    return node.ToString();
                return m_err_str;
            }
            catch(Exception ex)
            {
                return m_err_str;
            }
        }

        public async Task<string> set_attribute(string xpath, string val, string field = "innerText")
        {
            try
            {
                Object node = null;
                string script = "(function()" +
                                    "{" +
                                        "node = document.evaluate(\"" + xpath + "\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;" +
                                        "if (node==null) return '" + m_err_str + "';" +
                                        "node.setAttribute('" + field + "',\"" + val + "\");" +
                                        "return 'ok';" +
                                "})()";
                node = m_js.ExecuteScript(script);
                if (node != null)
                    return node.ToString();
                return m_err_str;
            }
            catch(Exception )
            {
                return m_err_str;
            }
        }

        public int occurence(string xpath)
        {
            try
            {
                return browser.FindElementsByXPath(xpath).Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void switch_tab(int idx = 0)
        {
            try
            {
                browser.SwitchTo().Window(browser.WindowHandles[idx]);
            }
            catch(Exception)
            {

            }
        }
        
        public async Task<bool> middle_click(string xpath)
        {
            try
            {
                var js = $"xpath = '{xpath}';" + @"
                    var mouseWheelClick = new MouseEvent('click', { 'button': 1, 'which': 1 });
                    y = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
                    y.dispatchEvent(mouseWheelClick)";
                browser.ExecuteScript(js);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<string> get_value(string xpath, string err = "", string field = "innerText")
        {
            try
            {
                var elem = browser.FindElementByXPath(xpath);
                if (elem == null)
                    return err;
                return elem.GetAttribute(field);
            }
            catch (Exception ex)
            {
                return err;
            }
        }

        public async Task<bool> is_element_visible(By by, double timeout = 0)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                do
                {
                    if (is_element_visible(browser.FindElement(by)))
                        return true;
                    await delay(0.1);
                } while (wt.ElapsedMilliseconds < timeout * 1000);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool is_element_visible(IWebElement element)
        {
            return element.Displayed && element.Enabled;
        }

        public void quit()
        {
            if (browser != null)
            {
                browser.Quit();
            }
        }

        public IBrowser()
        {
        }

        public void save_screen_shot(string screenshotName)
        {
            string path = $"screenshots/{m_ID}/screens/{Thread.CurrentThread.ManagedThreadId}/";
            Directory.CreateDirectory(path);
            browser.GetScreenshot().SaveAsFile(path + screenshotName);
        }


        public Process find_latest_chrome_process()
        {
            Process ret = null;
            foreach (Process process in new List<Process>((IEnumerable<Process>)Process.GetProcessesByName("chrome")))
            {
                if (ret == null || process.StartTime > ret.StartTime)
                    ret = process;
                //if (process.GetParentID() == ParantID)
            }
            return ret;
        }

        public void clear_chrome_data()
        {
            try
            {
                string path = "ChromeData";
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            catch
            {
            }
        }

        public static string get_random_agent()
        {
            string[] strArray = new string[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36",
                //"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.157 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36",
                //"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36 OPR/43.0.2442.991",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 YaBrowser/17.10.0.2052 Yowser/2.5 Safari/537.36",
                //"Mozilla/5.0 (Windows NT 5.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36 OPR/43.0.2442.991",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36",
                //"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36"
            };
            return strArray[new Random().Next(0, strArray.Length)];
        }

        public async Task delay(double delay)
        {
            await Task.Delay((int)(delay * 1000));
        }

        public async Task delay_random()
        {
            int delay = (new Random()).Next(1000, 3000);
            await Task.Delay(delay);
        }

        public void show_wait_window(string msg = "Wait please...")
        {
            string js = @"
                height = 500;
                width = 400;
                leftPosition = (window.screen.width / 2) - ((width / 2) + 10);
                topPosition = (window.screen.height / 2) - ((height / 2) + 50);
                var sel_window = window.open('','Wait...','status = no,height = ' + height + ',width = ' + width + ', resizable = yes,left = '+leftPosition+',top='+topPosition+',screenX='+ leftPosition + ',screenY='+ topPosition +', toolbar=no,menubar=no,scrollbars=no,location=no,directories=no');
                sel_window.document.write('<p>" + msg + @"</p>');
            ";
            m_js.ExecuteScript(js);
        }

        public void close_last_window(string msg = "")
        {
            browser.SwitchTo().Window(browser.WindowHandles.Last());
            browser.Close();
            browser.SwitchTo().Window(browser.WindowHandles.First());
            if (msg != "")
            {
                m_js.ExecuteScript($"alert({msg});");
            }
        }

        public void switch_to_last_tab()
        {
            browser.SwitchTo().Window(browser.WindowHandles.Last());
        }

        public void switch_to_first_tab()
        {
            browser.SwitchTo().Window(browser.WindowHandles.First());
        }

        public void switch_iframe(string xpath)
        {
            try
            {
                var iframe = browser.FindElementByXPath(xpath);
                browser.SwitchTo().Frame(iframe);
            }
            catch(Exception ex)
            {

            }
        }

        public void switch_iframe_back()
        {
            browser.SwitchTo().DefaultContent();
        }

        void check_new_tab()
        {
            try
            {
                var cur = browser.CurrentWindowHandle;
                foreach (var hnd in browser.WindowHandles)
                {
                    browser.SwitchTo().Window(hnd);
                    if (browser.Url.Contains("newtab"))
                    {
                        browser.Close();
                    }
                }
                browser.SwitchTo().Window(cur);
            }
            catch (Exception ex)
            { }
        }

        public void EnableDownloadInHeadlessChrome(string downloadDir)
        {
            ChromeDriver driver = this.browser;
            var param = new Dictionary<string, object>();
            param["behavior"] = "allow";
            param["downloadPath"] = downloadDir;
            driver.ExecuteChromeCommand("Page.setDownloadBehavior", param);
        }

        public async Task<string> get_base64_img(string xpath)
        {
            string js = @"
                    xpath=" + $"\"{xpath}\"" + @";
                    img=document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;             ;
                    var canvas = document.createElement('canvas');
                    canvas.width = img.width;
                    canvas.height = img.height;
                    var ctx = canvas.getContext('2d');
                    ctx.drawImage(img, 0, 0);
                    var dataURL = canvas.toDataURL('image/png');
                    return dataURL.replace(/^data:image\/(png|jpg);base64,/, '');
            ";
            var resp = m_js.ExecuteScript(js);
            return resp.ToString();
        }
    }
}
