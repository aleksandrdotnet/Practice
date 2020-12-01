> **Name**: Double-checked lock is not thread-safe
>
> **Description**: A repeated check on a non-volatile field is not thread-safe on some platforms, and could result in unexpected behavior.
> 
> **Kind**: problem
> 
> **Severity**: error

### Description
Double-checked locking requires that the underlying field is _volatile_, otherwise the program can behave incorrectly when running in multiple threads, for example by computing the field twice.

### Problem
The following code defines a property called _Name_, which calls the method _LoadNameFromDatabase_ the first time that the property is read, and then caches the result. This code is efficient but will not work properly if the property is accessed from multiple threads, because _LoadNameFromDatabase_ could be called several times.

```c#
string name;
 
public string Name
{
    get
    {
        // BAD: Not thread-safe
        if (name == null)
            name = LoadNameFromDatabase();
        return name;
    }
}
```
A common solution to this is _double-checked locking_, which checks whether the stored value is _null_ before locking the mutex. This is efficient because it avoids a potentially expensive lock operation if a value has already been assigned to _name_.
```c#
string name;    // BAD: Not thread-safe
 
public string Name
{
    get
    {
        if (name == null)
        {
            lock (mutex)
            {
                if (name == null)
                    name = LoadNameFromDatabase();
            }
        }
        return name;
    }
}
```

However this code is incorrect because the field name isn't volatile, which could result in name being computed twice on some systems.

### Recommendation 
#### Theory
* There are several ways to make the code thread-safe:
* Avoid double-checked locking, and simply perform everything within the lock statement.
* Make the field volatile using the _volatile_ keyword.
* Use the _System.Lazy class_, which is guaranteed to be thread-safe. This can often lead to more elegant code.
* Use _System.Threading.LazyInitializer_.

#### Practice

The first solution is to simply avoid double-checked locking

```c#
string name;
 
public string Name
{
    get
    {
        lock (mutex)    // GOOD: Thread-safe
        {
            if (name == null)
                name = LoadNameFromDatabase();
            return name;
        }
    }
}
```

Fix would be to make the field volatile

```c#
volatile string name;    // GOOD: Thread-safe
 
public string Name
{
    get
    {
        if (name == null)
        {
            lock (mutex)
            {
                if (name == null)
                    name = LoadNameFromDatabase();
            }
        }
        return name;
    }
}
```

Elegant to use the class _System.Lazy_, which is automatically thread-safe

```c#
Lazy<string> name;    // GOOD: Thread-safe
 
public Person()
{
    name = new Lazy<string>(LoadNameFromDatabase);
}
 
public string Name => name.Value;
```

### References
- MSDN: [Lazy<T> Class](https://docs.microsoft.com/en-us/dotnet/api/system.lazy-1?view=net-5.0).
- MSDN: [LazyInitializer.EnsureInitialized Method](https://docs.microsoft.com/en-us/dotnet/api/system.threading.lazyinitializer.ensureinitialized?view=net-5.0).
- MSDN: [Implementing Singleton in C#](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff650316(v=pandp.10)?redirectedfrom=MSDN).
- MSDN Magazine: [The C# Memory Model in Theory and Practice](https://docs.microsoft.com/en-us/archive/msdn-magazine/2012/december/csharp-the-csharp-memory-model-in-theory-and-practice).
- MSDN, C# Reference: [volatile](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile).
- Wikipedia: [Double-checked locking](https://en.wikipedia.org/wiki/Double-checked_locking).
- Common Weakness [Enumeration: CWE-609](https://cwe.mitre.org/data/definitions/609.html).
