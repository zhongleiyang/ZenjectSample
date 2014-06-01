<img src="ZenjectLogo.png?raw=true" alt="Zenject" width="600px" height="134px"/>

# Dependency Injection Framework for Unity3D

## Table Of Contents

* <a href="#introduction">Introduction</a>
* <a href="#features">Features</a>
* <a href="#history">History</a>
* Dependency Injection
    * <a href="#theory">Theory</a>
    * <a href="#misconceptions">Misconceptions</a>
* Zenject API
    * <a href="#overview-of-the-zenject-api">Overview of the Zenject API</a>
        * <a href="#hello_world">Hellow World Example</a>
        * <a href="#bindings">Binding</a>
        * <a href="#optional_bindings">Optional Binding</a>
        * <a href="#conditional_bindings">Conditional Bindings</a>
        * <a href="#dependency_root">The Dependency Root</a>
        * <a href="#tickables">ITickable</a>
        * <a href="#postinject">IInitializable and PostInject</a>
        * <a href="#composition_root">Composition Root, Installers, And Modules</a>
    * <a href="#operation_order">Zenject Order Of Operations</a>
    * <a href="#rules">Rules / Guidelines / Recommendations</a>
    * Advanced Features
        * <a href="#update_order">Update Order And Initialization Order</a>
        * <a href="#across_scenes">Injecting Data Across Scenes</a>
        * <a href="#settings">Using the Unity Inspector To Configure Settings</a>
        * <a href="#graph_validation">Object Graph Validation</a>
        * <a href="#dynamic_creation">Creating Objects Dynamically</a>
        * <a href="#bindscope">Using BindScope</a>
        * <a href="#disposables">Implementing IDisposable</a>
        * <a href="#automocking">Auto-Mocking Using Moq</a>
        * <a href="#graphviz">Visualizing Object Graph Automatically</a>
    * <a href="#help">Further Help</a>
    * <a href="#license">License</a>

## <a id="introduction"></a>Introduction

Zenject is a lightweight dependency injection framework built specifically to target Unity 3D.  It can be used to turn your Unity 3D application into a collection of loosely-coupled parts with highly segmented responsibilities.  Zenject can then glue the parts together in many different configurations to allow you to easily write, re-use, refactor and test your code in a scalable and extremely flexible way.

Tested in Unity 3D on the following platforms: PC/Mac/Linux, iOS, Android, and Webplayer

This project is open source.  You can find the official repository [here](https://github.com/modesttree/Zenject).  If you would like to contribute to the project pull requests are welcome!

For general support or bug requests, please feel free to create issues on the github page.  You can also email me directly at svermeulen@modesttree.com

## <a id="features"></a>Features

* Injection into normal C# classes or MonoBehaviours
* Constructor injection (can tag constructor if there are multiple)
* Field injection
* Property injection
* Conditional Binding Including Named injections (string, enum, etc.)
* Optional Dependencies
* Support For Building Dynamic Object Graphs At Runtime
* Auto-Mocking using the Moq library
* Injection across different Unity scenes
* Ability to print entire object graph as a UML image automatically

## <a id="history"></a>History

Unity is a fantastic game engine, however the approach that new developers are encouraged to take does not lend itself well to writing large, flexible, or scalable code bases.  In particular, the default way that Unity manages dependencies between different game components can often be awkward and error prone.

Having worked on non-unity projects that use dependency management frameworks (such as Ninject, which Zenject takes a lot of inspiration from), the problem irked me enough that I decided a custom framework was in order.  Upon googling for solutions, I found a series of great articles by Sebastiano Mandalà outlining the problem, which I strongly recommend that everyone read before firing up Zenject:

* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/)
* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/)

Sebastiano even wrote a proof of concept and open sourced it, which became the basis for this library.

What follows in the next section is a general overview of Dependency Injection from my perspective.  I highly recommend seeking other resources for more information on the subject, as there are many (often more intelligent) people that have written on the subject.  In particular, I highly recommend anything written by Mark Seeman on the subject - in particular his book 'Dependency Injection in .NET'.

Finally, I will just say that if you don't have experience with DI frameworks, and are writing object oriented code, then trust me, you will thank me later!  Once you learn how to write properly loosely coupled code using DI, there is simply no going back.

## <a id="theory"></a>Theory

When writing an individual class to achieve some functionality, it will likely need to interact with other classes in the system to achieve its goals.  One way to do this is to have the class itself create its dependencies, by calling concrete constructors:

    public class Foo
    {
        ISomeService _service;

        public Foo()
        {
            _service = new SomeService();
        }

        public void DoSomething()
        {
            _service.PerformTask();
            ...
        }
    }

This works fine for small projects, but as your project grows it starts to get unwieldy.  The class Foo is tightly coupled to class 'SomeService'.  If we decide later that we want to use a different concrete implementation then we have to go back into the Foo class to change it.

