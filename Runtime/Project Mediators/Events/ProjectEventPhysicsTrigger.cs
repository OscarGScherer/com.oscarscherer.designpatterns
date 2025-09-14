using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns.ProjectMediators
{
    public class ProjectEventPhysicsTrigger<T> : MonoBehaviour
    {
        public enum PhysicsMessage
        {
            None = -1,
            OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay,
            OnTriggerEnter2D, OnTriggerExit2D, OnTriggerStay2D, OnCollisionEnter2D, OnCollisionExit2D, OnCollisionStay2D,
        };
        public ProjectEvent<T> projectEvent;
        public T parameter;
        public PhysicsMessage triggerMessage = PhysicsMessage.None;

        private void RaiseEvent(PhysicsMessage physicsMessage)
        {
            if (physicsMessage == triggerMessage) projectEvent?.Raise(parameter);
        }
        //  Trigger
        private void OnTriggerEnter(Collider other) => RaiseEvent(PhysicsMessage.OnTriggerEnter);
        private void OnTriggerExit(Collider other) => RaiseEvent(PhysicsMessage.OnTriggerExit);
        private void OnTriggerStay(Collider other) => RaiseEvent(PhysicsMessage.OnTriggerStay);
        //  Collision
        private void OnCollisionEnter(Collision collision) => RaiseEvent(PhysicsMessage.OnCollisionEnter);
        private void OnCollisionExit(Collision collision) => RaiseEvent(PhysicsMessage.OnCollisionExit);
        private void OnCollisionStay(Collision collision) => RaiseEvent(PhysicsMessage.OnCollisionStay);
        //  Trigger 2D
        private void OnTriggerEnter2D(Collider2D other) => RaiseEvent(PhysicsMessage.OnTriggerEnter2D);
        private void OnTriggerExit2D(Collider2D other) => RaiseEvent(PhysicsMessage.OnTriggerExit2D);
        private void OnTriggerStay2D(Collider2D other) => RaiseEvent(PhysicsMessage.OnTriggerStay2D);
        //  Collision 2D
        private void OnCollisionEnter2D(Collision2D collision) => RaiseEvent(PhysicsMessage.OnCollisionEnter2D);
        private void OnCollisionExit2D(Collision2D collision) => RaiseEvent(PhysicsMessage.OnCollisionExit2D);
        private void OnCollisionStay2D(Collision2D collision) => RaiseEvent(PhysicsMessage.OnCollisionStay2D);
    }
}