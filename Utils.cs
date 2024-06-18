using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WorldBoxModLoader
{
    internal class Utils
    {
        public static T FindResource<T>(string name) where T : UnityEngine.Object
        {
            T[] first_search = Resources.FindObjectsOfTypeAll<T>();
            T result = null;
            foreach (var obj in first_search)
            {
                if (obj.name.ToLower() == name.ToLower())
                {
                    result = obj;
                }
            }
            return result;
        }
    }
}