After thinking about this, often you come to the realization that ultimately, Foo shouldn't bother itself with the details of choosing the specific implementation of the service.  All Foo should care about is fulfilling its own specific responsibilities.  As long as the service fulfills the abstract interface required by Foo, Foo is happy.  Our class then becomes:

    public class Foo
    {
        ISomeService _service;

        public Foo(ISomeService service)
        {
            _service = service;
        }

        public void DoSomething()
        {
            _service.PerformTask();
            ...
        }
    }

This is better, but now whatever class is creating Foo (let's call it Bar) has the problem of filling in Foo's extra dependencies:

    public class Bar
    {
        public void DoSomething()
        {
            var foo = new Foo(new SomeService());
            foo.DoSomething();
            ...
        }
    }

And class Bar probably also doesn't really care about what specific implementation of SomeService Foo uses.  Therefore we push the dependency up again:

    public class Bar
    {
        ISomeService _service;

        public Bar(ISomeService service)
        {
            _service = service;
        }

        public void DoSomething()
        {
            var foo = new Foo(_service);
            foo.DoSomething();
            ...
        }
    }

So we find that it is useful to push the responsibility of deciding which specific implementations of which classes to use further and further up in the 'object graph' of the application.  Taking this to an extreme, we arrive at the entry point of the application, at which point all dependencies must be satisfied before things start.  The dependency injection lingo for this part of the application is called the 'composition root'.

## <a id="misconceptions"></a>Misconceptions

There are many misconceptions about DI, due to the fact that it can be tricky to fully wrap your head around at first.  It will take time and experience before it fully 'clicks'.

As shown in the above example, DI can be used to easily swap different implementations of a given interface (in the example this was ISomeService).  However, this is only one of many benefits that DI offers.  In most cases the various responsibilities of an application have single, specific classes implementing them, so you will be injecting concrete references in those cases rather than interfaces (especially if you're like me and follow the [Reused Abstraction Principle](http://codemanship.co.uk/parlezuml/blog/?postid=934)).

More important than that is the fact that using a dependency injection framework like Zenject allows you to more easily follow the '[Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle)'.  By letting Zenject worry about wiring up the classes, the classes themselves can just focus on fulfilling their specific responsibilities.

Other benefits include:

* Testability - Writing automated unit tests or user-driven tests becomes very easy, because it is just a matter of writing a different 'composition root' which wires up the dependencies in a different way.  Want to only test one subsystem?  Simply create a new composition root.   In cases where you can't easily separate out a specific sub-system to test, you can also creates 'mocks' for the sub-systems that you don't care about. (more detail <a href="#automocking">below</a>)
* Refactorability - When code is loosely coupled, as is the case when using DI properly, the entire code base is much more resilient to changes.  You can completely change parts of the code base without having those changes wreak havoc on other parts.
* Encourages modular code - When using a DI framework you will naturally follow better design practices, because it forces you to think about the interfaces between classes.

## <a id="zenjectoverview"></a>Overview Of The Zenject API

What follows is a general overview of how DI patterns are applied using Zenject.  However, the best documentation right now is probably the included sample project itself (a kind of asteroids clone, which you can find by opening "Extras/SampleGame/Asteroids.unity").  I would recommend using that for reference after reading over these concepts.

## <a id="hello_world"></a>Hello World Example

    public class TestInstaller : MonoBehaviour, ISceneInstaller
    {
        public void InstallModules(DiContainer container)
        {
            container.Bind<Module>().ToSingle<StandardUnityModule>();
            container.Bind<Module>().ToSingle<TestModule>();
        }
    }

    public class TestModule : Module
    {
        public override void AddBindings()
        {
            _container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();

            _container.Bind<ITickable>().ToSingle<TestRunner>();
            _container.Bind<IInitializable>().ToSingle<TestRunner>();
        }
    }

    public class TestRunner : ITickable, IInitializable
    {
        public void Initialize()
        {
            Debug.Log("Hello World");
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Exiting!");
                Application.Quit();
            }
        }
    }

You can run this example by doing the following:

* Copy and paste the above code into a file named 'TestInstaller'
* Create a new scene in Unity
* Add a new GameObject and name it "CompositionRoot" (though the name does not really matter)
* Attach the CompositionRoot MonoBehaviour to your new GameObject
* Attach your TestInstaller script as well
* Run
* Observe unity console for output

The CompositionRoot MonoBehaviour is the entry point of the application, where Zenject sets up all the various dependencies before kicking off your scene.  To add content to your Zenject scene, you need to write what is referred to in Zenject as an 'Installer' and also some number of 'Modules'.  Don't worry if this or the above code isn't making sense yet.  We will return to this in a later section.

## <a id="bindings"></a>Binding

Every dependency injection framework is ultimately just a framework to bind types to instances.

In Zenject, dependency mapping done by adding bindings to something called a container.  The container should then 'know' how to create all the object instances in our application, by recursively resolving all dependencies for a given object.

The format for the bind command can be any of the following:

Inject by interface.  Note in this case it is injected as a singleton so there will only be one instance of Foo injected into any classes that use it.

    _container.Bind<IFoo>().ToSingle<Foo>();

Inject by concrete class.  Note again that it is ToSingle and therefore there will only be one instance of Foo

    _container.Bind<Foo>().ToSingle();

Inject interface as transient.  In this case a new instance of Foo will be generated each time it is injected.

    _container.Bind<IFoo>().ToTransient<Foo>();

Inject concrete class as transient.

    _container.Bind<Foo>().ToTransient();

For primitive types you have to use BindValue instead:

    _container.BindValue<float>().To(1.5f);
    _container.BindValue<int>().To(42);

Inject from unity prefab.  This will instantiates a new instance of the given prefab and inject the same one every time the given monobehaviour class is injected.  Note in this case specifying FooMonoBehaviour twice is redundant but necessary

    _container.Bind<FooMonoBehaviour>().ToSingleFromPrefab<FooMonoBehaviour>(PrefabGameObject);

A variation on inject from unity prefab injects a new instance of the given prefab every time the given monobehaviour class is injected.

    _container.Bind<FooMonoBehaviour>().ToTransientFromPrefab<FooMonoBehaviour>(PrefabGameObject);

Inject MonoBehaviour. Creates a new game object and attaches FooMonoBehaviour to it:

    _container.Bind<FooMonoBehaviour>().ToSingleGameObject();

Inject by custom method. You can customize creation logic yourself by defining a method:

    _container.Bind<IFoo>().ToMethod(SomeMethod);

    ...

    public IFoo SomeMethod(DiContainer container)
    {
        ...
        return new Foo();
    }

Inject many.  You can also bind multiple types to the same interface, with the result being a list of dependencies.  In this case Bar would get a list containing a new instance of Foo1, Foo2, and Foo3:

    _container.Bind<IFoo>().ToSingle<Foo1>();
    _container.Bind<IFoo>().ToSingle<Foo2>();
    _container.Bind<IFoo>().ToSingle<Foo3>();

    ...

    public class Bar
    {
        public Bar(List<IFoo> foos)
        {
        }
    }

Note that when defining List dependencies, the empty list will result in an error.  If the empty list is valid, then you can suppress the error by marking the List as optional as described <a href="#optional_bindings">here</a>.

## <a id="optional_bindings"></a>Optional Binding

You can declare some dependencies as optional as follows:

    public class Bar
    {
        public Bar(
            [InjectOptional]
            IFoo foo)
        {
            ...
        }
    }

In this case, if IFoo is not bound in any installers, then it will be passed as null.

Note that when declaring dependencies with primitive types as optional, they will be given their default value (eg. 0 for ints).  However, if you need to distiguish between being given a default value and the primitive dependency not being specified, you can do this as well by declaring it as nullable:

    public class Bar
    {
        int _foo;

        public Bar(
            [InjectOptional]
            int? foo)
        {
            if (foo == null)
            {
                // Use 5 if unspecified
                _foo = 5;
            }
            else
            {
                _foo = foo.Value;
            }
        }
    }

    ...

    // Can leave this commented or not and it will still work
    // _container.BindValue<int>().To(1);

## <a id="conditional_bindings"></a>Conditional Bindings

In many cases you will want to restrict where a given dependency is injected.  You can do this using the following syntax:

Use different implementations of IFoo in different cases:

    _container.Bind<IFoo>().ToSingle<Foo1>().WhenInjectedInto<Bar1>();
    _container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar2>();

Inject by name:

    _container.Bind<IFoo>().ToSingle<Foo>().WhenInjectedInto("foo");

    public class Bar
    {
        [Inject("foo")]
        Foo _foo;
    }

You can also inject by name and also restrict to only Bar class:

    _container.Bind<IFoo>().ToSingle<Foo>().WhenInjectedInto<Bar>("foo");

Note that both of these are simple shorthands.  The long version would be:

    _container.Bind<IFoo>().ToSingle<Foo>().When(context => context.Target == typeof(Bar) && identifier.Equals("foo"));

Note also that you can name dependencies with any type (and not just string) and that it applies to constructor arguments as well, for example:

    enum Foos
    {
        A,
    }

    public class Bar
    {
        Foo _foo;

        public Bar(
            [Inject(Foos.A)] Foo foo)
        {
        }
    }

## <a id="dependency_root"></a>The dependency root

Every Zenject app has one root object.  The dependencies of this object generates the full object graph for the application/game.  For example, in the sample project this is the GameRoot class which is declared as below:

    _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();

## <a id="tickables"></a>ITickable

I prefer to avoid MonoBehaviours when possible in favour of just normal C# classes.  Zenject allows you to do this much more easily by providing interfaces that mirror functionality that you would normally need to use a MonoBehaviour for.

For example, if you have code that needs to run per frame, then you can implement the ITickable interface:

    public class Ship : ITickable
    {
        public void Tick()
        {
            // Perform per frame tasks
        }
    }

Then it's just a matter of including the following in one of your installers (as long as you are using DependencyRootStandard or a subclass)

    _container.Bind<ITickable>().ToSingle<Ship>();

Note that the order that Tick() is called on all ITickables is also configurable, as outlined <a href="#update_order">here</a>.

## <a id="postinject"></a>IInitializable and PostInject

If you have some initialization that needs to occur on a given object, you can include this code in the constructor.  However, this means that the initialization logic would occur in the middle of the object graph being constructed, so it may not be ideal.

One alternative is implement IInitializable, and then perform initialization logic in an Initialize() method.  This method would be called immediately after the entire object graph is constructed.  The order that the Initialize() methods are called on all IInitialize's is also controllable in a similar way to ITickable, as explained <a href="#update_order">here</a>.

IInitializable works well for start-up initialization, but what about for objects that are created dynamically via factories?  (see <a href="#dynamic_creation">this section</a> for what I'm referring to here).

In these cases you can mark any methods that you want to be called after injection occurs with a [PostInject] attribute:

    public class Foo
    {
        [Inject]
        IBar _bar;

        [PostInject]
        public void Initialize()
        {
            ...
            _bar.DoStuff();
            ...
        }
    }

This still has the drawback that it is called in the middle of object graph construction, but can be useful in many cases.  In particular, if you are using property injection (which isn't generally recommended but necessary in some cases) then you will not have your dependencies in the constructor, and therefore you will need to define a [PostInject] method in this case.  You will also need to use [PostInject] for MonoBehaviours that you are creating dynamically, since MonoBehaviours cannot have contructors.

Note that if you do plan to use IInitializable and ITickable as described here that you will need to either use DependencyRootStandard as your root (that is, the type bound to IDependencyRoot) or a subclass.  This is because DependencyRootStandard includes the classes responsible for handling the IInitializable's and ITickable's.

This also means that you do not need to use this approach at all.  You can use a custom dependency root and handle your own updating and initialization yourself, or simply write all your code in MonoBehaviours, and still receive all the benefits of Zenject.

## <a id="composition_root"/></a>Composition Root / Installer / Modules

As touched on briefly above, every Zenject scene contains one and only one 'scene installer', which declares all the 'modules' that are used in the scene by binding them to the given container.  What does these mean exactly?

It is divided up this way because often it's nice to be able to group the bindings for a set of classes that relate to some functionality together (in a module) rather than just having a big collection of all the bindings for the scene in one place.

For small projects this may seem over-engineered, since you will likely just be dealing with a single module and a single installer.  However as your project grows, and as you add more and more scenes/tests to your project, you will likely want to avoid the code duplication that would otherwise result in the modules for all your different scenes.  You can do this by defining common, re-usable modules and using them in multiple scenes.

In general, the contents of the scene installer is very small - most of the work in setting up a scene is done in the modules themselves.  If this isn't making sense yet, it may be helpful to read the following sections then come back to this

## <a id="operation_order"></a>Zenject Order Of Operations

A Zenject driven application is executed by the following steps:

* Composition Root is started (via Unity Awake() method)
* Composition Root calls InstallModules() on the Scene Installer.  Note that it is assumed here that the scene installer is attached to the same game object as the composition root.
* The installer for the scene registers some number of modules on the given DiContainer by calling Bind<> methods.  It may also configure settings for each module through Bind<> commands as well.
* The Composition Root retrieves the full list of concrete instances bound to Module using the same container that was passed to the scene installer.  It then creates a *new* container, and traverses through this list of Modules.  For each module, it updates the `_container` member to refer to the newly created container, and calls AddBindings().
* Each Module then registers different sets of dependencies directly on to the given DiContainer by calling Bind<> and BindValue<> methods.  Note that the order that this binding occurs should not matter whatsoever.
* The Composition Root then traverses the entire scene heirarchy and injects all MonoBehaviours with their dependencies. Since MonoBehaviours are instantiated by Unity we cannot use constructor injection in this case and therefore field or property injection must be used (which is done by adding a [Inject] attribute to any member).  Any methods on these MonoBehaviour's marked with [PostInject] are called at this point as well.
* After filling in the scene dependencies the Composition Root then retrieves the instance of the root object (that is, whatever is bound to IDependencyRoot).  In most cases code does not need to be in MonoBehaviours and will be resolved as a result of this
* If any required dependencies cannot be resolved, a ZenjectResolveException is thrown
* Initialize() is called on all IInitializable objects in the order specified in the installers
* Unity Start() is called on all built-in MonoBehaviours
* Unity Update() is called, which results in Tick() being called for all ITickable objects (in the order specified in the installers)
* App is exited
* Dispose() is called on all objects mapped to IDisposable (see <a href="#disposables">here</a> for details)

## <a id="rules"></a>DI Rules / Guidelines / Recommendations

* The container should *only* be referenced in the composition root layer.  Note that factories are part of this layer and the container can be referenced there (which is necessary to create objects at runtime).  For example, see ShipStateFactory in the sample project.  See <a href="#dynamic_creation">here</a> for more details on this.
* Prefer constructor injection to field or property injection.
    * Constructor injection forces the dependency to only be resolved once, at class creation, which is usually what you want.  In many cases you don't want to expose a public property with your internal dependencies
    * Constructor injection guarantees no circular dependencies between classes, which is generally a bad thing to do
    * Constructor injection is more portable for cases where you decide to re-use the code without a DI framework such as Zenject.  You can do the same with public properties but it's more error prone.  It's possible to forget to initialize one field and leave the object in an invalid state
    * Finally, Constructor injection makes it clear what all the dependencies of a class are when another programmer is reading the code.  They can simply look at the parameter list of the constructor.

## <a id="update_order"></a>Update / Initialization Order

In many cases, especially for small projects, the order that classes update or initialize in does not matter.  This is why Unity does not have an easy way to control this (besides in Edit -> Project Settings -> Script Execution Order, though that is pretty awkward to use).  However, in larger projects update or initialization order can become an issue.  This can especially be an issue in Unity, since it is often difficult to predict in what order the Start(), Awake(), or Update() methods will be called in.

In Zenject, by default, ITickables and IInitializables are updated in the order that they are added, however for cases where the update or initialization order matters, there is a much better way.  By specifying their priorities explicitly in the installer.  For example, in the sample project you can find this code in the scene installer:

    public class AsteroidsSceneInstaller : MonoBehaviour, ISceneInstaller
    {
        ...

        void InitPriorities(DiContainer container)
        {
            container.Bind<Module>().ToSingle<InitializablePrioritiesModule>();
            container.Bind<List<Type>>().To(Initializables)
                .WhenInjectedInto<InitializablePrioritiesModule>();

            container.Bind<Module>().ToSingle<TickablePrioritiesModule>();
            container.Bind<List<Type>>().To(Tickables)
                .WhenInjectedInto<TickablePrioritiesModule>();
        }

        static List<Type> Initializables = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };

        static List<Type> Tickables = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };
    }

This way, you won't hit a wall at the end of the project due to some unforseen order-dependency.

Any ITickables or IInitializables that aren't given an explicit order are updated last.

## <a id="across_scenes"></a>Injecting data across scenes

In some cases it's useful to pass arguments from one scene to another.  The way Unity allows us to do this by default is fairly awkward.  Your options are to create a persistent GameObject and call DontDestroyOnLoad() to keep it alive when changing scenes, or use global static classes to temporarily store the data.

Let's pretend you want to specify a 'level' string to the next scene.  You have the following class that requires the input:

    public class LevelHandler : IInitializable
    {
        readonly string _startLevel;

        public LevelHandler(
            [InjectOptional]
            [Inject("StartLevelName")]
            string startLevel)
        {
            if (startLevel == null)
            {
                _startLevel = "default_level";
            }
            else
            {
                _startLevel = startLevel;
            }
        }

        public void Initialize()
        {
            ...
            [Load level]
            ...
        }
    }

You can load the scene containing `LessonStandaloneStart` and specify a particular level by using the following syntax:

    ZenUtil.LoadScene("NameOfSceneToLoad",
        delegate (DiContainer container)
        {
            container.Bind<string>().To("custom_level").WhenInjectedInto<LevelHandler>("StartLevelName");
        });

Note that you can still run the scene directly, in which case it will default to using "level01".  This is possible because we are using the InjectOptional flag.

An alternative way to do this would be to customize the module itself rather than the LevelHandler class.  In this case we can write our LevelHandler class like this (without the [InjectOptional] flag)

    public class LevelHandler : IInitializable
    {
        readonly string _startLevel;

        public LevelHandler(string startLevel)
        {
            _startLevel = startLevel;
        }

        public void Initialize()
        {
            ...
            [Load level]
            ...
        }
    }

Then, in the module for our scene we can include the following:

    public class GameModule : Module
    {
        [Inject("LevelName")]
        [InjectOptional]
        public string LevelName = "default_level";

        ...

        public override void AddBindings()
        {
            ...
            _container.Bind<string>().To(LevelName).WhenInjectedInto<LevelHandler>();
            ...
        }
    }

Then, instead of injecting directly into the LevelHandler we can inject into the module instead, by passing a second delegate.  Be careful to always note that the container used by the Scene Installer is different from the container used by the modules (this is why we need to use the delegate passed as the second parameter)

    ZenUtil.LoadScene("NameOfSceneToLoad",
        null,
        delegate (DiContainer container)
        {
            container.Bind<string>().To("level02").WhenInjectedInto<GameModule>("LevelName");
        });

Note that in this case I didn't need to use the "LevelName" identifier since there is only one string injected into the GameModule class, however I find it's sometimes nice to be explicit.

## <a id="settings"></a>Using the Unity Inspector To Configure Settings

One implication of writing most of your code as normal C# classes instead of MonoBehaviour's is that you lose the ability to configure data on them using the inspector.  You can however still take advantage of this in Zenject by using the following pattern, as seen in the sample project:

    public class AsteroidsSceneInstaller : MonoBehaviour, ISceneInstaller
    {
        public AsteroidsMainModule.Settings AsteroidSettings;

        public void InstallModules(DiContainer container)
        {
            ...
            container.Bind<AsteroidsMainModule.Settings>().To(AsteroidSettings);
            ...
        }
    }

Then in your module:

    public class AsteroidsMainModule : Module
    {
        [Inject]
        readonly Settings _settings;

        public override void AddBindings()
        {
            ...
            _container.Bind<ShipStateMoving.Settings>().ToSingle(_settings.StateMoving);
            ...
        }

        [Serializable]
        public class Settings
        {
            ...
            public ShipStateMoving.Settings StateMoving;
            ...
        }
    }

Note that if you follow this method, you will have to make sure to always include the [Serializable] attribute on your settings wrappers, otherwise they won't show up in the Unity inspector.

You can see this in action, start the asteroids scene and try adjusting `Ship -> State Moving -> Move Speed` setting and watch live as your ship changes speed.

## <a id="graph_validation"></a>Object Graph Validation

The usual workflow when setting up bindings using a DI framework is something like this:

* Add some number of bindings in code
* Execute your app
* Observe a bunch of DI related exceptions
* Modify your bindings to address problem
* Repeat

This works ok for small projects, but as the complexity of your project grows it is often a tedious process.  The problem gets worse if the startup time of your application is particularly bad.  What would be great is some tool to analyze your object graph and tell you exactly where all the missing bindings are, without requiring the cost of firing up your whole app.

You can do this in Zenject out-of-the-box by executing the menu item `Assets -> Zenject -> Validate Current Scene` or simply hitting CTRL+SHIFT+V with the scene open that you want to validate.  This will execute the scene installer for the current scene and construct a fully bound container.   It will then iterate through the object graphs and verify that all bindings can be found (without actually instantiating any of them).

Also, if you happen to be a fan of automated testing (as I am) then you can include object graph validation as part of that by calling `ZenUtil.ValidateInstaller([scene installer])`

## <a id="dynamic_graph_validation"></a>Dynamic Object Graph Validation

The above approach great for dependencies that are attached to the dependency root, as well as any dependencies that are attached to any MonoBehaviour's that are saved into the scene, but what about classes that are instantiated at runtime via factories?  How do you validate those object graphs?

If you want to be thorough (and I recommend it) then you can include these object graphs as well, by including an extra method in your modules to declare these object graphs.  For example, in the sample project, we define the following:

    public class AsteroidsMainModule : Module
    {
        ...
        public override IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            return Validate<Asteroid>().Concat(
                   Validate<ShipStateDead>(typeof(Ship))).Concat(
                   Validate<ShipStateMoving>(typeof(Ship))).Concat(
                   Validate<ShipStateWaitingToStart>(typeof(Ship)));
        }
        ...
    }

