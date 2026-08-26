using IPC2.Proyecto1.Estructuras;
using IPC2.Proyecto1.Modelos;

namespace IPC2.Proyecto1.Misiones;

public class PlanificadorMisiones
{
    public ResultadoMision ResolverRescate(Ciudad ciudad, ChapinRescue robot)
    {
        Cola<EstadoBusqueda> pendientes = new Cola<EstadoBusqueda>();
        ListaSimple<Coordenada> visitadas = new ListaSimple<Coordenada>();

        AgregarEntradasRescate(ciudad, pendientes, visitadas);

        while (!pendientes.EstaVacia())
        {
            EstadoBusqueda actual = pendientes.Desencolar();
            Celda celdaActual = ciudad.Mapa.ObtenerCelda(
                actual.Posicion.Fila,
                actual.Posicion.Columna);

            if (celdaActual.Tipo == TipoCelda.Civil)
            {
                return CrearResultado(actual);
            }

            AgregarVecinoRescate(ciudad, actual, actual.Posicion.Fila - 1, actual.Posicion.Columna, pendientes, visitadas);
            AgregarVecinoRescate(ciudad, actual, actual.Posicion.Fila, actual.Posicion.Columna + 1, pendientes, visitadas);
            AgregarVecinoRescate(ciudad, actual, actual.Posicion.Fila + 1, actual.Posicion.Columna, pendientes, visitadas);
            AgregarVecinoRescate(ciudad, actual, actual.Posicion.Fila, actual.Posicion.Columna - 1, pendientes, visitadas);
        }

        return ResultadoMision.Imposible();
    }

    public ResultadoMision ResolverExtraccion(Ciudad ciudad, ChapinFighter robot)
    {
        Cola<EstadoBusqueda> pendientes = new Cola<EstadoBusqueda>();
        ListaSimple<RegistroCelda> mejoresLlegadas = new ListaSimple<RegistroCelda>();

        AgregarEntradasExtraccion(ciudad, robot.CapacidadCombate, pendientes, mejoresLlegadas);

        while (!pendientes.EstaVacia())
        {
            EstadoBusqueda actual = pendientes.Desencolar();
            Celda celdaActual = ciudad.Mapa.ObtenerCelda(
                actual.Posicion.Fila,
                actual.Posicion.Columna);

            if (celdaActual.Tipo == TipoCelda.Recurso)
            {
                robot.CapacidadCombate = actual.CapacidadRestante;
                return CrearResultado(actual);
            }

            AgregarVecinoExtraccion(ciudad, actual, actual.Posicion.Fila - 1, actual.Posicion.Columna, pendientes, mejoresLlegadas);
            AgregarVecinoExtraccion(ciudad, actual, actual.Posicion.Fila, actual.Posicion.Columna + 1, pendientes, mejoresLlegadas);
            AgregarVecinoExtraccion(ciudad, actual, actual.Posicion.Fila + 1, actual.Posicion.Columna, pendientes, mejoresLlegadas);
            AgregarVecinoExtraccion(ciudad, actual, actual.Posicion.Fila, actual.Posicion.Columna - 1, pendientes, mejoresLlegadas);
        }

        return ResultadoMision.Imposible();
    }

