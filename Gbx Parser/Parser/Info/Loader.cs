using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Gbx.Parser.Info
{
    public abstract class Loader : ILoader
    {
        private bool _isLoaded;

        public void Load()
        {
            if (_isLoaded == true)
                throw new Exception("Class has been already loaded");
                //

            _isLoaded = true;

            LoadNeededClasses();

            var chunks = CreateChunkInfos();

            var primaryClass = CreateClassInfo();

            var aliases = GetAliases();

            foreach(var chunk in chunks)
            {
                primaryClass.Add(chunk);
            }

            GbxInfo.Add(primaryClass);

            foreach(uint alias in aliases)
            {
                GbxInfo.AddAlias(primaryClass, alias);
            }
        }

        protected virtual void LoadNeededClasses()
        {
            //Nothing
        }

        protected abstract IEnumerable<GbxChunkInfo> CreateChunkInfos();

        protected abstract GbxClassInfo CreateClassInfo();

        protected virtual IEnumerable<uint> GetAliases()
        {
            return Enumerable.Empty<uint>();
        }
    }
}