using System;

namespace GameExercise
{
    public class TitleMenuState : IGameObject
    {
        public void Render()
        {
            Console.WriteLine("Rendering Title");
        }

        public void Update(double elapsedTime)
        {
            Console.WriteLine("Updating Title");
        }
    }
}
