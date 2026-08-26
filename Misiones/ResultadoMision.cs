using IPC2.Proyecto1.Estructuras;

namespace IPC2.Proyecto1.Misiones;

public class ResultadoMision
{
    public bool Completada { get; }
    public string Mensaje { get; }
    public ListaSimple<Coordenada> Ruta { get; }
    public int CapacidadFinal { get; }

    public ResultadoMision(
        bool completada,
        string mensaje,
        ListaSimple<Coordenada> ruta,
        int capacidadFinal)
    {
        Completada = completada;
        Mensaje = mensaje;
        Ruta = ruta;
        CapacidadFinal = capacidadFinal;
    }

    public static ResultadoMision Imposible()
    {
        return new ResultadoMision(
            false,
            "Misión Imposible",
            new ListaSimple<Coordenada>(),
            0);
    }

    public string ObtenerRutaComoTexto()
    {
        if (!Completada)
        {
            return Mensaje;
        }

        string texto = "";

        for (int i = 0; i < Ruta.Cantidad; i++)
        {
            if (i > 0)
            {
                texto += " -> ";
            }

            texto += Ruta.Obtener(i).ToString();
        }

        return texto;
    }
}
