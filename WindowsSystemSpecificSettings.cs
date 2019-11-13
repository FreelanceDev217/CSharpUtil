
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OS_Setting
{
  internal class WindowsSystemSpecificSettings : INotifyPropertyChanged, IDisposable
  {
    private AutoLogonSettings _AutoLogonSettings;

    public AutoLogonSettings AutoLogonSettings
    {
      get
      {
        return this._AutoLogonSettings;
      }
      set
      {
        this._AutoLogonSettings = value;
        this.OnPropertyChanged(nameof (AutoLogonSettings));
      }
    }

    public WindowsSystemSpecificSettings()
    {
      this._AutoLogonSettings = new AutoLogonSettings();
    }

    ~WindowsSystemSpecificSettings()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      if (this._AutoLogonSettings == null)
        return;
      this._AutoLogonSettings.Dispose();
      this._AutoLogonSettings = (AutoLogonSettings) null;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      if (this.PropertyChanged == null)
        return;

      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    private void VerifyPropertyName(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        return;
      PropertyDescriptor property = TypeDescriptor.GetProperties((object) this)[propertyName];
    }
  }
}
