using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Rectangle = System.Drawing.Rectangle;
using Vector2N = System.Numerics.Vector2;

namespace ExileCore.RenderQ
{
    public class ImGuiRender
    {
        public delegate void UserCallbackDel(ImDrawListPtr list, ImDrawCmdPtr cmd);

        public static readonly ImGuiWindowFlags InvisibleWindow =
            ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings |
            ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground;

        private readonly RenderForm _form;
        public readonly int sizeOfImDrawIdx = 2;
        public readonly int sizeOfImDrawVert = 20;
        private ImDrawListPtr _backGroundTextWindowPtr;
        private ImDrawListPtr _backGroundWindowPtr;
        private int _indexBufferSize;
        private bool _transparentState1 = true;
        private int _vertexBufferSize;
        private BlendState blendState;
        private DepthStencilState depthStencilState;
        private readonly string LastFontName = "";
        private Matrix4x4 mvp2;
        private RawColor4 outputMergerBlendFactor;
        private SamplerState samplerState;
        private RasterizerState SolidState;
        private VertexBufferBinding vertexBuffer;
        private Viewport Viewport;

        public ImGuiRender(DX11 dx11, RenderForm form, CoreSettings coreSettings)
        {
            _form = form;
            Dx11 = dx11;
            CoreSettings = coreSettings;
            FormBounds = form.Bounds;

            using (new PerformanceTimer("Init ImGui"))
            {
                Initialize();
            }

            sizeOfImDrawVert = Utilities.SizeOf<ImDrawVert>();
            sizeOfImDrawIdx = Utilities.SizeOf<ushort>();
            VertexBufferSize = 10000;
            IndexBufferSize = 30000;

            // Compile the vertex shader code.
            var vertexShaderByteCode =
                ShaderBytecode.CompileFromFile("Shaders\\ImGuiVertexShader.hlsl", "VS", "vs_4_0");

            // Compile the pixel shader code.
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Shaders\\ImGuiPixelShader.hlsl", "PS", "ps_4_0");
            VertexShader = new VertexShader(Dx11.D11Device, vertexShaderByteCode);
            PixelShader = new PixelShader(Dx11.D11Device, pixelShaderByteCode);

            VertexBuffer = new Buffer(Dx11.D11Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.VertexBuffer,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = VertexBufferSizeBytes
                });

            IndexBuffer = new Buffer(Dx11.D11Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.IndexBuffer,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = IndexBufferSizeBytes
                });

            ConstantBuffer = new Buffer(Dx11.D11Device,
                new BufferDescription
                {
                    BindFlags = BindFlags.ConstantBuffer,
                    Usage = ResourceUsage.Dynamic,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = Utilities.SizeOf<Matrix4x4>()
                });

