using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace domino_cliente
{
    [DataContract]
    public class Paquete
    {
        [DataMember]
        public string identificador = "DOMINOCOMUNICACIONESI";

        public Paquete() { }

    }

    [DataContract]
    class Mesa : Paquete
    {
        [DataMember]
        public string nombre_mesa;

        public Mesa() { }

        public Mesa(string nombre)
        {
            nombre_mesa = nombre;
        }
    }

    [DataContract]
    class NombreJugador : Paquete
    {
        [DataMember]
        public string nombre_jugador;

        public NombreJugador() { }

        public NombreJugador(string nombre)
        {
            nombre_jugador = nombre;
        }
    }

    [DataContract]
    class Disponibilidad : Paquete
    {
        [DataMember]
        public string multicast_ip;
        [DataMember]
        public string jugador;

        public Disponibilidad() { }

        public Disponibilidad(string multicastIP, string jugador)
        {
            this.jugador = jugador;
            multicast_ip = multicastIP;
        }
    }

    [DataContract]
    class InicioDeJuego : Paquete
    {
        [DataMember]
        public int tipo = 0;
        [DataMember]
        public DatosJugador[] jugadores;

        public InicioDeJuego() { }

        public InicioDeJuego(DatosJugador[] jugadores)
        {
            this.jugadores = jugadores;
        }
    }

    [DataContract]
    public class DatosJugador
    {
        [DataMember]
        public string identificador;
        [DataMember]
        public string nombre;

        public DatosJugador() { }

        public DatosJugador(string identificador, string nombre)
        {
            this.identificador = identificador;
            this.nombre = nombre;
        }
    }

    [DataContract]
    class InicioRonda : Paquete
    {
        [DataMember]
        public int tipo = 1;
        [DataMember]
        public int ronda;

        public InicioRonda() { }

        public InicioRonda(int ronda)
        {
            this.ronda = ronda;
        }
    }

    [DataContract]
    public class Ficha : Token
    {
        [DataMember]
        public int entero_uno;
        [DataMember]
        public int entero_dos;

        public ValorFicha getValorFicha()
        {
            return new ValorFicha(entero_uno, entero_dos);
        }

        public Token getToken()
        {
            return new Token(this.token);
        }

        public Ficha()
        {
            token = "-1";
            entero_uno = entero_dos = -1;
        }

        public Ficha(string token, int uno, int dos)
        {
            this.token = token;
            entero_uno = uno;
            entero_dos = dos;
        }
    }

    [DataContract]
    public class Token
    {
        [DataMember]
        public string token;

        public Token()
        {
            token = "-1";
        }

        public Token(string token)
        {
            this.token = token;
        }
    }

    [DataContract]
    public class ValorFicha
    {
        [DataMember]
        public int entero_uno;
        [DataMember]
        public int entero_dos;

        public ValorFicha()
        {
            entero_uno = entero_dos = -1;
        }

        public string ToString()
        {
            return entero_uno + " | " + entero_dos;
        }

        public ValorFicha(int uno, int dos)
        {
            entero_uno = uno;
            entero_dos = dos;
        }
    }

    [DataContract]
    class Fichas : Paquete
    {
        [DataMember]
        public int tipo = 2;
        [DataMember]
        public Ficha[] fichas = new Ficha[7];

        public Fichas() { }

        public Fichas(Ficha[] fichas)
        {
            for (int i = 0; i < 7; i++)
            {
                this.fichas[i] = fichas[i];
            }
        }
    }

    [DataContract]
    class MensajeDeJuego : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int tipo = 3;
        [DataMember]
        public int punta_uno;
        [DataMember]
        public int punta_dos;
        [DataMember]
        public Evento evento_pasado;

        public MensajeDeJuego(string mac, int punta1, int punta2, Evento evento)
        {
            jugador = mac;
            punta_uno = punta1;
            punta_dos = punta2;
            evento_pasado = evento;
        }
    }

    [DataContract]
    class MensajeGeneral : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int tipo;
        [DataMember]
        public int punta_uno;
        [DataMember]
        public int punta_dos;
        [DataMember]
        public Evento evento_pasado;
        [DataMember]
        public string razon;
        [DataMember]
        public Puntaje[] puntuacion_general;
        [DataMember]
        public int puntuacion;

        public MensajeGeneral() { }
    }

    [DataContract]
    public class Evento
    {
        [DataMember]
        public int tipo;
        [DataMember]
        public string jugador;
        [DataMember]
        public ValorFicha ficha;
        [DataMember]
        public bool punta;

        public Evento()
        {

        }

        public Evento(int tipo, string mac, ValorFicha ficha, bool punta)
        {
            jugador = mac;
            this.tipo = tipo;
            this.ficha = ficha;
            this.punta = punta;
        }
    }

    [DataContract]
    class FinDePartida : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int tipo = 5;
        [DataMember]
        public Puntaje[] puntuacion_general;
        [DataMember]
        public string razon;

        public FinDePartida(string mac, string motivo, Puntaje[] puntuacion)
        {
            jugador = mac;
            razon = motivo;
            puntuacion_general = puntuacion;
        }
    }

    [DataContract]
    class Puntaje
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int puntuacion;

        public Puntaje(string mac, int puntuacion)
        {
            jugador = mac;
            this.puntuacion = puntuacion;
        }
    }

    [DataContract]
    class FinDeRonda : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int tipo = 4;
        [DataMember]
        public int puntuacion;
        [DataMember]
        public string razon;

        public FinDeRonda(string mac, string motivo, int puntuacion)
        {
            jugador = mac;
            razon = motivo;
            this.puntuacion = puntuacion;
        }
    }

    [DataContract]
    class Desconexion : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public int tipo = 6;

        public Desconexion(string mac, string motivo, int puntuacion)
        {
            jugador = mac;
        }
    }

    [DataContract]
    class Jugada : Paquete
    {
        [DataMember]
        public Token ficha;
        [DataMember]
        public bool punta;

        public Jugada(Token ficha, bool punta)
        {
            this.ficha = ficha;
            this.punta = punta;
        }
    }

    [DataContract]
    class Verificacion : Paquete
    {
        [DataMember]
        public string jugador;

        public Verificacion(string mac)
        {
            jugador = mac;
        }
    }

    [DataContract]
    class MensajeDeJugador : Paquete
    {
        [DataMember]
        public string jugador;
        [DataMember]
        public Token ficha;
        [DataMember]
        public bool punta;

        public MensajeDeJugador()
        {
        }
    }

}
