namespace IPC2.Proyecto1.Estructuras;

public class Cola<T>
{
    private Nodo<T>? primero;
    private Nodo<T>? ultimo;

    public int Cantidad { get; private set; }

    public void Encolar(T dato)
    {
        Nodo<T> nuevo = new Nodo<T>(dato);

        if (ultimo == null)
        {
            primero = nuevo;
            ultimo = nuevo;
        }
        else
        {
            ultimo.Siguiente = nuevo;
            ultimo = nuevo;
        }

        Cantidad++;
    }

    public T Desencolar()
    {
        if (primero == null)
        {
            throw new InvalidOperationException("La cola está vacía.");
        }

        T dato = primero.Dato;
        primero = primero.Siguiente;

        if (primero == null)
        {
            ultimo = null;
        }

        Cantidad--;
        return dato;
    }

    public bool EstaVacia()
    {
        return primero == null;
    }
}
