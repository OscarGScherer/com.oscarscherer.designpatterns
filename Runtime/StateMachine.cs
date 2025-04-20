using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public class StateMachine : MonoBehaviour
    {
        public abstract class Transition { public abstract Type Type(); }
        public class Transition<T> : Transition where T : State
        {
            public override Type Type() => typeof(T); 
        }

        public abstract class State
        {
            public State(GameObject gameObject) {}
            public virtual void OnEnter() {}
            public abstract Transition Update(float deltaTime);
            public virtual void OnExit() {}
            public virtual string ToDebugString() => "You can override \"ToDebugString()\" to display custom debug information about your state here!";
        }

        public State currentState;
        private Dictionary<Type,State> states = new Dictionary<Type, State>();

        private void AddState(Type stateType)
        {
            if(states.ContainsKey(stateType))
            {
                Debug.LogWarning("Adding a duplicate state of type " + stateType + " on state machine in " + name);
                return;
            }
            State state = (State) Activator.CreateInstance(stateType, gameObject);
            states.Add(state.GetType(), state);
        }

        private State GetStateOfType(Type stateType)
        {
            if(!states.ContainsKey(stateType)) AddState(stateType);
            return states[stateType];
        }

        protected void SetStartingState(Type stateType)
        {
            if(!states.ContainsKey(stateType)) AddState(stateType);
            currentState = GetStateOfType(stateType);
            currentState.OnEnter();
        }

        protected void UpdateStateMachine(float deltaTime)
        {
            Transition stateTransition = currentState.Update(deltaTime);
            HandleTransition(stateTransition);
        }

        private void HandleTransition(Transition stateTransition)
        {
            if(stateTransition == null) return;
            Type transition = stateTransition.Type();
            if(transition == currentState.GetType()) return;
            currentState.OnExit();
            currentState = GetStateOfType(transition);
            currentState.OnEnter();
        }
    }
}
