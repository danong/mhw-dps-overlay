// Decompiled with JetBrains decompiler
// Type: mhw_dps_wpf.MainWindow
// Assembly: mhw_dps_wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9AF2A04E-56DA-4D52-9297-848C4FCE5E85
// Assembly location: C:\Users\Daniel\Desktop\mhw_damage_meter_1_0.exe

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mhw_dps_wpf
{
  public partial class MainWindow : Window, IComponentConnector
  {
    private static Color[] player_colors = new Color[4]
    {
      Color.FromRgb((byte) 225, (byte) 65, (byte) 55),
      Color.FromRgb((byte) 53, (byte) 136, (byte) 227),
      Color.FromRgb((byte) 196, (byte) 172, (byte) 44),
      Color.FromRgb((byte) 42, (byte) 208, (byte) 55)
    };
    private DispatcherTimer dispatcherTimer = new DispatcherTimer();
    private string[] player_names = new string[4];
    private int[] player_damages = new int[4]{ 4, 5, 6, 7 };
    private int my_seat_id = -5;
    private Rectangle[] damage_bar_rects = new Rectangle[4];
    private TextBlock[] player_name_tbs = new TextBlock[4];
    private TextBlock[] player_dmg_tbs = new TextBlock[4];
    private double last_activated = MainWindow.time();
    private Process game;
    private bool init_finished;
    internal Canvas front_canvas;
    private bool _contentLoaded;

    public MainWindow()
    {
      this.Topmost = true;
      this.AllowsTransparency = true;
      this.WindowStyle = WindowStyle.None;
      this.Background = (Brush) Brushes.Transparent;
      this.find_game_proc();
      this.InitializeComponent();
    }

    private static void assert(bool flag, string reason = "", bool show_reason = true)
    {
      if (flag)
        return;
      if (show_reason)
      {
        int num = (int) MessageBox.Show("assertion failed: " + reason);
      }
      Application.Current.Shutdown();
    }

    private void find_game_proc()
    {
      IEnumerable<Process> source = ((IEnumerable<Process>) Process.GetProcesses()).Where<Process>((Func<Process, bool>) (x => x.ProcessName == "MonsterHunterWorld"));
      MainWindow.assert(source.Count<Process>() == 1, "frm_main_load: #proc not 1.", true);
      this.game = source.FirstOrDefault<Process>();
    }

    private void update_tick(object sender, EventArgs e)
    {
      if (this.game.HasExited)
        Application.Current.Shutdown();
      if (!this.init_finished)
        return;
      int[] teamDmg = mhw.get_team_dmg(this.game);
      string[] teamPlayerNames = mhw.get_team_player_names(this.game);
      int playerSeatId = mhw.get_player_seat_id(this.game);
      if (((IEnumerable<int>) teamDmg).Sum() != 0 && playerSeatId >= 0 && teamPlayerNames[0] != "")
      {
        this.player_damages = teamDmg;
        this.player_names = teamPlayerNames;
        this.my_seat_id = playerSeatId;
        this.update_info(false);
      }
      else
      {
        if (this.my_seat_id == -5)
          return;
        this.update_info(true);
      }
    }

    private void update_info(bool quest_end)
    {
      if (!this.init_finished)
        return;
      int num = ((IEnumerable<int>) this.player_damages).Sum();
      if (quest_end)
      {
        for (int index = 0; index < 4; ++index)
        {
          this.player_name_tbs[index].Text = this.player_names[index];
          this.player_dmg_tbs[index].Text = this.player_names[index] == "" ? "" : this.player_damages[index].ToString() + " (" + ((float) ((double) this.player_damages[index] / (double) num * 100.0)).ToString("0.0") + "%)";
        }
      }
      else
      {
        for (int index = 0; index < 4; ++index)
        {
          this.player_name_tbs[index].Text = this.player_names[index];
          this.player_dmg_tbs[index].Text = this.player_names[index] == "" ? "" : " " + ((float) ((double) this.player_damages[index] / (double) num * 100.0)).ToString("0.0") + "%";
        }
        if (num == 0)
        {
          for (int index = 0; index < 4; ++index)
            this.damage_bar_rects[index].Width = 0.0;
        }
        else
        {
          for (int index = 0; index < 4; ++index)
          {
            this.damage_bar_rects[index].Width = (double) this.player_damages[index] / (double) ((IEnumerable<int>) this.player_damages).Max() * this.front_canvas.ActualWidth;
            if (index == this.my_seat_id)
              this.damage_bar_rects[index].StrokeThickness = 1.0;
            else
              this.damage_bar_rects[index].StrokeThickness = 0.0;
          }
        }
      }
    }

    private void update_layout()
    {
      if (!this.init_finished)
        return;
      double num1 = (this.front_canvas.ActualHeight - 8.0) * 0.200000002980232;
      double num2 = (this.front_canvas.ActualHeight - num1) / 3.0 - 2.0;
      for (int index = 0; index < 4; ++index)
      {
        this.damage_bar_rects[index].Height = num1;
        Canvas.SetTop((UIElement) this.damage_bar_rects[index], (double) index * num2 + 3.0);
        Canvas.SetTop((UIElement) this.player_name_tbs[index], (double) index * num2 + 3.0 + 0.5 * num1 - 12.0);
        Canvas.SetTop((UIElement) this.player_dmg_tbs[index], (double) index * num2 + 3.0 + 0.5 * num1 - 14.0);
        Canvas.SetLeft((UIElement) this.player_dmg_tbs[index], this.front_canvas.ActualWidth - this.player_dmg_tbs[index].Width);
      }
      if (((IEnumerable<int>) this.player_damages).Sum() == 0)
      {
        for (int index = 0; index < 4; ++index)
          this.damage_bar_rects[index].Width = 0.0;
      }
      else
      {
        for (int index = 0; index < 4; ++index)
        {
          this.damage_bar_rects[index].Width = (double) this.player_damages[index] / (double) ((IEnumerable<int>) this.player_damages).Max() * this.front_canvas.ActualWidth;
          if (index == this.my_seat_id)
            this.damage_bar_rects[index].StrokeThickness = 1.0;
          else
            this.damage_bar_rects[index].StrokeThickness = 0.0;
        }
      }
    }

    private void init_canvas()
    {
      this.init_finished = true;
      double num1 = this.front_canvas.ActualHeight * 0.200000002980232 - 1.75;
      double num2 = (this.front_canvas.ActualHeight - num1) / 3.0;
      for (int index = 0; index < 4; ++index)
      {
        this.damage_bar_rects[index] = new Rectangle();
        this.damage_bar_rects[index].Stroke = (Brush) new SolidColorBrush(Colors.White);
        this.damage_bar_rects[index].StrokeThickness = 0.0;
        this.damage_bar_rects[index].Fill = (Brush) new SolidColorBrush(MainWindow.player_colors[index]);
        this.damage_bar_rects[index].Fill.Opacity = 0.65;
        this.damage_bar_rects[index].Width = 200.0;
        this.damage_bar_rects[index].Height = num1;
        Canvas.SetTop((UIElement) this.damage_bar_rects[index], (double) index * num2);
        this.front_canvas.Children.Add((UIElement) this.damage_bar_rects[index]);
        this.player_name_tbs[index] = new TextBlock();
        this.player_name_tbs[index].FontSize = 16.0;
        this.player_name_tbs[index].Width = 220.0;
        this.player_name_tbs[index].Height = 40.0;
        this.player_name_tbs[index].FontWeight = FontWeights.Bold;
        this.player_name_tbs[index].Foreground = (Brush) new SolidColorBrush(Colors.White);
        this.player_name_tbs[index].Effect = (Effect) new DropShadowEffect()
        {
          ShadowDepth = 0.0,
          Color = Colors.Black,
          BlurRadius = 4.0,
          Opacity = 1.0
        };
        Canvas.SetTop((UIElement) this.player_name_tbs[index], (double) index * num2 + 0.5 * num1 - 14.0);
        Canvas.SetLeft((UIElement) this.player_name_tbs[index], 3.0);
        this.front_canvas.Children.Add((UIElement) this.player_name_tbs[index]);
        this.player_dmg_tbs[index] = new TextBlock();
        this.player_dmg_tbs[index].TextAlignment = TextAlignment.Right;
        this.player_dmg_tbs[index].Text = (index * 4000).ToString() + " (125.4%)";
        this.player_dmg_tbs[index].Effect = (Effect) new DropShadowEffect()
        {
          ShadowDepth = 0.0,
          Color = Colors.Black,
          BlurRadius = 4.0,
          Opacity = 1.0
        };
        this.player_dmg_tbs[index].FontWeight = FontWeights.Bold;
        this.player_dmg_tbs[index].FontSize = 16.0;
        this.player_dmg_tbs[index].Foreground = (Brush) new SolidColorBrush(Colors.White);
        this.player_dmg_tbs[index].Width = 175.0;
        this.player_dmg_tbs[index].Height = 40.0;
        Canvas.SetTop((UIElement) this.player_dmg_tbs[index], (double) index * num2 + 0.5 * num1 - 14.0);
        Canvas.SetLeft((UIElement) this.player_dmg_tbs[index], this.front_canvas.ActualWidth - this.player_dmg_tbs[index].Width - 3.0);
        this.front_canvas.Children.Add((UIElement) this.player_dmg_tbs[index]);
      }
      this.player_name_tbs[0].Text = "拖统计条：移动窗口";
      this.player_name_tbs[1].Text = "drag bars to move";
      this.player_name_tbs[2].Text = "滚轮：放大缩小窗口";
      this.player_name_tbs[3].Text = "mouse wheel to zoom";
      this.update_layout();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.init_canvas();
      this.dispatcherTimer.Tick += new EventHandler(this.update_tick);
      this.dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
      this.dispatcherTimer.Start();
      this.ShowInTaskbar = false;
      this.ShowInTaskbar = true;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.update_layout();
    }

    private static double time()
    {
      return (DateTime.UtcNow - DateTime.MinValue).TotalSeconds;
    }

    private void Window_Activated(object sender, EventArgs e)
    {
      double num = MainWindow.time() - this.last_activated;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (e.LeftButton != MouseButtonState.Pressed)
          return;
        this.DragMove();
      }
      catch (Exception ex)
      {
      }
    }

    private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (e.RightButton == MouseButtonState.Pressed)
      {
        double num = this.Width + (double) e.Delta * 0.100000001490116;
        if (num <= this.MinWidth)
          return;
        this.Width = num;
      }
      else if (e.LeftButton == MouseButtonState.Pressed)
      {
        double num = this.Height + (double) e.Delta * 0.100000001490116;
        if (num <= this.MinHeight)
          return;
        this.Height = num;
      }
      else
      {
        double num1 = this.Width + (double) e.Delta * 0.0700000002980232;
        if (num1 > this.MinWidth)
          this.Width = num1;
        double num2 = this.Height + (double) e.Delta * 0.0299999993294477;
        if (num2 <= this.MinHeight)
          return;
        this.Height = num2;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/mhw_dps_wpf;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.front_canvas = (Canvas) target;
        else
          this._contentLoaded = true;
      }
      else
      {
        ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
        ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.Window_SizeChanged);
        ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.Window_MouseDown);
        ((UIElement) target).MouseWheel += new MouseWheelEventHandler(this.Window_MouseWheel);
      }
    }
  }
}
