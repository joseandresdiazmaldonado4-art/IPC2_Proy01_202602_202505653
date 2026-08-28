using IPC2.Proyecto1.Carga;
using IPC2.Proyecto1.Estructuras;
using IPC2.Proyecto1.Misiones;
using IPC2.Proyecto1.Modelos;
using IPC2.Proyecto1.Reportes;

namespace IPC2.Proyecto1.Interfaz;

public class VentanaPrincipal : Form
{
    private readonly DatosSistema datos = new DatosSistema();
    private readonly LectorXml lector = new LectorXml();
    private readonly PlanificadorMisiones planificador = new PlanificadorMisiones();
    private readonly GeneradorGrafico generador = new GeneradorGrafico();

    private readonly ComboBox ciudades = new ComboBox();
    private readonly ComboBox robots = new ComboBox();
    private readonly ComboBox objetivos = new ComboBox();
    private readonly RadioButton opcionRescate = new RadioButton();
    private readonly RadioButton opcionExtraccion = new RadioButton();
    private readonly TextBox resultado = new TextBox();
    private readonly Label estado = new Label();

    private ListaSimple<Robot> robotsDisponibles = new ListaSimple<Robot>();
    private ListaSimple<Coordenada> objetivosDisponibles = new ListaSimple<Coordenada>();
    private MisionEjecutada? ultimaMision;

    public VentanaPrincipal()
    {
        Text = "Proyecto IPC2";
        Width = 760;
        Height = 590;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        CrearControles();
    }

    private void CrearControles()
    {
        Label titulo = new Label();
        titulo.Text = "Planificador de misiones";
        titulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        titulo.AutoSize = true;
        titulo.Location = new Point(25, 20);
        Controls.Add(titulo);

        Button cargar = new Button();
        cargar.Text = "Cargar XML";
        cargar.Location = new Point(580, 22);
        cargar.Size = new Size(135, 38);
        cargar.Click += CargarXml;
        Controls.Add(cargar);

        estado.Text = "No hay datos cargados.";
        estado.AutoSize = true;
        estado.Location = new Point(28, 70);
        Controls.Add(estado);

        AgregarEtiqueta("Ciudad", 28, 112);
        ciudades.DropDownStyle = ComboBoxStyle.DropDownList;
        ciudades.Location = new Point(28, 137);
        ciudades.Size = new Size(330, 31);
        ciudades.SelectedIndexChanged += CambiarSeleccion;
        Controls.Add(ciudades);

        GroupBox tipoMision = new GroupBox();
        tipoMision.Text = "Tipo de misión";
        tipoMision.Location = new Point(390, 105);
        tipoMision.Size = new Size(325, 75);

        opcionRescate.Text = "Rescate";
        opcionRescate.Location = new Point(20, 30);
        opcionRescate.AutoSize = true;
        opcionRescate.Checked = true;
        opcionRescate.CheckedChanged += CambiarSeleccion;
        tipoMision.Controls.Add(opcionRescate);

        opcionExtraccion.Text = "Extracción";
        opcionExtraccion.Location = new Point(155, 30);
        opcionExtraccion.AutoSize = true;
        opcionExtraccion.CheckedChanged += CambiarSeleccion;
        tipoMision.Controls.Add(opcionExtraccion);
        Controls.Add(tipoMision);

        AgregarEtiqueta("Robot", 28, 198);
        robots.DropDownStyle = ComboBoxStyle.DropDownList;
        robots.Location = new Point(28, 223);
        robots.Size = new Size(330, 31);
        Controls.Add(robots);

        AgregarEtiqueta("Objetivo", 390, 198);
        objetivos.DropDownStyle = ComboBoxStyle.DropDownList;
        objetivos.Location = new Point(390, 223);
        objetivos.Size = new Size(325, 31);
        Controls.Add(objetivos);

        Button ejecutar = new Button();
        ejecutar.Text = "Ejecutar misión";
        ejecutar.Location = new Point(28, 282);
        ejecutar.Size = new Size(210, 42);
        ejecutar.Click += EjecutarMision;
        Controls.Add(ejecutar);

        Button grafico = new Button();
        grafico.Text = "Generar gráfico";
        grafico.Location = new Point(255, 282);
        grafico.Size = new Size(210, 42);
        grafico.Click += GenerarGrafico;
        Controls.Add(grafico);

        Button salir = new Button();
        salir.Text = "Salir";
        salir.Location = new Point(482, 282);
        salir.Size = new Size(210, 42);
        salir.Click += delegate { Close(); };
        Controls.Add(salir);

        AgregarEtiqueta("Resultado", 28, 350);
        resultado.Location = new Point(28, 375);
        resultado.Size = new Size(664, 145);
        resultado.Multiline = true;
        resultado.ReadOnly = true;
        resultado.ScrollBars = ScrollBars.Vertical;
        Controls.Add(resultado);
    }

    private void AgregarEtiqueta(string texto, int izquierda, int arriba)
    {
        Label etiqueta = new Label();
        etiqueta.Text = texto;
        etiqueta.AutoSize = true;
        etiqueta.Location = new Point(izquierda, arriba);
        Controls.Add(etiqueta);
    }

