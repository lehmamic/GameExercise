using System;
using Veldrid;

namespace GameExercise
{
    public class SplashScreenState : IGameObject
    {
        private readonly StateSystem stateSystem;
        private readonly IRendererContext context;
        private readonly Camera camera;

        private double delayInMilliseconds = 3000;

        public SplashScreenState(StateSystem stateSystem, IRendererContext context)
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
            this.camera = new Camera(this.context.Window.Width, this.context.Window.Height);
        }

        public void Render()
        {
            context.CommandList.Begin();

            context.CommandList.SetFramebuffer(context.GraphicsDevice.SwapchainFramebuffer);
            context.CommandList.SetFullViewports();
            context.CommandList.ClearColorTarget(0, RgbaFloat.White);

            context.CommandList.End();
            context.GraphicsDevice.SubmitCommands(context.CommandList);

            context.GraphicsDevice.SwapBuffers();
        }

        public void Update(double elapsedTime)
        {
            this.delayInMilliseconds -= elapsedTime;
            if (this.delayInMilliseconds <= 0)
            {
                this.delayInMilliseconds = 3;
                this.stateSystem.ChangeState("title_menu");
            }
        }
    }
}