This information is used when validating to cover the dynamic object graphs.  Note that in many cases the dynamically created object will get all of its required dependencies out of the container, but in some cases the dependencies will be provided manually, via calls to `[Factory].Create()` (for eg. the ship state classes above).  In these cases you need to tell Zenject to ignore these dependencies by passing in a list of types.

## <a id="dynamic_creation"></a>Creating Objects Dynamically

One of the things that often confuses people new to dependency injection is the question of how to create new objects dynamically, after the app/game has fully started up and after the composition root has resolved the dependency root.  For example, if you are writing a game in which you are spawning new enemies throughout the game, then you will want to construct a new object graph for the 'enemy' class.  How to do this?  The answer: Factories.

Remember that an important part of dependency injection is to reserve use of the container to strictly the "Composition Root Layer".  The container class (DiContainer) is itself included as a dependency in itself so there is nothing stopping you from ignoring this rule and injecting the container into any classes that you want.  For example, the following code will work:

    public class Enemy
    {
        DiContainer _container;

        public Enemy(DiContainer container)
        {
            _container = container;
        }

        public void Update()
        {
            ...
            var player = _container.Resolve<Player>();
            WalkTowards(player.Position);
            ...
            etc.
        }
    }

HOWEVER, the above code is an example of an anti-pattern.  This will work, and you can use the container to get access to all other classes in your app, however if you do this you will not really be taking advantage of the power of dependency injection.  This is known, by the way, as [Service Locator Pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

Of course, the dependency injection way of doing this would be the following:

    public class Enemy
    {
        Player _player;

        public Enemy(Player player)
        {
            _player = player;
        }

        public void Update()
        {
            ...
            WalkTowards(_player.Position);
            ...
        }
    }

The only exception to this rule is within factories and installers.  Again, factories and installers make up what we refer to as the "composition root layer".

For example, if you have a class responsible for spawning new enemies, before DI you might do something like this:

    public class EnemySpawner
    {
        List<Enemy> _enemies = new List<Enemy>();

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var enemy = new Enemy();
                _enemies.Add(enemy);
            }
        }
    }

