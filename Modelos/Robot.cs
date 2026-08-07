namespace IPC2.Proyecto1.Modelos;

public abstract class Robot
{
    public string Nombre { get; }

    protected Robot(string nombre)
    {
        Nombre = nombre;
    }

    public abstract string ObtenerTipo();
}
