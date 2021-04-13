using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Text;

namespace crop
{
    class World
    {
        Vector2i ChunckSize = (10, 10);

        char[] TileArray = {'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g',
                            'u', 'g', 'g', 'g', 'g', 'u', 'u', 'g', 'u', 'u',
                            'g', 'g', 't', 'g', 'g', 'g', 'u', 'g', 'u', 'g',
                            'u', 'g', 't', 't', 'g', 'g', 'u', 'g', 'u', 'g',
                            'g', 'g', 't', 'g', 'g', 'g', 'u', 'g', 'u', 'g',
                            'u', 'g', 'g', 'g', 'g', 'u', 'u', 'l', 'u', 'g',
                            'g', 'g', 'g', 'g', 'g', 'g', 'g', 'l', 'g', 'g',
                            'u', 'g', 'g', 'g', 'l', 'l', 'l', 'l', 'g', 'g',
                            'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g',
                            'u', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', 'g', };

        public World()
        {

        }

        public void DrawChunck()
        {
            for(int y = 0; y < ChunckSize.Y; y++)
                for(int x = 0; x < ChunckSize.X; x++)
                {
                    int index = x + y * ChunckSize.X;

                    SetTile(index, 10, x, y);
                }
        }        

        public void SetTile(int Index, byte Type, int x, int y)
        {
            Vector2 Position = ToNormalized((x, y));
            
            Renderer.Sprites[Index].X = Position.X;
            Renderer.Sprites[Index].Y = Position.Y;

            Renderer.Sprites[Index].Width = 2.0F;
            Renderer.Sprites[Index].Height = 1.0F;

            Renderer.Sprites[Index].U = (Type % 8) / 8.0F;
            Renderer.Sprites[Index].V = (Type / 8) / 8.0F;

            Renderer.Sprites[Index].TexWidth = 0.125F;
            Renderer.Sprites[Index].TexHeight = 0.125F;
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
            Transformation.Row0 = (21.0f / 44.0f, -1);
            Transformation.Row1 = (21.0f / 44.0f, 1);

            return Transformation * NormalizedCoords;
        }
    }
}