This will not work however, since in our case the Enemy class requires a reference to the Player class in its constructor.  We could add a dependency to the Player class to the EnemySpawner class, but then we have the problem described <a href="#theory">above</a>.  The EnemySpawner class doesn't care about filling in the dependencies for the Enemy class.  All the EnemySpawner class cares about is getting a new Enemy instance.

There are several ways to handle this case in Zenject.  The first way:

    public class EnemySpawner
    {
        IFactory<Enemy> _enemyFactory;
        List<Enemy> _enemies = new List<Enemy>();

        public EnemySpawner(IFactory<Enemy> enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var enemy = _enemyFactory.Create();
                _enemies.Add(enemy);
            }
        }
    }

Then in your installer, you would include:

    _container.BindFactory<Enemy>();

Which is simply shorthand for the following:

    _container.Bind<IFactory<Enemy>>().ToSingle<Factory<Enemy>>();

Doing it this way, all the dependencies for the Enemy class (such as the Player) will automatically be filled in.

However, in more complex examples, the EnemySpawner class may wish to pass in custom constructor arguments as well. For example, let's say we want to randomize the speed of each Enemy to add some interesting variation to our game.  Our enemy class becomes:

    public class Enemy
    {
        Player _player;
        float _runSpeed;

        public Enemy(Player player, float runSpeed)
        {
            _player = player;
            _runSpeed = runSpeed;
        }

        public void Update()
        {
            ...
            WalkTowards(_player.Position);
            ...
        }
    }

