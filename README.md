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
    * <a href="#getting_started">Getting Started</a>
    * <a href="#zenject_overview">API Overview</a>
    * <a href="#composition_root"/>Composition Root</a>
    * <a href="#dependency_root">Dependency Root</a>
    * <a href="#rules">Rules</a>
    * <a href="#tickables">ITickable & IInitializable</a>
    * <a href="#hello_world">Hello World</a>
    * <a href="#update_order">Update Order And Initialization Order</a>
    * <a href="#across_scenes">Injecting Data Across Scenes</a>
    * <a href="#automocking">Auto-Mocking</a>
    * <a href="#graphviz">Visualizing Your Object Graph</a>
    * <a href="#dynamic_creation">Creating Objects Dynamically</a>
    * <a href="#bindscope">Using BindScope</a>
    * <a href="#disposables">Implementing IDisposable</a>
* FAQ
    * <a href="#strange">How is this different from Strange IoC?</a>
    * More to come!

## <a id="introduction"></a>Introduction

Zenject is a lightweight dependency injection framework built specifically to target Unity 3D.  It can be used to turn your Unity 3D application into a collection of loosely-coupled parts with highly segmented responsibilities.  Zenject can then glue the parts together in many different configurations to allow you to easily write, re-use, refactor and test your code in a scalable and extremely flexible way.