    private void CargarXml(object? sender, EventArgs e)
    {
        using OpenFileDialog selector = new OpenFileDialog();
        selector.Title = "Seleccionar archivo XML";
        selector.Filter = "Archivos XML (*.xml)|*.xml";

        if (selector.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        try
        {
            lector.Cargar(selector.FileName, datos);
            estado.Text = $"Ciudades: {datos.Ciudades.Cantidad}   Robots: {datos.Robots.Cantidad}";
            resultado.Text = "Archivo cargado correctamente.";
            ultimaMision = null;
            ActualizarCiudades();
            ActualizarRobots();
            ActualizarObjetivos();
        }
        catch (Exception error)
        {
            MessageBox.Show("No se pudo cargar el archivo: " + error.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActualizarCiudades()
    {
        ciudades.Items.Clear();

        for (int i = 0; i < datos.Ciudades.Cantidad; i++)
        {
            Ciudad ciudad = datos.Ciudades.Obtener(i);
            ciudades.Items.Add($"{ciudad.Nombre} ({ciudad.Mapa.TotalFilas} x {ciudad.Mapa.TotalColumnas})");
        }

        if (ciudades.Items.Count > 0)
        {
            ciudades.SelectedIndex = 0;
        }
    }

    private void CambiarSeleccion(object? sender, EventArgs e)
    {
        ActualizarRobots();
        ActualizarObjetivos();
    }

    private void ActualizarRobots()
    {
        robots.Items.Clear();
        robotsDisponibles = new ListaSimple<Robot>();

        for (int i = 0; i < datos.Robots.Cantidad; i++)
        {
            Robot robot = datos.Robots.Obtener(i);
            bool coincide = opcionRescate.Checked && robot is ChapinRescue
                || opcionExtraccion.Checked && robot is ChapinFighter;

            if (coincide)
            {
                robotsDisponibles.AgregarFinal(robot);
                string capacidad = robot is ChapinFighter fighter
                    ? $" - capacidad {fighter.CapacidadCombate}"
                    : "";
                robots.Items.Add(robot.Nombre + capacidad);
            }
        }

        if (robots.Items.Count > 0)
        {
            robots.SelectedIndex = 0;
        }
    }

    private void ActualizarObjetivos()
    {
        objetivos.Items.Clear();
        objetivosDisponibles = new ListaSimple<Coordenada>();

        if (ciudades.SelectedIndex < 0)
        {
            return;
        }

        Ciudad ciudad = datos.Ciudades.Obtener(ciudades.SelectedIndex);
        TipoCelda tipo = opcionRescate.Checked ? TipoCelda.Civil : TipoCelda.Recurso;

        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                if (ciudad.Mapa.ObtenerCelda(fila, columna).Tipo == tipo)
                {
                    Coordenada coordenada = new Coordenada(fila, columna);
                    objetivosDisponibles.AgregarFinal(coordenada);
                    objetivos.Items.Add(coordenada.ToString());
                }
            }
        }

        if (objetivos.Items.Count > 0)
        {
            objetivos.SelectedIndex = 0;
        }

        objetivos.Enabled = objetivos.Items.Count > 1;
    }

    private void EjecutarMision(object? sender, EventArgs e)
    {
        if (ciudades.SelectedIndex < 0)
        {
            MessageBox.Show("No hay ciudades cargadas.");
            return;
        }

        if (robots.SelectedIndex < 0)
        {
            string tipoRobot = opcionRescate.Checked ? "ChapinRescue" : "ChapinFighter";
            MessageBox.Show("No hay robots " + tipoRobot + " disponibles para esta misión.");
            return;
        }

        if (objetivos.SelectedIndex < 0)
        {
            string tipoObjetivo = opcionRescate.Checked ? "unidades civiles" : "recursos";
            MessageBox.Show("La ciudad seleccionada no contiene " + tipoObjetivo + ".");
            return;
        }

        Ciudad ciudad = datos.Ciudades.Obtener(ciudades.SelectedIndex);
        Robot robot = robotsDisponibles.Obtener(robots.SelectedIndex);
        Coordenada objetivo = objetivosDisponibles.Obtener(objetivos.SelectedIndex);
        ResultadoMision mision;
        int capacidadInicial = 0;
        string tipo;

        if (opcionRescate.Checked)
        {
            tipo = "rescate";
            mision = planificador.ResolverRescate(ciudad, (ChapinRescue)robot, objetivo);
        }
        else
        {
            tipo = "extracción";
            capacidadInicial = ((ChapinFighter)robot).CapacidadCombate;
            mision = planificador.ResolverExtraccion(ciudad, (ChapinFighter)robot, objetivo);
        }

        if (!mision.Completada)
        {
            resultado.Text = "Misión Imposible";
            ultimaMision = null;
            return;
        }

        resultado.Text = "Misión Completada" + Environment.NewLine
            + "Robot: " + robot.Nombre + Environment.NewLine
            + "Tipo de misión: " + tipo + Environment.NewLine
            + "Objetivo: " + objetivo + Environment.NewLine
            + "Ruta: " + mision.ObtenerRutaComoTexto();

        if (robot is ChapinFighter)
        {
            resultado.Text += Environment.NewLine
                + $"Capacidad de combate inicial: {capacidadInicial}, "
                + $"Capacidad de combate final: {mision.CapacidadFinal}";
        }

        ultimaMision = new MisionEjecutada(tipo, ciudad, robot, objetivo, mision, capacidadInicial);
        CrearYAbrirGrafico();
    }

    private void GenerarGrafico(object? sender, EventArgs e)
    {
        if (ultimaMision == null)
        {
            MessageBox.Show("Primero debe completar una misión.");
            return;
        }

        CrearYAbrirGrafico();
    }

    private void CrearYAbrirGrafico()
    {
        try
        {
            string imagen = generador.GenerarImagen(ultimaMision!);
            generador.AbrirImagen(imagen);
        }
        catch (Exception error)
        {
            MessageBox.Show("No se pudo generar el gráfico: " + error.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
