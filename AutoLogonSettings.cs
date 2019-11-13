// Class to be used to set Windows to automatically login 
// David Piao
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OS_Setting
{
  internal class AutoLogonSettings : IDisposable
  {
    private RegistryKey m_RegKey;

        public bool AutoAdminLogon
        {
            get
            {
                int result;
                if (!int.TryParse((this.m_RegKey.GetValue(nameof(AutoAdminLogon), (object)"0") ?? (object)"0").ToString(), out result))
                    result = 0;
                return result > 0;
            }
            set
            {
                this.m_RegKey.SetValue(nameof(AutoAdminLogon), (object)(value ? 1 : 0).ToString(), RegistryValueKind.String);
            }
        }

        public string DefaultDomainName
    {
      get
      {
        return this.m_RegKey.GetValue(nameof (DefaultDomainName), (object) null) as string;
      }
      set
      {
        if (value == null)
          this.m_RegKey.DeleteValue(nameof (DefaultDomainName), false);
        else
          this.m_RegKey.SetValue(nameof (DefaultDomainName), (object) value, RegistryValueKind.String);
      }
    }

    public string DefaultUserName
    {
      get
      {
        return this.m_RegKey.GetValue(nameof (DefaultUserName), (object) null) as string;
      }
      set
      {
        if (value == null)
          this.m_RegKey.DeleteValue(nameof (DefaultUserName), false);
        else
          this.m_RegKey.SetValue(nameof (DefaultUserName), (object) value, RegistryValueKind.String);
      }
    }

    public string DefaultPassword
    {
      get
      {
        return this.m_RegKey.GetValue(nameof (DefaultPassword), (object) null) as string;
      }
      set
      {
        if (value == null)
          this.m_RegKey.DeleteValue(nameof (DefaultPassword), false);
        else
          this.m_RegKey.SetValue(nameof (DefaultPassword), (object) value, RegistryValueKind.String);
      }
    }

    public AutoLogonSettings()
    {
      this.m_RegKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
    }

    ~AutoLogonSettings()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.m_RegKey == null)
        return;
      this.m_RegKey.Dispose();
      this.m_RegKey = (RegistryKey) null;
    }
  }
}
