using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using SharpDX.Windows;
using Bitmap = System.Drawing.Bitmap;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace ExileCore.RenderQ
{
    public class SpritesRender
    {
        private static int MaxElements = 2000;
        private readonly RenderForm _form;
        private DrawCmd _currentDrawCmd;
        private int _indexBufferSize;
        private int _vertexBufferSize;
        private BlendState blendState;
        private DepthStencilState depthStencilState;
        private readonly List<DrawCmd> drawList = new List<DrawCmd>();
        private readonly ImagingFactory2 imagingFactory2;
        public int[] Indices = new int[MaxElements * 6];
        private string PrevTexture = "";
        private SamplerState samplerState;
        private SamplerStateDescription samplerStateDescription;
        private RasterizerState SolidState;
        private ShaderResourceView Texture;
        private VertexBufferBinding vertexBuffer;
        public Vertex[] Vertices = new Vertex[MaxElements * 4];
        private Viewport Viewport;

        public SpritesRender(DX11 dx11, RenderForm form, CoreSettings setCoreSettings)
        {
            _form = form;
            _dx11 = dx11;
            VertexBufferSize = MaxElements * Utilities.SizeOf<Vertex>() * 4;
            IndexBufferSize = MaxElements * Utilities.SizeOf<uint>() * 6;

            // Compile the vertex shader code.
            ShaderBytecode vertexShaderByteCode =
                ShaderBytecode.CompileFromFile("Shaders\\VertexShader.hlsl", "VS", "vs_4_0");

            // Compile the pixel shader code.
            ShaderBytecode pixelShaderByteCode =
                ShaderBytecode.CompileFromFile("Shaders\\PixelShader.hlsl", "PS", "ps_4_0");

            VertexShader = new VertexShader(_dx11.D11Device, vertexShaderByteCode);
            PixelShader = new PixelShader(_dx11.D11Device, pixelShaderByteCode);

            VertexBuffer = new Buffer(_dx11.D11Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.VertexBuffer,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = VertexBufferSizeBytes
                });

            IndexBuffer = new Buffer(_dx11.D11Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.IndexBuffer,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = IndexBufferSizeBytes
                });

            ConstantBuffer = new Buffer(_dx11.D11Device,
                new BufferDescription
                {
                    BindFlags = BindFlags.ConstantBuffer,
                    Usage = ResourceUsage.Dynamic,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SizeInBytes = Utilities.SizeOf<Vector2>() * 2
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

                    // Format = Format.R32G32B32A32_Float,
                    Format = Format.R8G8B8A8_UNorm,
                    Slot = 0,
                    AlignedByteOffset = InputElement.AppendAligned,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            Layout = new InputLayout(_dx11.D11Device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
                inputElements);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            CreateStates();
            imagingFactory2 = new ImagingFactory2();
        }

        private DX11 _dx11 { get; }
        private VertexShader VertexShader { get; }
        private PixelShader PixelShader { get; }
        private Buffer ConstantBuffer { get; }
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public InputLayout Layout { get; set; }

        private int VertexBufferSize
        {
            get => _vertexBufferSize;
            set
            {
                VertexBufferSizeBytes = value * 4;
                _vertexBufferSize = value;
            }
        }

        public int VertexBufferSizeBytes { get; set; }

        private int IndexBufferSize
        {
            get => _indexBufferSize;
            set
            {
                IndexBufferSizeBytes = value * 4;
                _indexBufferSize = value;
            }
        }

        public int IndexBufferSizeBytes { get; set; }
        public int ElementsCount { get; private set; }
        public int VertexCounter { get; private set; }
        public int IndexCounter { get; private set; }

        public Texture2D LoadBitmap(string name, Bitmap bitmap, ResourceUsage resourceUsage = ResourceUsage.Immutable,
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None)
        {
            var texture =
                TextureLoader.CreateTexture2DFromBitmap(_dx11.D11Device, bitmap, resourceUsage, cpuAccessFlags);

            Texture = new ShaderResourceView(_dx11.D11Device, texture);
            _dx11.AddOrUpdateTexture(name, Texture);
            if (resourceUsage != ResourceUsage.Dynamic) texture.Dispose();
            return texture;
        }

        public ShaderResourceView LoadPng(string fileName)
        {
            if (_dx11.HasTexture(fileName))
                return _dx11.GetTexture(fileName);
            if (!File.Exists(fileName))
            {
                DebugWindow.LogError($"{fileName} not found.");
                return null;
            }
            var texture = TextureLoader.CreateTexture2DFromBitmap(_dx11.D11Device,
                TextureLoader.LoadBitmap(imagingFactory2, fileName));
            Texture = new ShaderResourceView(_dx11.D11Device, texture);
            _dx11.AddOrUpdateTexture(fileName.Split('/').Last().Split('\\').Last(), Texture);
            texture.Dispose();
            return Texture;
        }

        public void CreateStates()
        {
            //Debug if texture broken
            var DeviceContext = _dx11.DeviceContext;
            var D11Device = _dx11.D11Device;

            //Blend
            var blendStateDescription = new BlendStateDescription();
            blendStateDescription.RenderTarget[0].IsBlendEnabled = true;
            blendStateDescription.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDescription.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTarget[0].BlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDescription.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            blendStateDescription.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendState = new BlendState(D11Device, blendStateDescription);

            //  DeviceContext.OutputMerger.BlendFactor = Color.White;

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
            depthStencilState = new DepthStencilState(D11Device, depthStencilStateDescription);

            Viewport = new Viewport
            {
                Height = _form.ClientSize.Height,
                Width = _form.ClientSize.Width,
                X = 0,
                Y = 0,
                MaxDepth = 1,
                MinDepth = 0
            };

            ResizeConstBuffer(_dx11.BackBuffer.Description);

            SolidState = new RasterizerState(_dx11.D11Device,
                new RasterizerStateDescription
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    IsScissorEnabled = false,
                    IsDepthClipEnabled = false
                });

            samplerStateDescription = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Wrap
            };

            samplerState = new SamplerState(_dx11.D11Device, samplerStateDescription);
        }

        public void ResizeConstBuffer(Texture2DDescription bufferDescription)
        {
            _dx11.DeviceContext.MapSubresource(ConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out var buffer);
            buffer.Write(new Vector2(bufferDescription.Width, bufferDescription.Height));
            _dx11.DeviceContext.UnmapSubresource(ConstantBuffer, 0);
        }

        public void SetStates()
        {
            var dx11DeviceContext = _dx11.DeviceContext;
            dx11DeviceContext.OutputMerger.SetBlendState(blendState);
            dx11DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState);

            // Setup and create the viewport for rendering.
            dx11DeviceContext.Rasterizer.State = SolidState;

            // dx11DeviceContext.Rasterizer.SetViewport(Viewport);
            // dx11DeviceContext.OutputMerger.SetRenderTargets(_dx11.RenderTargetView);

            //

            vertexBuffer = new VertexBufferBinding
                {Buffer = VertexBuffer, Stride = Utilities.SizeOf<Vertex>(), Offset = 0};

            dx11DeviceContext.InputAssembler.InputLayout = Layout;
            dx11DeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            dx11DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            dx11DeviceContext.VertexShader.Set(VertexShader);
            dx11DeviceContext.PixelShader.Set(PixelShader);
            dx11DeviceContext.VertexShader.SetConstantBuffer(0, ConstantBuffer);

            dx11DeviceContext.PixelShader.SetSampler(0, samplerState);
        }

        public void DrawImage(string fileName, RectangleF rect, RectangleF uv, Color color)
        {
            try
            {
                if (fileName != PrevTexture)
                {
                    _currentDrawCmd = new DrawCmd(_dx11.GetTexture(fileName));
                    drawList.Add(_currentDrawCmd);
                    PrevTexture = fileName;
                }

                if (ElementsCount + 100 > MaxElements)
                {
                    MaxElements = (int) (MaxElements * 1.75f);
                    DebugWindow.LogError($" -> New Max Elements = {MaxElements}");
                    Vertices = new Vertex[MaxElements * 4];
                    Indices = new int[MaxElements * 6];
                    VertexBufferSize = MaxElements * Utilities.SizeOf<Vertex>() * 4;
                    IndexBufferSize = MaxElements * Utilities.SizeOf<uint>() * 6;

                    VertexBuffer = new Buffer(_dx11.D11Device,
                        new BufferDescription
                        {
                            Usage = ResourceUsage.Dynamic,
                            BindFlags = BindFlags.VertexBuffer,
                            OptionFlags = ResourceOptionFlags.None,
                            CpuAccessFlags = CpuAccessFlags.Write,
                            SizeInBytes = VertexBufferSizeBytes
                        });

                    IndexBuffer = new Buffer(_dx11.D11Device,
                        new BufferDescription
                        {
                            Usage = ResourceUsage.Dynamic,
                            BindFlags = BindFlags.IndexBuffer,
                            OptionFlags = ResourceOptionFlags.None,
                            CpuAccessFlags = CpuAccessFlags.Write,
                            SizeInBytes = IndexBufferSizeBytes
                        });
                }

                Vertices[VertexCounter] = new Vertex(rect.TopLeft, uv.TopLeft, color); // left top
                Vertices[VertexCounter + 1] = new Vertex(rect.TopRight, uv.TopRight, color); // right top
                Vertices[VertexCounter + 2] = new Vertex(rect.BottomLeft, uv.BottomLeft, color); // left bot
                Vertices[VertexCounter + 3] = new Vertex(rect.BottomRight, uv.BottomRight, color); // rigth bot
                Indices[IndexCounter] = VertexCounter; // 0
                Indices[IndexCounter + 1] = VertexCounter + 1; // 1
                Indices[IndexCounter + 2] = VertexCounter + 2; // 2
                Indices[IndexCounter + 3] = VertexCounter + 2; // 2
                Indices[IndexCounter + 4] = VertexCounter + 1; // 1
                Indices[IndexCounter + 5] = VertexCounter + 3; // 3
                ElementsCount++;
                VertexCounter += 4;
                IndexCounter += 6;
                _currentDrawCmd.IncreaseElementsCount();
            }
            catch (OutOfMemoryException e)
            {
                DebugWindow.LogError($"{e} -> New Max Elements = {MaxElements}");
            }
            catch (Exception e)
            {
                DebugWindow.LogError(e.ToString());
            }
        }

        public void Clear()
        {
            ElementsCount = 0;
            VertexCounter = 0;
            IndexCounter = 0;
            PrevTexture = "";
            drawList.Clear();
            Array.Clear(Vertices, 0, Vertices.Length);
            Array.Clear(Indices, 0, Indices.Length);

            // drawList = new List<DrawCmd>(drawList.Count);
            //Vertices = new Vertex[Vertices.Length];
            //Indices  = new int[Indices.Length];
        }

        public void Render()
        {
            if (drawList.Count <= 0)
                return;

            SetStates();

            var vertexBufferDataBox =
                _dx11.DeviceContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);

            var indexBufferDataBox =
                _dx11.DeviceContext.MapSubresource(IndexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);

            Utilities.Write(vertexBufferDataBox.DataPointer, Vertices, 0, ElementsCount * 4);
            Utilities.Write(indexBufferDataBox.DataPointer, Indices, 0, ElementsCount * 6);
            _dx11.DeviceContext.UnmapSubresource(VertexBuffer, 0);
            _dx11.DeviceContext.UnmapSubresource(IndexBuffer, 0);
            var vtxOffsets = 0;
            var idxOffsets = 0;

            for (var index = 0; index < drawList.Count; index++)
            {
                var cmd = drawList[index];

                // cmd.CallBack?.Invoke();
                _dx11.DeviceContext.PixelShader.SetShaderResource(0, cmd.Texture);
                _dx11.DeviceContext.DrawIndexed(cmd.ElementsCount, idxOffsets, vtxOffsets);
                idxOffsets += cmd.ElementsCount;
            }

            Clear();
        }
    }

    internal class DrawCmd
    {
        // public Action CallBack { get; set; }

        public DrawCmd(ShaderResourceView texture)
        {
            Texture = texture;

            //  CallBack = null;
            ElementsCount = 0;
        }

        public ShaderResourceView Texture { get; }
        public int ElementsCount { get; private set; }

        public void IncreaseElementsCount()
        {
            ElementsCount += 6;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vertex
    {
        public Vector2 Position { get; }
        public Vector2 TexC { get; }
        public Color Color { get; }

        public Vertex(Vector2 pos, Vector2 uv, Color color) : this()
        {
            Position = pos;
            TexC = uv;
            Color = color;
        }
    }
}
