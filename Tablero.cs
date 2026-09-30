using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Tetris2D.Graficos;
using Tetris2D.UI;

namespace Tetris2D
{
    public class Tablero
    {
        public const int Columnas = 10;
        public const int Filas = 18;
        private readonly int[,] _celdas = new int[Columnas, Filas];

        public int ObtenerCelda(int x, int y)
        {
            return _celdas[x, y];
        }

        public void Limpiar()
        {
            for (int x = 0; x < Columnas; x++)
            {
                for (int y = 0; y < Filas; y++)
                {
                    _celdas[x, y] = 0;
                    
                }
            }
        }

        public bool Colisiona(IReadOnlyList<(int X, int Y)> bloques, int posX, int posY)
        {
            foreach ((int bx, int by) in bloques)
            {
                int x = posX + bx;
                int y = posY + by;

                // Verifica si la posicion sale de los limites del tablero
                if (x < 0 || x >= Columnas || y < 0 || y >= Filas)
                    return true; // Hay colision con las paredes o el suelo

                // Verifica si la celda ya esta ocupada por otro bloque
                if (_celdas[x, y] != 0)
                    return true; // Hay colision con un bloque existente
            }
            return false; // No hay colision, el movimiento es valido
        }

        // Copia de forma permanente los bloques de una pieza dentro de la matriz del tablero
        public void FijarPieza(Pieza pieza, int posX, int posY)
        {
            int valorColor = (int)pieza.Tipo + 1; // Asigna un ID numérico según el tipo de pieza
            foreach ((int bx, int by) in pieza.Bloques)
            {
                int x = posX + bx;
                int y = posY + by;

                // Guarda el valor de color en la celda correspondiente si esta dentro del rango
                if (x >= 0 && x < Columnas && y >= 0 && y < Filas)
                    _celdas[x, y] = valorColor;
            }
        }

        // Revisa el tablero de abajo hacia arriba y borra las filas que estén completas
        public int LimpiarLineasCompletas()
        {
            int lineasBorradas = 0;

            for (int y = Filas - 1; y >= 0; y--)
            {
                if (FilaCompleta(y))
                {
                    lineasBorradas++;
                    BajarFilasSuperiores(y); // Desplaza las filas de arriba hacia abajo
                    y++; // Revisa la misma posicion 'y' nuevamente porque cayeron nuevas celdas
                }
            }
            return lineasBorradas; // Devuelve la cantidad total de lineas borradas
        }

        // Revisa si todos los cuadros de una fila especifica estan llenos
        private bool FilaCompleta(int y)
        {
            for (int x = 0; x < Columnas; x++)
            {
                if (_celdas[x, y] == 0)
                    return false; // Si hay al menos un espacio vacio, no esta completa
            }
            return true; // Toda la fila esta llena
        }

        // Mueve todas las filas por encima de 'filaInicio' una posición hacia abajo
        private void BajarFilasSuperiores(int filaInicio)
        {
            for (int y = filaInicio; y > 0; y--)
            {
                for (int x = 0; x < Columnas; x++)
                {
                    _celdas[x, y] = _celdas[x, y - 1]; // Copia el contenido de la fila de arriba
                }
            }

            // Deja vacia la primera fila superior (fila 0)
            for (int x = 0; x < Columnas; x++)
                _celdas[x, 0] = 0;
        }

        // Renderizado del tablero en pantalla (fondo, cuadricula y bloques fijos)
        public void Renderizar(DibujadorCuadros cuadros, float x0, float y0, float tamCelda)
        {
            float ancho = Columnas * tamCelda;
            float alto = Filas * tamCelda;

            // Dibuja el fondo oscuro del juego y un borde estilo neon
            cuadros.DibujarRectangulo(TemaArcade.PanelOscuro, x0, y0, x0 + ancho, y0 + alto);
            float grosorBorde = MathF.Max(2f, tamCelda * 0.10f);
            cuadros.DibujarBordeRectangulo(TemaArcade.Cian, x0, y0, x0 + ancho, y0 + alto, grosorBorde);

            // Dibuja la cuadricula transparente del tablero
            Vector4 colorCuadricula = new(TemaArcade.TextoSuave.X, TemaArcade.TextoSuave.Y, TemaArcade.TextoSuave.Z, 0.10f);
            for (int x = 1; x < Columnas; x++)
            {
                float lx = x0 + x * tamCelda;
                cuadros.DibujarLinea(colorCuadricula, lx, y0, lx, y0 + alto, 1f);
            }
            for (int y = 1; y < Filas; y++)
            {
                float ly = y0 + y * tamCelda;
                cuadros.DibujarLinea(colorCuadricula, x0, ly, x0 + ancho, ly, 1f);
            }

            // Dibuja cada bloque fijo en el tablero con su correspondiente color
            Vector4 bordeOscuro = new(0f, 0f, 0f, 0.35f);
            float grosorBisel = MathF.Max(1f, tamCelda * 0.06f);

            for (int x = 0; x < Columnas; x++)
            {
                for (int y = 0; y < Filas; y++)
                {
                    int valor = _celdas[x, y];
                    if (valor == 0) continue; // Si la celda esta vacia, no dibuja nada

                    Vector4 color = PiezaColor(valor);
                    float px = x0 + x * tamCelda;
                    float py = y0 + y * tamCelda;

                    cuadros.DibujarRectangulo(color, px, py, px + tamCelda, py + tamCelda);
                    cuadros.DibujarBordeRectangulo(bordeOscuro, px, py, px + tamCelda, py + tamCelda, grosorBisel);
                }
            }
        }

        // Obtiene el color correspondiente según el tipo de pieza almacenado
        private static Vector4 PiezaColor(int valorCelda)
        {
            TipoPieza tipo = (TipoPieza)(valorCelda - 1);
            return new Pieza(tipo).Color;
        }
    }
}
                
     
