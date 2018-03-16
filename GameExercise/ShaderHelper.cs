using System.IO;
using Veldrid;

namespace GameExercise
{
    public class ShaderHelper
    {
        public static Shader LoadShader(GraphicsDevice graphicsDevice, ResourceFactory factory, ShaderStages stage)
        {
            string extension = null;
            switch (graphicsDevice.BackendType)
            {
                case GraphicsBackend.OpenGL:
                    extension = "glsl";
                    break;
                default: throw new System.InvalidOperationException();
            }

            string entryPoint = stage == ShaderStages.Vertex ? "VS" : "FS";
            string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"{stage.ToString()}.{extension}");
            byte[] shaderBytes = File.ReadAllBytes(path);

            return factory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }
    }
}
