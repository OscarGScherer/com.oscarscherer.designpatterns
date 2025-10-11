using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns.StateMachine
{
    public abstract class State
    {
        /// <returns> Text to be printed on the inpector, whenver the SM is in this state. </returns>
        public virtual string ToDebugString() => $"Override <color=\"yellow\">ToDebugString</color> in <color=\"#2dc46cff\">{GetType().Name}</color> to show custom debug input here!";
        public abstract void InactiveUpdate();
        public abstract void GeneralUpdate();
    }

    public class Transition<T>
    {
        private Transition(Type type) => this.type = type;
        public static Transition<T> To<DT  >() where DT : T => new Transition<T>(typeof(DT));
        public Type type = typeof(T);
    }

    /// <summary>
    /// Base state class. Inherit from it to create your own state.
    /// Each state machine can only have one state of each type of State.
    /// </summary>
    public abstract class State<TSM, TS, TSI> : State
        where TS : State<TSM, TS, TSI>
        where TSM : StateMachine<TSM, TS, TSI>
    {
        /// <summary>
        /// Called when the SM enters this state.
        /// </summary>
        public abstract void OnEnter(TSM stateMachine);
        /// <summary>
        /// Called whenever the SM exits this state
        /// </summary>
        public abstract void OnExit(TSM stateMachine);
        /// <summary>
        /// Implement this to get all necessary references for your state to run.
        /// This function will be called when a state is added to a state machine.
        /// </summary>
        /// <param name="stateMachine"> The state machine the outer SM is a part of </param>
        public abstract void Initialize(TSM stateMachine);
        /// <summary>
        /// Called every frame while the SM is in this state.
        /// Controls transitions to other states.
        /// </summary>
        /// <returns>
        /// The type of state the SM should transition to.
        /// </returns>
        public abstract Transition<TS> ActiveUpdate(TSI context);
        public override void GeneralUpdate() { /*NOOP*/ }
        public override void InactiveUpdate() { /*NOOP*/ }
    }

    [Serializable]
    public abstract class StateMachine : MonoBehaviour
    {
        [NonSerialized] public bool refreshInspector = false;
        private const int MAX_HISTORY_LENGTH = 100;
        public LinkedList<State> stateChangeHistory = new LinkedList<State>();

        protected void LogStateChange(State state)
        {
            stateChangeHistory.AddFirst(state);
            if (stateChangeHistory.Count > MAX_HISTORY_LENGTH) stateChangeHistory.RemoveLast();
            refreshInspector = true;
        }

        /// <summary>
        /// Current State object cast to the base State class.
        /// </summary>
        public abstract State CurrentStateObject { get; }
    }

    /// <summary>
    /// Base state machine class. Inherit from it to create your own.
    /// </summary>
    public abstract class StateMachine<TSM, TS, TSI> : StateMachine
        where TS : State<TSM, TS, TSI>
        where TSM : StateMachine<TSM, TS, TSI>
    {
        public override State CurrentStateObject => currentState;
        protected TS currentState;
        protected List<TS> states = new();

        private TS CreateState(Type stateType)
        {
            TS newState = (TS)Activator.CreateInstance(stateType);
            newState.Initialize((TSM)this);
            states.Add(newState);
            return newState;
        }
        /// <returns>
        /// The state of this SM of the given type.
        /// </returns>
        protected TS GetStateOf(Type stateType)
        {
            TS state = states.FirstOrDefault(state => stateType.IsInstanceOfType(state));
            if (state == null) state = CreateState(stateType);
            return state;
        }
        /// <returns>
        /// The state of this SM of type T.
        /// A new state of type T is added to the SM if it doesn't have one already.
        /// </returns>
        protected TS GetStateOfType<TS2>() where TS2 : TS, new() => GetStateOf(typeof(TS2));

        /// <summary>
        /// Updates the current state and handle transitions that might happen.
        /// </summary>
        public virtual void UpdateStateMachine(TSI context)
        {
            foreach(State state in states)
            {
                if (state == null) continue;
                state.GeneralUpdate();
                if (state != currentState) state.InactiveUpdate();
            }
            if (currentState == null) return;
            Type stateType = currentState.ActiveUpdate(context)?.type;
            SetState(stateType);
        }

        /// <summary>
        /// Transitions to the SM's state of the same type of the given state type.
        /// </summary>
        public void SetState<DTS>() where DTS : TS => SetState(typeof(DTS));
        public void AddState<DTS>(DTS state) where DTS : TS
        {
            TS existingState = states.FirstOrDefault(state => typeof(DTS).IsInstanceOfType(state));
            if (existingState != null) return;
            state.Initialize((TSM)this);
            states.Add(state);
        }
        public void RemoveState<DTS>() where DTS : TS
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (typeof(DTS).IsInstanceOfType(states[i]))
                {
                    states.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        /// Transitions to the SM's state of the same type of the given state, caling OnExit and OnEnter.
        /// If the given state is null, or is of the same type as the current state, the current state remains the same.
        /// </summary>
        /// <param name="state">
        /// The state of the type you want to transition to
        /// </param>
        private void SetState(Type stateType)
        {
            if (stateType == null) return;
            if (currentState != null)
            {
                if (stateType == currentState.GetType()) return;
                currentState.OnExit((TSM)this);
            }
            currentState = GetStateOf(stateType);
            currentState.OnEnter((TSM)this);
            LogStateChange(currentState);
        }
    }
}
