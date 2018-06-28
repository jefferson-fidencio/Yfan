using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VotacaoEstampas.Model;

namespace VotacaoEstampas.Extensions
{
    public static class Extensions
    {
        public static byte[] StreamToBytes(this Stream stream) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Colecao DeserializarColecao(this string serializedColecao)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Colecao));
                using (TextReader reader = new StringReader(serializedColecao))
                {
                    return ((Colecao)xmlSerializer.Deserialize(reader));
                }
            }
            catch (Exception)
            {
                return new Colecao();
            }
        }

        public static string SerializarColecao(this Colecao colecao)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(colecao.GetType());
                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, colecao);
                    return textWriter.ToString();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
