// ** Prototype of remote driver
// * This class is designed dedicatedly for Multilogin
using EditorBot;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OS_Util;
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
using Size = System.Drawing.Size;

namespace PCKLIB
{
    public class IRemoteDriver
    {
        public object m_locker = new object();
        public int m_ID;
        public Guid m_guid;
        public string m_creat_time;

        public RemoteWebDriver browser;

        public IJavaScriptExecutor m_js;

        public ProxyData m_proxy;

        public IEnumerable<int> PID = null;

        public string m_err_str = "##$$##$$";

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
        public async Task<bool> start(WorkerParam param)
        {
            try
            {
                MainApp.log_info($"#{m_ID} - Start...");
                try
                {
                    try
                    {
                        if(param.profile_id == "")
                        {
                            param.profile_id = await MultiloginHelper.get_profile_id_from_name(param.profile_name);
                            if(param.profile_id == "")
                            {
                                // create new multilogin profile
                                param.profile_id = await MultiloginHelper.create_profile(param);
                            }
                        }
                        // launch it and get the host
                        string hosting = await MultiloginHelper.launch_profile(param.profile_id);

                        // connect
                        browser = new RemoteWebDriver(new Uri(hosting), (new OpenQA.Selenium.Firefox.FirefoxOptions()).ToCapabilities());
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Fail to start chrome.exe. Please make sure any other chrome windows are not opened.");
                        return false;
                    }

                    m_js = (IJavaScriptExecutor)browser;
                    
                    MainApp.log_info($"#{m_ID} - Successfully started.");
                    return true;
                }
                catch (Exception ex)
                {
                    MainApp.log_error($"#{m_ID} - Failed to start. Exception:{ex.Message}");
                    try
                    {
                        browser.Quit();
                    }
                    catch
                    {
                        MainApp.log_error($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MainApp.log_error($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{ex.Message}");
            }
            return false;
        }

        public async Task<bool> wait_url_change(string url, int timeout = 7000)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                while (wt.ElapsedMilliseconds < timeout)
                {
                    if (browser.Url != url)
                        return true;
                    await task_delay(100);
                }
            }
            catch (Exception ex)
            {
                MainApp.log_error($"#{m_ID} - Failed to wait for url change. Exception:{ex.Message}");
            }
            return false;
        }

        public async Task<bool> try_select(By list, By optionToVerify, string textToSelect, int timeout = 5000)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < timeout)
            {
                if (browser.FindElement(optionToVerify).Text == textToSelect)
                    return true;
                browser.FindElement(list).SendKeys(textToSelect[0].ToString());
                await task_delay(100);
            }
            return false;
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

        public async Task<bool> wait_visible(string xpath, int TimeOut = 1000)
        {
            return await wait_visible(By.XPath(xpath), TimeOut);
        }
        public async Task<bool> wait_visible(By by, int TimeOut = 1000)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < TimeOut)
            {
                if (await is_element_visible(by))
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }

