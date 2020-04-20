using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class ParsingException : Exception
    {
        public ParsingException()
        {
        }

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class UnknownChunkException : ParsingException
    {
        public UnknownChunkException()
        {
        }

        public UnknownChunkException(string message) : base(message)
        {
        }

        public UnknownChunkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownChunkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class InternalException : Exception
    {
        public InternalException()
        {
        }

        public InternalException(string message) : base(message)
        {
        }

        public InternalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InternalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class UnsupportedMapBaseException : Exception
    {
        public UnsupportedMapBaseException()
        {
        }

        public UnsupportedMapBaseException(string message) : base(message)
        {
        }

        public UnsupportedMapBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedMapBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class UnknownConversionException : Exception
    {
        public UnknownConversionException()
        {
        }

        public UnknownConversionException(string message) : base(message)
        {
        }

        public UnknownConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