            var inputElements = new[]
            {
                new InputElement
                {
                    SemanticName = "POSITION",
                    SemanticIndex = 0,
                    Format = Format.R32G32_Float,
                    Slot = 0,
                    AlignedByteOffset = 0,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                },
                new InputElement
                {
                    SemanticName = "TEXCOORD",
                    SemanticIndex = 0,
                    Format = Format.R32G32_Float,
                    Slot = 0,
                    AlignedByteOffset = InputElement.AppendAligned,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                },
                new InputElement
                {
                    SemanticName = "COLOR",
                    SemanticIndex = 0,
                    Format = Format.R8G8B8A8_UNorm,
                    Slot = 0,
                    AlignedByteOffset = InputElement.AppendAligned,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            Layout = new InputLayout(Dx11.D11Device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
                inputElements);

            CreateStates();
            UpdateConstantBuffer();
            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
        }

        public DX11 Dx11 { get; set; }
        public CoreSettings CoreSettings { get; }
        public Rectangle FormBounds { get; set; }
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        private Buffer ConstantBuffer { get; }
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public InputLayout Layout { get; set; }
        public ImGuiIOPtr io { get; private set; }
        public ShaderResourceView FontTexture { get; private set; }

        private int VertexBufferSize
        {
            get => _vertexBufferSize;
            set
            {
                VertexBufferSizeBytes = value * sizeOfImDrawVert;
                _vertexBufferSize = value;
            }
        }

        public int VertexBufferSizeBytes { get; set; }

        private int IndexBufferSize
        {
            get => _indexBufferSize;
            set
            {
                IndexBufferSizeBytes = value * sizeOfImDrawIdx;
                _indexBufferSize = value;
            }
        }

        public int IndexBufferSizeBytes { get; set; }

        public bool TransparentState
        {
            get => _transparentState1;
            set
            {
                _transparentState1 = value;

                if (value)
                    WinApi.SetTransparent(_form.Handle);
                else
                    WinApi.SetNoTransparent(_form.Handle);
            }
        }

        public Dictionary<string, FontContainer> fonts { get; } = new Dictionary<string, FontContainer>();
        public ImDrawListPtr LowLevelApi => _backGroundWindowPtr;
        private FontContainer lastFontContainer { get; set; }
        public FontContainer CurrentFont => lastFontContainer;

        private void InitializeInputSystem()
        {
            // _form.MouseMove += (sender, args) => { io.MousePos = new Vector2Num(args.X, args.Y); };
            _form.MouseDown += (sender, args) =>
            {
                io.MousePos = Input.MousePositionNum;

                switch (args.Button)
                {
                    case MouseButtons.Left:
                        io.MouseDown[0] = true;
                        break;
                    case MouseButtons.None:
                        break;
                    case MouseButtons.Right:
                        io.MouseDown[1] = true;
                        break;
                    case MouseButtons.Middle:
                        io.MouseDown[2] = true;
                        break;
                    case MouseButtons.XButton1:
                        io.MouseDown[3] = true;
                        break;
                    case MouseButtons.XButton2:
                        io.MouseDown[4] = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            _form.MouseUp += (sender, args) =>
            {
                io.MousePos = Input.MousePositionNum;

                switch (args.Button)
                {
                    case MouseButtons.Left:
                        io.MouseDown[0] = false;
                        break;
                    case MouseButtons.None:
                        break;
                    case MouseButtons.Right:
                        io.MouseDown[1] = false;
                        break;
                    case MouseButtons.Middle:
                        io.MouseDown[2] = false;
                        break;
                    case MouseButtons.XButton1:
                        io.MouseDown[3] = false;
                        break;
                    case MouseButtons.XButton2:
                        io.MouseDown[4] = false;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            io.KeyMap[(int) ImGuiKey.Tab] = (int) Keys.Tab;
            io.KeyMap[(int) ImGuiKey.LeftArrow] = (int) Keys.Left;
            io.KeyMap[(int) ImGuiKey.RightArrow] = (int) Keys.Right;
            io.KeyMap[(int) ImGuiKey.UpArrow] = (int) Keys.Up;
            io.KeyMap[(int) ImGuiKey.DownArrow] = (int) Keys.Down;
            io.KeyMap[(int) ImGuiKey.PageUp] = (int) Keys.PageUp;
            io.KeyMap[(int) ImGuiKey.PageDown] = (int) Keys.PageDown;
            io.KeyMap[(int) ImGuiKey.Home] = (int) Keys.Home;
            io.KeyMap[(int) ImGuiKey.End] = (int) Keys.End;
            io.KeyMap[(int) ImGuiKey.Delete] = (int) Keys.Delete;
            io.KeyMap[(int) ImGuiKey.Backspace] = (int) Keys.Back;
            io.KeyMap[(int) ImGuiKey.Enter] = (int) Keys.Enter;
            io.KeyMap[(int) ImGuiKey.Escape] = (int) Keys.Escape;
            io.KeyMap[(int) ImGuiKey.A] = (int) Keys.A;
            io.KeyMap[(int) ImGuiKey.C] = (int) Keys.C;
            io.KeyMap[(int) ImGuiKey.V] = (int) Keys.V;
            io.KeyMap[(int) ImGuiKey.X] = (int) Keys.X;
            io.KeyMap[(int) ImGuiKey.Y] = (int) Keys.Y;
            io.KeyMap[(int) ImGuiKey.Z] = (int) Keys.Z;

            _form.KeyDown += (sender, args) =>
            {
                io.KeyAlt = args.Alt;
                io.KeyShift = args.Shift;
                io.KeyCtrl = args.Control;
                io.KeysDown[args.KeyValue] = true;
            };

            _form.KeyPress += (sender, args) => { io.AddInputCharacter(args.KeyChar); };

            _form.MouseWheel += (sender, args) =>
            {
                var scrollDelta = args.Delta;
                io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
            };

            _form.KeyUp += (sender, args) =>
            {
                io.KeyAlt = args.Alt;
                io.KeyShift = args.Shift;
                io.KeyCtrl = args.Control;
                io.KeysDown[args.KeyValue] = false;
            };
        }

        public event EventHandler GetFocus;
        public event EventHandler LostFocus;

        public void InputUpdate()
        {
            io.MousePos = Input.MousePositionNum;
            var ioWantCaptureMouse = io.WantCaptureMouse;
            var ioWantCaptureKeyboard = io.WantCaptureKeyboard;

            if ((ioWantCaptureMouse || ioWantCaptureKeyboard) && TransparentState)
            {
                GetFocus?.Invoke(this, EventArgs.Empty);
                TransparentState = false;
            }
            else if (!ioWantCaptureMouse && !ioWantCaptureKeyboard && !TransparentState)
            {
                LostFocus?.Invoke(this, EventArgs.Empty);
                io.KeyAlt = Input.GetKeyState(Keys.Alt);
                io.KeyShift = Input.GetKeyState(Keys.Shift);
                io.KeyCtrl = Input.GetKeyState(Keys.Control);
                TransparentState = true;
            }
        }

        private void Initialize()
        {
            try
            {
                var context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);
                io = ImGui.GetIO();

                // io.ConfigFlags = ImGuiConfigFlags.NavEnableKeyboard;
                SetSize(FormBounds);

                using (new PerformanceTimer("Load manual fonts"))
                {
                    try
                    {
                        SetManualFont();
                    }
                    catch (Exception e)
                    {
                        Core.Logger.Error($"Cant load fonts -> {e}");
                    }
                }

                using (new PerformanceTimer("Build Font Texture"))
                {
                    FontTexture = BuildFontTexture();
                }

                SetTexture(FontTexture);
                Dx11.AddOrUpdateTexture("ImGui Font", FontTexture);
                InitializeInputSystem();
            }
            catch (DllNotFoundException ex)
            {
                throw new DllNotFoundException("Need put in directory cimgui.dll");
            }
        }

        private unsafe void SetManualFont()
        {
            var folder = "fonts";
            var files = Directory.GetFiles(folder);

            if (!(Directory.Exists(folder) && files.Length > 0))
                return;

            var fontsForLoad = new List<(string, int)>();

            if (files.Contains($"{folder}\\config.ini"))
            {
                var lines = File.ReadAllLines($"{folder}\\config.ini");

                foreach (var line in lines)
                {
                    var split = line.Split(':');
                    fontsForLoad.Add(($"{folder}\\{split[0]}.ttf", int.Parse(split[1])));
                }
            }

            var imFontAtlasGetGlyphRangesCyrillic = ImGuiNative.ImFontAtlas_GetGlyphRangesCyrillic(io.Fonts.NativePtr);

            fonts["Default:13"] = new FontContainer(ImGuiNative.ImFontAtlas_AddFontDefault(io.Fonts.NativePtr, null),
                "Default", 13);

            foreach (var tuple in fontsForLoad)
            {
                var bytes = Encoding.UTF8.GetBytes(tuple.Item1);

                fixed (byte* f = &bytes[0])
                {
                    fonts[$"{tuple.Item1.Replace(".ttf", "").Replace("fonts\\", "")}:{tuple.Item2}"] =
                        new FontContainer(
                            ImGuiNative.ImFontAtlas_AddFontFromFileTTF(io.Fonts.NativePtr, f, tuple.Item2, null,
                                imFontAtlasGetGlyphRangesCyrillic), tuple.Item1, tuple.Item2);
                }
            }

            if (fonts.Count > 0) lastFontContainer = fonts.First().Value;

            CoreSettings.Font.Values = new List<string>(fonts.Keys);
        }

        private void SetTexture(ShaderResourceView fontTexture)
        {
            io.Fonts.SetTexID(fontTexture.NativePointer);
            io.Fonts.ClearTexData();
        }

        private void SetSize(Rectangle formBounds)
        {
            io.DisplaySize = new Vector2N(Dx11.BackBuffer.Description.Width, Dx11.BackBuffer.Description.Height);
        }

        public void UpdateConstantBuffer()
        {
            mvp2 = Matrix4x4.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);
            Dx11.DeviceContext.MapSubresource(ConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out var buffer);
            buffer.Write(mvp2);
            Dx11.DeviceContext.UnmapSubresource(ConstantBuffer, 0);
        }

        private unsafe ShaderResourceView BuildFontTexture()
        {
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);
            var rect = new DataRectangle(new IntPtr(pixelData), width * bytesPerPixel);

            var tex2D = new Texture2D(Dx11.D11Device,
                new Texture2DDescription
                {
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R8G8B8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                }, rect);

            var shaderResourceView = new ShaderResourceView(Dx11.D11Device, tex2D);
            tex2D.Dispose();
            return shaderResourceView;
        }

        public void BeginBackGroundWindow()
        {
            //Overlay 
            //BackGroundWindowPtr = ImGui.GetOverlayDrawList(); 
            ImGui.SetNextWindowContentSize(io.DisplaySize);
            ImGui.SetNextWindowPos(new Vector2N(0, 0));

            ImGui.Begin("Background Window",
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoBackground);

            _backGroundWindowPtr = ImGui.GetWindowDrawList();
            ImGui.End();

            ImGui.SetNextWindowContentSize(io.DisplaySize);
            ImGui.SetNextWindowPos(new Vector2N(0, 0));

            ImGui.Begin("Background Text Window",
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBackground);

            _backGroundTextWindowPtr = ImGui.GetWindowDrawList();
            ImGui.End();
        }

        public Vector2N MeasureText(string text)
        {
            return ImGui.CalcTextSize(text);
        }

        public Vector2N MeasureText(string text, int height)
        {
            return ImGui.CalcTextSize(text);
        }

        public unsafe Vector2N DrawText(string text, Vector2N position, Color color, int height, string fontName,
            FontAlign align)
        {
            try
            {
                var b = LastFontName != fontName;
                FontContainer fontContainer;

                if (fontName == null)
                {
                    if (fonts.TryGetValue(CoreSettings.Font.Value, out var fontN))
                    {
                        fontContainer = fontN;
                        fontName = CoreSettings.Font.Value;
                        lastFontContainer = fontContainer;
                    }
                    else
                    {
                        var keyValuePair = fonts.First();
                        fontContainer = keyValuePair.Value;
                        fontName = keyValuePair.Key;
                        lastFontContainer = fontContainer;
                    }
                }
                else
                {
                    fontName = $"{fontName}";

                    if (!fonts.TryGetValue(fontName, out fontContainer))
                    {
                        fontContainer = fonts.First().Value;

                        DebugWindow.LogError(
                            $"Font: {fontName} not found. Using: {fontContainer.Name}:{fontContainer.Size}");
                    }

                    lastFontContainer = fontContainer;
                }

                if (b) ImGui.PushFont(lastFontContainer.Atlas);

                if (height == -1) height = lastFontContainer.Size;
                var size = MeasureText(text, height);

                switch (align)
                {
                    case FontAlign.Left:
                        break;
                    case FontAlign.Center:
                        position.X -= size.X / 2;
                        break;
                    case FontAlign.Right:
                        position.X -= size.X;
                        break;
                }

                _backGroundTextWindowPtr.AddText(lastFontContainer.Atlas, lastFontContainer.Size, position,
                    color.ToImgui(), text);

                if (b && fontName != null) ImGui.PopFont();

                return size;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv)
        {
            _backGroundTextWindowPtr.AddImage(Dx11.GetTexture(fileName).NativePointer, rectangle.TopLeft.ToVector2Num(),
                rectangle.BottomRight.ToVector2Num(), uv.TopLeft.ToVector2Num(),
                uv.BottomRight.ToVector2Num());
        }

        public void DrawImage(string filename, Vector2N TopLeft, Vector2N BottomRight, Vector2N TopLeft_UV,
            Vector2N BottomRight_UV)
        {
            _backGroundTextWindowPtr.AddImage(Dx11.GetTexture(filename).NativePointer, TopLeft, BottomRight, TopLeft_UV,
                BottomRight_UV);
        }

        [Description("Count Colors means how many colors used in text, if you use a lot colors need put number more than colors you have." +
                     "This used for optimization.")]
        public Vector2N DrawMultiColoredText(string text, Vector2N position, FontAlign align = FontAlign.Left,
            int countColors = 10)
        {
            var readOnlySpan = text.AsSpan();
            var lastIndex = 0;
            Span<uint> colors = stackalloc uint[countColors];
            Span<int> coloredText = stackalloc int[countColors * 2 + 1];
            var indexColoredText = 0;
            var indexColors = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '#' && i + 10 < text.Length && text[i + 9] == '#')
                {
                    colors[indexColors++] = readOnlySpan.Slice(i + 1, 8).HexToUInt();

                    if (i != 0 && lastIndex == 0)
                    {
                        coloredText[indexColoredText++] = lastIndex;
                        coloredText[indexColoredText++] = i;
                        lastIndex = i + 10;
                        coloredText[indexColoredText++] = lastIndex;
                        i += 9;
                    }
                    else
                    {
                        if (i != 0) coloredText[indexColoredText++] = i - lastIndex;

                        lastIndex = i + 10;
                        i += 9;
                        coloredText[indexColoredText++] = lastIndex;
                    }
                }
            }

            coloredText[indexColoredText++] = text.Length - lastIndex;
            var listIndexSpanCount = (int) Math.Ceiling(indexColoredText / (float) indexColors);

            unsafe
            {
                ImGui.PushFont(lastFontContainer.Atlas);
            }

            if (align == FontAlign.Center)
                position.X -= MeasureText(text, lastFontContainer.Size).X / 4f;

            var xStart = position.X;

            if (listIndexSpanCount == 2)
            {
                var colorIndex = 0;

                for (var index = 0; index < indexColoredText; index += 2)
                {
                    var i = coloredText[index];
                    var i2 = coloredText[index + 1];
                    var clr = colors[colorIndex++];
                    DrawClrText2(ref readOnlySpan, ref position, xStart, align, i, i2, clr, index, indexColoredText);
                }
            }
            else if (listIndexSpanCount >= 3)
            {
                var colorIndex = 0;

                for (var index = 0; index < indexColoredText; index += 2)
                {
                    var i = coloredText[index];
                    var i2 = coloredText[index + 1];

                    if (index == 0)
                    {
                        DrawClrText2(ref readOnlySpan, ref position, xStart, align, i, i2, Color.White.ToImgui(), index,
                            indexColoredText, true);
                    }
                    else
                    {
                        var clr = colors[colorIndex++];

                        DrawClrText2(ref readOnlySpan, ref position, xStart, align, i, i2, clr, index,
                            indexColoredText);
                    }
                }
            }
            else
                throw new Exception("Something wrong");

            ImGui.PopFont();
            return position;
        }

        private unsafe Vector2N DrawClrText2(ref ReadOnlySpan<char> span, ref Vector2N position, float xStart, FontAlign align,
            int start, int len,
            uint clr, int index, int spanIndex, bool noColor = false)
        {
            var onlySpan = span.Slice(start, len);
            var textBegin = onlySpan.ToString();
            var size = MeasureText(textBegin, lastFontContainer.Size);

            switch (align)
            {
                case FontAlign.Left:
                    _backGroundWindowPtr.AddText(lastFontContainer.Atlas, lastFontContainer.Size, position, clr,
                        textBegin);

                    position.X += size.X;
                    break;
                case FontAlign.Center:
                    _backGroundWindowPtr.AddText(lastFontContainer.Atlas, lastFontContainer.Size, position, clr,
                        textBegin);

                    position.X += size.X;
                    break;
                case FontAlign.Right:
                    position.X -= size.X;

                    _backGroundWindowPtr.AddText(lastFontContainer.Atlas, lastFontContainer.Size, position, clr,
                        textBegin);

                    break;
            }

            if (textBegin[len - 1] == '\n')
            {
                position.X = xStart;
                position.Y += size.Y;
            }

            return size;
        }

        [Description(
            "Count Colors means how many colors used in text, if you use a lot colors need put number more than colors you have." +
            "This used for optimization.")]
        public void MultiColoredText(string text, int countColors = 10)
        {
            var readOnlySpan = text.AsSpan();
            var lastIndex = 0;
            Span<uint> colors = stackalloc uint[countColors];
            Span<int> coloredText = stackalloc int[countColors * 2 + 1];
            var indexColoredText = 0;
            var indexColors = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '#' && i + 10 < text.Length && text[i + 9] == '#')
                {
                    colors[indexColors++] = readOnlySpan.Slice(i + 1, 8).HexToUInt();

                    if (i != 0 && lastIndex == 0)
                    {
                        coloredText[indexColoredText++] = lastIndex;
                        coloredText[indexColoredText++] = i;
                        lastIndex = i + 10;
                        coloredText[indexColoredText++] = lastIndex;
                        i += 9;
                    }
                    else
                    {
                        if (i != 0) coloredText[indexColoredText++] = i - lastIndex;

                        lastIndex = i + 10;
                        i += 9;
                        coloredText[indexColoredText++] = lastIndex;
                    }
                }
            }

