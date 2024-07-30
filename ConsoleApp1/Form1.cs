using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace ConsoleApp1
{
    public partial class Form1 : Form
    {
        private TextBox[] raTextBoxes = new TextBox[4]; //4 circles radius for A
        private TextBox[] rbTextBoxes = new TextBox[4]; //4 circles radius for B
        private TextBox[] resultTextBoxes = new TextBox[16]; //16 combination overlap
        private TextBox? dTextBox; //distance between two center
        private Button? calculateButton;
        private SKControl[] skControls = new SKControl[16]; //SKControls for drawing
        private Label[] resultLabels = new Label[16]; //Label for names
        private float[] ra = new float[4]; //
        private float[] rb = new float[4];
        private float d;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // 将窗体居中显示
            this.WindowState = FormWindowState.Maximized; // 启动时最大化
            InitializeCustomComponents();
            CalculateButton_Click(null, EventArgs.Empty); // 自动进行第一次计算和绘图
        }

        private Label CreateLabel(string text, int top, int left)
        {
            return new Label{Text = text, Top = top, Left = left, AutoSize = true};
        }

        private TextBox CreateTextBox(int top, int left, string defaultText)
        {
            return new TextBox{
                Top = top, Left = left, Width = 100, Text = defaultText};
        }
        
        private void InitializeCustomComponents()
        {
            this.Text = "Circles Overlap";
            this.WindowState = FormWindowState.Maximized;

            // 创建一个TableLayoutPanel用于滚动查看内容
            TableLayoutPanel tablePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            this.Controls.Add(tablePanel);

            string[] reentrantLabels = { "e (horizontal distance):", "l (slant edge length):", "t (thickness):", "theta (angle):" };
            string[] defaultValues_1 = { "40", "10", "2", "0.959931" }; // theta = 55 degrees in radians

            AddReentrantLabelsAndTextBoxes(tablePanel, reentrantLabels, defaultValues_1);

            // 添加绘制re-entrant结构的Panel
            Panel drawingPanel = new Panel
            {
                Width = 500,
                Height = 500,
                BorderStyle = BorderStyle.FixedSingle
            };
            drawingPanel.Paint += DrawingPanel_Paint;
            tablePanel.Controls.Add(drawingPanel);
            tablePanel.SetColumnSpan(drawingPanel, 2);

            string[] raLabels = { "RA_1 (interior radius of A):", "RA_2 (interior boundary of A):", "RA_3 (boundary of A):", "RA_4 (exterior boundary of A):" };
            string[] rbLabels = { "RB_1 (interior radius of B):", "RB_2 (interior boundary of B):", "RB_3 (boundary of B):", "RB_4 (exterior boundary of B):" };
            string[] defaultValues_2 = { "15", "15.75", "17.75", "18.5" };

            AddLabelsAndTextBoxes(tablePanel, raLabels, rbLabels, defaultValues_2);

            // 计算AC之间的距离 d
            double e = Convert.ToDouble(defaultValues_1[0]);
            double l = Convert.ToDouble(defaultValues_1[1]);
            double theta = Convert.ToDouble(defaultValues_1[3]);
            double d = CalculateDistance(e, l, theta);

            Label dLabel = CreateLabel("d (distance between centers):", 0, 0);
            dTextBox = CreateTextBox(0, 0, d.ToString());
            tablePanel.Controls.Add(dLabel);
            tablePanel.Controls.Add(dTextBox);

            calculateButton = new Button { Text = "Update Calculate", AutoSize = true };
            calculateButton.Click += CalculateButton_Click;
            tablePanel.Controls.Add(calculateButton);
            tablePanel.SetColumnSpan(calculateButton, 2);

            CreateResultPanels(tablePanel);
        }
        private void AddReentrantLabelsAndTextBoxes(TableLayoutPanel panel, string[] labels, string[] defaultValues)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                Label label = CreateLabel(labels[i], 0, 0);
                TextBox textBox = CreateTextBox(0, 0, defaultValues[i]);
                panel.Controls.Add(label);
                panel.Controls.Add(textBox);
            }
        }

        private double CalculateDistance(double e, double l, double theta)
        {
            // 根据几何关系计算AC的距离d
            return Math.Sqrt(e * e + 4 * l * l - 4 * e * l * Math.Cos(theta));
        }

        private void AddLabelsAndTextBoxes(TableLayoutPanel panel, string[] raLabels, string[] rbLabels, string[] defaultValues)
        {
            for (int i = 0; i < 4; i++)
            {
                Label raLabel = CreateLabel(raLabels[i], 0, 0);
                TextBox raTextBox = CreateTextBox(0, 0, defaultValues[i]);
                raTextBoxes[i] = raTextBox;
                panel.Controls.Add(raLabel);
                panel.Controls.Add(raTextBox);

                Label rbLabel = CreateLabel(rbLabels[i], 0, 0);
                TextBox rbTextBox = CreateTextBox(0, 0, defaultValues[i]);
                rbTextBoxes[i] = rbTextBox;
                panel.Controls.Add(rbLabel);
                panel.Controls.Add(rbTextBox);
            }
        }

        private void CreateResultPanels(TableLayoutPanel panel)
        {
            for (int i = 0; i < 16; i++)
            {
                int row = i / 4;
                int col = i % 4;

                Panel resultPanel = new Panel
                {
                    Width = 200,
                    Height = 240,
                    BorderStyle = BorderStyle.FixedSingle
                };

                resultLabels[i] = CreateLabel($"Overlap Area (RA_{row + 1} & RB_{col + 1}):", 0, 0);
                resultTextBoxes[i] = CreateTextBox(0, 20, "");
                resultTextBoxes[i].ReadOnly = true;

                skControls[i] = new SKControl { Top = 40, Left = 0, Width = 180, Height = 180 };
                skControls[i].PaintSurface += (s, e) => SkControl_PaintSurface(e, ra[row], rb[col], d);

                resultPanel.Controls.Add(resultLabels[i]);
                resultPanel.Controls.Add(resultTextBoxes[i]);
                resultPanel.Controls.Add(skControls[i]);

                panel.Controls.Add(resultPanel);
            }
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 2);

            double eValue = Convert.ToDouble("40");
            double lValue = Convert.ToDouble("10");
            double tValue = Convert.ToDouble("2");
            double thetaValue = Convert.ToDouble("0.959931");

            float eFloat = (float)eValue;
            float l = (float)lValue;
            float t = (float)tValue;
            float theta = (float)thetaValue;

            float x0 = 250, y0 = 250;

            // 计算各点坐标
            float ax = x0, ay = y0;
            float bx = ax + eFloat / 2, by = ay - (float)(l * Math.Sin(theta));
            float cx = bx + (float)(l * Math.Cos(theta)), cy = by + (float)(l * Math.Sin(theta));
            float dx = cx, dy = cy + t;
            float ex = dx - (float)(l * Math.Cos(theta)), ey = dy + (float)(l * Math.Sin(theta));
            float fx = ax - eFloat / 2, fy = ey;
            float gx = fx + (float)(l * Math.Cos(theta)), gy = fy - (float)(l * Math.Sin(theta));
            float hx = gx, hy = gy - t;

            // 绘制re-entrant结构
            PointF[] points = { new PointF(ax, ay), new PointF(bx, by), new PointF(cx, cy), new PointF(dx, dy), new PointF(ex, ey), new PointF(fx, fy), new PointF(gx, gy), new PointF(hx, hy) };
            g.DrawPolygon(pen, points);

            // 标注顶点
            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;
            g.DrawString("A", font, brush, ax, ay);
            g.DrawString("B", font, brush, bx, by);
            g.DrawString("C", font, brush, cx, cy);
            g.DrawString("D", font, brush, dx, dy);
            g.DrawString("E", font, brush, ex, ey);
            g.DrawString("F", font, brush, fx, fy);
            g.DrawString("G", font, brush, gx, gy);
            g.DrawString("H", font, brush, hx, hy);
        }

        private void CalculateButton_Click(object? sender, EventArgs e)
        {
            // 读取所有半径值
            for (int i = 0; i < 4; i++)
            {
                ra[i] = float.Parse(raTextBoxes[i].Text);
                rb[i] = float.Parse(rbTextBoxes[i].Text);
            }
            d = float.Parse(dTextBox?.Text ?? "0");

            // 计算并显示所有重叠区域
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = i * 4 + j;
                    float overlapArea = Program.CalculateTwoCirclesOverlap(ra[i], rb[j], d);
                    resultTextBoxes[index].Text = overlapArea.ToString();
                    skControls[index]?.Invalidate();
                }
            }
        }

        private void SkControl_PaintSurface(SKPaintSurfaceEventArgs e, float R1, float R2, float d)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White); // 清空画布
            // 获取SKSurface的物理尺寸和比例
            float scale = Math.Min(e.Info.Width / 200f, e.Info.Height / 200f); // 根据控件大小调整比例
            float centerX = e.Info.Width / 2;
            float centerY = e.Info.Height / 2;

            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Green,
                StrokeWidth = 2
            })
            {
                // 调整圆圈大小和位置
                float adjustedR1 = R1 * scale;
                float adjustedR2 = R2 * scale;
                float adjustedD = d * scale;

                // 圆A的位置
                float ax = centerX - adjustedD / 2;
                float ay = centerY;

                // 圆B的位置
                float bx = centerX + adjustedD / 2;
                float by = centerY;

                // 绘制圆A
                canvas.DrawCircle(ax, ay, adjustedR1, paint);
                // 绘制圆B
                canvas.DrawCircle(bx, by, adjustedR2, paint);
            }
        }
    }
}
