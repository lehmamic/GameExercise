using System;
using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace GameExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            var windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 1024,
                WindowHeight = 768,
                WindowTitle = "Veldrid Tutorial"
            };

            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                Debug = true,
                SwapchainDepthFormat = PixelFormat.R16_UNorm
            };

            GraphicsDevice graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGL);

            var factory = new DisposeCollectorResourceFactory(graphicsDevice.ResourceFactory);
            var context = new RendererContext(window, graphicsDevice, factory);

            var stateSystem = new StateSystem();
            stateSystem.AddState("splash", new SplashScreenState(stateSystem, context));
            stateSystem.AddState("title_menu", new TitleMenuState(stateSystem, context));

            // Select the start state
            stateSystem.ChangeState("splash");

            Stopwatch stopwatch = Stopwatch.StartNew();
            long previousFrameTicks = 0;

            while (window.Exists)
            {
                long currentFrameTicks = stopwatch.ElapsedTicks;
                double deltaMilliseconds = (currentFrameTicks - previousFrameTicks) * (1000.0 / Stopwatch.Frequency);

                window.PumpEvents();

                stateSystem.Update(deltaMilliseconds);
                stateSystem.Render();
            }
        }
    }
}
