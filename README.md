# Unity Design Patters
Generic implementations of commonly used design patterns.

## Patterns
**Implemented:**
* [Singleton](#singleton)
* [State Machine](#state-machine)
* [Behaviour Tree](#behaviour-tree)
* [Event Bus](#event-bus)

**Planned:**
* Scriptable Object Variables and Events

## Singleton
To create a singleton, simply inherit from ```Singleton```. Note that if you want to use OnEnable and OnDisable, you must override the existing methods and call the base implementation for both, since those are used to keep track of each Singleton type.
```c#
public class ExampleSingleton : Singleton
{
    // Your custom functions and code here...

    // Optionally override OnEnable
    protected override void OnEnable()
    {
        base.OnEnable(); // Make sure you call the base method
        // Your custom OnEnable code here...
    }

    // Optionally override OnDisable
    protected override void OnDisable()
    {
        base.OnDisable(); // Make sure you call the base method
        // Your custom OnDisable code here...
    }
}
```
Below is how you would get your singleton from somewhere else in the code. If the singleton doesn't exist, an empty game object will be created and a new instance of the singleton will be added as a component to it.
```c#
ExampleSingleton exampleSingleton = Singleton.Get<ExampleSingleton>();
```

## State Machine
To create a state machine you need to inherit from the ```StateMachine``` class. Then you need to call ```SetStartingState<T>``` once to set it up, then call ```UpdateStateMachine``` to update it.
```c#
public class StateMachineExample : StateMachine
{
    void Start()
    {
        // Setting the starting to be this ExampleState
        SetStartingState<ExampleState>();
    }
    void Update()
    {
        // Udpating the SM every frame using deltaTime
        UpdateStateMachine(Time.deltaTime);
    }
}
```
To create states you need to inherit from the ```State``` class. You need to implement ```Initialize``` and ```Update```, and also may override ```OnEnter``` and ```OnExit```.
Inside ```Initialize``` you should get all the necessary components for your state to work, it is called once for every state, when the state machine enters it for the first time.
In order to define a transition to another state, inside ```Udpate``` you can return an instance of ```StateType<T>```, which is just an empty class holding a reference to some 
type that inherits from ```State```, the type defines which state the state machine should transition to, noting that each state machine is limited to having only one instance of a 
state per type (for instance, this state machine will only have a single instance of an ExampleState).
```c#
public class ExampleState : State
{
    public override void Initialize(GameObject gameObject)
    {
        // Get all necessary references for the state to work...
    }
    public override void OnEnter()
    {
        // Custom logic here...
    }
    public override StateType Update(float deltaTime)
    {
        // Custom logic here...
        // Return the state we want to transition to, through a StateType
        return new StateType<ExampleState>();
    }
    public override void OnExit()
    {
        // Custom logic here...
    }
}
```
The custom inspector lets you check the variables and properties of the current state through reflection.
You can also override ```ToDebugString()``` inside state to print some custom debug info.

![image](https://github.com/user-attachments/assets/48155fe0-b2e1-465b-bc38-5c1cce55785f)


## Behaviour Tree

## Event Bus
