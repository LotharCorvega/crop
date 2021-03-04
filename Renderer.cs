using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace crop
{
    class Renderer
    {
        public Renderer()
        {
        }
        
        const  int MaxQuads = 10;
        const  int MaxVertices = MaxQuads * 16;
        const  int MaxIndices = MaxQuads * 6;
        const  int MaxTextures = 32;

        static int VertexArrayObject;
        public static int VertexBufferObject;
        static int ElementBufferObject;

        static int ShaderProgram;
        static int Texture;

        //private static  Vertex[] Vertices;
        static float[] Vertices;
        static private int[] Indices;

        public static void Initialize()
        {
            //Initialize VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //Position
            GL.EnableVertexArrayAttrib(VertexArrayObject, 0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vertex.Size, 0 * sizeof(float));
            //Texture Coordinates
            GL.EnableVertexArrayAttrib(VertexArrayObject, 1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size, 2 * sizeof(float));

            //Initialize VBO
            Vertices = new float[MaxVertices];

            float j = 1.0f;

            for (int i = 0; i < MaxVertices; i += 16)
            {
                Vertices[i + 0] = 0.0f + j;
                Vertices[i + 1] = 0.0f;
                Vertices[i + 2] = 0.0f;
                Vertices[i + 3] = 0.0f;

                Vertices[i + 4] = 1.0f + j;
                Vertices[i + 5] = 0.0f;
                Vertices[i + 6] = 1.0f;
                Vertices[i + 7] = 0.0f;

                Vertices[i + 8] = 1.0f + j;
                Vertices[i + 9] = 1.0f;
                Vertices[i + 10] = 1.0f;
                Vertices[i + 11] = 1.0f;

                Vertices[i + 12] = 0.0f + j;
                Vertices[i + 13] = 1.0f;
                Vertices[i + 14] = 0.0f;
                Vertices[i + 15] = 1.0f;

                j++;
            }

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxVertices * Vertex.Size, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //Initialize EBO
            Indices = new int[MaxIndices];

            for (int i = 0; i < MaxIndices; i += 6)
            {
                Indices[i + 0] = 0;
                Indices[i + 1] = 1;
                Indices[i + 2] = 2;
                Indices[i + 3] = 0;
                Indices[i + 4] = 2;
                Indices[i + 5] = 3;
            }

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, MaxIndices * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            //Set up Shaders
            var ShaderSource = File.ReadAllText("Shaders/shader.vert");        //Here a using statement for the Source code would be good for efficiency but is tricky with strings 
            var VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, ShaderSource);
            GL.CompileShader(VertexShader);

            ShaderSource = File.ReadAllText("Shaders/shader.frag");
            var FragmentShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(FragmentShader, ShaderSource);
            GL.CompileShader(FragmentShader);

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);
            GL.UseProgram(ShaderProgram);

            //Set up Textures
            Texture = GL.GenTexture();         //Must be changed soon to support multiple textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.ProxyTexture2D, Texture);

            using (var Bitmap = new Bitmap("assets/tiles.png"))
            {
                var ImageTexture = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Bitmap.Width, Bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, ImageTexture.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            //Allow Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //Set Clear Color (obviously)
            GL.ClearColor(0.368f, 0.5f, 0.3f, 1.0f);
        }

        public static void Render(Matrix4 Projection)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BufferData(BufferTarget.ArrayBuffer, MaxVertices * Vertex.Size, Vertices, BufferUsageHint.DynamicDraw);
            GL.BindVertexArray(VertexArrayObject);

            GL.UseProgram(ShaderProgram);
            GL.UniformMatrix4(1, true, ref Projection);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);

            GL.DrawElements(PrimitiveType.Triangles, MaxIndices, DrawElementsType.UnsignedInt, Indices);
        }
    }
}
