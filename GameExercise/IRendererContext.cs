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

        DeviceBuffer ProjectionMatrixBuffer { get; }

        DeviceBuffer ViewMatrixBuffer { get; }
    }
}
