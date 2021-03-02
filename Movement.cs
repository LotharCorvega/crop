using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace crop
{
    class Movement
    {
        public Vector2 position;
        public Vector2 velocity;

        float movementspeed;
        float acceleration;
        float roughness;

        public Movement()
        {
            position = (0.0f, 0.0f);
            velocity = (0.0f, 0.0f);

            movementspeed = 2.0f;
            acceleration = 20.0f;
            roughness = 0.99f;
        }

        public void Update(Window window, float elapsedtime)
        {
            var keyboardstate = window.KeyboardState;
            var mousestate = window.MouseState;

            if (keyboardstate.IsKeyDown(Keys.W))
                velocity.Y += acceleration * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.S))
                velocity.Y -= acceleration * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.D))
                velocity.X += acceleration * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.A))
                velocity.X -= acceleration * elapsedtime;

            if (velocity.LengthFast > movementspeed)
            {
                velocity.NormalizeFast();
                velocity *= movementspeed;
            }

            position += velocity * elapsedtime;

            velocity *= roughness;
        }
    }
}
