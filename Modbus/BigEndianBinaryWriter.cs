﻿using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;

namespace Must.Modbus
{
    /// <summary>
    /// This class writes values in Big Endian or Network  byte order to the underlying stream. The BinaryWriter always writes bytes in Little Endian, so by swopping them before hand we
    /// </summary>
    public class BigEndianBinaryWriter : IDisposable
    {
        private readonly Stream _stream;
        private bool _disposed = false;
        private readonly byte[] _buffer = new byte[16];

        public BigEndianBinaryWriter(Stream stream)
        {
            _stream = stream;
        }

        public virtual void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public virtual void Write(ushort value)
        {
            Span<byte> span = _buffer;
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(0, 2), value);
            _stream.Write(_buffer, 0, 2);
        }

        public virtual void Close()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (!disposing) return;
            _stream.Close();
            _disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}