using Moq;
using Xunit;

namespace GameExercise.Tests
{
    public class StateSystemFixture
    {
        [Fact]
        public void TestAddedStateExists()
        {
            // arrange
            var stateSystem = new StateSystem();
            stateSystem.AddState("splash", new SplashScreenState (stateSystem, 
            Mock.Of<IRendererContext>()));

            // act
            bool exists = stateSystem.Exists("splash");

            // assert
            Assert.True(exists);
        }
    }
}
