using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1.Misiones;

public class MisionEjecutada
{
    public string Tipo { get; }
    public Ciudad Ciudad { get; }
    public Robot Robot { get; }
    public Coordenada Objetivo { get; }
    public ResultadoMision Resultado { get; }
    public int CapacidadInicial { get; }

    public MisionEjecutada(
        string tipo,
        Ciudad ciudad,
        Robot robot,
        Coordenada objetivo,
        ResultadoMision resultado,
        int capacidadInicial)
    {
        Tipo = tipo;
        Ciudad = ciudad;
        Robot = robot;
        Objetivo = objetivo;
        Resultado = resultado;
        CapacidadInicial = capacidadInicial;
    }
}
