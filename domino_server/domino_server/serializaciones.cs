using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace domino_server
{
    partial class server_udp
    {
        public static byte[] ObjectToByte(FinDePartida paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FinDePartida));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Mesa paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Mesa));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Disponibilidad paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Disponibilidad));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Fichas paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Fichas));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(MensajeDeJuego paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MensajeDeJuego));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(FinDeRonda paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FinDeRonda));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Desconexion paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Desconexion));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static MensajeJugador ReadToJugador(byte[] json)
        {

            MensajeJugador deserializedUser = new MensajeJugador();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as MensajeJugador;
            ms.Close();
            return deserializedUser;
        }

        public static MensajeDeJugador ReadToMensajeDeJugador(byte[] json)
        {

            MensajeDeJugador deserializedUser = new MensajeDeJugador();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as MensajeDeJugador;
            ms.Close();
            return deserializedUser;
        }
    }
}