Dependee
==============

XAML/C# Property Dependency Manager
--------------

A library which simplifies dependency management between properties in a XAML/C# project. Install it with NuGet:

    PM> Install-Package Dependee

Problem
--------------

Using the standard `INotifyPropertyChanged` interface, properties must notify their dependents when they change, as well as any properties which are dependent on them, and so on. Unfortunately, it is difficult to know who is dependent on you (especially indirectly), and it is not something you should be responsible for. This can often lead to confusing scenarios with difficult to find bugs.

```csharp
public partial class MyClass : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    private void updateProp(string name)
    {
        if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    private string data = null;
    public string Data
    {
        get { return data; }
        set
        {
            data = value;

            // Update dependents of Data
            updateProp("Data");
            updateProp("Data3"); // Must update dependencies
            updateProp("Data4"); // Even indirect ones

            updateProp("Who else is dependent on Data?");
            updateProp("Data5, wait no, that doesn't exist");
            updateProp("???");
            updateProp("AHHH!1111 Halp!");
        }
    }

    private string data2 = null;
    public string Data2
    {
        get { return data2; }
        set
        {
            data2 = value;

            // Update dependents of Data2
            updateProp("Data2");
            updateProp("Data3"); // Must update dependencies
            updateProp("Data4"); // Even indirect ones

            updateProp("Who else is dependent on Data2?");
            updateProp("Data5, wait no, that doesn't exist");
            updateProp("???");
            updateProp("AHHH!1111 Halp!");
        }
    }

    public string Data3 { get { return Data + ", " + Data2; } }

    public string Data4 { get { return Data3 + "; " + Data3; } }
	
	...
}
```

Solution
--------------

Dependee flips this system. Instead of prerequisites telling their dependencies when to update, dependencies listen for changes on their prerequisites. This allows each property to only be concerned with its own dependencies which creates a much more scalable and maintainable pattern.

```csharp
private string data = null;
public string Data
{
    get { return data; }
    set
    {
        data = value;
        DependeeManager.Update(this, "Data"); // Tell Dependee I updated
        // Who's dependent on me? I 'unno...
    }
}

private string data2 = null;
public string Data2
{
    get { return data2; }
    set
    {
        data2 = value;
        DependeeManager.Update(this, "Data2"); // Tell Dependee I updated
        // Who's dependent on me? Why would I care?
    }
}

[Dependency("Data")]  // I'm dependent on Data
[Dependency("Data2")] // I'm also dependent on Data2
public string Data3 { get { return Data + ", " + Data2; } }

[Dependency("Data3")] // I'm dependent on Data3
// This makes me indirectly dependent on Data and Data2, I wouldn't know or care though...
public string Data4 { get { return Data3 + "; " + Data3; } }
```

Properties on Other Objects
--------------

Dependee can even manage dependencies which cross between different objects, allowing a property on one view model to be dependent on a property from another.

```csharp
class MyInnerClass : IDependeeObject
{
    private string data = "Hello";
    public string Data
    {
        get { return data; }
        set
        {
            data = value;
            DependeeManager.Update(this, "Data");
        }
    }
	
    ...
}

class MyOuterClass : IDependeeObject
{
    public MyInnerClass inner { get; set; } = new MyInnerClass();
    
    [Dependency("inner.Data")] // Can reference other objects
    public string Data2 { get { return inner.Data + " World!" } }
	
    ...
}
```

Setup
--------------

In order for a class to take advantage of Dependee, it needs to set up a few things. It may seem like a lot, but most of it replaces code you'd have to write to use `INotifyPropertyChanged` anyways.

1. Install Dependee in the project using NuGet.
    * `PM> Install-Package Dependee`
2. Include necessary namespaces at the top of the file.
    * `using System.ComponentModel;`
    * `using Dependee;`
3. Implement `IDependeeObject` interface, just like you would for `INotifyPropertyChanged`.
    * `public class <ClassName> : IDependeeObject`
4. In the class constructor, mark the class as managed by Dependee
    * `DependeeManager.Manage(this);`
5. Declare `PropertyChanged` event, just like you would for `INotifyPropertyChanged`.
    * `public event PropertyChangedEventHandler PropertyChanged;`
6. Implement `PropertyChangedTrigger()` by invoking the `PropertyChanged` event.
```csharp
public void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args)
{
	if (PropertyChanged != null) PropertyChanged.Invoke(sender, args);
}
```
7. In any property setter, call `DependeeManager.Update()` to tell Dependee of the change.
    * `DependeeManager.Update(this, "PropertyName");`
8. On any property with a dependency, add `Dependency` attribute to declare it.
    * `[Dependency("PrerequisitePropertyName")]`
9. Run it!

Complete Example
--------------

```csharp
using Dependee; // Use Dependee library

public class MyClass : IDependeeObject // Implement Dependee Interface
{
    public event PropertyChangedEventHandler PropertyChanged; // Declare PropertyChanged event
    
    private string data = null;
    public string Data
    {
        get { return data; }
        set
        {
            data = value;
            DependeeManager.Update(this, "Data"); // Tell Dependee of property update
        }
    }

    private string data2 = null;
    public string Data2
    {
        get { return data2; }
        set
        {
            data2 = value;
            DependeeManager.Update(this, "Data2"); // Tell Dependee of property update
        }
    }

    [Dependency("Data")]  // Data3 is dependent on Data
    [Dependency("Data2")] // Data3 is dependent on Data2
    public string Data3 { get { return Data + ", " + Data2; } }

    [Dependency("Data3")] // Data4 is dependent on Data3
    // Data4 is indirectly dependent on Data and Data2, no need to specify
    public string Data4 { get { return Data3 + "; " + Data3; } }

    public MyClass()
    {
        InitializeComponent();

        DependeeManager.Manage(this); // Tell Dependee to manage this object
    }

    // Implement IDependeeObject with a function that triggers the PropertyChanged event
    public void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args)
    {
        if (PropertyChanged != null) PropertyChanged.Invoke(sender, args);
    }
}
```