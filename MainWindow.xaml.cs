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
        private int[] player_damages = new int[4] { 0, 0, 0, 0 };
        private int[] prev_player_damages = new int[4] { 0, 0, 0, 0 };
        private float[] player_damages_avg = new float[4] { 0, 0, 0, 0 };
        private float monster_hp = 0;
        private float monster_max_hp = 0;
        private float monter_percen = 0;
        private int my_seat_id = -5;
        private TextBlock[] monster_hp_txt = new TextBlock[1];
        private Rectangle[] damage_bar_rects = new Rectangle[4];
        private TextBlock[] player_name_tbs = new TextBlock[4];
        private TextBlock[] player_dmg_tbs = new TextBlock[4];
        private double last_activated = MainWindow.time();
        private static byte?[] pattern_1;
        private static byte?[] pattern_2;
        private static byte?[] pattern_3;
        private static byte?[] pattern_4;
        private Process game;
        private bool init_finished;
        private bool in_quest = false;
        private DateTime first_damage = new DateTime();
        private DateTime quest_end = new DateTime();
        public MainWindow()
        {
            this.Topmost = true;
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            this.Background = (Brush)Brushes.Transparent;
            this.find_game_proc();
            ulong[] patterns = memory.find_patterns(this.game, (IntPtr)5368725504L, (IntPtr)5452595200L, new List<byte?[]>()
              {
                MainWindow.pattern_1,
                MainWindow.pattern_2,
                MainWindow.pattern_3,
                MainWindow.pattern_4
              });
            MainWindow.assert(patterns[0] > 5369757695UL && patterns[1] > 5369757695UL && patterns[1] > 5369757695UL && patterns[3] > 5369757695UL, "failed to locate offsets (step 1).", true);
            ulong num1 = patterns[0] + (ulong)mhw.read_uint(this.game.Handle, (IntPtr)((long)patterns[0] + 2L)) + 6UL;
            ulong num2 = patterns[1] + 51UL + (ulong)mhw.read_uint(this.game.Handle, (IntPtr)((long)patterns[1] + 54L)) + 7UL;
            ulong num3 = patterns[2] + 15UL + (ulong)mhw.read_uint(this.game.Handle, (IntPtr)((long)patterns[2] + 15L + 2L)) + 6UL;
            ulong num4 = patterns[3] + (ulong)mhw.read_uint(this.game.Handle, (IntPtr)((long)patterns[3] + 3L)) + 7UL;
            Console.WriteLine(num1.ToString("X"));
            Console.WriteLine(num2.ToString("X"));
            Console.WriteLine(num3.ToString("X"));
            Console.WriteLine(num4.ToString("X"));
            MainWindow.assert(num1 > 5368725504UL && num1 < 5637144576UL && (num2 > 5368725504UL && num2 < 5637144576UL) && (num3 > 5368725504UL && num3 < 5637144576UL && num4 > 5368725504UL) && num4 < 5637144576UL, "failed to locate offsets (step 2).", true);
            mhw.loc1 = (long)num1;
            mhw.loc2 = (long)num2;
            mhw.loc3 = (long)num3;
            mhw.loc4 = (long)num4;
            this.InitializeComponent();
        }

        private static void assert(bool flag, string reason = "", bool show_reason = true)
        {
            if (flag)
                return;
            if (show_reason)
            {
                int num = (int)MessageBox.Show("assertion failed: " + reason);
            }
            Application.Current.Shutdown();
        }

        private void find_game_proc()
        {
            IEnumerable<Process> source = ((IEnumerable<Process>)Process.GetProcesses()).Where<Process>((Func<Process, bool>)(x => x.ProcessName == "MonsterHunterWorld"));
            MainWindow.assert(source.Count<Process>() == 1, "frm_main_load: #proc not 1.", true);
            this.game = source.FirstOrDefault<Process>();
        }

        private void update_tick(object sender, EventArgs e)
        {
            if (this.game.HasExited)
                Application.Current.Shutdown();
            if (!this.init_finished)
                return;


            this.monster_hp = mhw.get_monster_hp(this.game);
            this.monster_max_hp = mhw.get_monster_max_hp(this.game);

            //Console.WriteLine("monster_hp = " + monster_hp);
            if (this.monster_max_hp == 0)
                this.monster_hp_txt[0].Text = "";
            else
            {
                this.monter_percen = (this.monster_hp / this.monster_max_hp) * 100;
                this.monster_hp_txt[0].Text = this.monster_hp + "/" + this.monster_max_hp + "( " + this.monter_percen + "% )";
            }
            int[] teamDmg = mhw.get_team_dmg(this.game);
            string[] teamPlayerNames = mhw.get_team_player_names(this.game);
            int playerSeatId = mhw.get_player_seat_id(this.game);
            if (((IEnumerable<int>)teamDmg).Sum() != 0 && playerSeatId >= 0 && teamPlayerNames[0] != "")
            {
                if (!this.in_quest)
                {
                    this.in_quest = true;
                    this.first_damage = DateTime.UtcNow;
                    Array.Clear(this.prev_player_damages, 0, this.prev_player_damages.Length);
                    Array.Clear(this.player_damages, 0, this.player_damages.Length);
                }

                this.prev_player_damages = this.player_damages;
                this.player_damages = teamDmg;
                this.player_names = teamPlayerNames;
                this.my_seat_id = playerSeatId;
                this.update_info(false);
            }
            else
            {
                if (this.my_seat_id == -5)
                    return;
                if (this.in_quest)
                {
                    this.in_quest = false;
                    Array.Clear(this.player_damages_avg, 0, this.player_damages_avg.Length);
                    this.quest_end = DateTime.UtcNow;
                }
                this.update_info(true);
            }
        }


        private void update_info(bool quest_end)
        {
            if (!this.init_finished)
                return;
            int num = ((IEnumerable<int>)this.player_damages).Sum();
            if (quest_end)
            {
                for (int index = 0; index < 4; ++index)
                {
                    float dps = this.player_damages[index] / (float)(this.quest_end - this.first_damage).TotalSeconds;
                    this.player_name_tbs[index].Text = this.player_names[index];
                    this.player_dmg_tbs[index].Text = this.player_names[index] == "" ? "" : this.player_damages[index].ToString() + " (" + ((float)((double)this.player_damages[index] / (double)num * 100.0)).ToString("0.0") + "%) " + dps.ToString("0.0") + " DPS ";

                    this.player_dmg_tbs[index].TextAlignment = TextAlignment.Right;
                }
            }
            else
            {
                for (int index = 0; index < 4; ++index)
                {
                    int new_sample = this.player_damages[index] - this.prev_player_damages[index];
                    this.player_damages_avg[index] -= this.player_damages_avg[index] / 8;
                    this.player_damages_avg[index] += new_sample / 8;
                    this.player_name_tbs[index].Text = this.player_names[index];
                    //this.player_dmg_tbs[index].Text = this.player_names[index] == "" ? "" : " " + ((float)((double)this.player_damages[index] / (double)num * 100.0)).ToString("0.0") + "% " + this.player_damages_avg[index].ToString("0.0") + " DPS ";
                    this.player_dmg_tbs[index].Text = this.player_names[index] == "" ? "" : this.player_damages[index].ToString() + " (" + ((float)((double)this.player_damages[index] / (double)num * 100.0)).ToString("0.0") + "%) " + this.player_damages_avg[index].ToString("0.0") + " DPS ";

                    this.player_dmg_tbs[index].TextAlignment = TextAlignment.Right;
                }
                //Console.WriteLine(this.monster_hp);
                if (num == 0)
                {
                    for (int index = 0; index < 4; ++index)
                        this.damage_bar_rects[index].Width = 0.0;
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        this.damage_bar_rects[index].Width = (double)this.player_damages[index] / (double)((IEnumerable<int>)this.player_damages).Max() * this.front_canvas.ActualWidth;
                        if (index == this.my_seat_id)
                        {
                            this.damage_bar_rects[index].StrokeThickness = 1.0;
                        }
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
                Canvas.SetTop((UIElement)this.damage_bar_rects[index], (double)index * num2 + 3.0);
                Canvas.SetTop((UIElement)this.player_name_tbs[index], (double)index * num2 + 3.0 + 0.5 * num1 - 12.0);
                Canvas.SetTop((UIElement)this.player_dmg_tbs[index], (double)index * num2 + 3.0 + 0.5 * num1 - 14.0);
                Canvas.SetLeft((UIElement)this.player_dmg_tbs[index], this.front_canvas.ActualWidth - this.player_dmg_tbs[index].Width);
            }
            if (((IEnumerable<int>)this.player_damages).Sum() == 0)
            {
                for (int index = 0; index < 4; ++index)
                    this.damage_bar_rects[index].Width = 0.0;
            }
            else
            {
                for (int index = 0; index < 4; ++index)
                {
                    this.damage_bar_rects[index].Width = (double)this.player_damages[index] / (double)((IEnumerable<int>)this.player_damages).Max() * this.front_canvas.ActualWidth;
                    if (index == this.my_seat_id)
                        this.damage_bar_rects[index].StrokeThickness = 1.0;
                    else
                        this.damage_bar_rects[index].StrokeThickness = 0.0;
                }
            }
        }

        private void add_monter_hp()
        {
            //monster hp
            this.monster_hp_txt[0] = new TextBlock();
            this.monster_hp_txt[0].FontSize = 16.0;
            this.monster_hp_txt[0].Width = 220.0;
            this.monster_hp_txt[0].Height = 40.0;
            this.monster_hp_txt[0].FontWeight = FontWeights.Bold;
            this.monster_hp_txt[0].Foreground = (Brush)new SolidColorBrush(Colors.White);
            this.monster_hp_txt[0].Effect = (Effect)new DropShadowEffect()
            {
                ShadowDepth = 0.0,
                Color = Colors.Black,
                BlurRadius = 4.0,
                Opacity = 1.0
            };
            Canvas.SetTop((UIElement)this.monster_hp_txt[0], 0);
            Canvas.SetLeft((UIElement)this.monster_hp_txt[0], 0);
            this.monster_hp_ui.Children.Add((UIElement)this.monster_hp_txt[0]);
            this.monster_hp_txt[0].Text = "Monster HP Here";

            //
        }

        private void init_canvas()
        {
            this.init_finished = true;
            double num1 = this.front_canvas.ActualHeight * 0.200000002980232 - 1.75;
            double num2 = (this.front_canvas.ActualHeight - num1) / 3.0;

            this.add_monter_hp();
            for (int index = 0; index < 4; ++index)
            {
                this.damage_bar_rects[index] = new Rectangle();
                this.damage_bar_rects[index].Stroke = (Brush)new SolidColorBrush(Colors.White);
                this.damage_bar_rects[index].StrokeThickness = 0.0;
                this.damage_bar_rects[index].Fill = (Brush)new SolidColorBrush(MainWindow.player_colors[index]);
                this.damage_bar_rects[index].Fill.Opacity = 0.65;
                this.damage_bar_rects[index].Width = 200.0;
                this.damage_bar_rects[index].Height = num1;
                Canvas.SetTop((UIElement)this.damage_bar_rects[index], (double)index * num2);
                this.front_canvas.Children.Add((UIElement)this.damage_bar_rects[index]);
                this.player_name_tbs[index] = new TextBlock();
                this.player_name_tbs[index].FontSize = 16.0;
                this.player_name_tbs[index].Width = 220.0;
                this.player_name_tbs[index].Height = 40.0;
                this.player_name_tbs[index].FontWeight = FontWeights.Bold;
                this.player_name_tbs[index].Foreground = (Brush)new SolidColorBrush(Colors.White);
                this.player_name_tbs[index].Effect = (Effect)new DropShadowEffect()
                {
                    ShadowDepth = 0.0,
                    Color = Colors.Black,
                    BlurRadius = 4.0,
                    Opacity = 1.0
                };
                Canvas.SetTop((UIElement)this.player_name_tbs[index], (double)index * num2 + 0.5 * num1 - 14.0);
                Canvas.SetLeft((UIElement)this.player_name_tbs[index], 3.0);
                this.front_canvas.Children.Add((UIElement)this.player_name_tbs[index]);
                this.player_dmg_tbs[index] = new TextBlock();
                this.player_dmg_tbs[index].TextAlignment = TextAlignment.Right;
                //this.player_dmg_tbs[index].Background = Brushes.White;
                this.player_dmg_tbs[index].Text = (index * 4000).ToString() + " (125.4%)";
                this.player_dmg_tbs[index].Effect = (Effect)new DropShadowEffect()
                {
                    ShadowDepth = 0.0,
                    Color = Colors.Black,
                    BlurRadius = 4.0,
                    Opacity = 1.0
                };
                this.player_dmg_tbs[index].FontWeight = FontWeights.Bold;
                this.player_dmg_tbs[index].FontSize = 14.0;
                this.player_dmg_tbs[index].Foreground = (Brush)new SolidColorBrush(Colors.White);
                this.player_dmg_tbs[index].Width = 175.0;
                this.player_dmg_tbs[index].Height = 40.0;
                Canvas.SetTop((UIElement)this.player_dmg_tbs[index], (double)index * num2 + 0.5 * num1 - 14.0);
                Canvas.SetLeft((UIElement)this.player_dmg_tbs[index], this.front_canvas.ActualWidth - this.player_dmg_tbs[index].Width);
                this.front_canvas.Children.Add((UIElement)this.player_dmg_tbs[index]);
            }
            this.player_name_tbs[0].Text = "Drag to move this overlay";
            this.player_name_tbs[1].Text = "Mouse wheel to zoom";
            this.player_name_tbs[2].Text = "";
            this.player_name_tbs[3].Text = "";
            this.update_layout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.init_canvas();
            this.dispatcherTimer.Tick += new EventHandler(this.update_tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
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
                double num = this.Width + (double)e.Delta * 0.100000001490116;
                if (num <= this.MinWidth)
                    return;
                this.Width = num;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                double num = this.Height + (double)e.Delta * 0.100000001490116;
                if (num <= this.MinHeight)
                    return;
                this.Height = num;
            }
            else
            {
                double num1 = this.Width + (double)e.Delta * 0.0700000002980232;
                if (num1 > this.MinWidth)
                    this.Width = num1;
                double num2 = this.Height + (double)e.Delta * 0.0299999993294477;
                if (num2 <= this.MinHeight)
                    return;
                this.Height = num2;
            }
        }

        static MainWindow()
        {
            byte?[] nullableArray1 = new byte?[26];
            nullableArray1[0] = new byte?((byte)139);
            nullableArray1[1] = new byte?((byte)13);
            nullableArray1[6] = new byte?((byte)35);
            nullableArray1[7] = new byte?((byte)202);
            nullableArray1[8] = new byte?((byte)129);
            nullableArray1[9] = new byte?((byte)249);
            nullableArray1[10] = new byte?((byte)0);
            nullableArray1[11] = new byte?((byte)1);
            nullableArray1[12] = new byte?((byte)0);
            nullableArray1[13] = new byte?((byte)0);
            nullableArray1[14] = new byte?((byte)115);
            nullableArray1[15] = new byte?((byte)47);
            nullableArray1[16] = new byte?((byte)15);
            nullableArray1[17] = new byte?((byte)183);
            nullableArray1[23] = new byte?((byte)193);
            nullableArray1[24] = new byte?((byte)234);
            nullableArray1[25] = new byte?((byte)16);
            MainWindow.pattern_1 = nullableArray1;
            byte?[] nullableArray2 = new byte?[58];
            nullableArray2[0] = new byte?((byte)72);
            nullableArray2[1] = new byte?((byte)137);
            nullableArray2[2] = new byte?((byte)116);
            nullableArray2[3] = new byte?((byte)36);
            nullableArray2[4] = new byte?((byte)56);
            nullableArray2[5] = new byte?((byte)139);
            nullableArray2[6] = new byte?((byte)112);
            nullableArray2[7] = new byte?((byte)24);
            nullableArray2[8] = new byte?((byte)72);
            nullableArray2[9] = new byte?((byte)139);
            nullableArray2[15] = new byte?((byte)137);
            nullableArray2[16] = new byte?((byte)136);
            nullableArray2[17] = new byte?((byte)12);
            nullableArray2[18] = new byte?((byte)5);
            nullableArray2[19] = new byte?((byte)0);
            nullableArray2[20] = new byte?((byte)0);
            nullableArray2[21] = new byte?((byte)72);
            nullableArray2[22] = new byte?((byte)139);
            nullableArray2[28] = new byte?((byte)137);
            nullableArray2[29] = new byte?((byte)144);
            nullableArray2[30] = new byte?((byte)16);
            nullableArray2[31] = new byte?((byte)5);
            nullableArray2[32] = new byte?((byte)0);
            nullableArray2[33] = new byte?((byte)0);
            nullableArray2[34] = new byte?((byte)72);
            nullableArray2[35] = new byte?((byte)139);
            nullableArray2[41] = new byte?((byte)137);
            nullableArray2[42] = new byte?((byte)152);
            nullableArray2[43] = new byte?((byte)20);
            nullableArray2[44] = new byte?((byte)5);
            nullableArray2[45] = new byte?((byte)0);
            nullableArray2[46] = new byte?((byte)0);
            nullableArray2[47] = new byte?((byte)133);
            nullableArray2[48] = new byte?((byte)219);
            nullableArray2[49] = new byte?((byte)126);
            nullableArray2[51] = new byte?((byte)72);
            nullableArray2[52] = new byte?((byte)139);
            MainWindow.pattern_2 = nullableArray2;
            byte?[] nullableArray3 = new byte?[21];
            nullableArray3[0] = new byte?((byte)178);
            nullableArray3[1] = new byte?((byte)172);
            nullableArray3[2] = new byte?((byte)11);
            nullableArray3[3] = new byte?((byte)0);
            nullableArray3[4] = new byte?((byte)0);
            nullableArray3[5] = new byte?((byte)73);
            nullableArray3[6] = new byte?((byte)139);
            nullableArray3[7] = new byte?((byte)217);
            nullableArray3[8] = new byte?((byte)139);
            nullableArray3[9] = new byte?((byte)81);
            nullableArray3[10] = new byte?((byte)84);
            nullableArray3[11] = new byte?((byte)73);
            nullableArray3[12] = new byte?((byte)139);
            nullableArray3[13] = new byte?((byte)248);
            nullableArray3[14] = new byte?((byte)72);
            nullableArray3[15] = new byte?((byte)139);
            nullableArray3[16] = new byte?((byte)13);
            MainWindow.pattern_3 = nullableArray3;
            byte?[] nullableArray4 = new byte?[37];
            nullableArray4[0] = new byte?((byte)72);
            nullableArray4[1] = new byte?((byte)139);
            nullableArray4[2] = new byte?((byte)13);
            nullableArray4[7] = new byte?((byte)72);
            nullableArray4[8] = new byte?((byte)141);
            nullableArray4[9] = new byte?((byte)84);
            nullableArray4[10] = new byte?((byte)36);
            nullableArray4[11] = new byte?((byte)56);
            nullableArray4[12] = new byte?((byte)198);
            nullableArray4[13] = new byte?((byte)68);
            nullableArray4[14] = new byte?((byte)36);
            nullableArray4[15] = new byte?((byte)32);
            nullableArray4[16] = new byte?((byte)0);
            nullableArray4[17] = new byte?((byte)77);
            nullableArray4[18] = new byte?((byte)139);
            nullableArray4[19] = new byte?((byte)64);
            nullableArray4[20] = new byte?((byte)8);
            nullableArray4[21] = new byte?((byte)232);
            nullableArray4[26] = new byte?((byte)72);
            nullableArray4[27] = new byte?((byte)139);
            nullableArray4[28] = new byte?((byte)92);
            nullableArray4[29] = new byte?((byte)36);
            nullableArray4[30] = new byte?((byte)96);
            nullableArray4[31] = new byte?((byte)72);
            nullableArray4[32] = new byte?((byte)131);
            nullableArray4[33] = new byte?((byte)196);
            nullableArray4[34] = new byte?((byte)80);
            nullableArray4[35] = new byte?((byte)95);
            nullableArray4[36] = new byte?((byte)195);
            MainWindow.pattern_4 = nullableArray4;
            //    MainWindow.player_colors = new Color[4]
            //    {
            //Color.FromRgb((byte) 225, (byte) 65, (byte) 55),
            //Color.FromRgb((byte) 53, (byte) 136, (byte) 227),
            //Color.FromRgb((byte) 196, (byte) 172, (byte) 44),
            //Color.FromRgb((byte) 42, (byte) 208, (byte) 55)
            //    };
        }

    }
}
