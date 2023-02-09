using Gma.System.MouseKeyHook;
using PropertyChanged;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

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

        public BitmapImage ZoomImage { get; set; }
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
        // 돋보기 기능
        private Bitmap Zoom()
        {
            // 마우스 커서의 위치 가져오기
            int curX = Cursor.Position.X;
            int curY = Cursor.Position.Y;

            int screenWidth = 50;
            int screenHeight = 50;

            Size size= new Size(screenWidth, screenHeight);
            Bitmap bitmap = new Bitmap(size.Width, size.Height);

            // ?
            double rate = 2.0;

            Graphics graphics= Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(curX - (screenWidth / 2), curY - (screenHeight / 2), 0, 0, size);

            int zoominWidth = (int)(screenWidth * rate);
            int zoominHeight = (int)(screenHeight * rate);

            Bitmap zoomin = new Bitmap(zoominWidth, zoominHeight);

            for(int i =0; i< zoominWidth; i++) 
            { 
                for(int j=0; j<zoominHeight; j++) 
                {
                    int row = (int)(i / rate);
                    int col = (int)(j / rate);
                    zoomin.SetPixel(i,j,bitmap.GetPixel(row,col));
                }
            }

            return zoomin;
        }

        private void GlobalHook_MouseMove(object sender, MouseEventArgs e)
        {
            ChoiceColor = GetColorAt(e.Location);
            ZoomImage = Bitmap2BitmapImage(Zoom());
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

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            // 새 비트맵 이미지 객체 생성
            BitmapImage bitmapimage = new BitmapImage();
            // 메모리 스트림을 사용한다.
            using (MemoryStream stream = new MemoryStream())
            {
                // 메모리 스트림에 bitmap을 Bmp로 저장한다.
                bitmap.Save(stream, ImageFormat.Bmp);
                // 스트림 포지션 0으로 설정해 처음부터 잡는다.
                stream.Position = 0;
                // 비트맵 이미지 초기화
                bitmapimage.BeginInit();
                // 비트맵 캐시옵셥 : 이미지가 다 생성되야 stream 닫게설정
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                // 스트림소스에 스트림 내용을 집어 넣는다.
                bitmapimage.StreamSource = stream;
                // 비트맵 이미지 초기화 종료
                bitmapimage.EndInit();
                // 이미지 변경을 더 이상 하지 않는다고 선언(바인딩 권한 위해 필수)
                bitmapimage.Freeze();
                return bitmapimage;
            }
        }
        #endregion
    }
}