The first and easiest way would to handle this would be to change the EnemySpawner class to the following:

    public class EnemySpawner
    {
        ...
        public void Update()
        {
            ...
            var newSpeed = Random.Range(MIN_ENEMY_SPEED, MAX_ENEMY_SPEED);
            var enemy = _enemyFactory.Create(newSpeed);
            ...
        }
    }

This works because the IFactory<> interface accepts a variable number of arguments.  It will try and intelligently match the given set of arguments to the constructor of the object that its creating.

While this will work, it can be a bit error prone because the arguments that you supply to the `IFactory<Enemy>` class are not validated until run time.  So in some cases you may wish to write a custom factory to wrap the call to IFactory instead:

    public class EnemyFactory
    {
        private IFactory<Enemy> _factory;

        public EnemyFactory(DiContainer container)
        {
            _factory = new Factory<Enemy>(container);
        }

        public Enemy Create(float speed)
        {
            return _factory.Create(speed);
        }
    }

Or alternatively (which would be equivalent):

    public class EnemyFactory
    {
        private Instantiator _instantiator;

        public EnemyFactory(Instantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public Enemy Create(float speed)
        {
            return _instantiator.Instantiate<Enemy>(speed);
        }
    }


And then change our installer to include:

    _container.Bind<EnemyFactory>().ToSingle();

Note the following:

* We no longer need the line `_container.BindFactory<Enemy>();` since we are directly creating the Factory<> class in the EnemyFactory constructor (in the first method)
* We are injecting the DiContainer/Instantiator directly into the EnemyFactory class, which is generally a bad thing to do but ok in this case because it is a factory (and therefore part of the "composition root layer")

Our EnemySpawner class becomes:

    public class EnemySpawner
    {
        EnemyFactory _enemyFactory;
        List<Enemy> _enemies = new List<Enemy>();

        public EnemySpawner(EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var newSpeed = Random.Range(MIN_ENEMY_SPEED, MAX_ENEMY_SPEED);
                var enemy = _enemyFactory.Create(newSpeed);
                _enemies.Add(enemy);
            }
        }
    }

