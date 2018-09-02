// Decompiled with JetBrains decompiler
// Type: mhw_dps_wpf.App
// Assembly: mhw_dps_wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9AF2A04E-56DA-4D52-9297-848C4FCE5E85
// Assembly location: C:\Users\Daniel\Desktop\mhw_damage_meter_1_0.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace mhw_dps_wpf
{
  public class App : Application
  {
    // [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
    }

    [STAThread]
    // [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public static void Main()
    {
      App app = new App();
      app.InitializeComponent();
      app.Run();
    }
  }
}