            coloredText[indexColoredText++] = text.Length - lastIndex;
            var listIndexSpanCount = (int) Math.Ceiling(indexColoredText / (float) indexColors);

            if (listIndexSpanCount == 2)
            {
                var colorIndex = 0;

                for (var index = 0; index < indexColoredText; index += 2)
                {
                    var i = coloredText[index];
                    var i2 = coloredText[index + 1];
                    var clr = colors[colorIndex++];
                    DrawClrText(ref readOnlySpan, i, i2, Color.FromRgba(clr), index, indexColoredText);
                }
            }
            else if (listIndexSpanCount == 3)
            {
                var colorIndex = 0;

                for (var index = 0; index < indexColoredText; index += 2)
                {
                    var i = coloredText[index];
                    var i2 = coloredText[index + 1];

                    if (index == 0)
                        DrawClrText(ref readOnlySpan, i, i2, Color.Transparent, index, indexColoredText, true);
                    else
                    {
                        var clr = colors[colorIndex++];
                        DrawClrText(ref readOnlySpan, i, i2, Color.FromRgba(clr), index, indexColoredText);
                    }
                }
            }
            else
                throw new Exception("Something wrong");
        }

        private unsafe void DrawClrText(ref ReadOnlySpan<char> span, int start, int len, Color clr, int index, int spanIndex,
            bool noColor = false)
        {
            var onlySpan = span.Slice(start, len);

            fixed (char* ch = onlySpan)
            {
                var b = stackalloc byte[onlySpan.Length];
                Encoding.UTF8.GetBytes(ch, onlySpan.Length, b, onlySpan.Length);

                if (noColor)
                    ImGuiNative.igText(b);
                else
                    ImGuiNative.igTextColored(clr.ToImguiVec4(), b);

                if (index + 2 < spanIndex)
                {
                    if (ch[len - 1] == '\n')
                        return;

                    ImGuiNative.igSameLine(0, 0);
                }
            }
        }

        public unsafe void TestDraws()
        {
            var i = 0;

            foreach (var imFontPtr in fonts)
            {
                ImGui.PushFont(imFontPtr.Value.Atlas);
                var fontPtr = ImGui.GetFont();

                var textBegin =
                    $"{fontPtr.Ascent} {imFontPtr.Key.Split('\\').Last()} =>1234567890-qwertyuiop[asdfghjklzxcvbnm,йцукенгшщзхъфывапролджэячсмитьбю. ";

                var sw = Stopwatch.StartNew();
                var calcTextSize = ImGui.CalcTextSize(textBegin);
                var w = sw.ElapsedTicks;

                _backGroundWindowPtr.AddText(new Vector2N(50, 15 + i), Color.White.ToImgui(),
                    $"{textBegin} -> {calcTextSize} ({w} )");

                i += (int) calcTextSize.Y;
                ImGui.PopFont();
            }

            _backGroundWindowPtr.AddText(fonts.First(pair => true).Value.Atlas, 24, new Vector2N(10, 650),
                Color.OrangeRed.ToImgui(),
                $"MetricsActiveAllocations: {io.MetricsActiveAllocations} MetricsActiveWindows: {io.MetricsActiveWindows} " +
                $"{Environment.NewLine}" +
                $"RV {io.MetricsRenderVertices} : RI {io.MetricsRenderIndices} ^ RW {io.MetricsRenderWindows} {Environment.NewLine}" +
                $"{io.Framerate}");

            _backGroundWindowPtr.AddCircle(new Vector2N(100, 100), 25, (uint) Color.Pink.ToRgba());
            _backGroundWindowPtr.AddCircleFilled(new Vector2N(300, 200), 25, (uint) Color.SpringGreen.ToRgba());
            _backGroundWindowPtr.AddLine(new Vector2N(10, 10), new Vector2N(300, 300), Color.Blue.ToImgui(), 5);
        }

        public void Render()
        {
            ImGui.Render();
            var data = ImGui.GetDrawData();

            if (io.DisplaySize.X <= 0.0f || io.DisplaySize.Y <= 0.0f)
                return;

            if (data.TotalVtxCount == 0)
                return;

            if (data.TotalVtxCount > VertexBufferSize)
            {
                VertexBuffer.Dispose();
                VertexBufferSize = (int) (data.TotalVtxCount * 1.5f);

                VertexBuffer = new Buffer(Dx11.D11Device,
                    new BufferDescription
                    {
                        Usage = ResourceUsage.Dynamic,
                        BindFlags = BindFlags.VertexBuffer,
                        OptionFlags = ResourceOptionFlags.None,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        SizeInBytes = VertexBufferSizeBytes
                    });
            }

            if (data.TotalIdxCount > IndexBufferSize)
            {
                IndexBuffer.Dispose();
                IndexBufferSize = (int) (data.TotalIdxCount * 1.5f);

                IndexBuffer = new Buffer(Dx11.D11Device,
                    new BufferDescription
                    {
                        Usage = ResourceUsage.Dynamic,
                        BindFlags = BindFlags.IndexBuffer,
                        OptionFlags = ResourceOptionFlags.None,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        SizeInBytes = IndexBufferSizeBytes
                    });
            }

            data.ScaleClipRects(io.DisplayFramebufferScale);

            IntPtr vtxOffset;
            IntPtr idxOffset;
            var vertexMap = Dx11.DeviceContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            var indexMap = Dx11.DeviceContext.MapSubresource(IndexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            vtxOffset = vertexMap.DataPointer;
            idxOffset = indexMap.DataPointer;

            for (var i = 0; i < data.CmdListsCount; i++)
            {
                var cmdList = data.CmdListsRange[i];
                Utilities.CopyMemory(vtxOffset, cmdList.VtxBuffer.Data, cmdList.VtxBuffer.Size * sizeOfImDrawVert);
                Utilities.CopyMemory(idxOffset, cmdList.IdxBuffer.Data, cmdList.IdxBuffer.Size * sizeOfImDrawIdx);
                vtxOffset += cmdList.VtxBuffer.Size * sizeOfImDrawVert;
                idxOffset += cmdList.IdxBuffer.Size * sizeOfImDrawIdx;
            }

            Dx11.DeviceContext.UnmapSubresource(VertexBuffer, 0);
            Dx11.DeviceContext.UnmapSubresource(IndexBuffer, 0);
            var vertexOffset = 0;
            var indexOffset = 0;
            var pos = data.DisplayPos;
            SetRenderState();

            for (var i = 0; i < data.CmdListsCount; i++)
            {
                var cmdList = data.CmdListsRange[i];

                for (var n = 0; n < cmdList.CmdBuffer.Size; n++)
                {
                    var drawCmd = cmdList.CmdBuffer[n];

                    // User callback (registered via ImDrawList::AddCallback)
                    //Now working o
                    /*if (drawCmd.UserCallback != IntPtr.Zero)
                    {
                        var delegateForFunctionPointer =(UserCallbackDel) Marshal.GetDelegateForFunctionPointer(drawCmd.UserCallback, typeof(UserCallbackDel));
                       // delegateForFunctionPointer(cmdList, drawCmd);
                        delegateForFunctionPointer?.Invoke(cmdList,drawCmd);
                    }*/
                    if (!Dx11.HasTexture(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException(
                            $"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }

                    Dx11.DeviceContext.Rasterizer.SetScissorRectangle((int) (drawCmd.ClipRect.X - pos.X),
                        (int) (drawCmd.ClipRect.Y - pos.Y),
                        (int) (drawCmd.ClipRect.Z - pos.X),
                        (int) (drawCmd.ClipRect.W - pos.Y));

                    Dx11.DeviceContext.PixelShader.SetShaderResource(0, Dx11.GetTexture(drawCmd.TextureId));
                    var drawCmdElemCount = (int) drawCmd.ElemCount;
                    Dx11.DeviceContext.DrawIndexed(drawCmdElemCount, indexOffset, vertexOffset);
                    indexOffset += drawCmdElemCount;
                }

                vertexOffset += cmdList.VtxBuffer.Size;
            }
        }

        public void Resize(Rectangle formBounds)
        {
            SetSize(formBounds);
        }

        public void CreateStates()
        {
            /*Viewport = new Viewport
            {
                Height = (int) io.DisplaySize.Y,
                Width = (int) io.DisplaySize.X,
                X = 0,
                Y = 0,
                MinDepth = 0,
                MaxDepth = 1
            };*/

            samplerState = new SamplerState(Dx11.D11Device,
                new SamplerStateDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    MipLodBias = 0.0f,
                    ComparisonFunction = Comparison.Always,
                    MinimumLod = 0.0f,
                    MaximumLod = 0.0f
                });

            //Blend
            var blendStateDescription = new BlendStateDescription {AlphaToCoverageEnable = false};
            blendStateDescription.RenderTarget[0].IsBlendEnabled = true;
            blendStateDescription.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDescription.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTarget[0].BlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDescription.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            blendStateDescription.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendState = new BlendState(Dx11.D11Device, blendStateDescription);

            // outputMergerBlendFactor = new RawColor4(0, 0f, 0, 0f);
            //Depth
            var depthStencilStateDescription = new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Always,
                FrontFace =
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            };

            depthStencilStateDescription.BackFace = depthStencilStateDescription.FrontFace;
            depthStencilState = new DepthStencilState(Dx11.D11Device, depthStencilStateDescription);

            SolidState = new RasterizerState(Dx11.D11Device,
                new RasterizerStateDescription
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    IsScissorEnabled = true,
                    IsDepthClipEnabled = true
                });
        }

        public void SetRenderState()
        {
            Dx11.DeviceContext.Rasterizer.State = SolidState;

            //  Dx11.DeviceContext.Rasterizer.SetViewport(Viewport);

            Dx11.DeviceContext.InputAssembler.InputLayout = Layout;
            vertexBuffer = new VertexBufferBinding {Buffer = VertexBuffer, Stride = sizeOfImDrawVert, Offset = 0};
            Dx11.DeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            Dx11.DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R16_UInt, 0);
            Dx11.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Dx11.DeviceContext.VertexShader.Set(VertexShader);
            Dx11.DeviceContext.VertexShader.SetConstantBuffer(0, ConstantBuffer);
            Dx11.DeviceContext.PixelShader.Set(PixelShader);
            Dx11.DeviceContext.PixelShader.SetSampler(0, samplerState);

            //Dx11.DeviceContext.OutputMerger.BlendFactor = outputMergerBlendFactor;
            //Dx11.DeviceContext.OutputMerger.BlendSampleMask = Int32.MaxValue; 
            Dx11.DeviceContext.OutputMerger.SetBlendState(blendState);
            Dx11.DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState);
        }
    }
}
