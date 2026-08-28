using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using IPC2.Proyecto1.Misiones;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1.Reportes;

public class GeneradorGrafico
{
    public string GenerarImagen(MisionEjecutada mision)
    {
        if (!mision.Resultado.Completada)
        {
            throw new InvalidOperationException("No existe una ruta para graficar.");
        }

        string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "reportes");
        Directory.CreateDirectory(carpeta);

        string nombre = LimpiarNombre($"{mision.Ciudad.Nombre}_{mision.Tipo}");
        string rutaDot = Path.Combine(carpeta, nombre + ".dot");
        string rutaImagen = Path.Combine(carpeta, nombre + ".png");

        File.WriteAllText(rutaDot, CrearDot(mision), Encoding.UTF8);
        EjecutarGraphviz(rutaDot, rutaImagen);
        return rutaImagen;
    }

    public void AbrirImagen(string rutaImagen)
    {
        ProcessStartInfo inicio = new ProcessStartInfo();
        inicio.FileName = rutaImagen;
        inicio.UseShellExecute = true;
        Process.Start(inicio);
    }

    private string CrearDot(MisionEjecutada mision)
    {
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("digraph Mapa {");
        dot.AppendLine("  graph [labelloc=\"t\", fontsize=20, fontname=\"Arial\"];");
        dot.AppendLine($"  label=\"{EscaparTexto("Ruta de " + mision.Tipo)}\";");
        dot.AppendLine("  node [fontname=\"Arial\"];");
        dot.AppendLine("  mapa [shape=plain, label=<");
        dot.AppendLine("    <TABLE BORDER=\"1\" CELLBORDER=\"1\" CELLSPACING=\"0\">");

        for (int fila = 0; fila < mision.Ciudad.Mapa.TotalFilas; fila++)
        {
            dot.AppendLine("      <TR>");

            for (int columna = 0; columna < mision.Ciudad.Mapa.TotalColumnas; columna++)
            {
                Celda celda = mision.Ciudad.Mapa.ObtenerCelda(fila, columna);
                string color = ObtenerColor(celda, mision.Resultado, fila, columna);
                string contenido = ObtenerContenido(celda, fila, columna);
                dot.AppendLine($"        <TD WIDTH=\"65\" HEIGHT=\"55\" BGCOLOR=\"{color}\">{contenido}</TD>");
            }

            dot.AppendLine("      </TR>");
        }

        dot.AppendLine("    </TABLE>");
        dot.AppendLine("  >];");
        dot.AppendLine($"  resumen [shape=box, style=\"rounded\", label=\"{EscaparTexto(CrearResumen(mision))}\"];");
        dot.AppendLine("  mapa -> resumen [style=invis];");
        dot.AppendLine("}");
        return dot.ToString();
    }

    private string ObtenerColor(Celda celda, ResultadoMision resultado, int fila, int columna)
    {
        if (EstaEnRuta(resultado, fila, columna))
        {
            return "#66D9EF";
        }

        return celda.Tipo switch
        {
            TipoCelda.Intransitable => "#333333",
            TipoCelda.Entrada => "#A8E6A3",
            TipoCelda.Civil => "#A9D6F5",
            TipoCelda.Recurso => "#FFD966",
            TipoCelda.Militar => "#F28B82",
            _ => "#FFFFFF"
        };
    }

    private bool EstaEnRuta(ResultadoMision resultado, int fila, int columna)
    {
        for (int i = 0; i < resultado.Ruta.Cantidad; i++)
        {
            Coordenada posicion = resultado.Ruta.Obtener(i);

            if (posicion.Fila == fila && posicion.Columna == columna)
            {
                return true;
            }
        }

        return false;
    }

    private string ObtenerContenido(Celda celda, int fila, int columna)
    {
        string simbolo = celda.ObtenerSimbolo().ToString();

        if (simbolo == " ")
        {
            simbolo = "&#160;";
        }

        string detalle = $"<FONT POINT-SIZE=\"8\">{fila + 1},{columna + 1}</FONT><BR/>{simbolo}";

        if (celda.Tipo == TipoCelda.Militar)
        {
            detalle += $"<BR/><FONT POINT-SIZE=\"8\">{celda.CapacidadMilitar}</FONT>";
        }

        return detalle;
    }

    private string CrearResumen(MisionEjecutada mision)
    {
        string resumen = $"Objetivo: {mision.Objetivo}{Environment.NewLine}Robot: {mision.Robot.Nombre}";

        if (mision.Robot is ChapinFighter)
        {
            resumen += $"{Environment.NewLine}Capacidad: {mision.CapacidadInicial} -> {mision.Resultado.CapacidadFinal}";
        }

        resumen += $"{Environment.NewLine}Ruta: {mision.Resultado.ObtenerRutaComoTexto()}";
        return resumen;
    }

    private void EjecutarGraphviz(string rutaDot, string rutaImagen)
    {
        ProcessStartInfo inicio = new ProcessStartInfo();
        inicio.FileName = BuscarGraphviz();
        inicio.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaImagen}\"";
        inicio.UseShellExecute = false;
        inicio.CreateNoWindow = true;

        Process? proceso;

        try
        {
            proceso = Process.Start(inicio);
        }
        catch (Win32Exception)
        {
            throw new InvalidOperationException("Graphviz no está instalado.");
        }

        using (proceso)
        {
            if (proceso == null)
            {
                throw new InvalidOperationException("No se pudo iniciar Graphviz.");
            }

            proceso.WaitForExit();

            if (proceso.ExitCode != 0 || !File.Exists(rutaImagen))
            {
                throw new InvalidOperationException("Graphviz no pudo generar la imagen.");
            }
        }
    }

    private string BuscarGraphviz()
    {
        string rutaPrincipal = @"C:\Program Files\Graphviz\bin\dot.exe";
        string rutaAlterna = @"C:\Program Files (x86)\Graphviz\bin\dot.exe";

        if (File.Exists(rutaPrincipal))
        {
            return rutaPrincipal;
        }

        if (File.Exists(rutaAlterna))
        {
            return rutaAlterna;
        }

        return "dot";
    }

    private string LimpiarNombre(string texto)
    {
        string limpio = "";

        for (int i = 0; i < texto.Length; i++)
        {
            char caracter = texto[i];
            limpio += char.IsLetterOrDigit(caracter) ? caracter : '_';
        }

        return limpio;
    }

    private string EscaparTexto(string texto)
    {
        return texto
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "")
            .Replace("\n", "\\n");
    }
}
