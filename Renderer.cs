using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace crop
{
    unsafe class Renderer
    {
        const int MaxQuads = 100;
        const int MaxVertices = MaxQuads * 4;         //Problematic! Here not the actual maximal Amout of vertices is meant, just the maximal amout of floats in the vertex buffer
        const int MaxIndices = MaxQuads * 6;
        const int MaxTextures = 32;

        private static int VertexArrayObject;
        private static int VertexBufferObject;
        private static int ElementBufferObject;

        private static int ShaderProgram;
        private static int Texture1;

        private static uint[] Indices;
        private static Vertex[] Vertices;

        struct Vertex
        {
            public float X;
            public float Y;

            public float U;
            public float V;
        }

        public static void Initialize()
        {
            //Initialize VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //Set VBO
            Vertices = new Vertex[MaxVertices];

            Vertices[0].X = -1.0f;
            Vertices[0].Y = 0.5f;
            Vertices[0].U = 0.0f;
            Vertices[0].V = 0.0f;

            Vertices[1].X = 1.0f;
            Vertices[1].Y = 0.5f;
            Vertices[1].U = 1.0f;
            Vertices[1].V = 0.0f;

            Vertices[2].X = 1.0f;
            Vertices[2].Y = -0.5f;
            Vertices[2].U = 1.0f;
            Vertices[2].V = 1.0f;

            Vertices[3].X = -1.0f;
            Vertices[3].Y = -0.5f;
            Vertices[3].U = 0.0f;
            Vertices[3].V = 1.0f;

            //Initialize VBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxVertices * sizeof(Vertex), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //Set EBO
            Indices = new uint[MaxIndices];

            for (uint i = 0, j = 0; i < MaxIndices; i += 6)
            {
                Indices[i + 0] = 0 + j;
                Indices[i + 1] = 1 + j;
                Indices[i + 2] = 2 + j;
                Indices[i + 3] = 0 + j;
                Indices[i + 4] = 2 + j;
                Indices[i + 5] = 3 + j;

                j += 4;
            }

            //Initialize EBO
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, MaxIndices * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            //Set VAO (Position)
            GL.EnableVertexArrayAttrib(VertexArrayObject, 0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), 0 * sizeof(float));

            //Set VAO (Texture Coordinates)
            GL.EnableVertexArrayAttrib(VertexArrayObject, 1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), 2 * sizeof(float));

            //Load Shaders
            var ShaderSource = File.ReadAllText("Shaders/shader.vert");
            var VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, ShaderSource);
            GL.CompileShader(VertexShader);

            ShaderSource = File.ReadAllText("Shaders/shader.frag");
            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, ShaderSource);
            GL.CompileShader(FragmentShader);

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);

            GL.DetachShader(ShaderProgram, VertexShader);
            GL.DetachShader(ShaderProgram, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            //Load Texture
            Texture1 = GL.GenTexture();                                //Must be changed soon to support multiple textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture1);

            using (var Bitmap = new Bitmap("assets/test.png"))
            {
                var ImageTexture = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Bitmap.Width, Bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, ImageTexture.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture0);                 //Idk if necessary, doesn't break it if removed
            GL.BindTexture(TextureTarget.Texture2D, Texture1);

            //Allow Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.368f, 0.5f, 0.3f, 1.0f);
        }

        public static void Render(Matrix4 Projection)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(VertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxVertices * sizeof(Vertex), Vertices, BufferUsageHint.DynamicDraw);

            GL.UseProgram(ShaderProgram);
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "transform"), true, ref Projection);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture1);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public static void Unload()
        {
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);

            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);

            GL.DeleteProgram(ShaderProgram);
            GL.DeleteTexture(Texture1);
        }
    }
}
