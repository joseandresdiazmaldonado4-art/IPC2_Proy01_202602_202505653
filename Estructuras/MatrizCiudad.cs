using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1.Estructuras;

// Matriz propia implementada como una lista enlazada de filas.
// Cada fila contiene otra lista enlazada de celdas.
public class MatrizCiudad
{
    private readonly ListaSimple<ListaSimple<Celda>> filas;

    public int TotalFilas { get; }
    public int TotalColumnas { get; }

    public MatrizCiudad(int totalFilas, int totalColumnas)
    {
        if (totalFilas <= 0 || totalColumnas <= 0)
        {
            throw new ArgumentException("La matriz debe tener filas y columnas.");
        }

        TotalFilas = totalFilas;
        TotalColumnas = totalColumnas;
        filas = new ListaSimple<ListaSimple<Celda>>();

        for (int fila = 0; fila < TotalFilas; fila++)
        {
            ListaSimple<Celda> nuevaFila = new ListaSimple<Celda>();

            for (int columna = 0; columna < TotalColumnas; columna++)
            {
                nuevaFila.AgregarFinal(new Celda(fila, columna, TipoCelda.Transitable));
            }

            filas.AgregarFinal(nuevaFila);
        }
    }

    public Celda ObtenerCelda(int fila, int columna)
    {
        ValidarPosicion(fila, columna);
        return filas.Obtener(fila).Obtener(columna);
    }

    public void ColocarCelda(Celda celda)
    {
        ValidarPosicion(celda.Fila, celda.Columna);
        filas.Obtener(celda.Fila).Reemplazar(celda.Columna, celda);
    }

    private void ValidarPosicion(int fila, int columna)
    {
        if (fila < 0 || fila >= TotalFilas || columna < 0 || columna >= TotalColumnas)
        {
            throw new ArgumentOutOfRangeException("La posición no existe en la matriz.");
        }
    }
}
