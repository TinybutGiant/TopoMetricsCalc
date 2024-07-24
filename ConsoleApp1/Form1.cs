using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace ConsoleApp1
{
    public partial class Form1 : Form
    {
        private TextBox? ra1TextBox, ra2TextBox, rb1TextBox, rb2TextBox, dTextBox, resultTextBox1, resultTextBox2, resultTextBox3, resultTextBox4;
        private Button? calculateButton;
        private SKControl? skControl1, skControl2, skControl3, skControl4;
        private Label? resultLabel1, resultLabel2, resultLabel3, resultLabel4;
        private float ra1, ra2, rb1, rb2, d;

        public Form1()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            InitializeCustomComponents();
            CalculateButton_Click(null, EventArgs.Empty); // 自动进行第一次计算和绘图
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Concentric Circles Overlap";
            this.Size = new System.Drawing.Size(400, 500);

            Label ra1Label = new Label { Text = "RA_1 (exterior radius of A):", Top = 20, Left = 20, AutoSize = true  };
            ra1TextBox = new TextBox { Top = 20, Left = 250, Width = 100 ,Text = "50"};

            Label ra2Label = new Label { Text = "RA_2 (interior radius of A):", Top = 50, Left = 20, AutoSize = true  };
            ra2TextBox = new TextBox { Top = 50, Left = 250, Width = 100 ,Text = "35"};

            Label rb1Label = new Label { Text = "RB_1 (exterior radius of B):", Top = 80, Left = 20, AutoSize = true  };
            rb1TextBox = new TextBox { Top = 80, Left = 250, Width = 100 ,Text = "50"};

            Label rb2Label = new Label { Text = "RB_2 (interior radius of B)):", Top = 110, Left = 20, AutoSize = true  };
            rb2TextBox = new TextBox { Top = 110, Left = 250, Width = 100, Text = "35"};

            Label dLabel = new Label { Text = "d (distance betweeen centers):", Top = 140, Left = 20, AutoSize = true  };
            dTextBox = new TextBox { Top = 140, Left = 250, Width = 100, Text = "80"};

            calculateButton = new Button { Text = "Update Calculate", Top = 170, Left = 150, AutoSize = true  };
            calculateButton.Click += CalculateButton_Click;

            resultLabel1 = new Label { Text = "Overlap Area (RA_1 & RB_1): ", Top = 200, Left = 20, Width = 300, AutoSize = true };
            resultTextBox1 = new TextBox { Top = 200, Left = 250, Width = 100, ReadOnly = true };

            resultLabel2 = new Label { Text = "Overlap Area (RA_1 & RB_2): ", Top = 230, Left = 20, Width = 300, AutoSize = true };
            resultTextBox2 = new TextBox { Top = 230, Left = 250, Width = 100, ReadOnly = true };

            resultLabel3 = new Label { Text = "Overlap Area (RA_2 & RB_1): ", Top = 260, Left = 20, Width = 300, AutoSize = true };
            resultTextBox3 = new TextBox { Top = 260, Left = 250, Width = 100, ReadOnly = true };

            resultLabel4 = new Label { Text = "Overlap Area (RA_2 & RB_2): ", Top = 290, Left = 20, Width = 300, AutoSize = true };
            resultTextBox4 = new TextBox { Top = 290, Left = 250, Width = 100, ReadOnly = true };// 用于显示结果的文本框

            skControl1 = new SKControl { Top = 320, Left = 20, Width = 400, Height = 200 };
            skControl1.PaintSurface += (s, e) => SkControl_PaintSurface(e, ra1, rb1, d);

            skControl2 = new SKControl { Top = 530, Left = 20, Width = 400, Height = 200 };
            skControl2.PaintSurface += (s, e) => SkControl_PaintSurface(e, ra1, rb2,d);

            skControl3 = new SKControl { Top = 320, Left = 450, Width = 400, Height = 200 };
            skControl3.PaintSurface += (s, e) => SkControl_PaintSurface(e, ra2, rb1, d);

            skControl4 = new SKControl { Top = 530, Left = 450, Width = 400, Height = 200 };
            skControl4.PaintSurface += (s, e) => SkControl_PaintSurface(e, ra2, rb2,d );

            this.Controls.Add(ra1Label);//1
            this.Controls.Add(ra1TextBox); //2
            this.Controls.Add(ra2Label); //3
            this.Controls.Add(ra2TextBox);//4
            this.Controls.Add(rb1Label);//5
            this.Controls.Add(rb1TextBox);//6
            this.Controls.Add(rb2Label);//7
            this.Controls.Add(rb2TextBox);//8
            this.Controls.Add(dLabel);//9
            this.Controls.Add(dTextBox);//10
            this.Controls.Add(calculateButton);//11
            this.Controls.Add(resultLabel1);
            this.Controls.Add(resultTextBox1);
            this.Controls.Add(resultLabel2);
            this.Controls.Add(resultTextBox2);
            this.Controls.Add(resultLabel3);
            this.Controls.Add(resultTextBox3);
            this.Controls.Add(resultLabel4);
            this.Controls.Add(resultTextBox4);
            this.Controls.Add(skControl1);
            this.Controls.Add(skControl2);
            this.Controls.Add(skControl3);
            this.Controls.Add(skControl4);
        }

        private void CalculateButton_Click(object? sender, EventArgs e)
        {
            ra1 = float.Parse(ra1TextBox?.Text ?? "0");
            ra2 = float.Parse(ra2TextBox?.Text ?? "0");
            rb1 = float.Parse(rb1TextBox?.Text ?? "0");
            rb2 = float.Parse(rb2TextBox?.Text ?? "0");
            d = float.Parse(dTextBox?.Text ?? "0");

            float overlapArea1 = Program.CalculateTwoCirclesOverlap(ra1, rb1, d);
            float overlapArea2 = Program.CalculateTwoCirclesOverlap(ra1, rb2, d);
            float overlapArea3 = Program.CalculateTwoCirclesOverlap(ra2, rb1, d);
            float overlapArea4 = Program.CalculateTwoCirclesOverlap(ra2, rb2, d);

            resultTextBox1!.Text = overlapArea1.ToString();
            resultTextBox2!.Text = overlapArea2.ToString();
            resultTextBox3!.Text = overlapArea3.ToString();
            resultTextBox4!.Text = overlapArea4.ToString();

            skControl1?.Invalidate();
            skControl2?.Invalidate();
            skControl3?.Invalidate();
            skControl4?.Invalidate();
        }

        private void SkControl_PaintSurface(SKPaintSurfaceEventArgs e, float R1, float R2, float d)
        {
            var canvas = e.Surface.Canvas;
            Program.DrawTwoCircles(canvas, R1, R2, d, true);
        }
    }
}