This project is open source.  You can find the official repository [here](https://github.com/modesttree/Zenject).  If you would like to contribute to the project pull requests are welcome!

For general support or bug requests, please feel free to create issues on the github page.  You can also email me directly at svermeulen@modesttree.com

## <a id="features"></a>Features

* Injection into normal C# classes or MonoBehaviours
* Constructor injection (can tag constructor if there are multiple)
* Field injection
* Property injection
* Named injections (string, enum, etc.)
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

* Testability - Writing automated unit tests or user-driven tests becomes very easy, because it is just a matter of writing a different 'composition root' which wires up the dependencies in a different way.  Want to only test one subsystem?  Simply create a new composition root which creates 'mocks' for all other systems in the application. (more detail on this below)
* Refactorability - When code is loosely coupled, as is the case when using DI properly, the entire code base is much more resilient to changes.  You can completely change parts of the code base without having those changes wreak havoc on other parts.
* Encourages modular code - When using a DI framework you will naturally follow better design practices, because it forces you to think about the interfaces between classes.

## <a id="getting_started"></a>How should I get started?

Once you have an understanding of the theory behind Zenject (if not see the previous sections), then I recommend diving in to the included sample application to get started, by extracting the included unity package "ZenjectSampleGame.unitypackage" into your unity project.

## <a id="zenject_overview"></a>Overview Of The Zenject API

What follows is a general overview of how DI patterns are applied using Zenject.  However, the best documentation right now is probably the included sample project itself.  I would recommend using that for reference when reading over these concepts.

## <a id="composition_root"/></a>Composition Root / Installers

If you look at the sample application (a kind of asteroids clone) you will see that at the top of the scene heirarchy (in scene 'Main') we have a game object with the name CompositionRoot.   This is where Zenject resolves all dependencies before kicking off your application.

To add dependency bindings to your application, you need to write what is referred to in Zenject as an 'Installer' which usually looks something like this:

    [Serializable]
    public class GameInstaller : Installer
    {
        public string SomeSetting;

        public override void RegisterBindings()
        {
            ...
            _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();
            ...
        }
    }

    public class GameInstallerWrapper : InstallerMonoBehaviourWrapper<GameInstaller>
    {
    }

The RegisterBindings() method is called once at the entry point of the application by the composition root.  Note here that the Installer class is not a MonoBehaviour and therefore cannot be dragged onto unity game objects.  This is to allow installers to easily trigger other installers and also to allow installers to be used in non-unity contexts (eg: NUnit tests).  However, it is also very useful to be able to simply drag and drop different sets of installers into a given unity scene, which is why in many cases you will want to provide the extra wrapper class.

Once RegisterBindings() is called the installer can begin mapping out the object graph to be used in the application.  The syntax here will be familiar to users of many other DI frameworks.

Like many other DI frameworks, dependency mapping done by adding the binding to something called the container.  The container should then 'know' how to create all the object instances in our application, by recursively resolving all dependencies for a given object.  You can do this by calling the Resolve method:

    Foo foo = _container.Resolve<Foo>()

However, any use of the container should be restricted to the composition root or factory classes (see rules/guidelines section below)

## <a id="dependency_root"></a>The dependency root

Every Zenject app has one root object.  The dependencies of this object generates the full object graph for the application/game.  For example, in the sample project this is the GameRoot class which is declared as below:

    _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();

A Zenject driven application is executed by the following steps:

* Composition Root is started (via Awake() method)
* Composition Root calls RegisterBindings() on all installers that are attached below it in the scene heirarchy
* Each Installer registers different sets of dependencies directly on to the DiContainer by calling Bind<> and BindValue<> methods.  Note that the order that this binding occurs should not matter.
* The Composition Root then traverses the scene heirarchy again and injects all MonoBehaviours with their dependencies.  Since MonoBehaviours are instantiated by Unity we cannot use constructor injection in this case and therefore field or property injection must be used (which is done by adding a [Inject] attribute to any member)
* After filling in the scene dependencies the CR then calls `_container.Resolve` on the root object (that is, whatever is bound to IDependencyRoot).  In most cases code does not need to be in MonoBehaviours and will be resolved this way
* If a dependency cannot be resolved, a ZenjectResolveException is thrown

## <a id="rules"></a>DI Rules / Guidelines / Recommendations

* The container should *only* be referenced in the composition root layer.  Note that factories are part of this layer and the container can be referenced there (which is necessary to create objects at runtime).  For example, see ShipStateFactory in the sample project.  See <a href="#dynamic_creation">Creating Objects Dynamically</a>
* Prefer constructor injection to field or property injection.
    * Constructor injection forces the dependency to only be resolved once, at class creation, which is usually what you want.  In many cases you don't want to expose a public property with your internal dependencies
    * Constructor injection guarantees no circular dependencies between classes, which is generally a bad thing to do
    * Constructor injection is more portable for cases where you decide to re-use the code without a DI framework such as Zenject.  You can do the same with public properties but it's more error prone.  It's possible to forget to initialize one field and leave the object in an invalid state
    * Finally, Constructor injection makes it clear what all the dependencies of a class are when another programmer is reading the code.  They can simply look at the parameter list of the constructor.

## <a id="tickables"></a>Tickables / IInitializables

I prefer to avoid MonoBehaviours when possible in favour of just normal C# classes.  Zenject allows you to do this much more easily by providing interfaces that mirror functionality that you would normally need to use a MonoBehaviour for.

For example, if you have code that needs to run per frame, then you can implement the ITickable interface:

    public class Ship : ITickable
    {
        public void Tick()
        {
            // Perform per frame tasks
        }
    }

Then it's just a matter of including the following in one of your installers (as long as you also include a few other dependencies as outlined in the hello world example below)

    _container.Bind<ITickable>().ToSingle<Ship>();

The same goes for IInitializable, for cases where you have code that you want to run on startup.  (side note: using IInitializable is generally better than putting too much work in constructors).  IInitializable can also be used for objects that are created via factories (in which case Initialize() is called automatically, as long as you use one of the built in Zenject factory classes).

Note that you do not need to use this approach (that is, ITickables and IInitializables) to use Zenject. You can continue writing all your code in MonoBehaviours and still receive all the benefits of Zenject.

## <a id="hello_world"></a>Zenject Hello World

    public class TestInstallerWrapper : InstallerMonoBehaviourWrapper<TestInstaller>
    {
    }

    [Serializable]
    public class TestInstaller : Installer
    {
        public string Name;

        public override void RegisterBindings()
        {
            Install<StandardUnityInstaller>();

            _container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();

            _container.Bind<ITickable>().ToSingle<TestRunner>();
            _container.Bind<IInitializable>().ToSingle<TestRunner>();
            _container.Bind<string>().ToSingle(Name).WhenInjectedInto<TestRunner>();
        }
    }

    public class TestRunner : ITickable, IInitializable
    {
        string _name;

        public TestRunner(string name)
        {
            _name = name;
        }

        public void Initialize()
        {
            Debug.Log("Hello " + _name + "!");
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

You can run this example by copying and pasting the above code into a file named 'TestInstallerWrapper'.  Then create a new scene, add a GameObject. Attach CompositionRoot to the GameObject.  Attach TestInstallerWrapper.  Run.  Observe unity console.

Some notes:

* The `Install<StandardUnityInstaller>()` line is necessary to tell zenject to initialize some basic unity helper classes (including the Zenject class which updates all ITickables and the class which calls Initialize on all IInitializables).  It is done this way because in some cases you might not want to use the whole ITickable/IInitializable approach at all.  Or maybe you aren't even using Unity. Etc.
* You will also need to define a dependency root otherwise Zenject will not create your object graph
* Note that all Installers use the [Serializable] attribute.  This is so that Installers can expose settings to their MonoBehaviour wrapper.  In this case, we expose a "Name" variable.
* Note the usage of WhenInjectedInto.  This is good because otherwise any class which had a string parameter in its constructor would get our Name parameter.

## <a id="update_order"></a>Update / Initialization Order

In many cases, especially for small projects, the order that classes update or initialize in does not matter.  This is why Unity does not have an easy way to control this (besides in Edit -> Project Settings -> Script Execution Order but that is pretty awkward to use).  In Unity, after adding a bunch of MonoBehaviours to your scene, it can be difficult to predict in what order the Start(), Awake(), or Update() methods will be called in.

By default, ITickables and IInitializables are updated in the order that they are added, however for cases where the update or initialization order matters, there is a much better way.  By specifying their priorities explicitly in the installer.  For example, in the sample project you can find this code:

        public override void RegisterBindings()
        {
            ...
            new TickablePrioritiesInstaller(_container, Tickables).RegisterBindings();
            new InitializablePrioritiesInstaller(_container, Initializables).RegisterBindings();
        }

        static List<Type> Tickables = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };

        static List<Type> Initializables = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };

This way, you won't hit a wall at the end of the project due to some unforseen order-dependency.

Any ITickables or IInitializables that aren't given an explicit order are updated after everything else.

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
                _startLevel = "level01";
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

    ZenUtil.LoadLevel("NameOfSceneToLoad",
        delegate (DiContainer container)
        {
            container.Bind<string>().ToSingle("level02").WhenInjectedInto<LevelHandler>("StartLevelName");
        });

Note that you can still run the scene directly, in which case it will default to using "level01".  This is possible because we are using the InjectOptional flag.

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

Note that by default, Auto-mocking is not enabled in Zenject.  If you wish to use the auto-mocking feature then go to your Zenject install directory and extract the contents of AddOns/ZenjectAutoMocking.unitypackage to your unity project.  Note also that AutoMocking is incompatible with webplayer builds, and you will also need to change your "Api Compatibility Level" from ".NET 2.0 Subset" to ".NET 2.0" (you can find this in PC build settings)

After extracting the auto mocking package it is just a matter of using the following syntax to mock out various parts of your project:

    _container.Bind<IFoo>().ToMock();

However, this approach will not allow you to take advantage of the advanced features of Moq.  In order to do that, I recommend peeking in to the ToMock() method to see how that works.

## <a id="graphviz"></a>Visualizing Object Graphs Automatically

Zenject allows users to generate UML-style images of the object graphs for their applications.  You can do this simply by running your Zenject-driven app, then selectin from the menu `Assets -> Zenject -> Output Object Graph For Current Scene`.  You will be prompted for a location to save the generated image file.

Note that you will need to have graphviz installed for this to work (which you can find [here](http://www.graphviz.org/)).  You will be prompted to choose the location.

The result is two files (Foo.dot and Foo.png).  The dot file is included in case you want to add custom graphviz commands.

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

This will not work however, since in our case the Enemy class requires a reference to the Player class in its constructor.  We could add a dependency to the Player class to the EnemySpawner class, but then we have the problem described <a id="theory">above</a>.  The EnemySpawner class doesn't care about filling in the dependencies for the Enemy class.  All the EnemySpawner class cares about is getting a new Enemy instance.

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

    _container.Bind<IFactory<TContract>>().ToSingle<Factory<TContract>>();

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

And then change our installer to include:

    _container.Bind<EnemyFactory>().ToSingle();

Note the following:
* We no longer need the line `_container.BindFactory<Enemy>();` since we are directly creating the Factory<> class in the EnemyFactory constructor.
* We are injecting the DiContainer directly into the EnemyFactory class, which is generally a bad thing to do but ok in this case because it is a factory (and therefore part of the "composition root layer")

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

    _container.Bind<IInitializable>().Bind<Logger>();
    _container.Bind<IDisposable>().Bind<Logger>();

This works because when the scene changes or your unity application is closed, the unity event OnDestroy() is called on all MonoBehaviours, including the CompositionRoot class.  The CompositionRoot class, which owns your DiContainer, calls Dispose() on the DiContainer, which then calls Dispose() on all objects that are bound to IDisposable.

Note that this example may or may not be a good idea (for example, the file will be left open if your app crashes), but illustrates the point  :)

## <a id="strange"></a>How is this different from Strange IoC?

Zenject is a pure dependency injection framework and does not offer the suite of features that Strange IoC does.  It is kept extremely lightweight to focus on its single purpose: Simple, reliable, and flexible dependency management.

****
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

