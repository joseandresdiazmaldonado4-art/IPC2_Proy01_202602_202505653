namespace IPC2.Proyecto1.Modelos;

public class ChapinFighter : Robot
{
    public int CapacidadCombate { get; set; }

    public ChapinFighter(string nombre, int capacidadCombate) : base(nombre)
    {
        CapacidadCombate = capacidadCombate;
    }

    public override string ObtenerTipo()
    {
        return "ChapinFighter";
    }
}
