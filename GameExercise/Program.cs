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
            stateSystem.AddState("sprite_test", new DrawSpriteState(stateSystem, context));

            // Select the start state
            stateSystem.ChangeState("sprite_test");

            Stopwatch stopwatch = Stopwatch.StartNew();
            long previousFrameTicks = 0;

            while (window.Exists)
            {
                long currentFrameTicks = stopwatch.ElapsedTicks;
                double deltaMilliseconds = (currentFrameTicks - previousFrameTicks) * (1000.0 / Stopwatch.Frequency);
                previousFrameTicks = currentFrameTicks;

                Console.WriteLine($"Delta milliseconds: {deltaMilliseconds}");

                window.PumpEvents();

                stateSystem.Update(deltaMilliseconds / 1000);
                stateSystem.Render();
            }
        }
    }
}
