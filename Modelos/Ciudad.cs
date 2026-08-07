using IPC2.Proyecto1.Estructuras;

namespace IPC2.Proyecto1.Modelos;

public class Ciudad
{
    public string Nombre { get; }
    public MatrizCiudad Mapa { get; }

    public Ciudad(string nombre, int filas, int columnas)
    {
        Nombre = nombre;
        Mapa = new MatrizCiudad(filas, columnas);
    }
}
