using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BasketballBallBrandsCMS.Helpers
{
    public class DataIO
    {
        public void SerializeObject<T>(T serializableObject,string filePath)
        {
            if (serializableObject == null) return;
            
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());

                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);

                    string directoryPath = Path.GetDirectoryName(filePath);
                    if(!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath); // ako nema napravi
                    }
                    xmlDocument.Save(filePath);
                }
            }
            catch(Exception)
            {

            }
        }
        public T DeSerializeObject<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return default(T);

            T objectOut = default(T);
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);
                string xmlString = xmlDocument.OuterXml;

                using(StringReader stringReader = new StringReader(xmlString))
                {
                    Type objectType = typeof(T);
                    XmlSerializer serializer = new XmlSerializer(objectType);

                    using (XmlReader xmlReader = new XmlTextReader(stringReader))
                    {
                        objectOut = (T)serializer.Deserialize(xmlReader);
                    }
                }
            }catch(Exception)
            {
            }
            return objectOut;
        }
    }
}
