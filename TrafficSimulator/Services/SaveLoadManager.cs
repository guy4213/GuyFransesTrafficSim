using System;

namespace TrafficSimulator
{
    public static class SaveLoadManager
    {
        public static void Save(TrafficObjectCollection collection, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
#pragma warning disable SYSLIB0011
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(fs, collection);
#pragma warning restore SYSLIB0011
            }
        }

        public static TrafficObjectCollection Load(string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
#pragma warning disable SYSLIB0011
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (TrafficObjectCollection)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011
            }
        }
    }
}
