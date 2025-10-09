using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    namespace StateMachine
    {
        public abstract class State
        {
            /// <returns> Text to be printed on the inpector, whenver the SM is in this state. </returns>
            public virtual string ToDebugString() => "You can override <color=#FFDE59>ToDebugString</color>() to display custom debug information about your state here!";
        }

        /// <summary>
        /// Base state class. Inherit from it to create your own state.
        /// Each state machine can only have one state of each type of State.
        /// </summary>
        public abstract class State<TStateMachine, TState, TStateInput> : State
            where TState : State<TStateMachine, TState, TStateInput>, new()
            where TStateMachine : StateMachine<TStateMachine, TState, TStateInput>
        {
            public abstract void Initialize(TStateMachine stateMachine);
            /// <summary>
            /// Called when the SM enters this state.
            /// </summary>
            public virtual void OnEnter() { }
            public abstract TState Update(TStateInput input);
            /// <summary>
            /// Called whenever the SM exits this state
            /// </summary>
            public virtual void OnExit() { }
        }

        public abstract class StateMachine : MonoBehaviour
        {
            public abstract State currentStateObject { get; }
        }

        /// <summary>
        /// Base state machine class. Inherit from it to create your own.
        /// </summary>
        public abstract class StateMachine<TStateMachine, TState, TStateInput> : StateMachine
            where TState : State<TStateMachine, TState, TStateInput>, new()
            where TStateMachine : StateMachine<TStateMachine, TState, TStateInput>
        {
            public override State currentStateObject => currentState;
            public TState currentState;
            protected Dictionary<Type, TState> states = new Dictionary<Type, TState>();
            /// <summary>
            /// Adds a state to the SM, the state will not be added if the SM already has a state of that type.
            /// </summary>
            protected void AddState(TState stateType)
            {
                if (states.ContainsKey(stateType.GetType()))
                {
                    Debug.LogWarning("Adding a duplicate state of type " + stateType.GetType() + " on state machine in " + name);
                    return;
                }
                TState newState = new TState();
                newState.Initialize((TStateMachine)this);
                states.Add(newState.GetType(), newState);
            }
            /// <summary> Adds a state of type T to the SM, it will not be added if the SM already has a state of type T. </summary>
            protected void AddState<T>() where T : TState, new() => AddState(new TState());
            protected void SetState<T>() where T : TState, new() => HandleTransition(new T());
            /// <returns>
            /// The state of this SM of the same type as the given state.
            /// The state is added to the SM if the SM doesn't have one of its type.
            /// </returns>
            protected TState GetStateOf(TState stateType)
            {
                if (!states.ContainsKey(stateType.GetType())) AddState(stateType);
                return states[stateType.GetType()];
            }
            protected void UpdateStateMachine(TStateInput input)
            {
                if (currentState == null) return;
                TState newState = currentState.Update(input);
                HandleTransition(newState);
            }
            protected void HandleTransition(TState newState)
            {
                if (newState == null) return;
                if (newState.GetType() == currentState?.GetType()) return;
                currentState?.OnExit();
                currentState = GetStateOf(newState);
                currentState.OnEnter();
            }
        }
    }
}
