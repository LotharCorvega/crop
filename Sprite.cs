using System;
using System.Collections.Generic;
using System.Text;

namespace crop
{
    class Sprite
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public int Texture;

        
        Renderer.Vertex[] Vertices;


        public Sprite(ref Renderer.Vertex[] a)
        {
        }
    }
}
