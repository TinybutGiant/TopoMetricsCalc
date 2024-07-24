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
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
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
            this.Size = new System.Drawing.Size(800, 500);

            string[] raLabels = { "RA_1 (interior radius of A):", "RA_2 (interior boundary of A):", "RA_3 (boundary of A):", "RA_4 (exterior boundary of A):" };
            string[] rbLabels = { "RB_1 (interior radius of B):", "RB_2 (interior boundary of B):", "RB_3 (boundary of B):", "RB_4 (exterior boundary of B):" };
            string[] defaultValues = { "15", "15.75", "17.75", "18.5" };

            for (int i = 0; i < 4; i++)
            {
                Label raLabel = CreateLabel(raLabels[i], 20 + 30 * i, 20);
                TextBox raTextBox = CreateTextBox(20 + 30 * i, 250, defaultValues[i]);
                raTextBoxes[i] = raTextBox;
                this.Controls.Add(raLabel);
                this.Controls.Add(raTextBox);

                Label rbLabel = CreateLabel(rbLabels[i], 20 + 30 * i, 400);
                TextBox rbTextBox = CreateTextBox(20 + 30 * i, 630, defaultValues[i]);
                rbTextBoxes[i] = rbTextBox;
                this.Controls.Add(rbLabel);
                this.Controls.Add(rbTextBox);
            }

            Label dLabel = CreateLabel("d (distance between centers):", 140, 20);
            dTextBox = CreateTextBox(140, 250, "32");
            this.Controls.Add(dLabel);
            this.Controls.Add(dTextBox);

            calculateButton = new Button { Text = "Update Calculate", Top = 170, Left = 150, AutoSize = true };
            calculateButton.Click += CalculateButton_Click;
            this.Controls.Add(calculateButton);

            // Create result labels and text boxes for overlap areas
            for (int i = 0; i < 16; i++)
            {
                int row = i / 4;
                int col = i % 4;
                resultLabels[i] = CreateLabel($"Overlap Area (RA_{row + 1} & RB_{col + 1}): ", 200 + 30 * i, 20);
                resultTextBoxes[i] = CreateTextBox(200 + 30 * i, 250, "");
                resultTextBoxes[i].ReadOnly = true;
                this.Controls.Add(resultLabels[i]);
                this.Controls.Add(resultTextBoxes[i]);
            }

            // Create SKControls for drawing
            for (int i = 0; i < 16; i++)
            {
                int row = i / 4;
                int col = i % 4;
                skControls[i] = new SKControl { Top = 800 + (i / 4) * 210, Left = 20 + (i % 4) * 210, Width = 200, Height = 200 };
                skControls[i].PaintSurface += (s, e) => SkControl_PaintSurface(e, ra[row], rb[col], d);
                this.Controls.Add(skControls[i]);
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
            Program.DrawTwoCircles(canvas, R1, R2, d, true);
        }
    }
}
