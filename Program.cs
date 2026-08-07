using IPC2.Proyecto1.Estructuras;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Prueba de la Sesión 1 ===");

        Ciudad ciudad = new Ciudad("Ciudad de prueba", 3, 4);
        ciudad.Mapa.ColocarCelda(new Celda(0, 0, TipoCelda.Entrada));
        ciudad.Mapa.ColocarCelda(new Celda(1, 1, TipoCelda.Intransitable));
        ciudad.Mapa.ColocarCelda(new Celda(2, 3, TipoCelda.Recurso));

        ListaSimple<Robot> robots = new ListaSimple<Robot>();
        robots.AgregarFinal(new ChapinRescue("Rescate 1"));
        robots.AgregarFinal(new ChapinFighter("Peleador 1", 25));

        Console.WriteLine($"Ciudad: {ciudad.Nombre}");
        Console.WriteLine($"Tamaño: {ciudad.Mapa.TotalFilas} x {ciudad.Mapa.TotalColumnas}");
        Console.WriteLine($"Celda [0,0]: {ciudad.Mapa.ObtenerCelda(0, 0).ObtenerSimbolo()}");
        Console.WriteLine($"Celda [1,1]: {ciudad.Mapa.ObtenerCelda(1, 1).ObtenerSimbolo()}");
        Console.WriteLine($"Celda [2,3]: {ciudad.Mapa.ObtenerCelda(2, 3).ObtenerSimbolo()}");
        Console.WriteLine($"Robots guardados: {robots.Cantidad}");

        for (int i = 0; i < robots.Cantidad; i++)
        {
            Robot robot = robots.Obtener(i);
            Console.WriteLine($"- {robot.Nombre}: {robot.ObtenerTipo()}");
        }
    }
}
