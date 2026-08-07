namespace IPC2.Proyecto1.Estructuras;

// Nodo básico que puede guardar cualquier tipo de objeto.
public class Nodo<T>
{
    public T Dato { get; set; }
    public Nodo<T>? Siguiente { get; set; }

    public Nodo(T dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}
