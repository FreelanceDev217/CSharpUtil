// To be used for custom Keyboard Shortcut. (Winform)
// David Piao

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;

namespace KeyUtil
{
    class KeyUtility
    {
        public static Object str2key(string str_keys)
        {
            //do stuff with pressed and modifier keys
            var converter = new KeysConverter();
            return converter.ConvertFromString(str_keys);
        }

        public static string key2str(System.Windows.Forms.KeyEventArgs e)
        {

            //do stuff with pressed and modifier keys
            var converter = new KeysConverter();
            return converter.ConvertToString(e.KeyData);
        }

        public static string key2str(Keys key)
        {
            //do stuff with pressed and modifier keys
            var converter = new KeysConverter();
            return converter.ConvertToString(key);
        }

        public static Key vk2key(int VKCode)
        {
            return System.Windows.Input.KeyInterop.KeyFromVirtualKey(VKCode);
        }
    }
}
