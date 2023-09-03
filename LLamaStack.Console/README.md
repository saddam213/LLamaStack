# LLamaStack.Console - Dev Playground For LLamaStack

**LLamaStack.Console** is an application designed for developers to showcase new features, create examples for others to follow, or even to reproduce and debug issues.

Creating an example is simple. Just add a new class to the `Examples` folder, ensuring that your class implements the `IExampleRunner` interface. Once you've done that, your example will automatically be added to the main menu at runtime.

Example:
```cs
    public class MyCoolExample : IExampleRunner
    {
        public string Name => "My Cool Example";

        public string Description => "A really cool examplethat does nothing much helpful";

        public async Task RunAsync()
        {
             OutputHelpers.WriteConsole("I'm Helping!", ConsoleColor.Cyan);
             OutputHelpers.ReadConsole(ConsoleColor.Green)
        }
    }
```


All LLamaStack services can be injected into the Examples as normal

```cs
    public class MyCoolExample : IExampleRunner
    {
        private readonly IModelSessionService<string> _modelSessionService;

        public MyCoolExample(IModelSessionService<string> modelSessionService)
        {
            _modelSessionService = modelSessionService;
        }

        .....
    }
```