using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.core;

namespace gbx.parser.visitor
{
    public class FileParserVisitor : InOutVisitor<ParsingArgs, object>
    {
        protected internal override object Visit(GbxComponent component, ParsingArgs arg)
        {
            throw new NotImplementedException();
        }

        protected internal override object Visit<T>(GbxArray<T> array, ParsingArgs arg)
        {
            var count = arg.Reader.ReadUInt32();
            if (count < 0)
                throw new Exception();

            array.Clear();
            for(int i = 0; i < count; i++)
            {
                Dispatch(array.AddNew(), arg); //Parse all the child elements
            }

            throw new NotImplementedException();
        }
    }

    public class ParsingArgs
    {
        public BinaryReader Reader { get; }

        public ParsingArgs(BinaryReader reader)
        {
            Reader = reader;
        }
    }
}
