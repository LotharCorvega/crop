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

        public float AspectRatio;
        public float Zoom;

        public Movement Movement = new Movement();
        private Matrix4 Projection;

        public Camera()
        {
        }

        public void Update(Window window, float elapsedtime)
        {
            Movement.Update(window, elapsedtime);

            Width = window.Size.X;
            Height = window.Size.Y;

            AspectRatio = Width / (float)Height;
            Zoom = -window.MouseState.Scroll.Y + 3.0f;

            Projection = Matrix4.CreateTranslation(-Movement.position.X, -Movement.position.Y, 0) * Matrix4.CreateOrthographic(AspectRatio * Zoom, Zoom, 0.0f, 0.1f);
        }

        public void Render()
        {
            Renderer.Render(Projection);
        }
    }
}