## <a id="bindscope"></a>Using BindScope

Right now, the difference between using our custom factory vs simply IFactory directly isn't very much.  However, there may be more complicated construction scenarios in the real world where the value of this approach would be more clear.

For example, suppose one day we decide to add further runtime constructor arguments to the Enemy class:

    public class Enemy
    {
        public Enemy(EnemyWeapon weapon)
        {
            ...
        }
    }

    public class EnemyWeapon
    {
        public EnemyWeapon(float damage)
        {
            ...
        }
    }

And let's say we want the damage of the EnemyWeapon class to be specified by the EnemySpawner class.  How do we pass that argument down to EnemyWeapon?  The answer:  'BindScope':

    public class EnemyFactory
    {
        IFactory<Enemy> _factory;
        DiContainer _container;

        public EnemyFactory(DiContainer container)
        {
            _factory = new Factory<Enemy>(container);
            _container = container;
        }

        public Enemy Create(float weaponDamage)
        {
            using (BindScope scope = _container.CreateScope())
            {
                scope.Bind<float>().ToSingle(weaponDamage).WhenInjectedInto<EnemyWeapon>();
                return _factory.Create();
            }
        }
    }

BindScope can be used in factories to temporarily configure the container in a similar way that's done in installers.  This can be very useful when creating complex object graphs at runtime.  After the function returns, whatever bindings you added in the using{} block are automatically removed.  BindScope can also be used to specify injection identifiers as well (which can be less error prone than passing extra parameters as variable arguments to IFactory)

