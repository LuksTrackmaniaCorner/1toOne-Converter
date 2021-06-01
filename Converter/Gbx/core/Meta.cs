using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Core
{
    public class Meta : Structure
    {
        public static readonly string IDKey = "ID";
        public static readonly string collectionKey = "Collection";
        public static readonly string authorKey = "Author";

        private GBXLBS id;
        private GBXLBS collection;
        private GBXLBS author;

        public GBXLBS ID { get => id; set { id = value; AddChildNew(value); } }
        public GBXLBS Collection { get => collection; set { collection = value; AddChildNew(value); } }
        public GBXLBS Author { get => author; set { author = value; AddChildNew(value); } }

        private Meta()
        {

        }

        public Meta(GBXLBS id, GBXLBS collection, GBXLBS author)
        {
            ID = id;
            Collection = collection;
            Author = author;
        }

        public Meta(Stream s, GBXLBSContext context)
        {
            ID = context.ReadLookBackString(s);
            Collection = context.ReadLookBackString(s);
            Author = context.ReadLookBackString(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(IDKey, ID);
            result.AddChild(collectionKey, collection);
            result.AddChild(authorKey, Author);
            return result;
        }

        public override FileComponent DeepClone()
        {
            var result = new Meta();
            result.ID = (GBXLBS)this.ID.DeepClone();
            result.Collection = (GBXLBS)this.Collection.DeepClone();
            result.Author = (GBXLBS)this.Author.DeepClone();
            return result;
        }
    }
}
