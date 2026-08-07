namespace IPC2.Proyecto1.Modelos;

public class Celda
{
    public int Fila { get; }
    public int Columna { get; }
    public TipoCelda Tipo { get; set; }
    public int CapacidadMilitar { get; set; }

    public Celda(int fila, int columna, TipoCelda tipo, int capacidadMilitar = 0)
    {
        Fila = fila;
        Columna = columna;
        Tipo = tipo;
        CapacidadMilitar = capacidadMilitar;
    }

    public char ObtenerSimbolo()
    {
        return Tipo switch
        {
            TipoCelda.Intransitable => '*',
            TipoCelda.Entrada => 'E',
            TipoCelda.Civil => 'C',
            TipoCelda.Recurso => 'R',
            TipoCelda.Militar => 'M',
            _ => ' '
        };
    }
}
