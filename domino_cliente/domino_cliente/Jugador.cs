using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domino_cliente
{
    public class Jugador
    {
        List<ValorFicha> fichas = new List<ValorFicha>();
        string nombre;
        int puntuacion;
        int pos;
        public parametro_string_entero modificarLabel;

        public Jugador(string nombre, int pos, parametro_string_entero delegado)
        {
            this.nombre = nombre;
            this.pos = pos;
            modificarLabel = delegado;
            modificarLabel(label(), pos);
        }

        public void agregarFicha(ValorFicha f)
        {
            fichas.Add(f);
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
            modificarLabel(label(), pos);
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
            this.puntuacion = puntuacion;
            modificarLabel(label(), pos);
        }

        public int getPuntuacion()
        {
            return puntuacion;
        }
    }
}
