using Gma.System.MouseKeyHook;
using PropertyChanged;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorPic.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        #region 필드
        private int _mouseX;
        private int _mouseY;


        private IKeyboardMouseEvents globalHook;

        #endregion

        #region 프로퍼티

        // RGB
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public string ChoiceColor { get; set; }

        public int MouseX { get; set; }
        public string MouseY { get; set; }
        #endregion

        

        #region 생성자
        public MainViewModel()
        {
            globalHook = Hook.GlobalEvents();

            _mouseX = default;
            _mouseY = default;

            globalHook.MouseMove += GlobalHook_MouseMove;
            globalHook.MouseClick += GlobalHook_MouseClick;


        }
        #endregion

        #region 메소드
        private void GlobalHook_MouseMove(object sender, MouseEventArgs e)
        {
            ChoiceColor = GetColorAt(e.Location);
        }

        private void GlobalHook_MouseClick(object sender, MouseEventArgs e)
        {
            //ChoiceColor = "#000000";
            //Bitmap bitmap = new Bitmap(100,100);

        }
        
        // getColor 필요한 dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);
        
        public string GetColorAt(Point cursor)
        {
            int x = (int)cursor.X, y = (int)cursor.Y;
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);

            //MouseX = a;
            Color color = Color.FromArgb(255, (byte)((a >> 0) & 0xff), (byte)((a >> 8) & 0xff), (byte)((a >> 16) & 0xff));
            string hexR = color.R.ToString("x2");
            string hexG = color.G.ToString("x2");
            string hexB = color.B.ToString("x2");
            string hex = $"#{hexR}{hexG}{hexB}";
            return hex;
        }
    #endregion
    }
}
