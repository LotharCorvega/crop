using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;


namespace crop
{
    class Camera
    {
        public int Width;
        public int Height;
        float AspectRatio;

        float Zoom;

        public Vector2 Position;
        public Matrix4 Projection;

        public Camera ()
        {
            Position = (0.0f, 0.0f);
        }

        public void Update(Window window)
        {
            AspectRatio = window.Size.X / (float)window.Size.Y;
            Zoom = window.MouseState.Scroll.Y + 3.0f;

            Projection = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0) * Matrix4.CreateOrthographic(AspectRatio * Zoom, Zoom, 0.0f, 0.1f);
        }

        public void Render()
        {
            Renderer.Render(Projection);
        }
    }
}
