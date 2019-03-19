﻿using System.IO;

namespace SkiaSharp.QrCode.Image
{
    public class QrCode
    {
        /// <summary>
        /// The various formats used by a SkiaSharp.SKCodec.
        /// </summary>
        public enum ImageFormat { Bmp = 0, Jpeg, Png }

        private readonly string content;
        private readonly SKImageInfo qrInfo;
        private readonly SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png;
        private readonly int quality = 100;

        public QrCode(string content, Vector2Slim qrSize)
            => (this.content, this.qrInfo) = (content, new SKImageInfo(qrSize.X, qrSize.Y));
        public QrCode(string content, Vector2Slim qrSize, ImageFormat outputFormat) : this(content, qrSize)
        {
            switch (outputFormat)
            {
                case ImageFormat.Bmp:
                    this.imageFormat = SKEncodedImageFormat.Bmp;
                    break;
                case ImageFormat.Jpeg:
                    this.imageFormat = SKEncodedImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    this.imageFormat = SKEncodedImageFormat.Png;
                    break;
            }
        }
        public QrCode(string content, Vector2Slim qrSize, ImageFormat outputFormat, int quality) : this(content, qrSize, outputFormat)
            => this.quality = quality;


        /// <summary>
        /// Generate QR Code and output to stream
        /// </summary>
        /// <param name="outputImage"></param>
        public void GenerateImage(Stream outputImage)
        {
            using (var generator = new QRCodeGenerator())
            {
                var qr = generator.CreateQrCode(content, ECCLevel.L);

                using (var qrSurface = SKSurface.Create(qrInfo))
                {
                    var qrCanvas = qrSurface.Canvas;
                    qrCanvas.Render(qr, qrInfo.Width, qrInfo.Height);

                    using (var qrImage = qrSurface.Snapshot())
                    {
                        Save(qrImage, outputImage);
                    }
                }
            }
        }

        /// <summary>
        /// Generate QR Code and conbine with base image, then output to stream
        /// </summary>
        /// <param name="outputImage"></param>
        /// <param name="baseImage"></param>
        /// <param name="baseQrSize"></param>
        /// <param name="qrPosition"></param>
        public void GenerateImage(Stream outputImage, Stream baseImage, Vector2Slim baseQrSize, Vector2Slim qrPosition)
        {
            using (var generator = new QRCodeGenerator())
            {
                var qr = generator.CreateQrCode(content, ECCLevel.L);

                using (var qrSurface = SKSurface.Create(qrInfo))
                {
                    var qrCanvas = qrSurface.Canvas;
                    qrCanvas.Render(qr, qrInfo.Width, qrInfo.Height);

                    using (var qrImage = qrSurface.Snapshot())
                    {
                        SaveCombinedImage(qrImage, baseImage, baseQrSize, qrPosition, outputImage);
                    }
                }
            }
        }
        /// <summary>
        /// Generate QR Code and conbine with base image, then output to stream
        /// </summary>
        /// <param name="outputImage"></param>
        /// <param name="baseImage"></param>
        /// <param name="baseQrSize"></param>
        /// <param name="qrPosition"></param>
        public void GenerateImage(Stream outputImage, byte[] baseImage, Vector2Slim baseQrSize, Vector2Slim qrPosition)
        {
            using (var generator = new QRCodeGenerator())
            {
                var qr = generator.CreateQrCode(content, ECCLevel.L);

                using (var qrSurface = SKSurface.Create(qrInfo))
                {
                    var qrCanvas = qrSurface.Canvas;
                    qrCanvas.Render(qr, qrInfo.Width, qrInfo.Height);

                    using (var qrImage = qrSurface.Snapshot())
                    {
                        SaveCombinedImage(qrImage, baseImage, baseQrSize, qrPosition, outputImage);
                    }
                }
            }
        }


        private void Save(SKImage qrImage, Stream output)
        {
            using (var data = qrImage.Encode(imageFormat, quality))
            {
                data.SaveTo(output);
            }
        }

        private void SaveCombinedImage(SKImage qrImage, Stream baseImage, Vector2Slim baseImageSize, Vector2Slim qrPosition, Stream output)
        {
            var baseInfo = new SKImageInfo(baseImageSize.X, baseImageSize.Y);
            using (var baseSurface = SKSurface.Create(baseInfo))
            using (SKBitmap baseBitmap = SKBitmap.Decode(baseImage))
            {
                // combine with base image
                var baseCanvas = baseSurface.Canvas;
                baseCanvas.DrawBitmap(baseBitmap, 0, 0);
                baseCanvas.DrawImage(qrImage, qrPosition.X, qrPosition.Y);

                using (var image = baseSurface.Snapshot())
                using (var data = image.Encode(imageFormat, quality))
                {
                    data.SaveTo(output);
                }
            }
        }

        private void SaveCombinedImage(SKImage qrImage, byte[] baseImage, Vector2Slim baseImageSize, Vector2Slim qrPosition, Stream output)
        {
            var baseInfo = new SKImageInfo(baseImageSize.X, baseImageSize.Y);
            using (var baseSurface = SKSurface.Create(baseInfo))
            using (SKBitmap baseBitmap = SKBitmap.Decode(baseImage))
            {
                // combine with base image
                var baseCanvas = baseSurface.Canvas;
                baseCanvas.DrawBitmap(baseBitmap, 0, 0);
                baseCanvas.DrawImage(qrImage, qrPosition.X, qrPosition.Y);

                using (var image = baseSurface.Snapshot())
                using (var data = image.Encode(imageFormat, quality))
                {
                    data.SaveTo(output);
                }
            }
        }
    }
}
