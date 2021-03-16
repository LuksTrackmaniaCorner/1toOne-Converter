using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterInterfaces
{
    /// <summary>
    /// A Converter has the job to guide multiple maps through conversion.
    /// 
    /// This interface is intended as a layer of abstraction, so that the library of the conversion
    /// can be swapped out easily.
    /// 
    /// Maps can be added and removed off the converter by the AddMaps and ClearMaps methods
    /// Each map will have these states, in this order:
    /// 
    /// NotLoaded -> Loaded -> Converted -> Written
    /// 
    /// If anything goes wrong, the map will be put in the error state.
    /// 
    /// Please note, that all methods have to be block until completed. They must also be threadsafe.
    /// They must be able to be called from arbitrary threads in arbitrary order,
    /// without causing internal havoc.
    /// </summary>
    public interface IConverter
    {
        #region Adding and removing Maps
        /// <summary>
        /// Adds these maps to the converted, putting them in the NotLoaded State
        /// </summary>
        /// <param name="filepaths"></param>
        void AddMaps(IEnumerable<string> filepaths);

        /// <summary>
        /// Removes all maps from the converter, regardless the state
        /// </summary>
        void ClearMaps();
        #endregion

        #region Converting Map
        /// <summary>
        /// Tries to advance all Notloaded maps to the Loaded State
        /// </summary>
        /// <returns>the number of maps loaded</returns>
        int LoadMaps();

        /// <summary>
        /// Tries to advance all NotLoaded and Loaded maps to the Converted State
        /// </summary>
        /// <returns>the number of maps converted</returns>
        int ConvertMaps();

        /// <summary>
        /// Tries to advanve all NotLoaded, Loaded and Converted Maps to the Stored State
        /// </summary>
        /// <returns></returns>
        int StoreMaps();
        #endregion

        #region Output
        /// Needed?
        #endregion

        /// <summary>
        /// This method allows to get information specifically for one map, e.g:
        /// - General info about the map, like thumbnail, author, authortime etc.
        /// - The Conversion Status
        /// - Statistics about the conversion
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        IMetaData GetMetaData(string filepath);
    }
}
