namespace IPC2.Proyecto1.Estructuras;

public class Pila<T>
{
    private Nodo<T>? cima;

    public int Cantidad { get; private set; }

    public void Apilar(T dato)
    {
        Nodo<T> nuevo = new Nodo<T>(dato);
        nuevo.Siguiente = cima;
        cima = nuevo;
        Cantidad++;
    }

    public T Desapilar()
    {
        if (cima == null)
        {
            throw new InvalidOperationException("La pila está vacía.");
        }

        T dato = cima.Dato;
        cima = cima.Siguiente;
        Cantidad--;
        return dato;
    }

    public bool EstaVacia()
    {
        return cima == null;
    }
}
