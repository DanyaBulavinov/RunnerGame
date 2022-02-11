using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunnerGame
{
    public partial class Form1 : Form
    {
        static List<Wall> walls = new List<Wall>();
        static List<Pie> pies = new List<Pie>();
        static Random r = new Random();
        static Person p = new Person();
        DateTime dt = DateTime.Now;
        SoundPlayer splayer = new SoundPlayer();

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                walls.Add(new Wall());
                walls[i].Generate(this.ClientRectangle);
            }
            for (int i = 0; i < 10; i++)
            {
                pies.Add(new Pie());
                pies[i].Generate(this.ClientRectangle);
            }
            p.Generate(this.ClientRectangle);
            splayer.SoundLocation = @"C:\WINDOWS\Media\ding.wav";
        }
        void MusicPlayer()
        {
            try
            {
                splayer.LoadAsync();
                splayer.Play();

            }
            catch (System.IO.FileNotFoundException err)
            {
            }
            catch (FormatException err)
            {

            }
            catch
            {
            }
        }

        public void DrawWall()
        {
            Graphics g = this.CreateGraphics();

            foreach (var w in walls)
            {
                SolidBrush brush = new SolidBrush(w.color);
                g.FillRectangle(brush, w.rect);
                brush.Dispose();
            }
        }

        public void DrawPie()
        {
            Graphics g = this.CreateGraphics();

            foreach (var pie in pies)
            {
                SolidBrush brush = new SolidBrush(pie.color);
                g.FillRectangle(brush, pie.rect);
                brush.Dispose();
            }
        }

        public void DrawPerson()
        {
            EattingPie();
            isWin();
            Graphics g = this.CreateGraphics();
            SolidBrush brush = new SolidBrush(p.color);
            g.FillRectangle(brush, p.rect);
            brush.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawWall();
            DrawPie();
            DrawPerson();
        }
        class Wall
        {
            static Random r = new Random();

            //Point point = new Point();
            public Color color = Color.Aqua;
            public Rectangle rect = new Rectangle();

            public Wall()
            {

            }
            public void Generate(Rectangle rectangle)
            {
                rect.Width = r.Next(15, 30);
                rect.Height = r.Next(15, 30);
                rect.X = r.Next(1, rectangle.Width - rect.Width);
                rect.Y = r.Next(1, rectangle.Height - rect.Height);
            }

        }
        class Pie : Wall
        {
            public Pie()
            {
                color = Color.Green;
            }

            public void Generate(Rectangle rectangle)
            {
                rect.Width = r.Next(5, 10);
                rect.Height = r.Next(5, 10);
                while (true)
                {
                    bool isOk = true;
                    var x = r.Next(1, rectangle.Width - rect.Width);
                    var y = r.Next(1, rectangle.Height - rect.Height);
                    foreach (var w in walls)
                    {
                        if (w.rect.IntersectsWith(new Rectangle(x, y, rect.Width, rect.Height)))
                        {
                            isOk = false;
                        }
                    }
                    if (isOk)
                    {
                        rect.X = x;
                        rect.Y = y;
                        break;
                    }
                }
            }

        }
        class Person : Pie
        {
            public int speed;
            public Person()
            {
                color = Color.Firebrick;
                speed = 7;
            }
            public Person(Rectangle rectangle)
            {
                rect = rectangle;
            }
            public void Generate(Rectangle rectangle)
            {
                rect.Width = 20;
                rect.Height = 20;
                while (true)
                {
                    bool isOk = true;
                    var x = r.Next(1, rectangle.Width - rect.Width);
                    var y = r.Next(1, rectangle.Height - rect.Height);
                    foreach (var w in walls)
                    {
                        if (w.rect.IntersectsWith(new Rectangle(x, y, rect.Width, rect.Height)))
                        {
                            isOk = false;
                        }
                    }
                    foreach (var pie in pies)
                    {
                        if (pie.rect.IntersectsWith(new Rectangle(x, y, rect.Width, rect.Height)))
                        {
                            isOk = false;
                        }
                    }
                    if (isOk)
                    {
                        rect.X = x;
                        rect.Y = y;
                        break;
                    }
                }
            }
        }

        private bool EattingPie()
        {
            bool isPie = false;
            foreach (var pie in pies)
            {
                if (new Rectangle(p.rect.X, p.rect.Y, p.rect.Width, p.rect.Height).IntersectsWith(pie.rect))
                {
                    isPie = true;
                    if (pie.color != this.BackColor) MusicPlayer();
                    pie.color = this.BackColor;
                    DrawPie();

                }

            }
            if (!isPie) return true;
            else return false;
        }
        private bool WallChecking()
        {
            bool isWall = false;
            foreach (var w in walls)
            {
                if (w.rect.IntersectsWith(new Rectangle(p.rect.X, p.rect.Y, p.rect.Width, p.rect.Height))) isWall = true;
            }
            if (!isWall) return false;
            else return true;
        }

        private void isWin()
        {
            bool isOk = true;
            foreach (var pie in pies)
            {
                if (pie.color != this.BackColor)
                {
                    isOk = false;
                }
            }
            if (isOk)
            {
                MessageBox.Show("CONGRATULATIONS! Time: " + (DateTime.Now - dt).Minutes + ":" + (DateTime.Now - dt).Seconds);
                Application.Restart();
            }

        }


        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.KeyData);
            //p.color = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
            p.color = this.BackColor;
            DrawPerson();
            p.color = Color.Firebrick;
            if (e.KeyData == Keys.Up)
            {
                p.rect.Y -= p.speed;
                if (WallChecking() || p.rect.Y < 0)
                {
                    p.rect.Y += p.speed;
                }
                DrawPerson();
            }
            else if (e.KeyData == Keys.Down)
            {
                p.rect.Y += p.speed;
                if (WallChecking() || p.rect.Y + p.rect.Height > 450)
                {
                    p.rect.Y -= p.speed;
                }
                DrawPerson();
            }
            else if (e.KeyData == Keys.Left)
            {
                p.rect.X -= p.speed;
                if (WallChecking() || p.rect.X < 0)
                {
                    p.rect.X += p.speed;
                }
                DrawPerson();
            }
            else if (e.KeyData == Keys.Right)
            {
                p.rect.X += p.speed;
                if (WallChecking() || p.rect.X + p.rect.Width > 800)
                {
                    p.rect.X -= p.speed;
                }
                DrawPerson();
            }
            else DrawPerson();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            this.Text = (DateTime.Now - dt).Minutes + ":" + (DateTime.Now - dt).Seconds;
        }
    }
}
