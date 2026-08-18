namespace IPC2.Proyecto1.Modelos;

public class UnidadMilitar
{
    public int Fila { get; }
    public int Columna { get; }
    public int CapacidadCombate { get; }

    public UnidadMilitar(int fila, int columna, int capacidadCombate)
    {
        Fila = fila;
        Columna = columna;
        CapacidadCombate = capacidadCombate;
    }
}
