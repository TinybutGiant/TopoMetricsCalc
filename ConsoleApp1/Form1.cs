using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace ConsoleApp1
{
    public partial class Form1 : Form
    {
        private TextBox[] reentrantTextBoxes = new TextBox[4]; // Reentrant input, eValue, lValue , tValue, thetaValue
        private TextBox[] raTextBoxes = new TextBox[4]; // 4 circles radius for A
        private TextBox[] rbTextBoxes = new TextBox[4]; // 4 circles radius for B
        private TextBox[] resultTextBoxes = new TextBox[16]; // 16 combination overlap
        private TextBox? dTextBox; // Distance between two centers
        private Button? calculateButton;
        private SKControl[] skControls = new SKControl[16]; // SKControls for drawing
        private Label[] resultLabels = new Label[16]; // Labels for names
        private float[] ra = new float[4];
        private float[] rb = new float[4];
        private float d;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // Center the form on the screen
            this.WindowState = FormWindowState.Maximized; // Maximize the window on startup
            InitializeCustomComponents();
            CalculateButton_Click(null, EventArgs.Empty); // Automatically perform the first calculation and drawing
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Circles Overlap";
            this.WindowState = FormWindowState.Maximized;

            // Create a TableLayoutPanel for scrolling content
            TableLayoutPanel tablePanel = CreateTablePanel();
            this.Controls.Add(tablePanel);

            // Create the first set of parameters - Reentrant input
            string[] reentrantLabels = { "e (horizontal distance):", "l (slant edge length):", "t (thickness):", "theta (angle):" };
            string[] defaultValues_1 = { "40", "10", "2", "0.959931" }; // theta = 55 degrees in radians

            AddLabelsAndTextBoxes(tablePanel, reentrantLabels, defaultValues_1, reentrantTextBoxes);

            // Add drawing panel for re-entrant structure
            AddDrawingPanel(tablePanel);

            // Create the second set of parameters - QTR circles input
            string[] raLabels = { "RA_1 (interior radius of A):", "RA_2 (interior boundary of A):", "RA_3 (boundary of A):", "RA_4 (exterior boundary of A):" };
            string[] rbLabels = { "RB_1 (interior radius of B):", "RB_2 (interior boundary of B):", "RB_3 (boundary of B):", "RB_4 (exterior boundary of B):" };
            string[] defaultValues_2 = { "15", "15.75", "17.75", "18.5" };

            AddLabelsAndTextBoxes(tablePanel, raLabels, defaultValues_2, raTextBoxes);
            AddLabelsAndTextBoxes(tablePanel, rbLabels, defaultValues_2, rbTextBoxes);

            // Calculate distance between centers d
            dTextBox = CreateTextBox("0");
            Label dLabel = CreateLabel("d (distance between centers):");
            tablePanel.Controls.Add(dLabel);
            tablePanel.Controls.Add(dTextBox);

            // Add calculate button
            AddCalculateButton(tablePanel);

            // Create result panels
            CreateResultPanels(tablePanel);
        }

        private TableLayoutPanel CreateTablePanel()
        {
            return new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
        }

        private void AddDrawingPanel(TableLayoutPanel tablePanel)
        {
            Panel drawingPanel = new Panel
            {
                Width = 500,
                Height = 500,
                BorderStyle = BorderStyle.FixedSingle
            };
            drawingPanel.Paint += DrawingPanel_Paint;
            tablePanel.Controls.Add(drawingPanel);
            tablePanel.SetColumnSpan(drawingPanel, 2);
        }

        private void AddCalculateButton(TableLayoutPanel tablePanel)
        {
            calculateButton = new Button { Text = "Update Calculate", AutoSize = true };
            calculateButton.Click += CalculateButton_Click;
            tablePanel.Controls.Add(calculateButton);
            tablePanel.SetColumnSpan(calculateButton, 2);
        }

        private void AddLabelsAndTextBoxes(TableLayoutPanel panel, string[] labels, string[] defaultValues, TextBox[] textBoxes)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                Label label = CreateLabel(labels[i]);
                TextBox textBox = CreateTextBox(defaultValues[i]);
                textBoxes[i] = textBox;
                panel.Controls.Add(label);
                panel.Controls.Add(textBox);
            }
        }

        private Label CreateLabel(string text)
        {
            return new Label { Text = text, AutoSize = true };
        }

        private TextBox CreateTextBox(string defaultText)
        {
            return new TextBox { Width = 100, Text = defaultText };
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

                resultLabels[i] = CreateLabel($"Overlap Area (RA_{row + 1} & RB_{col + 1}):");
                resultTextBoxes[i] = CreateTextBox("");
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
            float scale = 10.0f; // Scaling factor

            // Get parameters from reentrantTextBoxes
            double eValue = Convert.ToDouble(reentrantTextBoxes[0].Text);
            double lValue = Convert.ToDouble(reentrantTextBoxes[1].Text);
            double tValue = Convert.ToDouble(reentrantTextBoxes[2].Text);
            double thetaValue = Convert.ToDouble(reentrantTextBoxes[3].Text);

            float eFloat = (float)eValue * scale;
            float l = (float)lValue * scale;
            float t = (float)tValue * scale;
            float theta = (float)thetaValue;

            float x0 = 50, y0 = 250;

            // Calculate coordinates
            PointF[] points = CalculateReentrantPoints(x0, y0, eFloat, l, theta);

            // Draw re-entrant structure polygon ABCDEF
            g.DrawPolygon(pen, points);

            // Draw additional lines GF and CH
            DrawAdditionalLines(g, points, pen);

            // Label vertices
            LabelVertices(g, points);

            // Calculate distance between points A and C
            double distanceAC = CalculateDistance(points[0], points[2], scale);

            dTextBox.Text = distanceAC.ToString(); // Update dTextBox
        }

        private PointF[] CalculateReentrantPoints(float x0, float y0, float eFloat, float l, float theta)
        {
            float ax = x0, ay = y0;
            float bx = ax + eFloat, by = ay;
            float cx = bx - (float)(l * Math.Sin(theta)), cy = by - (float)(l * Math.Cos(theta));
            float dx = bx, dy = by - (float)(2 * l * Math.Cos(theta));
            float ex = ax, ey = dy;
            float fx = ax + (float)(l * Math.Sin(theta)), fy = cy;
            float gx = fx - l, gy = fy;
            float hx = cx + l, hy = gy;

            return new PointF[] { new PointF(ax, ay), new PointF(bx, by), new PointF(cx, cy), new PointF(dx, dy), new PointF(ex, ey), new PointF(fx, fy) };
        }

        private void DrawAdditionalLines(Graphics g, PointF[] points, Pen pen)
        {
            g.DrawLine(pen, points[5].X - 10, points[5].Y, points[5].X - 10, points[5].Y - 10); // Draw line GF
            g.DrawLine(pen, points[2].X + 10, points[2].Y, points[2].X + 10, points[2].Y + 10); // Draw line CH
        }

        private void LabelVertices(Graphics g, PointF[] points)
        {
            string[] labels = { "A", "B", "C", "D", "E", "F", "G", "H" };
            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;

            for (int i = 0; i < points.Length; i++)
            {
                g.DrawString(labels[i], font, brush, points[i].X - 10, points[i].Y - 10);
            }
        }

        private double CalculateDistance(PointF pointA, PointF pointC, float scale)
        {
            float deltaX = pointC.X - pointA.X;
            float deltaY = pointC.Y - pointA.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY) / scale;
        }

        private void CalculateButton_Click(object? sender, EventArgs e)
        {
            // Get parameters from reentrantTextBoxes
            double eValue = Convert.ToDouble(reentrantTextBoxes[0].Text);
            double lValue = Convert.ToDouble(reentrantTextBoxes[1].Text);
            double thetaValue = Convert.ToDouble(reentrantTextBoxes[3].Text);

            float scale = 10.0f; // Scaling factor
            float eFloat = (float)eValue * scale;
            float l = (float)lValue * scale;
            float theta = (float)thetaValue;

            float x0 = 50, y0 = 250;

            // Calculate coordinates
            float ax = x0, ay = y0;
            float cx = x0 + eFloat - (float)(l * Math.Sin(theta)), cy = y0 - (float)(l * Math.Cos(theta));

            // Calculate distance between points A and C
            d = (float)CalculateDistance(new PointF(ax, ay), new PointF(cx, cy), scale);

            dTextBox.Text = d.ToString(); // Update dTextBox

            // Read all radius values
            for (int i = 0; i < 4; i++)
            {
                ra[i] = float.Parse(raTextBoxes[i].Text);
                rb[i] = float.Parse(rbTextBoxes[i].Text);
            }
            d = float.Parse(dTextBox?.Text ?? "0");

            // Calculate and display all overlap areas
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
            canvas.Clear(SKColors.White); // Clear the canvas

            // Get SKSurface physical dimensions and scale
            float scale = Math.Min(e.Info.Width / 200f, e.Info.Height / 200f); // Adjust scale based on control size
            float centerX = e.Info.Width / 2;
            float centerY = e.Info.Height / 2;

            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Green,
                StrokeWidth = 1
            })
            {
                // Adjust circle sizes and positions
                float adjustedR1 = R1 * scale;
                float adjustedR2 = R2 * scale;
                float adjustedD = d * scale;

                // Circle A position
                float ax = centerX - adjustedD / 2;
                float ay = centerY;

                // Circle B position
                float bx = centerX + adjustedD / 2;
                float by = centerY;

                // Draw circle A
                canvas.DrawCircle(ax, ay, adjustedR1, paint);
                // Draw circle B
                canvas.DrawCircle(bx, by, adjustedR2, paint);
            }
        }
    }
}
