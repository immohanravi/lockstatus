using LockStatus.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LockStatus
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static ContextMenuStrip cm;
        static ToolStripMenuItem about;
        static ToolStripMenuItem exit;
        static bool isAboutLoaded = false;
        static int capsS = 0;
        static int numS = 0;
       static NotifyIcon caps;
       static NotifyIcon num;
       private const int WH_KEYBOARD_LL = 13;
       private const int WM_KEYDOWN = 0x0100;
       private static LowLevelKeyboardProc _proc = HookCallback;
       private static IntPtr _hookID = IntPtr.Zero;
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            cm = new ContextMenuStrip();
            about = new ToolStripMenuItem();
            about.Text = "About";
            about.Image = Resources.About;
            about.Click += new System.EventHandler(About_Click);
            exit = new ToolStripMenuItem();
            exit.Text = "exit";
            exit.Image = Resources.Exit;
            exit.Click += new System.EventHandler(Exit_Click);
            cm.Items.Add(about);
            cm.Items.Add(exit);

            

            caps = new NotifyIcon();
            num = new NotifyIcon();
            caps.MouseClick += new MouseEventHandler(ni_MouseClick);
            num.MouseClick += new MouseEventHandler(ni_MouseClick);
            caps.ContextMenuStrip = cm;
            num.ContextMenuStrip = cm;
            int state = status();
            if (state == 0)
            {
                caps.Icon = Resources.capsOFF;
                num.Icon = Resources.numOFF;
                caps.Text = "Caps Lock OFF";
                num.Text = "Num Lock OFF";
                caps.Visible = true;
                num.Visible = true;
            }
            else if (state == 1)
            {
                caps.Icon = Resources.capsOFF;
                num.Icon = Resources.numON;
                caps.Text = "Caps Lock OFF";
                num.Text = "Num Lock ON";
                caps.Visible = true;
                num.Visible = true;
                numS = 1;
            }
            else if (state == 2)
            {
                caps.Icon = Resources.capsON;
                num.Icon = Resources.numOFF;
                caps.Text = "Caps Lock ON";
                num.Text = "Num Lock OFF";
                caps.Visible = true;
                num.Visible = true;
                capsS = 1;
            }

            else if (state == 3)
            {
                caps.Icon = Resources.capsON; 
                num.Icon = Resources.numON;
                caps.Text = "Caps Lock ON";
                num.Text = "Num Lock ON";
                caps.Visible = true;
                num.Visible = true;
                capsS = 1;
                numS = 1;
            }
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        static void ni_MouseClick(object sender, MouseEventArgs e)
        {
            // Handle mouse button clicks.
            if (e.Button == MouseButtons.Right)
            {
                // Start Windows Explorer.
                
            }
        }

        static int status()
        {
            bool[] answers = new bool[2];
            bool cap = Control.IsKeyLocked(Keys.CapsLock);
            bool num = Control.IsKeyLocked(Keys.NumLock);
            if (!cap && !num)
                return 0;
            else if (!cap && num)
                return 1;
            else if (cap && !num)
                return 2;

            return 3;
        }
        static void changeStatus(Keys key)
        {
            
            if (key.ToString().Equals("Capital"))
            {
                capsS++;
                if (capsS % 2 == 0)
                {
                    caps.Icon = Resources.capsOFF;
                    caps.Text = "CapsLock OFF";
                }
                else
                {
                    caps.Icon = Resources.capsON;
                    caps.Text = "CapsLock ON";
                }
                    

            }
            else if (key.ToString().Equals("NumLock"))
            {
                numS++;
                if (numS % 2 == 0)
                {
                    
                    num.Icon = Resources.numOFF;
                    num.Text = "Num Lock OFF";
                }
                else
                {
                    num.Icon = Resources.numON;
                    num.Text = "Num Lock ON";
                }
                    
            }
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                changeStatus((Keys)vkCode);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

       static void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new AboutBox().ShowDialog();
                isAboutLoaded = false;
            }
        }

        static void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }
        public static void Dispose()
        {
            // When the application closes, this will remove the icon from the system tray immediately.
            caps.Dispose();
            num.Dispose();
        }
    }
    
}
