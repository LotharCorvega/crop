using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace crop
{
    public class Window : GameWindow
    {
        Camera Camera1 = new Camera();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            Renderer.Initialize();
            //DrawTest();

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Camera1.Update(this, (float)e.Time);
            Camera1.Render();

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        World World1 = new World();
        double timepassed;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            timepassed += e.Time;

            World1.DrawChunck((byte)(32 + (int)(timepassed * 10) % 11));

            Title = $"{(int)(1 / e.Time)} FPS, " +
                           "Pos: (" + string.Format("{0:0.00}", Camera1.Movement.position.X) + ", " + string.Format("{0:0.00}", Camera1.Movement.position.Y) + ") " +
                           "Vel: (" + string.Format("{0:0.00}", Camera1.Movement.velocity.X) + ", " + string.Format("{0:0.00}", Camera1.Movement.velocity.Y) + ") ";

            base.OnUpdateFrame(e);
        }

        public Vector2i WindowSize;

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            WindowSize = (e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            Renderer.Unload();

            base.OnUnload();
        }

        private void DrawTest()
        {
            Renderer.Sprites[0].X = 0.0F;
            Renderer.Sprites[0].Y = 0.0F;

            Renderer.Sprites[0].Width = 2.0F;
            Renderer.Sprites[0].Height = 1.0F;

            Renderer.Sprites[0].U = 0.0F;
            Renderer.Sprites[0].V = 0.0F;

            Renderer.Sprites[0].TexWidth = 0.125F;
            Renderer.Sprites[0].TexHeight = 0.125F;
        }
    }
}