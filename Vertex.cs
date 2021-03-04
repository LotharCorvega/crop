using System;
using OpenTK.Mathematics;

namespace crop
{
    class Vertex
    {
        public const int Size = 4 * sizeof(float);          //Number of bytes in a single Vertex

        public Vector2 Position;
        public Vector2 TextureCoordinates;

        public Vertex()
        {
        }
    }
}
