namespace IPC2.Proyecto1.Misiones;

public class RegistroCelda
{
    public int Fila { get; }
    public int Columna { get; }
    public int MejorCapacidad { get; set; }

    public RegistroCelda(int fila, int columna, int mejorCapacidad)
    {
        Fila = fila;
        Columna = columna;
        MejorCapacidad = mejorCapacidad;
    }
}
