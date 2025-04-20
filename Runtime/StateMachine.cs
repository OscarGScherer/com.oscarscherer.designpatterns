using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public class StateMachine : MonoBehaviour
    {
        public abstract class State
        {
            public abstract void Initialize(GameObject gameObject);
            public virtual void OnEnter() {}
            public abstract State Update(float deltaTime);
            public virtual void OnExit() {}
            public virtual string ToDebugString() => "You can override \"ToDebugString()\" to display custom debug information about your state here!";
        }

        public State currentState;
        protected Dictionary<Type,State> states = new Dictionary<Type, State>();

        protected void AddState(State newState)
        {
            if(states.ContainsKey(newState.GetType()))
            {
                Debug.LogWarning("Adding a duplicate state of type " + newState.GetType() + " on state machine in " + name);
                return;
            }
            newState.Initialize(gameObject);
            states.Add(newState.GetType(), newState);
        }

        protected void AddState<T>() where T : State, new() => AddState(new T());

        protected State GetStateOfSameTypeAs(State state)
        {
            if(!states.ContainsKey(state.GetType())) AddState(state);
            return states[state.GetType()];
        }

        protected State GetStateOfType<T>() where T : State, new()
        {
            if(!states.ContainsKey(typeof(T))) AddState<T>();
            return states[typeof(T)];
        }

        protected void SetStartingState<T>() where T : State, new()
        {
            if(!states.ContainsKey(typeof(T))) AddState<T>();
            currentState = GetStateOfType<T>();
            currentState.OnEnter();
        }

        protected void UpdateStateMachine(float deltaTime)
        {
            State newState = currentState.Update(deltaTime);
            HandleTransition(newState);
        }

        protected void HandleTransition(State newState)
        {
            if(newState == null) return;
            if(newState.GetType() == currentState.GetType()) return;
            currentState.OnExit();
            currentState = GetStateOfSameTypeAs(newState);
            currentState.OnEnter();
        }
    }
}
