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
        public Vector2 camerapos = (0, 0);
        public Vector2 playerpos = (0, 0);
        public float movementspeed = 2.0f;
        public float zoom = 10;

        Vector2i gridsize = (10, 10);

        private float[] vertices;
        private uint[] indices;

        World defaultworld = new World();

        private void GenerateObjects()
        {
            Vertex testvertex = new Vertex();

            testvertex.Position.X = 0.5f;

            int i = 0, j = 0;
            uint k = 0;
            vertices = new float[gridsize.X * gridsize.Y * 16];
            indices = new uint[gridsize.X * gridsize.Y * 6];

            for (int y = 0; y < gridsize.Y; y++)
            {
                for (int x = 0; x < gridsize.X; x++)
                {
                    Vector2 grid = (x, y);
                    Vector2 norm = World.ToNormalized(grid);

                    float xp = norm.X;
                    float yp = norm.Y;

                    vertices[i + 0] = -1.0f + xp;
                    vertices[i + 1] = 0.5f + yp;
                    vertices[i + 2] = defaultworld.GetTextureCoordinates(x, y).X;
                    vertices[i + 3] = defaultworld.GetTextureCoordinates(x, y).Y;

                    vertices[i + 4] = 1.0f + xp;
                    vertices[i + 5] = 0.5f + yp;
                    vertices[i + 6] = defaultworld.GetTextureCoordinates(x, y).X + 0.125f;
                    vertices[i + 7] = defaultworld.GetTextureCoordinates(x, y).Y;

                    vertices[i + 8] = 1.0f + xp;
                    vertices[i + 9] = -0.5f + yp;
                    vertices[i + 10] = defaultworld.GetTextureCoordinates(x, y).X + 0.125f;
                    vertices[i + 11] = defaultworld.GetTextureCoordinates(x, y).Y + 0.125f;

                    vertices[i + 12] = -1.0f + xp;
                    vertices[i + 13] = -0.5f + yp;
                    vertices[i + 14] = defaultworld.GetTextureCoordinates(x, y).X;
                    vertices[i + 15] = defaultworld.GetTextureCoordinates(x, y).Y + 0.125f;

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

        Movement movement = new Movement();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            GenerateObjects();

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
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, Vertex.Size, 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, Vertex.Size, 2 * sizeof(float));

            //Load Texture
            _texture = Texture.LoadFromFile("assets/tiles.png");
            _texture.Use(TextureUnit.Texture0);

            //Allow Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            float aspectratio = Size.X / (float)Size.Y;
            Matrix4 projection = Matrix4.CreateTranslation(-camerapos.X, -camerapos.Y, 0) * Matrix4.CreateOrthographic(aspectratio * zoom, zoom, 0.0f, 0.1f);

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
            var mousestate = MouseState;

            movement.Update(this, (float)e.Time);

            playerpos = movement.position;

            camerapos = playerpos;//World.ToNormalized(playerpos);

            zoom = -mousestate.Scroll.Y + 3.0f;

            string info = $"{(int)(1 / e.Time)} FPS, " +
                           "Pos: (" + string.Format("{0:0.00}", movement.position.X) + ", " + string.Format("{0:0.00}", movement.position.Y) + ") " +
                           "Vel: (" + string.Format("{0:0.00}", movement.velocity.X) + ", " + string.Format("{0:0.00}", movement.velocity.Y) + ") ";


            Title = info;

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
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);

            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);

            GL.DeleteProgram(_shader.Handle);
            GL.DeleteTexture(_texture.Handle);
            base.OnUnload();
        }
    }
}