using IPC2.Proyecto1.Estructuras;

namespace IPC2.Proyecto1.Modelos;

public class DatosSistema
{
    public ListaSimple<Ciudad> Ciudades { get; }
    public ListaSimple<Robot> Robots { get; }

    public DatosSistema()
    {
        Ciudades = new ListaSimple<Ciudad>();
        Robots = new ListaSimple<Robot>();
    }

    public void GuardarCiudad(Ciudad ciudad)
    {
        for (int i = 0; i < Ciudades.Cantidad; i++)
        {
            if (Ciudades.Obtener(i).Nombre.Equals(ciudad.Nombre, StringComparison.OrdinalIgnoreCase))
            {
                Ciudades.Reemplazar(i, ciudad);
                return;
            }
        }

        Ciudades.AgregarFinal(ciudad);
    }

    public void GuardarRobot(Robot robot)
    {
        for (int i = 0; i < Robots.Cantidad; i++)
        {
            if (Robots.Obtener(i).Nombre.Equals(robot.Nombre, StringComparison.OrdinalIgnoreCase))
            {
                Robots.Reemplazar(i, robot);
                return;
            }
        }

        Robots.AgregarFinal(robot);
    }
}
