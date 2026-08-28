using IPC2.Proyecto1.Carga;
using IPC2.Proyecto1.Estructuras;
using IPC2.Proyecto1.Misiones;
using IPC2.Proyecto1.Modelos;
using IPC2.Proyecto1.Reportes;

namespace IPC2.Proyecto1.Interfaz;

public class MenuPrincipal
{
    private readonly DatosSistema datos;
    private readonly LectorXml lector;
    private readonly PlanificadorMisiones planificador;
    private readonly GeneradorGrafico generador;
    private MisionEjecutada? ultimaMision;

    public MenuPrincipal(DatosSistema datos)
    {
        this.datos = datos;
        lector = new LectorXml();
        planificador = new PlanificadorMisiones();
        generador = new GeneradorGrafico();
    }

    public void Ejecutar()
    {
        int opcion;

        do
        {
            MostrarOpciones();
            opcion = LeerNumero("Seleccione una opción: ", 0, 4);

            if (opcion == 1)
            {
                CargarXml();
            }
            else if (opcion == 2)
            {
                EjecutarRescate();
            }
            else if (opcion == 3)
            {
                EjecutarExtraccion();
            }
            else if (opcion == 4)
            {
                GenerarGrafico();
            }
        }
        while (opcion != 0);
    }

    private void MostrarOpciones()
    {
        Console.WriteLine();
        Console.WriteLine("Proyecto IPC2");
        Console.WriteLine("1. Cargar o actualizar XML");
        Console.WriteLine("2. Ejecutar misión de rescate");
        Console.WriteLine("3. Ejecutar misión de extracción");
        Console.WriteLine("4. Generar gráfico de la última misión");
        Console.WriteLine("0. Salir");
    }

    private void CargarXml()
    {
        Console.Write("Ruta del archivo XML: ");
        string ruta = Console.ReadLine() ?? "";

        try
        {
            lector.Cargar(ruta, datos);
            Console.WriteLine("Archivo cargado correctamente.");
            Console.WriteLine($"Ciudades: {datos.Ciudades.Cantidad}");
            Console.WriteLine($"Robots: {datos.Robots.Cantidad}");
        }
        catch (Exception error)
        {
            Console.WriteLine($"No se pudo cargar el archivo: {error.Message}");
        }
    }

    private void EjecutarRescate()
    {
        Ciudad? ciudad = SeleccionarCiudad();

        if (ciudad == null)
        {
            return;
        }

        ChapinRescue? robot = SeleccionarRobotRescate();

        if (robot == null)
        {
            return;
        }

        Coordenada? objetivo = SeleccionarObjetivo(ciudad, TipoCelda.Civil, "civil");

        if (objetivo == null)
        {
            Console.WriteLine("Misión Imposible");
            return;
        }

        ResultadoMision resultado = planificador.ResolverRescate(ciudad, robot, objetivo);
        MostrarResultado(resultado);

        if (resultado.Completada)
        {
            ultimaMision = new MisionEjecutada("rescate", ciudad, robot, objetivo, resultado, 0);
        }
    }

    private void EjecutarExtraccion()
    {
        Ciudad? ciudad = SeleccionarCiudad();

        if (ciudad == null)
        {
            return;
        }

        ChapinFighter? robot = SeleccionarRobotCombate();

        if (robot == null)
        {
            return;
        }

        Coordenada? objetivo = SeleccionarObjetivo(ciudad, TipoCelda.Recurso, "recurso");

        if (objetivo == null)
        {
            Console.WriteLine("Misión Imposible");
            return;
        }

        int capacidadInicial = robot.CapacidadCombate;
        ResultadoMision resultado = planificador.ResolverExtraccion(ciudad, robot, objetivo);
        MostrarResultado(resultado);

        if (resultado.Completada)
        {
            Console.WriteLine($"Capacidad restante: {resultado.CapacidadFinal}");
            ultimaMision = new MisionEjecutada(
                "extracción",
                ciudad,
                robot,
                objetivo,
                resultado,
                capacidadInicial);
        }
    }

