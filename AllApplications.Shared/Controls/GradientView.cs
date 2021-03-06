﻿//using SkiaSharp;
//using SkiaSharp.Views.Forms;
//using Xamarin.Forms;

//namespace Core.Controls
//{
//    public partial class GradientView : ContentView
//    {
//        public GradientView()
//        {
//            SKCanvasView canvasView = new SKCanvasView();
//            canvasView.PaintSurface += OnCanvasViewPaintSurface;
//            Content = canvasView;
//        }

//        public Color EndColor { get; set; } = Color.Transparent;
//        public bool Horizontal { get; set; } = false;
//        public Color StartColor { get; set; } = Color.Transparent;

//        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
//        {
//            SKImageInfo info = args.Info;
//            SKSurface surface = args.Surface;
//            SKCanvas canvas = surface.Canvas;

//            canvas.Clear();

//            var colors = new SKColor[] { StartColor.ToSKColor(), EndColor.ToSKColor() };
//            SKPoint startPoint = new SKPoint(0, 0);
//            SKPoint endPoint = Horizontal ? new SKPoint(info.Width, 0) : new SKPoint(0, info.Height);

//            var shader = SKShader.CreateLinearGradient(startPoint, endPoint, colors, null, SKShaderTileMode.Clamp);

//            SKPaint paint = new SKPaint
//            {
//                Style = SKPaintStyle.Fill,
//                Shader = shader
//            };

//            canvas.DrawRect(new SKRect(0, 0, info.Width, info.Height), paint);
//        }
//    }
//}