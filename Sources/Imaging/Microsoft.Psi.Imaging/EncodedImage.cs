﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.Psi.Imaging
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an encoded image.
    /// </summary>
    public class EncodedImage : IDisposable
    {
        /// <summary>
        /// The pixel format was added as a private optional field backed property
        /// in order to maintain back-compatibility with an earlier version where
        /// no pixel format was stored on the image.
        /// </summary>
        [OptionalField]
        private readonly PixelFormat pixelFormat;

        /// <summary>
        /// The memory stream storing the encoded bytes.
        /// </summary>
        private MemoryStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedImage"/> class.
        /// </summary>
        /// <param name="width">Width of encoded image in pixels.</param>
        /// <param name="height">Height of encoded image in pixels.</param>
        /// <param name="pixelFormat">Pixel format of the encoded image.</param>
        public EncodedImage(int width, int height, PixelFormat pixelFormat)
        {
            this.Width = width;
            this.Height = height;
            this.pixelFormat = pixelFormat;
            this.stream = new MemoryStream();
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the pixel format for the encoded image.
        /// </summary>
        public PixelFormat PixelFormat => this.pixelFormat;

        /// <summary>
        /// Releases the image.
        /// </summary>
        public void Dispose()
        {
            this.stream.Dispose();
            this.stream = null;
        }

        /// <summary>
        /// Returns the image data as stream.
        /// </summary>
        /// <returns>A new stream containing the image data.</returns>
        public Stream ToStream()
        {
            // This method will only fail if the internal buffer is not set to be publicly
            // visible, but we create the memory stream ourselves so this should not be an issue
            if (!this.stream.TryGetBuffer(out ArraySegment<byte> buffer))
            {
                throw new InvalidOperationException("The internal buffer is not publicly visible");
            }

            return new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, false);
        }

        /// <summary>
        /// Returns the image data as a byte array.
        /// </summary>
        /// <returns>Byte array containing the image data.</returns>
        public byte[] GetBuffer()
        {
            return this.stream.GetBuffer();
        }

        /// <summary>
        /// Encodes a specified image with a specified encoder into the current encoded image.
        /// </summary>
        /// <param name="image">The image to encode.</param>
        /// <param name="imageEncoder">The image encoder to use.</param>
        /// <remarks>The image width, height and pixel format must match. The method should not be called concurrently.</remarks>
        public void EncodeFrom(Image image, IImageToStreamEncoder imageEncoder)
        {
            if (image.Width != this.Width || image.Height != this.Height || image.PixelFormat != this.PixelFormat)
            {
                throw new InvalidOperationException("Cannot encode from an image that has a different width, height, or pixel format.");
            }

            // reset the underlying MemoryStream - this also resets Position to 0
            this.stream.SetLength(0);
            imageEncoder.EncodeToStream(image, this.stream);
        }

        /// <summary>
        /// Decodes the image using a specified decoder.
        /// </summary>
        /// <param name="imageDecoder">The image decoder to use.</param>
        /// <returns>A new, corresponding decoded image.</returns>
        public Image Decode(IImageFromStreamDecoder imageDecoder)
        {
            var image = new Image(this.Width, this.Height, this.PixelFormat);
            image.DecodeFrom(this, imageDecoder);
            return image;
        }
    }
}
