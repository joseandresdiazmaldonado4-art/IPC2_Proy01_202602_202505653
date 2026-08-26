namespace IPC2.Proyecto1.Misiones;

public class EstadoBusqueda
{
    public Coordenada Posicion { get; }
    public int CapacidadRestante { get; }
    public EstadoBusqueda? Anterior { get; }

    public EstadoBusqueda(
        Coordenada posicion,
        int capacidadRestante,
        EstadoBusqueda? anterior)
    {
        Posicion = posicion;
        CapacidadRestante = capacidadRestante;
        Anterior = anterior;
    }
}
