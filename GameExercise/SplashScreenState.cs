using System;

namespace GameExercise
{
    public class SplashScreenState : IGameObject
    {
        private readonly StateSystem stateSystem;

        public SplashScreenState(StateSystem stateSystem)
        {
            this.stateSystem = stateSystem;
        }

        public void Render()
        {
            Console.WriteLine("Rendering Splash");
        }

        public void Update(double elapsedTime)
        {
            Console.WriteLine("Updating Splash");
        }
    }
}
