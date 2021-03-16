using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterInterfaces
{
    /// <summary>
    /// This enum is used to represend the state of a single map.
    /// It will progress usually look like this:
    /// 
    /// NotLoaded -> Loaded -> Converted -> Written
    /// </summary>
    public enum MapState
    {
        Error = 0,
        NotLoaded,
        Loaded,
        Converted,
        Stored,
    }
}
