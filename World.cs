using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Text;

namespace crop
{
    class World
    {
        Vector2 PlayerPosition = (0, 0);
        Vector2i WorldSize = (10, 10);

        char[] TileArray = {'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g',
                            'g', 'g', 'g', 'g', 'g', 'u', 'u', 'g', 'u', 'u',
                            'g', 'g', 't', 'g', 'g', 'g', 'u', 'g', 'u', 'g',
                            'g', 'g', 't', 't', 'g', 'g', 'u', 'g', 'u', 'g',
                            'g', 'g', 't', 'g', 'g', 'g', 'u', 'g', 'u', 'g',
                            'g', 'g', 'g', 'g', 'g', 'u', 'u', 'l', 'u', 'g',
                            'g', 'g', 'g', 'g', 'g', 'g', 'g', 'l', 'g', 'g',
                            'g', 'g', 'g', 'g', 'l', 'l', 'l', 'l', 'g', 'g',
                            'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g',
                            'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', };

        public World()
        {

        }

        public Vector2 GetTextureCoordinates(int x, int y)
        {
            char Tile = TileArray[y * WorldSize.Y + x];

            switch (Tile)
            {
                case 'g':
                    return (0.0f, 0.125f);
                case 't':
                    return (0.125f, 0.125f);
                case 'l':
                    return (0.125f, 0.0f);
                default:
                    return (0, 0);
            }
        }

        //Conversion from Worldcoordinates to Normalized Device Coordinates using Matries
        public static Vector2 ToNormalized(Vector2 GridCoords)
        {
            Matrix2 Transformation;
            Transformation.Row0 = (22.0f / 21.0f, 22.0f / 21.0f);
            Transformation.Row1 = (-0.5f, 0.5f);

            return Transformation * GridCoords;
        }

        //Same thing using the inverse of the matrix above
        public static Vector2 ToGrid(Vector2 NormalizedCoords)
        {
            Matrix2 Transformation;
            Transformation.Row0 = (22.0f / 44.0f, -1);
            Transformation.Row1 = (22.0f / 44.0f, 1);

            return Transformation * NormalizedCoords;
        }
    }
}
