using System;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace GameExercise
{
    public class TitleMenuState : IGameObject
    {
        private readonly StateSystem stateSystem;
        private readonly IRendererContext context;

        private double currentRotation = 0;
        private VertexPositionColor[] vertices;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;

        public TitleMenuState(StateSystem stateSystem, IRendererContext context)
        {
            if(stateSystem == null)
            {
                throw new ArgumentNullException(nameof(stateSystem));
            }

            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.stateSystem = stateSystem;
            this.context = context;

            this.context.CommandList.Begin();

            this.vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-0.5f, 0f, 0f), new RgbaFloat(1.0f, 0.0f, 0.0f, 0.5f)),
                new VertexPositionColor(new Vector3(0.5f, 0f, 0f), new RgbaFloat(0.0f, 1.0f, 0.0f, 0.5f)),
                new VertexPositionColor(new Vector3(0f, 0.5f, 0f), new RgbaFloat(0.0f, 0.0f, 1.0f, 0.5f)),
            };
            this.vertexBuffer = this.context.ResourceFactory.CreateBuffer(new BufferDescription((uint)this.vertices.Length * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            this.context.CommandList.UpdateBuffer(vertexBuffer, 0, this.vertices);

            ushort[] verticesIndices = Enumerable.Range(0, vertices.Length)
                .Select(i => (ushort)i)
                .ToArray();
            this.indexBuffer = this.context.ResourceFactory.CreateBuffer(new BufferDescription((uint)this.vertices.Length * sizeof(ushort), BufferUsage.IndexBuffer));
            this.context.CommandList.UpdateBuffer(this.indexBuffer, 0, verticesIndices);

            this.context.CommandList.End();
            this.context.GraphicsDevice.SubmitCommands(this.context.CommandList);
            this.context.GraphicsDevice.WaitForIdle();
        }

        public void Render()
        {
            var modelMatrix = Matrix4x4.CreateRotationX(MathUtils.Radians((float)this.currentRotation));
            this.context.SetModelMatrix(modelMatrix);

            context.CommandList.Begin();

            this.context.CommandList.SetFramebuffer(this.context.GraphicsDevice.SwapchainFramebuffer);
            this.context.CommandList.SetFullViewports();
            this.context.CommandList.ClearColorTarget(0, RgbaFloat.Black);
            this.context.CommandList.ClearDepthStencil(1f);

            // Set all relevant state to draw our triangle.
            this.context.CommandList.SetPipeline(this.context.Pipeline);
            this.context.CommandList.SetVertexBuffer(0, vertexBuffer);
            this.context.CommandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            this.context.CommandList.SetGraphicsResourceSet(0, this.context.ProjectionViewResourceSet);
            this.context.CommandList.SetGraphicsResourceSet(1, this.context.ModelTextureResourceSet);

            // Issue a Draw command for a single instance with 12 * 3 (6 faced with 2 triangles per face) indices.
            this.context.CommandList.DrawIndexed(
                indexCount: (uint)this.vertices.Length,
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
            currentRotation = 10 * elapsedTime / 1000;
        }
    }
}
