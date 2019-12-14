using _1toOne_Converter.src.gbx.core.chunks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.core
{
    public struct NamedChild
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement(ElementName = "Data")]
        public FileComponent Child { get; set; }

        internal NamedChild(string name, FileComponent child)
        {
            this.Name = name;
            this.Child = child;
        }
    }

    public abstract class Structure : FileComponent
    {
        private List<NamedChild> children;

        private bool childrenNeedUpdating;

        [XmlIgnore]
        public NamedChild[] Children
        {
            get
            {
                if (children == null || children.Count == 0 || childrenNeedUpdating)
                {
                    children = GenerateChildren();
                    childrenNeedUpdating = false;
                }
                return children.ToArray();
            }
            protected set
            {
                if (children == null)
                    children = new List<NamedChild>();
                children.Clear();
                foreach (var namedChild in value)
                {
                    children.Add(namedChild);
                    namedChild.Child.Parent = this;
                }
            }
        }

        public Structure()
        {
            children = new List<NamedChild>();
        }

        public virtual List<NamedChild> GenerateChildren()
        {
            return null;
        }

        //public List<NamedChild> GenerateChildren()
        //{
        //    var type = this.GetType();
        //    var props = from property in type.GetProperties()
        //                where Attribute.IsDefined(property, typeof(AutoStructureAttribute))
        //                orderby ((AutoStructureAttribute)property
        //                          .GetCustomAttributes(typeof(AutoStructureAttribute), false)
        //                          .Single()).Order
        //                select property;

        //    //Todo Cache props

        //    var namedChildGetters = new List<NamedChildGetter>();

        //    var result = new List<NamedChild>();
        //    foreach(var prop in Props)
        //    {
        //        if(prop.Value() is FileComponent value)
        //            result.Add(new NamedChild(prop.Name, value);
        //    }

        //    return result;
        //}

        protected void AddChildDeprevated(string key, FileComponent child)
        {
            if (child != null)
                children.Add(new NamedChild(key, child));
        }

        protected void AddChildNew(FileComponent child)
        {
            child.Parent = this;
            MarkAsChanged();
        }

        protected void MarkAsChanged() => childrenNeedUpdating = true;

        public override void WriteBack(Stream s)
        {
            foreach (var namedChild in Children) {
                //Should be in the order the data was read.
                namedChild.Child.WriteBack(s);
            }
        }

        public FileComponent Get(string key)
        {
            foreach (var namedChild in Children)
            {
                if (namedChild.Name == key)
                {
                    return namedChild.Child;
                }
            }

            return null;
        }

        public override LinkedList<string> Dump()
        {
            var result = new LinkedList<string>();

            foreach (var namedChild in Children)
            {
                foreach (var s in namedChild.Child.Dump())
                {
                    result.AddLast(namedChild.Name + " / " + s);
                }
            }

            return result;
        }

        public IEnumerator<FileComponent> GetEnumerator()
        {
            foreach(var namedChild in children)
            {
                yield return namedChild.Child;
            }
        }
    }

    public static class ListExtension
    {
        public static void AddChild(this List<NamedChild> list, string key, FileComponent child)
        {
            if (child != null)
                list.Add(new NamedChild(key, child));
        }
    }
}
