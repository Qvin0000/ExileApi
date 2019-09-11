using System.Drawing.Imaging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Bitmap = System.Drawing.Bitmap;
using Device = SharpDX.Direct3D11.Device;
using PixelFormat = SharpDX.WIC.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace ExileCore.RenderQ
{
    public class TextureLoader
    {
        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static BitmapSource LoadBitmap(ImagingFactory2 factory, string filename)
        {
            var bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand);
            var formatConverter = new FormatConverter(factory);

            formatConverter.Initialize(bitmapDecoder.GetFrame(0), PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0.0,
                BitmapPaletteType.Custom);

            return formatConverter;
        }

        /// <summary>
        /// Creates a <see cref="SharpDX.Direct3D11.Texture2D" /> from a WIC <see cref="SharpDX.WIC.BitmapSource" />
        /// </summary>
        /// <param name="device">The Direct3D11 device</param>
        /// <param name="bitmapSource">The WIC bitmap source</param>
        /// <returns>A Texture2D</returns>
        public static Texture2D CreateTexture2DFromBitmap(Device device, BitmapSource bitmapSource)
        {
            // Allocate DataStream to receive the WIC image pixels
            var stride = bitmapSource.Size.Width * 4;

            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);

                return new Texture2D(
                    device,
                    new Texture2DDescription
                    {
                        Width = bitmapSource.Size.Width,
                        Height = bitmapSource.Size.Height,
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        Usage = ResourceUsage.Immutable,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0)
                    }, new DataRectangle(buffer.DataPointer, stride));
            }
        }

        public static Texture2D CreateTexture2DFromBitmap(Device device, Bitmap bitmap, ResourceUsage Usage = ResourceUsage.Immutable,
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Allocate DataStream to receive the WIC image pixels
            var stride = bitmapData.Stride;

            var t = new Texture2D(
                device,
                new Texture2DDescription
                {
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = Usage,
                    CpuAccessFlags = cpuAccessFlags,
                    Format = Format.B8G8R8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                }, new DataRectangle(bitmapData.Scan0, stride));

            bitmap.UnlockBits(bitmapData);
            return t;
        }
    }
}
