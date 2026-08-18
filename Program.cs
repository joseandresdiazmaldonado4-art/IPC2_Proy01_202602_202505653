using IPC2.Proyecto1.Carga;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1;

public class Program
{
    public static void Main(string[] args)
    {
        DatosSistema datos = new DatosSistema();
        LectorXml lector = new LectorXml();

        if (args.Length == 0)
        {
            lector.Cargar("datos_prueba.xml", datos);
            lector.Cargar("datos_actualizacion.xml", datos);
        }
        else
        {
            for (int i = 0; i < args.Length; i++)
            {
                lector.Cargar(args[i], datos);
            }
        }

        MostrarDatos(datos);
    }

    private static void MostrarDatos(DatosSistema datos)
    {
        Console.WriteLine($"Ciudades: {datos.Ciudades.Cantidad}");

        for (int i = 0; i < datos.Ciudades.Cantidad; i++)
        {
            Ciudad ciudad = datos.Ciudades.Obtener(i);
            Console.WriteLine($"{ciudad.Nombre}: {ciudad.Mapa.TotalFilas} x {ciudad.Mapa.TotalColumnas}");

            for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
            {
                for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
                {
                    Console.Write(ciudad.Mapa.ObtenerCelda(fila, columna).ObtenerSimbolo());
                }

                Console.WriteLine();
            }

            MostrarUnidades(ciudad);
        }

        Console.WriteLine($"Robots: {datos.Robots.Cantidad}");

        for (int i = 0; i < datos.Robots.Cantidad; i++)
        {
            Robot robot = datos.Robots.Obtener(i);

            if (robot is ChapinFighter fighter)
            {
                Console.WriteLine($"{fighter.Nombre}: {fighter.ObtenerTipo()}, capacidad {fighter.CapacidadCombate}");
            }
            else
            {
                Console.WriteLine($"{robot.Nombre}: {robot.ObtenerTipo()}");
            }
        }
    }

    private static void MostrarUnidades(Ciudad ciudad)
    {
        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                Celda celda = ciudad.Mapa.ObtenerCelda(fila, columna);

                if (celda.Tipo == TipoCelda.Militar)
                {
                    Console.WriteLine(
                        $"Unidad militar [{fila + 1},{columna + 1}]: {celda.CapacidadMilitar}");
                }
            }
        }
    }
}