    private Ciudad? SeleccionarCiudad()
    {
        if (datos.Ciudades.Cantidad == 0)
        {
            Console.WriteLine("No hay ciudades cargadas.");
            return null;
        }

        Console.WriteLine("Ciudades disponibles:");

        for (int i = 0; i < datos.Ciudades.Cantidad; i++)
        {
            Ciudad ciudad = datos.Ciudades.Obtener(i);
            Console.WriteLine($"{i + 1}. {ciudad.Nombre} ({ciudad.Mapa.TotalFilas} x {ciudad.Mapa.TotalColumnas})");
        }

        int opcion = LeerNumero("Seleccione una ciudad: ", 1, datos.Ciudades.Cantidad);
        return datos.Ciudades.Obtener(opcion - 1);
    }

    private ChapinRescue? SeleccionarRobotRescate()
    {
        ListaSimple<ChapinRescue> disponibles = new ListaSimple<ChapinRescue>();

        for (int i = 0; i < datos.Robots.Cantidad; i++)
        {
            if (datos.Robots.Obtener(i) is ChapinRescue robot)
            {
                disponibles.AgregarFinal(robot);
            }
        }

        if (disponibles.Cantidad == 0)
        {
            Console.WriteLine("No hay robots ChapinRescue disponibles.");
            return null;
        }

        Console.WriteLine("Robots disponibles:");

        for (int i = 0; i < disponibles.Cantidad; i++)
        {
            Console.WriteLine($"{i + 1}. {disponibles.Obtener(i).Nombre}");
        }

        int opcion = LeerNumero("Seleccione un robot: ", 1, disponibles.Cantidad);
        return disponibles.Obtener(opcion - 1);
    }

    private ChapinFighter? SeleccionarRobotCombate()
    {
        ListaSimple<ChapinFighter> disponibles = new ListaSimple<ChapinFighter>();

        for (int i = 0; i < datos.Robots.Cantidad; i++)
        {
            if (datos.Robots.Obtener(i) is ChapinFighter robot)
            {
                disponibles.AgregarFinal(robot);
            }
        }

        if (disponibles.Cantidad == 0)
        {
            Console.WriteLine("No hay robots ChapinFighter disponibles.");
            return null;
        }

        Console.WriteLine("Robots disponibles:");

        for (int i = 0; i < disponibles.Cantidad; i++)
        {
            ChapinFighter robot = disponibles.Obtener(i);
            Console.WriteLine($"{i + 1}. {robot.Nombre} (capacidad {robot.CapacidadCombate})");
        }

        int opcion = LeerNumero("Seleccione un robot: ", 1, disponibles.Cantidad);
        return disponibles.Obtener(opcion - 1);
    }

    private Coordenada? SeleccionarObjetivo(Ciudad ciudad, TipoCelda tipo, string nombre)
    {
        ListaSimple<Coordenada> objetivos = new ListaSimple<Coordenada>();

        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                if (ciudad.Mapa.ObtenerCelda(fila, columna).Tipo == tipo)
                {
                    objetivos.AgregarFinal(new Coordenada(fila, columna));
                }
            }
        }

        if (objetivos.Cantidad == 0)
        {
            Console.WriteLine($"La ciudad no contiene ningún {nombre}.");
            return null;
        }

        Console.WriteLine($"Seleccione el {nombre}:");

        for (int i = 0; i < objetivos.Cantidad; i++)
        {
            Console.WriteLine($"{i + 1}. {objetivos.Obtener(i)}");
        }

        int opcion = LeerNumero("Seleccione un objetivo: ", 1, objetivos.Cantidad);
        return objetivos.Obtener(opcion - 1);
    }

    private void MostrarResultado(ResultadoMision resultado)
    {
        if (!resultado.Completada)
        {
            Console.WriteLine("Misión Imposible");
            return;
        }

        Console.WriteLine("Misión Completada");
        Console.WriteLine(resultado.ObtenerRutaComoTexto());
    }

    private void GenerarGrafico()
    {
        if (ultimaMision == null)
        {
            Console.WriteLine("Primero debe completar una misión.");
            return;
        }

        try
        {
            string rutaImagen = generador.GenerarImagen(ultimaMision);
            Console.WriteLine($"Imagen generada: {rutaImagen}");
            generador.AbrirImagen(rutaImagen);
        }
        catch (Exception error)
        {
            Console.WriteLine($"No se pudo generar el gráfico: {error.Message}");
        }
    }

    private int LeerNumero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine() ?? "";

            if (int.TryParse(texto, out int numero)
                && numero >= minimo
                && numero <= maximo)
            {
                return numero;
            }

            Console.WriteLine("Opción inválida.");
        }
    }
}