## <a id="disposables"></a>Implementing IDisposable

If you have external resources that you want to clean up when the app closes, the scene changes, or for whatever reason the composition root object is destroyed, you can do the following:

    public class Logger : IInitializable, IDisposable
    {
        FileStream _outStream;

        public void Initialize()
        {
            _outStream = File.Open("log.txt", FileMode.Open);
        }

        public void Log(string msg)
        {
            _outStream.WriteLine(msg);
        }

        public void Dispose()
        {
            _outStream.Close();
        }
    }

Then in your installer you can include:

    _container.Bind<Logger>().Bind();
    _container.Bind<IInitializable>().Bind<Logger>();
    _container.Bind<IDisposable>().Bind<Logger>();

This works because when the scene changes or your unity application is closed, the unity event OnDestroy() is called on all MonoBehaviours, including the CompositionRoot class.  The CompositionRoot class, which owns your DiContainer, calls Dispose() on the DiContainer, which then calls Dispose() on all objects that are bound to IDisposable.

Note that this example may or may not be a good idea (for example, the file will be left open if your app crashes), but illustrates the point  :)

## <a id="automocking"></a>Auto-Mocking using Moq

One of the really cool features of DI is the fact that it makes testing code much, much easier.  This is because you can easily substitute one dependency for another by using a different Composition Root.  For example, if you only want to test a particular class (let's call it Foo) and don't care about testing its dependencies, you might write 'mocks' for them so that you can isolate Foo specifically.

    public class Foo
    {
        IWebServer _webServer;

        public Foo(IWebServer webServer)
        {
            _webServer = webServer;
        }

        public void Initialize()
        {
            ...
            var x = _webServer.GetSomething();
            ...
        }
    }

In this example, we have a class Foo that interacts with a web server to retrieve content.  This would normally be very difficult to test for the following reasons:

* You would have to set up an environment where it can properly connect to a web server (configuring ports, urls, etc.)
* Running the test could be slower and limit how much testing you can do
* The web server itself could contain bugs so you couldn't with certainty isolate Foo as the problematic part of the test
* You can't easily configure the values returned from the web server to test sending various inputs to the Foo class

However, if we create a mock class for IWebServer then we can address all these problems:

    public class MockWebServer : IWebServer
    {
        ...
    }

Then hook it up in our installer:

    _container.Bind<IWebServer>().ToSingle<MockWebServer>();

Then you can implement the fields of the IWebServer interface and configure them based on what you want to test on Foo. Hopefully You can see how this can make life when writing tests much easier.

Zenject also allows you to even avoid having to write the MockWebServer class in favour of using a very useful library called "Moq" which does all the work for you.

Note that by default, Auto-mocking is not enabled in Zenject.  If you wish to use the auto-mocking feature then you need to go to your Zenject install directory and extract the contents of "Extras/ZenjectAutoMocking.zip".  Note also that AutoMocking is incompatible with webplayer builds, and you will also need to change your "Api Compatibility Level" from ".NET 2.0 Subset" to ".NET 2.0" (you can find this in PC build settings)

After extracting the auto mocking package it is just a matter of using the following syntax to mock out various parts of your project:

    _container.Bind<IFoo>().ToMock();

However, this approach will not allow you to take advantage of the advanced features of Moq.  In order to do that, I recommend peeking in to the ToMock() method to see how that works.

## <a id="graphviz"></a>Visualizing Object Graphs Automatically

Zenject allows users to generate UML-style images of the object graphs for their applications.  You can do this simply by running your Zenject-driven app, then selecting from the menu `Assets -> Zenject -> Output Object Graph For Current Scene`.  You will be prompted for a location to save the generated image file.

Note that you will need to have graphviz installed for this to work (which you can find [here](http://www.graphviz.org/)).  You will be prompted to choose the location the first time.

The result is two files (Foo.dot and Foo.png).  The dot file is included in case you want to add custom graphviz commands.  As an example, this is the graph that is generated when run on the sample project:

<img src="ExampleObjectGraph.png?raw=true" alt="Example Object Graph" width="600px" height="127px"/>

## <a id="help"></a>Further Help

There currently does not exist a support forum yet.  In the meantime, I would recommend creating an issue on the Zenject github repository, which you can find [here](https://github.com/modesttree/Zenject).

Alternatively, you can contact me directly at svermeulen@modesttree.com

## <a id="license"></a>License

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

