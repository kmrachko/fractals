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
    public partial class MainFractal : Form
    {
        private List<PointF> points_gen;
        private List<PointF> points_fract;
        private int depth;
        private float scale;
        public MainFractal()
        {
            InitializeComponent();
            this.depth = 1;
            this.scale = 1;
            points_gen = new List<PointF>();
            points_fract = new List<PointF>();

            points_gen.Add(new PointF(0, 0));
            points_gen.Add(new PointF(18, 0));

            this.pictureBox2.Paint += DrawGenerator;
            //this.pictureBox1.Paint += fractal.DrawGrid;
            this.pictureBox1.Paint += DrawFractal;
        }
        private float GetLineLength(PointF p1, PointF p2)
        {
            double l = (p2.X-p1.X)*(p2.X-p1.X) + (p2.Y-p1.Y)*(p2.Y-p1.Y);
            return (float)Math.Pow(l, 0.5f);
        }
        private double GetAngle(PointF p1, PointF p2, PointF pp1, PointF pp2)
        {
            double A1 = p1.Y - p2.Y;
            double B1 = p2.X - p1.X;
            double A2 = pp1.Y - pp2.Y;
            double B2 = pp2.X - pp1.X;
            double a = ((A1*A2) + (B1*B2))/(Math.Pow(A1*A1+B1*B1,0.5f)*Math.Pow(A2*A2+B2*B2,0.5f));
            if (A1 > 0) return -1*Math.Acos(a);
            return Math.Acos(a);
        }
        private List<PointF> GetFractalCoords(PointF p1, PointF p2)
        {
            List<PointF> coords = new List<PointF>();
            double A = p1.Y - p2.Y;
            double B = p2.X - p1.X;
            double alpha = GetAngle(p1, p2, new PointF(0, 0), new PointF(5, 0));
            PointF temp;
            PointF[] scaled_points = new PointF[points_gen.Count];
            float l1 = GetLineLength(points_gen[0], points_gen[points_gen.Count-1]);
            float l2 = GetLineLength(p1, p2);
            float k = l2 / l1;
            int u=0;
            foreach (PointF p in this.points_gen)
            {
                scaled_points[u].X = p.X * k;
                scaled_points[u++].Y = p.Y * k;
            }
            foreach (PointF p in scaled_points)
            {
                double x = p.X * Math.Cos(alpha) - p.Y * Math.Sin(alpha) + p1.X;
                double y = p.X * Math.Sin(alpha) + p.Y * Math.Cos(alpha) + p1.Y;
                temp = new PointF((float)(x), (float)(y));
                coords.Add(temp);
            }
            coords.RemoveAt(0);
            coords.RemoveAt(coords.Count-1);
            return coords;
        }
        private void DrawFractal(object sender, PaintEventArgs e)
        {
            this.label2.Text = "";
            PictureBox Pbox = sender as PictureBox;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            points_fract.Clear();
            //GetFractalCoords(new PointF(0, 6), new PointF(3, 0));
            points_fract.Add(new PointF(-9f*scale, 0));
            points_fract.Add(new PointF(9f * scale, 0));
            List<PointF> temp = new List<PointF>();
            List<PointF> temp_fract = new List<PointF>(points_fract);
            PointF last;
            int j=1;
            for (int i = 0; i < this.depth; i++)
            {
                for (j = 1; j < temp_fract.Count; j++)
                {
                    temp = GetFractalCoords(temp_fract[j-1],temp_fract[j]);
                    last = temp_fract[j];
                    points_fract.Remove(last);
                    points_fract.AddRange(temp);
                    points_fract.Add(last);
                }
                temp_fract = new List<PointF>(points_fract);
            }
            Point[] pts_px = new Point[points_fract.Count];
            Point temp_px;
            int h = 0;
            PointF newt;
            int mark = Pbox.Width / 20;
            foreach (PointF p in points_fract)
            {
                newt = new PointF(p.X , p.Y);
                temp_px = fractal.GetPixels(newt, new Point( Pbox.Width/2, Pbox.Height/2), mark);
                pts_px[h++] = temp_px;
            }

            Pen line = new Pen(Color.Black, (depth<3)?2:1);
            Brush pnt = new SolidBrush(Color.LightSeaGreen);
            fractal.DrawPointLine(e.Graphics, line, pnt, pts_px, 0);
        }
        private void DrawGenerator(object sender, PaintEventArgs e)
        {
            PictureBox Pbox = sender as PictureBox;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Point[] pts = new Point[points_gen.Count];
            Point temp;
            int i = 0;
            Point center = new Point(Pbox.Width / 20, Pbox.Height / 2);
            int mark = Pbox.Width / 20;
            foreach (PointF p in points_gen)
            {
                temp = fractal.GetPixels(p, center, mark);
                pts[i++] = temp;
            }
            Pen line = new Pen(Color.Black, 1);
            Brush pnt = new SolidBrush(Color.LightSeaGreen);
            fractal.DrawPointLine(e.Graphics, line, pnt, pts, 2);
        }
        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            this.label1.Text = this.trackBar1.Value.ToString();
            this.depth = this.trackBar1.Value;
            this.pictureBox1.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            fractal CreateFractal = new fractal(points_gen, new Point(20,200), 20);
            DialogResult res = CreateFractal.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.points_gen = CreateFractal.points;
                this.pictureBox1.Invalidate();
                this.pictureBox2.Invalidate();
            }
            else
            {
                points_gen.Clear();
                points_gen.Add(new PointF(0, 0));
                points_gen.Add(new PointF(18, 0));
                this.pictureBox1.Invalidate();
                this.pictureBox2.Invalidate();
            }
            
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.label4.Text = (this.trackBar2.Value*10).ToString()+"%";
            this.scale = this.trackBar2.Value/10f;
            this.pictureBox1.Invalidate();
        }
    }
}
