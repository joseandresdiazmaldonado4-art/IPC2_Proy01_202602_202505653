namespace IPC2.Proyecto1.Estructuras;

// Lista enlazada propia. No utiliza List<T> ni otra colección de .NET.
public class ListaSimple<T>
{
    public Nodo<T>? Primero { get; private set; }
    public int Cantidad { get; private set; }

    public ListaSimple()
    {
        Primero = null;
        Cantidad = 0;
    }

    public void AgregarFinal(T dato)
    {
        Nodo<T> nuevo = new Nodo<T>(dato);

        if (Primero == null)
        {
            Primero = nuevo;
        }
        else
        {
            Nodo<T> actual = Primero;
            while (actual.Siguiente != null)
            {
                actual = actual.Siguiente;
            }

            actual.Siguiente = nuevo;
        }

        Cantidad++;
    }

    public T Obtener(int indice)
    {
        if (indice < 0 || indice >= Cantidad)
        {
            throw new ArgumentOutOfRangeException(nameof(indice));
        }

        Nodo<T> actual = Primero!;
        int posicion = 0;

        while (posicion < indice)
        {
            actual = actual.Siguiente!;
            posicion++;
        }

        return actual.Dato;
    }

    public void Reemplazar(int indice, T dato)
    {
        if (indice < 0 || indice >= Cantidad)
        {
            throw new ArgumentOutOfRangeException(nameof(indice));
        }

        Nodo<T> actual = Primero!;
        int posicion = 0;

        while (posicion < indice)
        {
            actual = actual.Siguiente!;
            posicion++;
        }

        actual.Dato = dato;
    }
}
