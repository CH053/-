using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using WinForms = System.Windows.Forms;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Microsoft.Win32;

namespace kclock
{
    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static void makeNormal(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }
    }
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        private DispatcherTimer _timers;
        private int _seconds = 0;
        private int _minite = 0;
        private int _hour = 0;

        System.Windows.Forms.NotifyIcon notifyIcon;

        public string FGG { get; private set; }

        public Window1()
        {
            InitializeComponent();

            //הפעל בהפעל היישום
            RegisterInStartup(true);
            FGG = "אל תפעיל בהפעלת המחשב";
            

            //סמל באזור ההודעות סוגר יישום בלחיצה כפולה
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.MouseDown += new WinForms.MouseEventHandler(notifier_MouseDown);
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.Icon = Properties.Resources.Icon11;
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "לחץ לחיצה ימנית על הסמל לקבלת אפשרויות נוספות." +
                            Environment.NewLine +
                            "מרכז ההצלה - להצלת בחורים ואברכים מסכנת הגיוס בהנחית גדולי ישראל.";
            notifyIcon.ShowBalloonTip(5000);

            ReleaseMouseCapture();
            _timers = new DispatcherTimer();
            _timers.Tick += new EventHandler(timers_Tick);
            _timers.Interval = new TimeSpan(10000000);
            _timers.Start();
        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            string targetURL = "https://www.matara.pro/nedarimplus/online/?mosad=7000603";
            System.Diagnostics.Process.Start(targetURL);
            //System.Windows.Application.Current.Shutdown();
            // OR
            // Environment.Exit(0)
        }
        void notifier_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu menu = (ContextMenu)this.FindResource("NotifierContext");
                menu.IsOpen = true;
            }
        }

        private static string GetHebDate(DateTime date, string format)
        {

            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture("he-IL");
            ci.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();
            return date.ToString(format, ci);

        }

        void timers_Tick(object sender, EventArgs e)
        {
            roundDial.Opacity = 70;
            תאריך.Content = GetHebDate(DateTime.Now, "dddd dd MMMM yyyy");
            // get seconds, and move the second hand
            int nowSecond = DateTime.Now.Second * 6;    // times 6 to get rotate angle
            secs.RenderTransform = new RotateTransform(nowSecond, 100, 100);

            int nowMinute = DateTime.Now.Minute * 6;    // times 6 to get rotate angle
            mins.RenderTransform = new RotateTransform(nowMinute, 100, 100);

            int nowHour = DateTime.Now.Hour;
            if (nowHour > 12)
            {
                nowHour -= 12;    // only 12 hours on the clock face
            }
            nowHour *= 30;              // angle for the hourhand
            nowHour += nowMinute / 12;  // plus offset for minutes (60 minutes / 5)
            hrs.RenderTransform = new RotateTransform(nowHour, 100, 100);
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
            {
                DragMove();
            }
            
        }

        private void Blue_Click(object sender, RoutedEventArgs e)
        {
            roundDial.Fill = centerEllipse.Fill = closeBtn.Background = Brushes.Blue;            
        }

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            roundDial.Fill = centerEllipse.Fill = closeBtn.Background = Brushes.Red;
        }

        private void Green_Click(object sender, RoutedEventArgs e)
        {
            roundDial.Fill = centerEllipse.Fill = closeBtn.Background = Brushes.Green;
        }

        private void Grey_Click(object sender, RoutedEventArgs e)
        {
            roundDial.Fill = centerEllipse.Fill = closeBtn.Background = Brushes.Gray;            
        }

        private void canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        private void border1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                string targetURL = "https://www.matara.pro/nedarimplus/online/?mosad=7000603";
                System.Diagnostics.Process.Start(targetURL);
                //System.Windows.MessageBox.Show("Double Click");
            }
        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Menu_Open(object sender, RoutedEventArgs e)
        {        
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.makeNormal(hwnd);        
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            string targetURL = "https://www.matara.pro/nedarimplus/online/?mosad=7000603";
            System.Diagnostics.Process.Start(targetURL);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            string targetURL = "https://forms.gle/7DV9EJTkmyFw3VsL8";
            System.Diagnostics.Process.Start(targetURL);
        }
        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("ApplicationName", WinForms.Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue("ApplicationName");
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (FGG == "אל תפעיל בהפעלת המחשב")
            {
                RegisterInStartup(false);
                FGG = "הפעלת השעון בהפעלת המחשב";
                
            }
            else
            {
                RegisterInStartup(true);
                FGG = "אל תפעיל בהפעלת המחשב";
                
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //RegisterInStartup(false);
        }
    }
}
