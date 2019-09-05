using System;
using System.Drawing;
using Exile.RenderQ;
using Shared.Helpers;
using ImGuiNET;
using Shared.Enums;
using SharpDX;
using Color = SharpDX.Color;
using Rectangle = System.Drawing.Rectangle;
using RectangleF = SharpDX.RectangleF;
using Vector2N = System.Numerics.Vector2;

namespace Exile
{
    public class Graphics
    {
        private readonly DX11 _dx11;
        private readonly CoreSettings _settings;
        private static RectangleF DefaultUV = new RectangleF(0, 0, 1, 1);
        private ImGuiRender ImGuiRender;
        private SpritesRender SpritesRender;

        public DX11 LowLevel => _dx11;

        public Graphics(DX11 dx11, CoreSettings settings) {
            _dx11 = dx11;
            _settings = settings;
            ImGuiRender = dx11.ImGuiRender;
            SpritesRender = dx11.SpritesRender;
        }

        public bool TransparentState => ImGuiRender.TransparentState;

        public System.Numerics.Vector2 DrawText(string text, Vector2 position, Color color, string fontName = null,
                                                FontAlign align = FontAlign.Left) =>
            ImGuiRender.DrawText(text, position.ToVector2Num(), color, -1, fontName, align);


        public FontContainer Font => ImGuiRender.fonts[_settings.Font];
        public FontContainer LastFont => ImGuiRender.CurrentFont;

        public System.Numerics.Vector2 DrawText(string text, Vector2N position, Color color, string fontName = null,
                                                FontAlign align = FontAlign.Left) =>
            ImGuiRender.DrawText(text, position, color, -1, fontName, align);

        public Vector2N DrawText(string text, Vector2N position, Color color, int height, FontAlign align = FontAlign.Left) =>
            ImGuiRender.DrawText(text, position, color, height, _settings.Font, align);

        public System.Numerics.Vector2 DrawText(string text, Vector2N position, Color color, int height, string fontName,
                                                FontAlign align = FontAlign.Left) =>
            ImGuiRender.DrawText(text, position, color, height, fontName, align);

        public Vector2N DrawText(string text, Vector2 position, Color color) =>
            DrawText(text, position.ToVector2Num(), color, _settings.FontSize, _settings.Font);

        public Vector2N DrawText(string text, Vector2 position, Color color, FontAlign align = FontAlign.Left) =>
            DrawText(text, position.ToVector2Num(), color, _settings.FontSize, _settings.Font, align);

        public Vector2N DrawText(string text, Vector2N position, Color color) =>
            DrawText(text, position, color, _settings.FontSize, _settings.Font);

        public Vector2N DrawText(string text, Vector2N position) => DrawText(text, position, Color.White);

        public Vector2N DrawText(string text, Vector2N position, FontAlign align = FontAlign.Left) =>
            DrawText(text, position, Color.White, _settings.FontSize, align);

        public Vector2N DrawText(string text, Vector2 position, FontAlign align = FontAlign.Left) =>
            DrawText(text, position.ToVector2Num(), Color.White, _settings.FontSize, align);

        public Vector2N MeasureText(string text) => ImGuiRender.MeasureText(text);

        public Vector2N MeasureText(string text, int height) => ImGuiRender.MeasureText(text, height);

        public void DrawLine(Vector2N p1, Vector2N p2, float borderWidth, Color color) =>
            ImGuiRender.LowLevelApi.AddLine(p1, p2, color.ToImgui(), borderWidth);

        public void DrawLine(Vector2 p1, Vector2 p2, float borderWidth, Color color) =>
            ImGuiRender.LowLevelApi.AddLine(p1.ToVector2Num(), p2.ToVector2Num(), color.ToImgui(), borderWidth);

        public void DrawFrame(Vector2N p1, Vector2N p2, Color color, float rounding, int thickness, int flags) =>
            ImGuiRender.LowLevelApi.AddRect(p1, p2, color.ToImgui(), rounding, flags, thickness);

        public void DrawBox(Vector2N p1, Vector2N p2, Color color, float rounding = 0) =>
            ImGuiRender.LowLevelApi.AddRectFilled(p1, p2, color.ToImgui(), rounding);

        public void DrawImage(string fileName, RectangleF rectangle) => DrawImage(fileName, rectangle, DefaultUV, Color.White);

        public void DrawImage(string fileName, RectangleF rectangle, Color color) => DrawImage(fileName, rectangle, DefaultUV, color);

        public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv, Color color) =>
            SpritesRender.DrawImage(fileName, rectangle, uv, color);

        public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv) => DrawImage(fileName, rectangle, uv, Color.White);

        public void DrawImageGui(string fileName, RectangleF rectangle, RectangleF uv) => ImGuiRender.DrawImage(fileName, rectangle, uv);

        public void DrawImageGui(string fileName, Vector2N TopLeft, Vector2N BottomRight, Vector2N TopLeft_UV, Vector2N BottomRight_UV) =>
            ImGuiRender.DrawImage(fileName, TopLeft, BottomRight, TopLeft_UV, BottomRight_UV);

        public void DrawBox(RectangleF rect, Color color) => DrawBox(rect, color, 0);


        public void DrawBox(RectangleF rect, Color color, float rounding) =>
            DrawBox(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, rounding);

        public void DrawFrame(RectangleF rect, Color color, float rounding, int thickness, int flags) =>
            DrawFrame(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, rounding, thickness, flags);

        public void DrawFrame(RectangleF rect, Color color, int thickness) =>
            DrawFrame(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, 0, thickness, 0);

        public void InitImage(string name, bool textures = true) => SpritesRender.LoadPng(textures ? $"textures/{name}" : name);

        public void DisposeTexture(string name) => _dx11.DisposeTexture(name);
    }
}