    private void AgregarEntradasRescate(
        Ciudad ciudad,
        Cola<EstadoBusqueda> pendientes,
        ListaSimple<Coordenada> visitadas)
    {
        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                if (ciudad.Mapa.ObtenerCelda(fila, columna).Tipo == TipoCelda.Entrada)
                {
                    Coordenada entrada = new Coordenada(fila, columna);
                    visitadas.AgregarFinal(entrada);
                    pendientes.Encolar(new EstadoBusqueda(entrada, 0, null));
                }
            }
        }
    }

    private void AgregarEntradasExtraccion(
        Ciudad ciudad,
        int capacidad,
        Cola<EstadoBusqueda> pendientes,
        ListaSimple<RegistroCelda> mejoresLlegadas)
    {
        for (int fila = 0; fila < ciudad.Mapa.TotalFilas; fila++)
        {
            for (int columna = 0; columna < ciudad.Mapa.TotalColumnas; columna++)
            {
                if (ciudad.Mapa.ObtenerCelda(fila, columna).Tipo == TipoCelda.Entrada)
                {
                    Coordenada entrada = new Coordenada(fila, columna);
                    mejoresLlegadas.AgregarFinal(new RegistroCelda(fila, columna, capacidad));
                    pendientes.Encolar(new EstadoBusqueda(entrada, capacidad, null));
                }
            }
        }
    }

    private void AgregarVecinoRescate(
        Ciudad ciudad,
        EstadoBusqueda anterior,
        int fila,
        int columna,
        Cola<EstadoBusqueda> pendientes,
        ListaSimple<Coordenada> visitadas)
    {
        if (!ExistePosicion(ciudad, fila, columna) || EstaVisitada(visitadas, fila, columna))
        {
            return;
        }

        Celda celda = ciudad.Mapa.ObtenerCelda(fila, columna);

        if (celda.Tipo == TipoCelda.Intransitable || celda.Tipo == TipoCelda.Militar)
        {
            return;
        }

        Coordenada posicion = new Coordenada(fila, columna);
        visitadas.AgregarFinal(posicion);
        pendientes.Encolar(new EstadoBusqueda(posicion, 0, anterior));
    }

    private void AgregarVecinoExtraccion(
        Ciudad ciudad,
        EstadoBusqueda anterior,
        int fila,
        int columna,
        Cola<EstadoBusqueda> pendientes,
        ListaSimple<RegistroCelda> mejoresLlegadas)
    {
        if (!ExistePosicion(ciudad, fila, columna))
        {
            return;
        }

        Celda celda = ciudad.Mapa.ObtenerCelda(fila, columna);

        if (celda.Tipo == TipoCelda.Intransitable)
        {
            return;
        }

        int capacidadNueva = anterior.CapacidadRestante;

        if (celda.Tipo == TipoCelda.Militar)
        {
            if (capacidadNueva <= celda.CapacidadMilitar)
            {
                return;
            }

            capacidadNueva -= celda.CapacidadMilitar;
        }

        if (!EsMejorLlegada(mejoresLlegadas, fila, columna, capacidadNueva))
        {
            return;
        }

        Coordenada posicion = new Coordenada(fila, columna);
        pendientes.Encolar(new EstadoBusqueda(posicion, capacidadNueva, anterior));
    }

    private bool EsMejorLlegada(
        ListaSimple<RegistroCelda> mejoresLlegadas,
        int fila,
        int columna,
        int capacidad)
    {
        for (int i = 0; i < mejoresLlegadas.Cantidad; i++)
        {
            RegistroCelda registro = mejoresLlegadas.Obtener(i);

            if (registro.Fila == fila && registro.Columna == columna)
            {
                if (capacidad <= registro.MejorCapacidad)
                {
                    return false;
                }

                registro.MejorCapacidad = capacidad;
                return true;
            }
        }

        mejoresLlegadas.AgregarFinal(new RegistroCelda(fila, columna, capacidad));
        return true;
    }

    private bool EstaVisitada(ListaSimple<Coordenada> visitadas, int fila, int columna)
    {
        for (int i = 0; i < visitadas.Cantidad; i++)
        {
            Coordenada posicion = visitadas.Obtener(i);

            if (posicion.Fila == fila && posicion.Columna == columna)
            {
                return true;
            }
        }

        return false;
    }

    private bool ExistePosicion(Ciudad ciudad, int fila, int columna)
    {
        return fila >= 0 && fila < ciudad.Mapa.TotalFilas
            && columna >= 0 && columna < ciudad.Mapa.TotalColumnas;
    }

    private ResultadoMision CrearResultado(EstadoBusqueda final)
    {
        Pila<Coordenada> recorridoInverso = new Pila<Coordenada>();
        EstadoBusqueda? actual = final;

        while (actual != null)
        {
            recorridoInverso.Apilar(actual.Posicion);
            actual = actual.Anterior;
        }

        ListaSimple<Coordenada> ruta = new ListaSimple<Coordenada>();

        while (!recorridoInverso.EstaVacia())
        {
            ruta.AgregarFinal(recorridoInverso.Desapilar());
        }

        return new ResultadoMision(true, "Misión Completada", ruta, final.CapacidadRestante);
    }
}
