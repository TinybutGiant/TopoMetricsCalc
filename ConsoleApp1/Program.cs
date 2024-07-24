using SkiaSharp;
using System;
using System.IO;
namespace ConsoleApp1;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    public static float CalculateOverlapArea(float RA_1, float RA_2, float RB_1, float RB_2, float d)
    {
        float overlapArea1 = CalculateTwoCirclesOverlap(RA_1, RB_1, d);
        float overlapArea2 = CalculateTwoCirclesOverlap(RA_1, RB_2, d);
        float overlapArea3 = CalculateTwoCirclesOverlap(RA_2, RB_1, d);
        float overlapArea4 = CalculateTwoCirclesOverlap(RA_2, RB_2, d);

        Console.WriteLine($"Overlap Area between RA_1 and RB_1: {overlapArea1}");
        Console.WriteLine($"Overlap Area between RA_1 and RB_2: {overlapArea2}");
        Console.WriteLine($"Overlap Area between RA_2 and RB_1: {overlapArea3}");
        Console.WriteLine($"Overlap Area between RA_2 and RB_2: {overlapArea4}");

        return overlapArea1 + overlapArea2 + overlapArea3 + overlapArea4;
    }

    public static float CalculateTwoCirclesOverlap(float R1, float R2, float d)
    {
        if (d >= R1 + R2) return 0;
        if (d <= Math.Abs(R1 - R2)) return (float)(Math.PI * Math.Min(R1, R2) * Math.Min(R1, R2));

        float r1Sq = R1 * R1;
        float r2Sq = R2 * R2;
        float dSq = d * d;

        float angle1 = 2 * (float)Math.Acos((dSq + r1Sq - r2Sq) / (2 * d * R1));
        float angle2 = 2 * (float)Math.Acos((dSq + r2Sq - r1Sq) / (2 * d * R2));

        float area1 = 0.5f * r1Sq * (angle1 - (float)Math.Sin(angle1));
        float area2 = 0.5f * r2Sq * (angle2 - (float)Math.Sin(angle2));

        return area1 + area2;
    }

    public static void DrawTwoCircles(SKCanvas canvas, float R1, float R2, float d, bool highlightOverlap = false)
    {
        canvas.Clear(SKColors.White);

        var paint = new SKPaint
        {
            Color = SKColors.Green,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };

        var highlightPaint = new SKPaint
        {
            Color = SKColors.LightBlue.WithAlpha(128),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        // 绘制第一个同心圆
        canvas.DrawCircle(100, 100, R1, paint);

        // 绘制第二个同心圆
        canvas.DrawCircle(100 + d, 100, R2, paint);

        // 绘制重叠区域
        if (highlightOverlap && d < R1 + R2 && d > Math.Abs(R1 - R2))
        {
            float r1Sq = R1 * R1;
            float r2Sq = R2 * R2;
            float dSq = d * d;

            float angle1 = 2 * (float)Math.Acos((dSq + r1Sq - r2Sq) / (2 * d * R1));
            float angle2 = 2 * (float)Math.Acos((dSq + r2Sq - r1Sq) / (2 * d * R2));

            var path = new SKPath();
            path.MoveTo(100, 100);
            path.ArcTo(new SKRect(100 - R1, 100 - R1, 100 + R1, 100 + R1), 0, angle1, false);
            path.LineTo(100 + d, 100);
            path.ArcTo(new SKRect(100 + d - R2, 100 - R2, 100 + d + R2, 100 + R2), 180, angle2, false);
            path.Close();

            canvas.DrawPath(path, highlightPaint);
        }
    }

    private static void DrawOverlap(SKCanvas canvas, float x1, float y1, float R1, float x2, float y2, float R2, SKPaint paint)
    {
        if (Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) < R1 + R2)
        {
            canvas.DrawCircle(x1, y1, R1, paint);
            canvas.DrawCircle(x2, y2, R2, paint);
        }
    }


    

}