        public async Task<bool> wait_unvisible(string xpath, int TimeOut = 1000)
        {
            return await wait_unvisible(By.XPath(xpath), TimeOut);
        }
        public async Task<bool> wait_unvisible(By by, int TimeOut = 1000)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < TimeOut)
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
                await task_delay(100);
            }
            return false;
        }

        public async Task<bool> wait_present(string xpath, int TimeOut = 1000)
        {
            return await wait_present(By.XPath(xpath), TimeOut);
        }
        public async Task<bool> wait_present(By by, int TimeOut = 5000)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            do
            {
                if (is_element_present(by))
                    return true;
                await Task.Delay(100);
            }
            while (wt.ElapsedMilliseconds < TimeOut);
            return false;
        }

        public async Task<By> wait_present(List<By> by, int TimeOut = 1000)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < TimeOut)
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
            return null;
        }

        public async Task<bool> click_element_all(string xpath)
        {
            bool ret = await click_element(xpath, 0) || await click_element(xpath, 1);
            if (ret == false)
            {
                try
                {
                    m_js.ExecuteAsyncScript($"document.evaluate('{xpath}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                    ret = true;
                }
                catch (Exception ex)
                {
                    ret = false;
                }
            }
            if (ret == false)
                MainApp.log_error($"{m_ID} : Clicking all ways failed. {xpath}");
            return ret;
        }
        public async Task<bool> click_element(string xpath, int mode = 0, int delay = 100)
        {
            try
            {
                await click_element(By.XPath(xpath), mode);
                await task_delay(delay);
                return true;
            }
            catch (Exception ex)
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
                else
                {
                    Actions action = new Actions(browser);
                    action.MoveToElement(browser.FindElement(by)).Perform();
                    action.Click(browser.FindElement(by)).Perform();
                }
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public async Task<bool> enter_text(string xpath, string textToEnter, string atributeToEdit = "value", int TimeOut = 10000, bool manualyEnter = false)
        {
            return await enter_text(By.XPath(xpath), textToEnter, atributeToEdit, TimeOut, manualyEnter);
        }
        public async Task<bool> enter_text(By by, string textToEnter, string atributeToEdit = "value", int TimeOut = 10000, bool manualyEnter = false)
        {
            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < TimeOut)
            {
                try
                {
                    if (is_element_present(by) && await is_element_visible(by))
                    {
                        browser.FindElement(by).SendKeys((string)Keys.Control + "a");
                        if (manualyEnter)
                            browser.FindElement(by).SendKeys(textToEnter);
                        else
                            browser.ExecuteScript($"arguments[0].value = '{textToEnter}';", ((RemoteWebDriver)browser).FindElement(by));

                        for (int index = 0; index < 11; ++index)
                        {
                            if ((string)browser.ExecuteScript("return arguments[0].value;", browser.FindElement(by)) == textToEnter)
                            {
                                return true;
                            }
                            await task_delay(100);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MainApp.log_error($"#{m_ID} - Failed to enter text. Exception:{ex.Message}");
                    return false;
                }
                await Task.Delay(100);
            }
            return false;
        }
        public async Task<string> set_value(string xpath, string val, string field = "value")
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
        public async Task<string> get_value(string xpath, string field = "value")
        {
            try
            {
                var elem = browser.FindElementByXPath(xpath);
                if (elem != null)
                {
                    return elem.GetAttribute(field);
                }
                return "";
            }
            catch(Exception)
            {
                return "";
            }
        }
        public async Task<bool> click_and_wait(string toClick, string toWait, int mode = 0, int TimeOut = 10000)
        {
            return await click_and_wait(By.XPath(toClick), By.XPath(toWait), mode, TimeOut);
        }
        public async Task<bool> click_and_wait(By toClick, By toWait, int mode = 0, int TimeOut = 10000)
        {
            if (!await wait_present(toClick, 3000))
            {
                MainApp.log_info($"#{m_ID} - Element to be clicked is not present! mode:{mode} By: {toClick}");
                return false;
            }

            Stopwatch wt = new Stopwatch();
            wt.Start();
            while (wt.ElapsedMilliseconds < TimeOut)
            {
                try
                {
                    if (mode == 1)
                    {
                        string script = @"(function(x) {
                            var el = document.evaluate('" + toClick + @"', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
                            let hoverEvent = document.createEvent ('MouseEvents');
                            hoverEvent.initEvent ('mouseover', true, true);
                            el.dispatchEvent (hoverEvent);

                            let downEvent = document.createEvent ('MouseEvents');
                            downEvent.initEvent ('mousedown', true, true);
                            el.dispatchEvent (downEvent);

                            let upEvent = document.createEvent ('MouseEvents');
                            upEvent.initEvent ('mouseup', true, true);
                            el.dispatchEvent (upEvent);

                            let clickEvent = document.createEvent ('MouseEvents');
                            clickEvent.initEvent ('click', true, true);
                            el.dispatchEvent (clickEvent);
                            })();";
                        browser.ExecuteScript(script);
                        if (!await wait_present(toWait, TimeOut))
                        {
                            MainApp.log_info($"#{m_ID} - Click failed for waiting! mode:{mode} By: {toClick}");
                            return false;
                        }
                        MainApp.log_info($"#{m_ID} - Click success! mode:{mode} By: {toClick}");
                        return true;
                    }
                    else if (mode == 0)
                    {
                        browser.ExecuteScript("arguments[0].click('');", browser.FindElement(toClick));
                        if (!await wait_present(toWait, TimeOut))
                        {
                            MainApp.log_info($"#{m_ID} - Click failed for waiting! mode:{mode} By: {toClick}");
                            return false;
                        }

                        MainApp.log_info($"#{m_ID} - Click success! mode:{mode} By: {toClick}");
                        return true;
                    }
                    else if (mode == 2)
                    {
                        browser.FindElement(toClick).Click();
                        if (!await wait_present(toWait, TimeOut))
                        {
                            MainApp.log_info($"#{m_ID} - Click failed for waiting! mode:{mode} By: {toClick}");
                            return false;
                        }

                        MainApp.log_info($"#{m_ID} - Click success! mode:{mode} By: {toClick}");
                        return true;
                    }
                    else if (mode == 3)
                    {
                        Actions action = new Actions(browser);
                        action.MoveToElement(browser.FindElement(toClick)).Perform();
                        action.Click(browser.FindElement(toClick)).Perform();
                        if (!await wait_present(toWait, TimeOut))
                        {
                            MainApp.log_info($"#{m_ID} - Click failed for waiting! mode:{mode} By: {toClick}");
                            return false;
                        }
                        MainApp.log_info($"#{m_ID} - Click success! mode:{mode} By: {toClick}");
                        return true;
                    }
                }
                catch
                {

                }
            }
            MainApp.log_info($"#{m_ID} - Click failed for waiting! mode:{mode} By: {toClick}");
            return false;
        }

        public async Task<bool> is_element_visible(By by, int timeout = 0)
        {
            try
            {
                Stopwatch wt = new Stopwatch();
                wt.Start();
                do
                {
                    if (is_element_visible(browser.FindElement(by)))
                        return true;
                    await task_delay(100);
                } while (wt.ElapsedMilliseconds < timeout);
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
            new Thread((ThreadStart)(() =>
            {
                try
                {
                    browser.Quit();
                    browser.Dispose();
                }
                catch (Exception) { }
            })).Start();
        }

        public void save_screenshot(string screenshotName)
        {
            string path = $"screenshots/{m_ID}/screens/{Thread.CurrentThread.ManagedThreadId}_{m_creat_time}/";
            Directory.CreateDirectory(path);
            browser.GetScreenshot().SaveAsFile(path + screenshotName);
        }

        public async Task task_delay(int delay)
        {
            await Task.Delay(delay);
        }


        public async Task<IWebElement> expand_shadow_element(IWebElement element)
        {
            try
            {
                var shadow_root = (IWebElement)m_js.ExecuteAsyncScript("return arguments[0].shadowRoot", element);
                return shadow_root;
            }
            catch(Exception ex)
            {
                MainApp.log_error($"Failed to expand the shadow element. {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
