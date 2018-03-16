using System;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace GameExercise
{
    public class TitleMenuState : IGameObject
    {
        private static readonly VertexPositionColor[] Vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-0.5f, 0f, 0f), new RgbaFloat(1.0f, 0.0f, 0.0f, 0.5f)),
                new VertexPositionColor(new Vector3(0.5f, 0f, 0f), new RgbaFloat(0.0f, 1.0f, 0.0f, 0.5f)),
                new VertexPositionColor(new Vector3(0f, 0.5f, 0f), new RgbaFloat(0.0f, 0.0f, 1.0f, 0.5f)),
            };

        private static readonly ushort[] Indices = new ushort[] { 0, 1, 2 };

        private readonly Camera camera;
        private readonly StateSystem stateSystem;
        private readonly IRendererContext context;

        private double currentRotation = 0;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private DeviceBuffer modelMatrixBuffer;
        private Pipeline pipeline;
        private ResourceSet projectionViewResourceSet;
        private ResourceSet modelTextureResourceSet;

        public TitleMenuState(StateSystem stateSystem, IRendererContext context)
        {
            if (stateSystem == null)
            {
                throw new ArgumentNullException(nameof(stateSystem));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.stateSystem = stateSystem;
            this.context = context;

            this.camera = new Camera(this.context.Window.Width, this.context.Window.Height);
            this.camera.Position = new Vector3(0f, 0f, 3f);

            this.CreateDeviceObjects();
        }

        public void Render()
        {
            CommandList commands = this.context.CommandList;
            commands.Begin();

            this.camera.WindowResized(context.Window.Width, context.Window.Height);

            var projection = this.camera.ProjectionMatrix;
            commands.UpdateBuffer(this.context.ProjectionMatrixBuffer, 0, projection);

            var view = this.camera.ViewMatrix;
            commands.UpdateBuffer(this.context.ViewMatrixBuffer, 0, view);

            var model = Matrix4x4.CreateRotationY(MathUtils.Radians((float)this.currentRotation));
            commands.UpdateBuffer(this.modelMatrixBuffer, 0, model);

            commands.SetFramebuffer(this.context.GraphicsDevice.SwapchainFramebuffer);

            // var framebufferWidth = this.context.GraphicsDevice.SwapchainFramebuffer.Width;
            // var framebufferHeight = this.context.GraphicsDevice.SwapchainFramebuffer.Height;
            // commands.SetViewport(0, new Viewport(0, 0, framebufferWidth, framebufferHeight, 0, 1));
            commands.SetFullViewports();
            commands.ClearColorTarget(0, RgbaFloat.Black);
            commands.ClearDepthStencil(1f);

            // Set all relevant state to draw our triangle.
            commands.SetPipeline(this.pipeline);
            commands.SetVertexBuffer(0, vertexBuffer);
            commands.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commands.SetGraphicsResourceSet(0, this.projectionViewResourceSet);
            commands.SetGraphicsResourceSet(1, this.modelTextureResourceSet);

            // Issue a Draw command for a single instance with 12 * 3 (6 faced with 2 triangles per face) indices.
            commands.DrawIndexed(
                indexCount: (uint)Indices.Length,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            context.CommandList.End();
            context.GraphicsDevice.SubmitCommands(context.CommandList);

            context.GraphicsDevice.SwapBuffers();
        }

        public void Update(double elapsedTime)
        {
            currentRotation += 10 * elapsedTime / 1000;
        }

        private void CreateDeviceObjects()
        {
            this.context.CommandList.Begin();

            this.modelMatrixBuffer = this.context.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            this.vertexBuffer = this.context.ResourceFactory.CreateBuffer(new BufferDescription((uint)Vertices.Length * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            this.context.CommandList.UpdateBuffer(vertexBuffer, 0, Vertices);

            this.indexBuffer = this.context.ResourceFactory.CreateBuffer(new BufferDescription((uint)Indices.Length * sizeof(ushort), BufferUsage.IndexBuffer));
            this.context.CommandList.UpdateBuffer(this.indexBuffer, 0, Indices);

            this.context.CommandList.End();
            this.context.GraphicsDevice.SubmitCommands(this.context.CommandList);
            this.context.GraphicsDevice.WaitForIdle();

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4))
                },
                new[]
                {
                    ShaderHelper.LoadShader(this.context.GraphicsDevice, this.context.ResourceFactory, ShaderStages.Vertex),
                    ShaderHelper.LoadShader(this.context.GraphicsDevice, this.context.ResourceFactory, ShaderStages.Fragment)
                });

            ResourceLayout projectionViewLayout = this.context.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout modelTextureLayout = this.context.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var rasterizeState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            this.pipeline = this.context.ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                rasterizeState,
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { projectionViewLayout, modelTextureLayout },
                this.context.GraphicsDevice.SwapchainFramebuffer.OutputDescription));

            this.projectionViewResourceSet = this.context.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                projectionViewLayout,
                this.context.ProjectionMatrixBuffer,
                this.context.ViewMatrixBuffer));

            this.modelTextureResourceSet = this.context.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                modelTextureLayout,
                this.modelMatrixBuffer));
        }
    }
}
