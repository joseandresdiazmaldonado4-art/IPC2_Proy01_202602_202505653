using System.Xml;
using IPC2.Proyecto1.Estructuras;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1.Carga;

public class LectorXml
{
    public void Cargar(string ruta, DatosSistema datos)
    {
        XmlReaderSettings opciones = new XmlReaderSettings();
        opciones.IgnoreComments = true;
        opciones.IgnoreWhitespace = true;

        using XmlReader lector = XmlReader.Create(ruta, opciones);

        while (lector.Read())
        {
            if (lector.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            if (lector.Name == "ciudad")
            {
                using XmlReader bloqueCiudad = lector.ReadSubtree();
                Ciudad ciudad = LeerCiudad(bloqueCiudad);
                datos.GuardarCiudad(ciudad);
            }
            else if (lector.Name == "robot")
            {
                using XmlReader bloqueRobot = lector.ReadSubtree();
                Robot robot = LeerRobot(bloqueRobot);
                datos.GuardarRobot(robot);
            }
        }
    }

    private Ciudad LeerCiudad(XmlReader lector)
    {
        string nombre = "";
        int totalFilas = 0;
        int totalColumnas = 0;
        ListaSimple<string> contenidoFilas = new ListaSimple<string>();
        ListaSimple<UnidadMilitar> unidades = new ListaSimple<UnidadMilitar>();

        lector.Read();
        nombre = lector.GetAttribute("nombre") ?? nombre;
        int.TryParse(lector.GetAttribute("filas"), out totalFilas);
        int.TryParse(lector.GetAttribute("columnas"), out totalColumnas);

        while (!lector.EOF)
        {
            if (lector.NodeType == XmlNodeType.Element && lector.Name == "nombre")
            {
                nombre = lector.ReadElementContentAsString();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "filas")
            {
                totalFilas = lector.ReadElementContentAsInt();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "columnas")
            {
                totalColumnas = lector.ReadElementContentAsInt();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "fila")
            {
                contenidoFilas.AgregarFinal(lector.ReadElementContentAsString());
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "unidadMilitar")
            {
                using XmlReader bloqueUnidad = lector.ReadSubtree();
                unidades.AgregarFinal(LeerUnidadMilitar(bloqueUnidad));
                lector.Skip();
                continue;
            }

            lector.Read();
        }

        ValidarCiudad(nombre, totalFilas, totalColumnas, contenidoFilas);

        Ciudad ciudad = new Ciudad(nombre, totalFilas, totalColumnas);
        CargarCeldas(ciudad, contenidoFilas);
        CargarUnidades(ciudad, unidades);
        return ciudad;
    }

    private UnidadMilitar LeerUnidadMilitar(XmlReader lector)
    {
        int fila = 0;
        int columna = 0;
        int capacidad = 0;

        lector.Read();
        int.TryParse(lector.GetAttribute("fila"), out fila);
        int.TryParse(lector.GetAttribute("columna"), out columna);
        int.TryParse(lector.GetAttribute("capacidad"), out capacidad);

        while (!lector.EOF)
        {
            if (lector.NodeType == XmlNodeType.Element && lector.Name == "fila")
            {
                fila = lector.ReadElementContentAsInt();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "columna")
            {
                columna = lector.ReadElementContentAsInt();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "capacidad")
            {
                capacidad = lector.ReadElementContentAsInt();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Text
                && int.TryParse(lector.Value.Trim(), out int capacidadLeida))
            {
                capacidad = capacidadLeida;
            }

            lector.Read();
        }

        return new UnidadMilitar(fila - 1, columna - 1, capacidad);
    }

    private Robot LeerRobot(XmlReader lector)
    {
        string nombre = "";
        string tipo = "";
        int capacidad = 0;

        lector.Read();
        nombre = lector.GetAttribute("nombre") ?? nombre;
        tipo = lector.GetAttribute("tipo") ?? tipo;
        int.TryParse(lector.GetAttribute("capacidad"), out capacidad);

        while (!lector.EOF)
        {
            if (lector.NodeType == XmlNodeType.Element && lector.Name == "nombre")
            {
                nombre = lector.ReadElementContentAsString();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "tipo")
            {
                tipo = lector.ReadElementContentAsString();
                continue;
            }
            else if (lector.NodeType == XmlNodeType.Element && lector.Name == "capacidad")
            {
                capacidad = lector.ReadElementContentAsInt();
                continue;
            }

            lector.Read();
        }

        if (nombre == "")
        {
            throw new InvalidDataException("El robot no tiene nombre.");
        }

        if (tipo.Equals("ChapinRescue", StringComparison.OrdinalIgnoreCase))
        {
            return new ChapinRescue(nombre);
        }

        if (tipo.Equals("ChapinFighter", StringComparison.OrdinalIgnoreCase))
        {
            return new ChapinFighter(nombre, capacidad);
        }

        throw new InvalidDataException($"El tipo de robot {tipo} no es válido.");
    }

    private void ValidarCiudad(
        string nombre,
        int totalFilas,
        int totalColumnas,
        ListaSimple<string> contenidoFilas)
    {
        if (nombre == "")
        {
            throw new InvalidDataException("La ciudad no tiene nombre.");
        }

        if (totalFilas <= 0 || totalColumnas <= 0)
        {
            throw new InvalidDataException($"Las dimensiones de {nombre} no son válidas.");
        }

        if (contenidoFilas.Cantidad != totalFilas)
        {
            throw new InvalidDataException($"La cantidad de filas de {nombre} no coincide.");
        }

        for (int i = 0; i < contenidoFilas.Cantidad; i++)
        {
            if (contenidoFilas.Obtener(i).Length != totalColumnas)
            {
                throw new InvalidDataException($"La fila {i + 1} de {nombre} no coincide con las columnas.");
            }
        }
    }

    private void CargarCeldas(Ciudad ciudad, ListaSimple<string> contenidoFilas)
    {
        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            string textoFila = contenidoFilas.Obtener(fila);

            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                TipoCelda tipo = ConvertirTipo(textoFila[columna]);
                ciudad.Mapa.ColocarCelda(new Celda(fila, columna, tipo));
            }
        }
    }

    private TipoCelda ConvertirTipo(char simbolo)
    {
        return simbolo switch
        {
            '*' => TipoCelda.Intransitable,
            'E' => TipoCelda.Entrada,
            'C' => TipoCelda.Civil,
            'R' => TipoCelda.Recurso,
            ' ' => TipoCelda.Transitable,
            _ => throw new InvalidDataException($"El símbolo {simbolo} no es válido.")
        };
    }

    private void CargarUnidades(Ciudad ciudad, ListaSimple<UnidadMilitar> unidades)
    {
        for (int i = 0; i < unidades.Cantidad; i++)
        {
            UnidadMilitar unidad = unidades.Obtener(i);
            Celda celda = ciudad.Mapa.ObtenerCelda(unidad.Fila, unidad.Columna);
            celda.Tipo = TipoCelda.Militar;
            celda.CapacidadMilitar = unidad.CapacidadCombate;
        }
    }
}
