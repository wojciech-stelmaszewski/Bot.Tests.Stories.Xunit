# Bot.Tests.Stories.Xunit

[![Build status](https://ci.appveyor.com/api/projects/status/yinx2sypuempoi3g?svg=true)](https://ci.appveyor.com/project/ObjectivityAdminsTeam/bot-tests-stories-xunit)

Tests library for Bot Framework dialogs using XUnit and Objectivity Test Stories.

## Note

This project is still a work in progress, all contributions from your site will be very desirable!

## Installation

You can install the package using the nuget:

```powershell
Install-Package Objectivity.Bot.Tests.Stories.Xunit
```

## Limitations

At the moment test base classes are permitted only for dialogs returning object type.

## Usage

## Simple dialogs

To develop a unit test for a dialog, create new test class inheriting from `Objectivity.Bot.Tests.Stories.Xunit.DialogUnitTestBase<T>` class, providing your dialog Type as generic parameter. Then for each test please go through the following steps:

* Record a story
* Rewind
* Play (assert)

Example:

```cs
public class EchoDialogTests : DialogUnitTestBase<EchoDialog>
{
    [Fact]
    public async Task HelloTest()
    {
        var story = StoryRecorder
            .Record()
            .User.Says("Hello")
            .Bot.Says("You said Hello")
            .Rewind();

        await this.Play(story);
    }
}
```

### LUIS dialogs

To develop a unit test for a LUIS dialog (inheriting `LuisDialog<object>` class), create new test class inheriting from `Objectivity.Bot.Tests.Stories.Xunit.LuisDialogUnitTestBase<T>` class, providing your dialog Type as generic parameter. Then for each test please go through the following steps:

* Register utterance (for intent test)
* Record a story
* Rewind
* Play (assert)

Example:

```cs
public class PizzaOrderDialogTests : LuisDialogUnitTestBase<EchoDialog>
{
    [Fact]
    public async Task HelloTest()
    {
        var story = StoryRecorder
            .Record()
            .User.Says("Hello")
            .Bot.Says("You said Hello")
            .Rewind();

        await this.Play(story);
    }
}
```

### Injecting dependencies

If your dialog requires some dependencies injected using Autofac, you can provide them by overloading `RegisterAdditionalTypes` protected method. Example:

```cs
protected override void RegisterAdditionalTypes(ContainerBuilder builder)
{
    builder.RegisterType<EchoService>().As<IEchoService>();
}
```