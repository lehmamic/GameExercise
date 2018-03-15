using System;
using System.Collections.Generic;

namespace GameExercise
{
    public class StateSystem
    {
        private readonly Dictionary<string, IGameObject> stateStore = new Dictionary<string, IGameObject>();

        private IGameObject currentState = null;

        public void Update(double elapsedTime)
        {
            if (currentState == null)
            {
                return; // nothing to update
            }

            currentState.Update(elapsedTime);
        }
        public void Render()
        {
            if (currentState == null)
            {
                return; // nothing to render
            }

            currentState.Render();
        }
        public void AddState(string stateId, IGameObject state)
        {
            System.Diagnostics.Debug.Assert( Exists(stateId) == false );
            stateStore.Add(stateId, state);
        }

        public void ChangeState(string stateId)
        {
            if(!Exists(stateId))
            {
                throw new ArgumentException("A state with the provided id does not exist.", nameof(stateId));
            }

            currentState = stateStore[stateId];
        }
        public bool Exists(string stateId)
        {
            return stateStore.ContainsKey(stateId);
        }
    }
}
