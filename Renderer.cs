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
        const int MaxSprites = 100;
        const int MaxTextures = 6;

        private static int VertexArrayObject;
        private static int SpriteBuffer;

        private static int ShaderProgram;
        private static int[] Textures;
        private static string[] TextureImports;

        public struct Sprite
        {
            public float X;
            public float Y;

            public float Width;
            public float Height;

            public float U;
            public float V;

            public float TexWidth;
            public float TexHeight;
        }

        public static Sprite[] Sprites;

        public static void Initialize()
        {
            //Initialize VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //Initialize IBO
            SpriteBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, SpriteBuffer);

            Sprites = new Sprite[MaxSprites];

            Random rand = new Random();

            for(int i = 0; i < MaxSprites; i++)
            {
                Sprites[i].X = (float)rand.NextDouble();
                Sprites[i].Y = (float)rand.NextDouble();

                Sprites[i].Width = 2.0F;
                Sprites[i].Height = 1.0F;

                Sprites[i].U = 0.0F;
                Sprites[i].V = 0.0F;

                Sprites[i].TexWidth = 1.0F;
                Sprites[i].TexHeight = 1.0F;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(Sprite) * Sprites.Length, Sprites, BufferUsageHint.DynamicDraw);

            //Set VAO (Position)
            GL.EnableVertexArrayAttrib(VertexArrayObject, 0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(Sprite), 0 * sizeof(float));

            GL.EnableVertexArrayAttrib(VertexArrayObject, 1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(Sprite), 2 * sizeof(float));

            GL.EnableVertexArrayAttrib(VertexArrayObject, 2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Sprite), 4 * sizeof(float));

            GL.EnableVertexArrayAttrib(VertexArrayObject, 3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, sizeof(Sprite), 6 * sizeof(float));

            //Load Shaders
            var ShaderSource = File.ReadAllText("Shaders/shader.vert");
            var VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, ShaderSource);
            GL.CompileShader(VertexShader);

            ShaderSource = File.ReadAllText("Shaders/shader.geom");
            var GeometryShader = GL.CreateShader(ShaderType.GeometryShader);
            GL.ShaderSource(GeometryShader, ShaderSource);
            GL.CompileShader(GeometryShader);

            ShaderSource = File.ReadAllText("Shaders/shader.frag");
            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, ShaderSource);
            GL.CompileShader(FragmentShader);

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, GeometryShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);

            GL.DetachShader(ShaderProgram, VertexShader);
            GL.DetachShader(ShaderProgram, GeometryShader);
            GL.DetachShader(ShaderProgram, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(GeometryShader);
            GL.DeleteShader(VertexShader);

            //Load Texture
            TextureImports = new string[]
            {
                "assets/empty.png",
                "assets/test.png",
                "assets/grass.png",
                "assets/trunk.png",
                "assets/ascii.png",
                "assets/tiles.png",
            };
            Textures = new int[MaxTextures];

            for (int i = 0; i < MaxTextures; i++)
            {
                Textures[i] = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, Textures[i]);

                using (var Bitmap = new Bitmap(TextureImports[i]))
                {
                    var ImageTexture = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Bitmap.Width, Bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, ImageTexture.Scan0);
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            int[] TextureSlots = new int[MaxTextures];
            for (int i = 0; i < MaxTextures; i++)
                TextureSlots[i] = i;

            GL.UseProgram(ShaderProgram);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "Textures"), MaxTextures, TextureSlots);

            //Allow Transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.368f, 0.5f, 0.3f, 1.0f);
        }

        public static void Render(Matrix4 ViewMatrix)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(ShaderProgram);
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "ViewMatrix"), true, ref ViewMatrix);

            GL.BindVertexArray(SpriteBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(Sprite) * Sprites.Length, Sprites, BufferUsageHint.DynamicDraw);
            GL.DrawArrays(PrimitiveType.Points, 0, Sprites.Length);
        }

        public static void Unload()
        {
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);

            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteVertexArray(SpriteBuffer);

            GL.DeleteProgram(ShaderProgram);
            for (int i = 0; i < MaxTextures; i++)
                GL.DeleteTexture(Textures[i]);
        }
    }
}