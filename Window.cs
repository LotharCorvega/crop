using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace crop
{
    public class Window : GameWindow
    {
        public Vector2 playerpos = (0, 0);
        public float movementspeed = 2.0f;
        public float zoom = 4;

        Vector2i gridsize = (10, 10);

        private int sizeofvertex = 4;


        private readonly float[] vertices =
        {
        //   Position     Texture coordinates
            -1.0f,  0.5f, 0.0f, 0.0f, //First Tile
             1.0f,  0.5f, 1.0f, 0.0f,
             1.0f, -0.5f, 1.0f, 1.0f,
            -1.0f, -0.5f, 0.0f, 1.0f,

             1.0f,  0.5f, 0.0f, 0.0f, //Second Tile
             3.0f,  0.5f, 1.0f, 0.0f,
             3.0f, -0.5f, 1.0f, 1.0f,
             1.0f, -0.5f, 0.0f, 1.0f,
        };

        private uint[] indices =
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7,
        };

        private int VertexBufferObject;
        private int VertexArrayObject;
        private int ElementBufferObject;

        private Shader _shader;
        private Texture _texture;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.368f, 0.5f, 0.3f, 1.0f);

            //Initialize VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //Initialize VBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //Initialize EBO
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, sizeofvertex * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, sizeofvertex * sizeof(float), 2 * sizeof(float));

            //Load Texture
            _texture = Texture.LoadFromFile("assets/test.png");
            _texture.Use(TextureUnit.Texture0);

            //Allow Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            float aspectratio = Size.X / (float)Size.Y;
            Matrix4 projection = Matrix4.CreateTranslation(-playerpos.X, -playerpos.Y, 0) * Matrix4.CreateOrthographic(aspectratio * zoom, zoom, 0.0f, 0.1f);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(VertexArrayObject);
            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            _shader.SetMatrix4("transform", projection);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboardstate = KeyboardState;
            var mousestate = MouseState;
            float elapsedtime = (float)e.Time;

            if (keyboardstate.IsKeyDown(Keys.W))
                playerpos.Y += movementspeed * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.S))
                playerpos.Y -= movementspeed * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.D))
                playerpos.X += movementspeed * elapsedtime;
            if (keyboardstate.IsKeyDown(Keys.A))
                playerpos.X -= movementspeed * elapsedtime;

            zoom = mousestate.Scroll.Y + 3.0f;

            Title = $"{(int)(1 / elapsedtime)} FPS, {vertices.Length / sizeofvertex} vertices";

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
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            GL.DeleteTexture(_texture.Handle);
            base.OnUnload();
        }
    }
}