/*
 * The format parsed by this class is as follows:
 * 
 ******************************
 * # comments
 * object1.property1 = bool1
 * object1.property2 = int1
 * object2.property1 = string1
 * object2.property3 = float1
 * object2.property4 = float2
 ******************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RibbonsRedux.Content
{
    /// <summary>
    /// A class that parses Text information in the format specified in TextDictionary.cs.
    /// </summary>
    public class TextDictionary
    {
        Dictionary<string, Dictionary<string, string>> dict;

        string cachedString;
        Dictionary<string, string> cachedDict;

        /// <summary>
        /// Creates a new text dictionary based on a text file.
        /// </summary>
        /// <param name="textFile">The file to parse.</param>
        public TextDictionary(Text textFile)
        {
            dict = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < textFile.Length; i++)
            {
                //clean string
                int pound = textFile[i].IndexOf('#');
                string s;
                if (pound >= 0)
                    s = textFile[i].Substring(0, pound).Replace(" ", "");
                else
                    s = textFile[i].Replace(" ", "");
                //get dot and equals locations
                int dot = s.IndexOf('.');
                int set = s.IndexOf('=');
                if (dot == -1 || set == -1 || set < dot)
                    continue;
                string first = s.Substring(0, dot);
                string second = s.Substring(dot + 1, set - dot - 1);
                string value = s.Substring(set + 1);
                if (!dict.ContainsKey(first))
                    dict[first] = new Dictionary<string, string>();
                dict[first][second] = value;
            }
          
            cachedString = null;
            cachedDict = null;
        }

        public string[] GetProperties(string obj)
        {
            Dictionary<string, string> propDict;
            bool success = dict.TryGetValue(obj, out propDict);
            if (success)
            {
                cachedDict = propDict;
                return propDict.Keys.ToArray<string>();
            }
            else
                return new string[0];
        }

        /// <summary>
        /// Look up whether an object exists.
        /// </summary>
        /// <param name="obj">The object to query.</param>
        /// <returns>A boolean value.</returns>
        public bool CheckObjectExists(string obj)
        {
            Dictionary<string, string> propDict;
            bool success = dict.TryGetValue(obj, out propDict);
            if (success)
                cachedDict = propDict;
            return success;
        }

        /// <summary>
        /// Look up whether a property exists.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A boolean value.</returns>
        public bool CheckPropertyExists(string obj, string property)
        {
            if (!CheckObjectExists(obj)) return false;
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            return cachedDict.ContainsKey(property);
        }

        /// <summary>
        /// Look up a boolean property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A boolean value.</returns>
        public bool LookupBoolean(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            return Convert.ToBoolean(cachedDict[property]);
        }

        /// <summary>
        /// Look up an int property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>An int value.</returns>
        public int LookupInt32(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            return Convert.ToInt32(cachedDict[property]);
        }

        /// <summary>
        /// Look up a float property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A float value.</returns>
        public float LookupSingle(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            return Convert.ToSingle(cachedDict[property]);
        }

        /// <summary>
        /// Look up a Vector2 property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A Vector2 value.</returns>
        public Vector2 LookupVector2(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            string s = cachedDict[property];
            s = s.Replace('(', ' ');
            s = s.Replace(')', ' ');
            s = s.Trim();
            string[] components = s.Split(',');
            if (components.Length < 2)
                throw new ArgumentException();
            return new Vector2(Convert.ToSingle(components[0]), Convert.ToSingle(components[1]));
        }

        /// <summary>
        /// Look up a Vector3 property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A Vector3 value.</returns>
        public Vector3 LookupVector3(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            string s = cachedDict[property];
            s = s.Replace('(', ' ');
            s = s.Replace(')', ' ');
            s = s.Trim();
            string[] components = s.Split(',');
            if (components.Length < 3)
                throw new ArgumentException();
            return new Vector3(Convert.ToSingle(components[0]), Convert.ToSingle(components[1]), Convert.ToSingle(components[2]));
        }

        /// <summary>
        /// Look up a Vector4 property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A Vector4 value.</returns>
        public Vector4 LookupVector4(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            string s = cachedDict[property];
            s = s.Replace('(', ' ');
            s = s.Replace(')', ' ');
            s = s.Trim();
            string[] components = s.Split(',');
            if (components.Length < 4)
                throw new ArgumentException();
            return new Vector4(Convert.ToSingle(components[0]), Convert.ToSingle(components[1]), Convert.ToSingle(components[2]), Convert.ToSingle(components[3]));
        }

        /// <summary>
        /// Look up a Color property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A Color value.</returns>
        public Color LookupColor(string obj, string property)
        {
            try { return new Color(LookupVector4(obj, property)); }
            catch { return new Color(LookupVector3(obj, property)); }
        }

        /// <summary>
        /// Look up a string property.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="property">The property to query.</param>
        /// <returns>A string.</returns>
        public string LookupString(string obj, string property)
        {
            if (cachedString == null || !cachedString.Equals(obj))
            {
                cachedString = obj;
                cachedDict = dict[obj];
            }
            return cachedDict[property];
        }
    }
}
