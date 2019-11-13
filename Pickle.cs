// Used for user setting pickling
// David Piao

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Pickle_NS
{
    public class Pickle<T> where T : new()
    {
        private const string DEFAULT_FILENAME = "pickle.txt";
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            try
            {
                serializer.MaxJsonLength = Int32.MaxValue;
                File.WriteAllText(fileName, serializer.Serialize(this));
            }
            catch (Exception e)
            {
                Console.WriteLine("## App Setting Saving Failed : " + e.Message);
            }
        }

        public static void Save(T pObj, string fileName = DEFAULT_FILENAME)
        {
            serializer.MaxJsonLength = Int32.MaxValue;
            File.WriteAllText(fileName, serializer.Serialize(pObj));
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            try
            {
                serializer.MaxJsonLength = Int32.MaxValue;
                T t = new T();
                if (File.Exists(fileName))
                    t = serializer.Deserialize<T>(File.ReadAllText(fileName));
                else
                    return default(T);
                return t;
            }
            catch (Exception e)
            {
                Console.WriteLine("## App Setting Loading Failed : " + e.Message);
                return default(T);
            }
        }
    }
}
