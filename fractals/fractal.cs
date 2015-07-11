using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fractals
{
    public partial class fractal : Form
    {
        public List<PointF> points { get; private set; }
        private Point center;
        private int mark;
        public class ComparePoints:IComparer<PointF> 
        {
            public int Compare(PointF p1, PointF p2)
            {
                if (p1.X > p2.X) return 1;
                else if (p2.X > p1.X) return -1;
                else return 0;
            }
        }
        public fractal(List<PointF> start_pts, Point line_center, int start_mark)
        {
            InitializeComponent();
            points = start_pts;
            center = line_center;
            mark = start_mark;
            this.pictureBox1.Paint += DrawGrid;
            this.pictureBox1.Paint += DrawLine;
            this.pictureBox1.MouseClick += ClickHandler;
        }
        public static PointF GetCoords(Point p, Point center, int mark)
        {
            PointF temp = new PointF();
            temp.Y = (float)(p.Y - center.Y) / mark;
            temp.Y *= -1;
            temp.X = (float)(p.X - center.X) / mark;
            return temp;
        }
        public static Point GetPixels(PointF p, Point center, int mark)
        {
            Point temp = new Point();
            temp.X = center.X + Convert.ToInt32(p.X * mark);
            temp.Y = center.Y - Convert.ToInt32(p.Y * mark);
            return temp;
        }
        public static void DrawPointLine(Graphics pbox, Pen line_pen, Brush point_brush, Point[] pts_px, int point_r)
        {
            Rectangle point;
            for (int i = 0; i < pts_px.Length-1; i++)
            {
                pbox.DrawLine(line_pen, pts_px[i], pts_px[i + 1]);
            }
            for (int i = 0; i < pts_px.Length; i++)
            {
                point = new Rectangle(pts_px[i].X - point_r, pts_px[i].Y - point_r, point_r * 2, point_r*2);
                pbox.DrawEllipse(new Pen(Color.Black,1), point);
                pbox.FillEllipse(point_brush, point);
            }
            return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void DrawLine(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Point[] pts = new Point[points.Count];            
            Point temp;
            int i = 0;
            foreach (PointF p in points)
            {
                temp = GetPixels(p, center, mark);
                pts[i++] = temp;
            }
            Pen line = new Pen (Color.Black,2);
            Brush pnt = new SolidBrush(Color.LightSeaGreen);
            DrawPointLine(e.Graphics, line, pnt, pts, 4);
        }
        public static void DrawGrid(object sender, PaintEventArgs e)
        {
            PictureBox Pbox = sender as PictureBox;
            Pen gridpen = new Pen(Color.LightGray, 1);
            int l_mark = Pbox.Width/20;
            for (int i = l_mark; i < Pbox.Width; i += l_mark)
                e.Graphics.DrawLine(gridpen, new Point(i, 0), new Point(i, Pbox.Height));
            for (int i = l_mark; i < Pbox.Height; i += l_mark)
                e.Graphics.DrawLine(gridpen, new Point(0, i), new Point(Pbox.Width, i));
        }
        private void ClickHandler(object sender, MouseEventArgs e)
        {
            PictureBox Pbox = sender as PictureBox;
            PointF temp;
            temp = GetCoords(new Point(e.X, e.Y), center, mark);
            if (this.checkBox1.Checked == true && Math.Abs(temp.X - Math.Floor(temp.X) - 0.5) >= 0.0001 && Math.Abs(temp.Y - Math.Floor(temp.Y) - 0.5) >= 0.0001)
            {
                double x = Math.Round(Convert.ToDouble(temp.X));
                double y = Math.Round(Convert.ToDouble(temp.Y));
                temp = new PointF((float)x, (float)y);
            }
            ComparePoints PointsComparer = new ComparePoints();
            points.Sort(PointsComparer);

            while (points.BinarySearch(temp, PointsComparer) >= 0)
            {
                temp.X += 0.0001f;
            }
            this.points.Add(temp);
            points.Sort(PointsComparer);
            Pbox.Invalidate();
            return;
        }
    }
}
