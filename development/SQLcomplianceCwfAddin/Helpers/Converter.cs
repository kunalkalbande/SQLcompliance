using System.Collections;
using System.Collections.Generic;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class Converter
    {
        public delegate T2 ConvertDelegate<T1, T2>(T1 item);

        public static List<T2> ConvertList<T1, T2>(IEnumerable t1List, ConvertDelegate<T1, T2> converter)
        {
            var t2List = new List<T2>();

            foreach (T1 item in t1List)
            {
                var convertedItem = converter(item);
                t2List.Add(convertedItem);
            }

            return t2List;
        }

        public static List<T2> ConvertReferenceList<T1, T2>(IEnumerable t1List, ConvertDelegate<T1,T2> converter) 
            where T1 : class 
            where T2 : class
        {
            var t2List = new List<T2>();

            if (t1List != null)
            {
                foreach (T1 item in t1List)
                {
                    if (item != null)
                    {
                        var convertedItem = converter(item);
                        if (convertedItem != null)
                        {
                            t2List.Add(convertedItem);
                        }
                    }
                }
            }

            return t2List;
        }

        public static List<T> GetListFromDictionary<T>(IDictionary dictionary)
            where T : class
        {
            var list = new List<T>(dictionary.Count);

            foreach (var key in dictionary.Keys)
            {
                var item = dictionary[key] as T;

                if (item != null)
                {
                    list.Add(item);
                }
            }

            return list;
        }
       
    }
}
