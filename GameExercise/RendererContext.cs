using System;
using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace GameExercise
{
    public class RendererContext : IRendererContext
    {
        private readonly DeviceBuffer projectionBuffer;
        private readonly DeviceBuffer viewBuffer;
        private readonly DeviceBuffer modelBuffer;

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

            this.CommandList = factory.CreateCommandList();
            this.projectionBuffer = this.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            this.viewBuffer = this.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            this.modelBuffer = this.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4))
                },
                new[]
                {
                    LoadShader(graphicsDevice, factory, ShaderStages.Vertex),
                    LoadShader(graphicsDevice, factory, ShaderStages.Fragment)
                });

            ResourceLayout projectionViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout modelTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var rasterizeState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            this.Pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                rasterizeState,
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { projectionViewLayout, modelTextureLayout },
                graphicsDevice.SwapchainFramebuffer.OutputDescription));

            this.ProjectionViewResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                projectionViewLayout,
                projectionBuffer,
                viewBuffer));

            this.ModelTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                modelTextureLayout,
                modelBuffer));

            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathUtils.Radians(45.0f),
                (float)window.Width / window.Height,
                0.1f,
                100f);
            this.SetProjectionMatrix(projection);

            var view = Matrix4x4.CreateLookAt(
              new Vector3(4, 3, 3),
              new Vector3(0, 0, 0),
              new Vector3(0, 1, 0));
            this.SetViewMatrix(view);

            // identity matrix, model is at 0,0,0 location
            var model = Matrix4x4.Identity;
            this.SetModelMatrix(model);
        }

        public Sdl2Window Window { get; }

        public GraphicsDevice GraphicsDevice { get; }

        public ResourceFactory ResourceFactory { get; }

        public CommandList CommandList { get; }

        public Pipeline Pipeline { get; }

        public ResourceSet ProjectionViewResourceSet { get; }

        public ResourceSet ModelTextureResourceSet { get; }

        public void SetProjectionMatrix(Matrix4x4 projection)
        {
            this.CommandList.Begin();

            this.GraphicsDevice.UpdateBuffer(this.projectionBuffer, 0, projection);

            this.CommandList.End();
            this.GraphicsDevice.SubmitCommands(this.CommandList);
            this.GraphicsDevice.WaitForIdle();
        }

        public void SetViewMatrix(Matrix4x4 view)
        {
            this.CommandList.Begin();

            this.GraphicsDevice.UpdateBuffer(this.viewBuffer, 0, view);

            this.CommandList.End();
            this.GraphicsDevice.SubmitCommands(this.CommandList);
            this.GraphicsDevice.WaitForIdle();
        }

        public void SetModelMatrix(Matrix4x4 model)
        {
            this.CommandList.Begin();

            this.GraphicsDevice.UpdateBuffer(this.modelBuffer, 0, model);

            this.CommandList.End();
            this.GraphicsDevice.SubmitCommands(this.CommandList);
            this.GraphicsDevice.WaitForIdle();
        }

        private static Shader LoadShader(GraphicsDevice graphicsDevice, ResourceFactory factory, ShaderStages stage)
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
