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

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Camera1.Update(this, (float)e.Time);
            Camera1.Render();

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
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
    }
}