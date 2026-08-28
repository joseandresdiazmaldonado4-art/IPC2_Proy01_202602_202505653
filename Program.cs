using IPC2.Proyecto1.Carga;
using IPC2.Proyecto1.Interfaz;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1;

public class Program
{
    public static void Main(string[] args)
    {
        DatosSistema datos = new DatosSistema();
        LectorXml lector = new LectorXml();

        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                lector.Cargar(args[i], datos);
            }
            catch (Exception error)
            {
                Console.WriteLine($"No se pudo cargar {args[i]}: {error.Message}");
            }
        }

        MenuPrincipal menu = new MenuPrincipal(datos);
        menu.Ejecutar();
    }
}
