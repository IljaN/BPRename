using System;
using System.ComponentModel;
using System.IO;

namespace ID3TagLib {
	
	internal enum UnsyncMode {
			Read,
			Write
	}
	
	internal class UnsyncFilterStream : Stream {
		
		private Stream underlyingStream;
		private UnsyncMode mode;
		private bool leaveOpen;
		private int lastValue;
		
		public UnsyncFilterStream(Stream underlyingStream, UnsyncMode mode, bool leaveOpen) {
			if (underlyingStream == null) {
				throw new ArgumentNullException("underlyingStream");
			}
			if (!Enum.IsDefined(typeof(UnsyncMode), mode)) {
				throw new InvalidEnumArgumentException("mode", (int)mode, typeof(UnsyncMode));
			}
			
			this.underlyingStream = underlyingStream;
			this.mode = mode;
			this.leaveOpen = leaveOpen;
		}
		
		public Stream BaseStream {
			get {
				return underlyingStream;
			}
		}
		
		public override bool CanRead {
			get {
				return mode == UnsyncMode.Read && underlyingStream.CanRead;
			}
		}
		
		public override bool CanSeek {
			get {
				return false;
			}
		}
		
		public override bool CanWrite {
			get {
				return mode == UnsyncMode.Write && underlyingStream.CanWrite;
			}
		}
		
		public override long Length {
			get {
				throw new NotSupportedException("Cannot get Length due to unsynchronization");
			}
		}
		
		public override long Position {
			get {
				throw new NotSupportedException("Cannot get Position due to unsynchronization");
			}
			
			set {
				throw new NotSupportedException("Cannot set Position due to unsynchronization");
			}
		}
		
		public override void Flush() {
			underlyingStream.Flush();
		}
		
		public override void Close() {
			if (!leaveOpen) {
				underlyingStream.Close();
			}
		}
		
		protected override void Dispose(bool disposing) {
			if (!leaveOpen) {
				underlyingStream.Dispose();
			}
		}
		
		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException("Cannot Seek due to unsynchronization");
		}
		
		public override void SetLength(long length) {
			throw new NotSupportedException("Cannot set Length due to unsynchronization");
		}
		
		public override int ReadByte() {
			int value;
			
			if (mode == UnsyncMode.Write) {
				throw new InvalidOperationException("Cannot read from stream when selected mode is Write.");
			}
			
			value = underlyingStream.ReadByte();
			if (lastValue == 0xFF && value == 0x00) {
				/* FF00(00|E0|EOF) */
				value = underlyingStream.ReadByte();
				if (value != 0x00 && value < 0xE0 && value != -1) {
					throw new ID3UnsyncException("Unsync mismatch found.");
				}
			}
			lastValue = value;
			
			return value;
		}
		
		public override int Read(byte[] buffer, int offset, int count) {
			int index = offset, value = 0;
			
			if (mode == UnsyncMode.Write) {
				throw new InvalidOperationException("Cannot read from stream when selected mode is Write.");
			}		
			
			while (index < offset + count && value != -1) {
				value = underlyingStream.ReadByte();
				if (lastValue == 0xFF && value == 0x00) {
					/* FF00(00|E0|EOF) */
					value = underlyingStream.ReadByte();
					if (value != 0x00 && value < 0xE0 && value != -1) {
						throw new ID3UnsyncException("Unsync mismatch found.");
					}
				}
				if (value != -1) {
					buffer[index++] = (byte)(value & 0xFF);
				}
				lastValue = value;
			}
			
			return index - offset;
		}
		
		public override void WriteByte(byte value) {
			if (mode == UnsyncMode.Read) {
				throw new InvalidOperationException("Cannot write to stream when selected mode is Read.");
			}
			
			if (lastValue == 0xFF && (value >= 0xE0 || value == 0)) {
				underlyingStream.WriteByte(0x00);
			}
			underlyingStream.WriteByte(value);
			lastValue = value;
		}
		
		public override void Write(byte[] buffer, int offset, int count) {
			if (mode == UnsyncMode.Read) {
				throw new InvalidOperationException("Cannot write to stream when selected mode is Read.");
			}
			
			for (int index = offset; index < offset + count; index++) {
				byte curByte = buffer[index];
				
				if (lastValue == 0xFF && (curByte >= 0xE0 || curByte == 0)) {
					underlyingStream.WriteByte(0x00);
				}
				underlyingStream.WriteByte(curByte);
				lastValue = curByte;
			}
		}
		
		public void ApplyFinalization() {
			if (mode == UnsyncMode.Read) {
				throw new InvalidOperationException("Cannot apply Finalization when selected mode is Read.");
			}
			
			if (lastValue == 0xFF) {
				underlyingStream.WriteByte(0x00);
				lastValue = 0x00;
			}
		}
	}
}