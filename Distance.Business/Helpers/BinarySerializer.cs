using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Distance.Business.Helpers
{
    public class BinaryDeserializer<TType> where TType : class 
    {
        public static TType Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var obj = new BinaryFormatter().Deserialize(stream);
                return obj as TType;
            }
        }
    }

    public class BinarySerializer 
    {
        public static byte[] Serialize(object contact)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, contact);
                return stream.ToArray();
            }
        }
    }
}
