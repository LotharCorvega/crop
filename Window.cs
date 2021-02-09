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

        private float[] vertices;
        private uint[] indices;

        private int sizeofvertex = 4;

        private void GenerateObjects()
        {
            int i = 0, j = 0;
            uint k = 0;
            vertices = new float[gridsize.X * gridsize.Y * 16];
            indices = new uint[gridsize.X * gridsize.Y * 6];

            for (int y = 0; y < gridsize.Y; y++)
            {
                for (int x = 0; x < gridsize.X; x++)
                {
                    float xp = (x + y) * 22.0f / 21.0f;
                    float yp = 0.5f * (y - x);

                    vertices[i + 0] = -1.0f + xp;
                    vertices[i + 1] = 0.5f + yp;
                    vertices[i + 2] = 0.0f;
                    vertices[i + 3] = 0.0f;

                    vertices[i + 4] = 1.0f + xp;
                    vertices[i + 5] = 0.5f + yp;
                    vertices[i + 6] = 1.0f;
                    vertices[i + 7] = 0.0f;

                    vertices[i + 8] = 1.0f + xp;
                    vertices[i + 9] = -0.5f + yp;
                    vertices[i + 10] = 1.0f;
                    vertices[i + 11] = 1.0f;

                    vertices[i + 12] = -1.0f + xp;
                    vertices[i + 13] = -0.5f + yp;
                    vertices[i + 14] = 0.0f;
                    vertices[i + 15] = 1.0f;

                    indices[j + 0] = 0 + k;
                    indices[j + 1] = 1 + k;
                    indices[j + 2] = 2 + k;
                    indices[j + 3] = 0 + k;
                    indices[j + 4] = 2 + k;
                    indices[j + 5] = 3 + k;

                    i += 16;
                    j += 6;
                    k += 4;
                }
            }
        }

        /*
        private readonly float[] _vertices =
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

        private uint[] _indices =
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7,
        };*/

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
            GenerateObjects();
            System.Diagnostics.Debug.WriteLine(vertices.Length);

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
            _texture = Texture.LoadFromFile("assets/grass.png");
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