using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace domino_server
{
    public class Jugador
    {
        public List<Ficha> fichas = new List<Ficha>();
        string nombre;
        public string identificador;
        public IPEndPoint ip;
        int puntuacion;
        public parametro_string agregarLinea;

        public Jugador(string nombre, string identificador, IPEndPoint ip, int pos, parametro_string delegado)
        {
            this.nombre = nombre;
            this.identificador = identificador;
            this.ip = ip;
            agregarLinea = delegado;
            agregarLinea(label());
        }

        public void agregarFicha(Ficha f)
        {
            fichas.Add(f);
        }

        public Ficha buscarToken(Token t)
        {
            Ficha f = null;
            foreach (var item in fichas)
            {
                if (item.token == t.token)
                {
                    f = item;
                    break;
                }
            }
            if (f != null)
            {
                fichas.Remove(f);
            }
            return f;
        }

        public int calcularPintas()
        {
            int pintas = 0;
            foreach (var item in fichas)
            {
                pintas += item.entero_uno + item.entero_dos;
            }
            return pintas;
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
            agregarLinea(label());
        }

        string label()
        {
            return nombre + ": " + puntuacion + " puntos";
        }

        public string getNombre()
        {
            return nombre;
        }

        public void setPuntuacion(int puntuacion)
        {
            this.puntuacion += puntuacion;
            agregarLinea(label());
        }

        public int getPuntuacion()
        {
            return puntuacion;
        }
    }
}
