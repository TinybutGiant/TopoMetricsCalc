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
            this.WindowState = FormWindowState.Maximized; // 启动时最大化

            // 创建一个Panel用于滚动查看内容
            Panel scrollPanel = new Panel();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;
            this.Controls.Add(scrollPanel); 

            string[] raLabels = { "RA_1 (interior radius of A):", "RA_2 (interior boundary of A):", "RA_3 (boundary of A):", "RA_4 (exterior boundary of A):" };
            string[] rbLabels = { "RB_1 (interior radius of B):", "RB_2 (interior boundary of B):", "RB_3 (boundary of B):", "RB_4 (exterior boundary of B):" };
            string[] defaultValues = { "15", "15.75", "17.75", "18.5" };

            for (int i = 0; i < 4; i++)
            {
                Label raLabel = CreateLabel(raLabels[i], 20 + 30 * i, 20);
                TextBox raTextBox = CreateTextBox(20 + 30 * i, 250, defaultValues[i]);
                raTextBoxes[i] = raTextBox;
                scrollPanel.Controls.Add(raLabel);
                scrollPanel.Controls.Add(raTextBox);

                Label rbLabel = CreateLabel(rbLabels[i], 20 + 30 * i, 400);
                TextBox rbTextBox = CreateTextBox(20 + 30 * i, 630, defaultValues[i]);
                rbTextBoxes[i] = rbTextBox;
                scrollPanel.Controls.Add(rbLabel);
                scrollPanel.Controls.Add(rbTextBox);
            }

            Label dLabel = CreateLabel("d (distance between centers):", 140, 20);
            dTextBox = CreateTextBox(140, 250, "32");
            scrollPanel.Controls.Add(dLabel);
            scrollPanel.Controls.Add(dTextBox);

            calculateButton = new Button { Text = "Update Calculate", Top = 170, Left = 150, AutoSize = true };
            calculateButton.Click += CalculateButton_Click;
            scrollPanel.Controls.Add(calculateButton);

            // Create panels for each SKControl with its label and text box
            for (int i = 0; i < 16; i++)
            {
                int row = i / 4;
                int col = i % 4;

                Panel panel = new Panel
                {
                    Top = 200 + (i / 4) * 240,
                    Left = 20 + (i % 4) * 200,
                    Width = 180,
                    Height = 240,
                    BorderStyle = BorderStyle.FixedSingle
                };

                resultLabels[i] = CreateLabel($"Overlap Area (RA_{row + 1} & RB_{col + 1}):", 0, 0);
                resultTextBoxes[i] = CreateTextBox(20, 0, "");
                resultTextBoxes[i].ReadOnly = true;

                skControls[i] = new SKControl { Top = 40, Left = 0, Width = 180, Height = 180 };
                skControls[i].PaintSurface += (s, e) => SkControl_PaintSurface(e, ra[row], rb[col], d);

                panel.Controls.Add(resultLabels[i]);
                panel.Controls.Add(resultTextBoxes[i]);
                panel.Controls.Add(skControls[i]);

                scrollPanel.Controls.Add(panel);
            }
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
