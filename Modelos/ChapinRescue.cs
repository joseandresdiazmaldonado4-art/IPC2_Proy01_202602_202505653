namespace IPC2.Proyecto1.Modelos;

public class ChapinRescue : Robot
{
    public ChapinRescue(string nombre) : base(nombre)
    {
    }

    public override string ObtenerTipo()
    {
        return "ChapinRescue";
    }
}
