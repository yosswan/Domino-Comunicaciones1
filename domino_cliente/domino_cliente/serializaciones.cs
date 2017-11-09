using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace domino_cliente
{
    partial class cliente_udp
    {
        public static byte[] ObjectToByte(Paquete paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Paquete));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(NombreJugador paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NombreJugador));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Jugada paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Jugada));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public static byte[] ObjectToByte(Verificacion paquete)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Verificacion));
            ser.WriteObject(ms, paquete);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        // Deserialize a JSON stream to a User object.  
        public static MensajeGeneral ReadToMensajeGeneral(byte[] json)
        {

            MensajeGeneral deserializedUser = new MensajeGeneral();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as MensajeGeneral;
            ms.Close();
            return deserializedUser;
        }

        public static Mesa ReadToMesa(byte[] json)
        {

            Mesa deserializedUser = new Mesa();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as Mesa;
            ms.Close();
            return deserializedUser;
        }

        public static Disponibilidad ReadToDisponibilidad(byte[] json)
        {

            Disponibilidad deserializedUser = new Disponibilidad();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as Disponibilidad;
            ms.Close();
            return deserializedUser;
        }

        public static Fichas ReadToFichas(byte[] json)
        {

            Fichas deserializedUser = new Fichas();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as Fichas;
            ms.Close();
            return deserializedUser;
        }

        public static InicioDeJuego ReadToInicioDeJuego(byte[] json)
        {

            InicioDeJuego deserializedUser = new InicioDeJuego();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as InicioDeJuego;
            ms.Close();
            return deserializedUser;
        }

        public static InicioRonda ReadToInicioRonda(byte[] json)
        {

            InicioRonda deserializedUser = new InicioRonda();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as InicioRonda;
            ms.Close();
            return deserializedUser;
        }
    }
}