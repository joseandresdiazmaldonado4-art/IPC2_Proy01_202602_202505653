using IPC2.Proyecto1.Interfaz;

namespace IPC2.Proyecto1;

public class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new VentanaPrincipal());
    }
}
