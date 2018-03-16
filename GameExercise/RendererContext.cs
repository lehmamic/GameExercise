using System;
using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace GameExercise
{
    public class RendererContext : IRendererContext
    {
        public RendererContext(Sdl2Window window, GraphicsDevice graphicsDevice, ResourceFactory factory)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.Window = window;
            this.GraphicsDevice = graphicsDevice;
            this.ResourceFactory = factory;

            this.CommandList = this.ResourceFactory.CreateCommandList();
            this.ProjectionMatrixBuffer = this.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            this.ViewMatrixBuffer = this.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        }

        public Sdl2Window Window { get; }

        public GraphicsDevice GraphicsDevice { get; }

        public ResourceFactory ResourceFactory { get; }

        public CommandList CommandList { get; }

        public DeviceBuffer ProjectionMatrixBuffer { get; }

        public DeviceBuffer ViewMatrixBuffer { get; }
    }
}
