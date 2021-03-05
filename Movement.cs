using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace crop
{
    class Movement
    {
        public Vector2 position;
        public Vector2 velocity;

        float movementspeed;
        float acceleration;
        float deceleration;

        public Movement()
        {
            position = (0.0f, 0.0f);
            velocity = (0.0f, 0.0f);

            movementspeed = 4.0f;
            acceleration = 20.0f;
            deceleration = 10.0f;
        }

        public void Update(Window window, float elapsedtime)
        {
            var keyboardstate = window.KeyboardState;
            var mousestate = window.MouseState;

            float da = acceleration * elapsedtime;

            if (keyboardstate.IsKeyDown(Keys.W))
                velocity.Y += da;
            if (keyboardstate.IsKeyDown(Keys.S))
                velocity.Y -= da;
            if (keyboardstate.IsKeyDown(Keys.D))
                velocity.X += da;
            if (keyboardstate.IsKeyDown(Keys.A))
                velocity.X -= da;

            if (!keyboardstate.IsAnyKeyDown)
                velocity -= velocity * movementspeed * elapsedtime;

            if (velocity.LengthFast > movementspeed)
            {
                velocity.NormalizeFast();
                velocity *= movementspeed;
            }

            position += velocity * elapsedtime;
        }
    }
}
