namespace IPC2.Proyecto1.Misiones;

public class Coordenada
{
    public int Fila { get; }
    public int Columna { get; }

    public Coordenada(int fila, int columna)
    {
        Fila = fila;
        Columna = columna;
    }

    public override string ToString()
    {
        return $"({Fila + 1},{Columna + 1})";
    }
}
