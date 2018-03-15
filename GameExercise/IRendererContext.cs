using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace GameExercise
{
    public interface IRendererContext
    {
        Sdl2Window Window { get; }

        GraphicsDevice GraphicsDevice { get; }

        ResourceFactory ResourceFactory { get; }

        CommandList CommandList { get; }

        Pipeline Pipeline { get; }

        ResourceSet ProjectionViewResourceSet { get; }

        ResourceSet ModelTextureResourceSet { get; }

        void SetProjectionMatrix(Matrix4x4 projection);

        void SetViewMatrix(Matrix4x4 view);

        void SetModelMatrix(Matrix4x4 model);
    }
}
