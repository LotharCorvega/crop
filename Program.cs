using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace crop
{
    class Program
    {
        static void Main(string[] args)
        {

            Vertex MyVertex = new Vertex();

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Title = "CROP",
                //Icon = 
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
