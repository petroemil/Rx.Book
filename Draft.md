# Introduction

## The problem - asynchrony

If you are developing modern applications (whether it's a mobile, desktop or server side application), at some point you will reach the limitations of synchronously running code and find yourself facing the difficulties of asynchronous and/or event-driven development. If you want to build a performant and responsive application and handle I/O operations, web service calls or CPU intensive tasks asynchronously, your code will become much more difficult to handle. Unusual methods must be used for coordination, exception handling and you might have to deal with cancellability of long -and the synchronization of parallel running operations.

Reactive Extensions (hereinafter referred to as Rx) is a class library that allows us to build asynchronous and/or event-driven applications by representing the asynchronous data as "Observable" streams that we can use LINQ operations on and the execution, handling of race conditions, synchronization, marshalling of threads and more are handled by so called "Schedulers".

In short: Rx = Observables + LINQ + Schedulers

## What is an Observable?

To understand the concept of Observables, you first have to understand the basic difference between synchronous and asynchronous programming.

Depending on what kind of application you are building, there is a good chance that most of the business logic is synchronous code which means that all the lines of code you write will be executed sequentially, one after the other. The computer takes a line, executes it, waits for it to finish and only after that will it go for the next line of code to execute it. A typical console application only contains synchronous code, to the level that if it have to wait for input from the user, it will just wait there doing exactly nothing.

In contrast with that, asynchrony means you want to do things in response to events you don't necessarily know when they happen.

A good example for this is the `Click` event of a button on the UI, but also the response from a web service call. In the case of the latter it's very much possible that in terms of the execution order of the commands you know exactly that you only want to do things after you have the response, but you have to consider the chance that this service call could take seconds to return, and if it would be written in a synchronous way, the execution on the caller thread would be blocked and it couldn't do anything until the service call returns. And this can lead to very unpleasant experience for the users as they will just see the UI becoming unresponsive or "frozen". To make sure it doesn't happen you will have to do asynchronous call which means that the logic will be executed (or waited in the case of I/O operations) in the background without blocking the calling thread and you will get the result through some kind of callback mechanism. If you have been around in the developer industry for a while, you might remember the dark days of callback chains, but fortunately in C# 5.0 / .NET 4.5 (in 2012) Microsoft introduced the `async` and `await` keywords that makes it possible to write asynchronous code that looks like just a regular synchronous code. Just think about the difficulties of implementing simple language constructs like a conditional operation, a loop or a try-catch block with the traditional form (callbacks) of asynchronous programming. With these new keywords you can naturally use all of these while still having a performant, responsive and scalable asynchronous code. But even though it makes life significantly easier in many scenarios, the moment we want to do something a little more complicated, like a retry, timeout or adding paging functionality to a web service call, we have to start writing complex logic because hiding the difficulties of dealing with callbacks won't save us from implementing these custom operations.

Events (in C#) has a serious problem though, they are not objects, you can't pass them around as method parameters, and tracking the subscribers of an event is also not trivial. This is where the Observer design patter comes in handy, because it pretty much re-implements events, but this time your event source and the subscribers will be objects and the subscription will be an explicit operation.

The Observer design pattern consists of two simple interfaces.

One of them is the `IObservable<T>` which represents an "observable" data source. This interface only has a `Subscribe()` method which kind of translates to the `+=` operator of the "event world".

The other one is the `IObserver<T>` which represents an "observer" object that we can pass to the `IObservable<T>`'s `Subscribe()` method. In the original design pattern this interface has only a `Notify()` method that gets called by the event source when an event occurs.

In the case of Rx, the implementation is slightly different. The `IObserver<T>` interface defines three methods: `OnNext()`, `OnError()` and `OnCompleted()`. `OnNext()` gets called for each new event from the event source, `OnError()` if something bad happened and `OnCompleted()` if the source wants to signal that it will not produce any more events and the subscription can be disposed.

Rx is built on top of these two interfaces, and using this simple design pattern it makes dealing with asynchronous programming extremely easy (and I would even go as far as saying fun). To understand how, let's talk a little bit about LINQ.

## What is LINQ?

LINQ (Language Integrated Query) is making dev's life easier since .NET 3.5 (2007) when it comes to dealing with processing collections. This technology, or more like the language features that have been introduced with it, brought the first seeds of functional programming to C#. I won't go into details about what functional programming means because it's way out of the scope of this book, but I would like to quote Luca Bolognese's analogy from a PDC presentation from 2008.

>It's like going to a bar and telling the guy behind the counter <br/>
> \- I want a cappuccino <br/>
>or going to the bar and telling the same guy <br/>
> \- Now you grind the coffee, then get the water, warm it, at the same time prepare the milk, and I want these two thing to go in parallel...

To bring a bit better example, think about how would you solve the following problem: Given a range of numbers, get the even numbers and sum their squares.

The very first solution would probably look something like the Code Sample 1-1.

```csharp
// Code Sample 1-1
// Sum of the square of the even numbers - naive approach

List<int> numbers = new List<int>();
numbers.Add(1);
numbers.Add(2);
numbers.Add(3);
numbers.Add(4);
numbers.Add(5);
numbers.Add(6);
numbers.Add(7);
numbers.Add(8);
numbers.Add(9);
numbers.Add(10);

int accumulator = 0;
foreach (var number in numbers)
{
    if (number % 2 == 0)
    {
        accumulator += (number * number);
    }
}

return accumulator;
```

What could you do to make this code look prettier? How could you compress it a little bit, make it less verbose? Let's start with the initialization of the list (Code Sample 1-2).

```csharp
// Code Sample 1-2
// Collection initializer

var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
```

Let's go through this piece of code from keyword to keyword.

It starts with a `var` keyword. When you declare (and also initialize in the same time) a variable, the compiler can infer the type of the variable based on the right side of the equals sign. In this example if you would move the mouse cursor over the `var` keyword, it would show that the numbers variable has the type of `List<int>`. This is a handy little feature of the language that makes the code a bit more compact and readable, but you will see examples for situations where it has a deeper role to enable certain functionality.

Let's move on and take a look at the initialization of the list. The line doesn't end with calling the parameter less constructor, but the items of the list are defined within curly braces. This is called the object (or in this case collection) initializer syntax. With this you can initialize the properties of an object or the elements of a collection with a more lightweight syntax.

Even though it doesn't belong to this specific example, but if you would combine the `var` keyword and the object initializer syntax, you would arrive to the so called anonymous types. This is really useful when you want to store temporary information in the middle of your algorithm, in a nice and structured format, without explicitly creating the class(es) that represent that structure. You can see a simple example in Code Sample 1-3

```csharp
// Code Sample 1-3
// Anonymus Type

var person = new { Name = "Emil", Age = 27 };
```

Here the compiler can look at the right side of the equals sign, it won't be able to guess the type of the variable... so it generates one behind the scene.

Let's get back to the original example, and take the refactoring of the code one step further.

```csharp
// Code Sample 1-4
// Sum of the square of the even numbers - LINQ approach

return numbers
    .Where(num => num % 2 == 0)
    .Select(num => num * num)
    .Sum();
```

If you remember, the numbers variable is a collection of integers. How can you call a method called `Where()` on it, when the `List<T>` type doesn't have such method? The answer for this question are extension methods. In the good old days when you wanted to add new functionality to a type, you inherited a new class from the original type and defined the new method on it. This approach has two main problems though. On one side it's possible that the type you want to extend is sealed, the other problem is that even if you can define your own "super type", you will have to use that throughout your application and convert back and forth between that and the original type on the edges of your library.

You don't have to think something too complicated. Just think about a function that extends the `string` with some special kind of formatting. If you want to use this logic in multiple places, you either have to move it to a static helper class or define your `SuperString` class. The latter in this specific example is not actually possible because the string type is sealed. So you are left with the static class. But think about how it would look of you would have to use more of these operations after each other...

To solve these kind of problems, Microsoft introduced the extension methods in C#, which makes it possible to extend any class (or even interface) using the following syntax:

```csharp
// Code Sample 1-5
// Extension Method signature

public static string SpecialFormat(this string s, int spaces)
```

You have to place these extension methods in a static class. The interesting part of this method signature is it's very first parameter that has the `this` keyword in front of it. It means that you are extending the `string` class with this method. Whenever you have a `string` in your application, you can just call this method, the system will automatically hide this first parameter by implicitly passing the reference of the `string` there, and you only have to provide - in this example - the `int spaces` parameter. The Code Sample 1-6 shows an example for the usage of this method.

```csharp
// Code Sample 1-6
// Using an Extension Method

"Hello World".SpecialFormat(42);
```

The way LINQ uses these extension methods brings this to the next level. To stay at the original example, let's take a quick look at the Where method's signature.

```csharp
// Code Sample 1-7
// Signature of LINQ's Where() Extension Method

public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
```

The thing that you should pay attention to is that it extends the `IEnumerable<T>` interface and returns an object of type `IEnumerable<T>` as well. Almost all of the LINQ operators' signature looks like this, which is good because it enables the composition of these operators, so you can use as many of these operators after each other as many you want. The current example shows this very well by using the `Where()`, `Select()` and `Sum()` operators without introducing any kind of intermediate variables.

The other typical feature of LINQ operators is that they leave the important part of the work to you. In case of the `Where()` operator, the implementation only deals with iterating the original collection and building a new one, but the predicate for the selection logic is a parameter and it's up to you to define your logic to say which element stays and which one has to go. For this it uses a `Func<T, bool>` delegate, that represents a method that receives one element of the collection (the one that the `Where()` operator is internally just iterating through) and returns a `bool` which tells the operator whether the element can stay or it has to go.

If you continue reading the sample code, you very quickly arrive to an interesting expression passed as a parameter to the Where (and later to the Select) method.

```csharp
// Code Sample 1-8
// Lambda Expression

num => num % 2 == 0
```

This is called a lambda expression, or anonymous method. With this lightweight syntax it's possible to write a method (and pass it to some other part of the code as a parameter) without actually writing a method. With this you have the ability to write your logic in place instead of writing a method somewhere and referring to that as a delegate.

The full syntax of a lambda expression looks like this:

```csharp
// Code Sample 1-9
// Lambda Expression in it's "full glory"

(param1, param2) =>
{
    // ...
    return true;
};
```

In front of the arrow operator (`=>`), between parentheses you get the parameters, and after the arrow, between curly braces you have the body of the expression.
The reason you don't have to provide any type information for the parameters or the return type is because you are passing this expression as a delegate, so the compiler knows already all those information. 
There are some shortcuts as well.
If there's only one parameter, you can leave the parentheses, but if there's no parameter, you have to include an empty parentheses.
If the expression only contains one line of code, you also don't have to wrap it between curly braces and don't have to use the explicit return keyword.
And of course don't forget about the case where you actually have the method that matches the delegate's signature, in which case you can provide only the name of the method, so you don't have to write something like `Method(x => F(x))`, it's enough to just write `Method(F)`.
And just as a "fun fact" I'd like to mention that parallelizing the aforementioned example would only require one more operator in the beginning of the expression.

```csharp
// Code Sample 1-10
// Parallel LINQ

return numbers
    .AsParallel()
    .Where(num => num % 2 == 0)
    .Select(num => num * num)
    .Sum();
```

Thanks to the `AsParallel()` operator the whole query operation will run in parallel. Just think about what would you do if this requirement would come up after only having the original implementation in place.

LINQ is actually more than these language elements, there is actual language level support to write query expressions, but that's not really important from this book's point of view. What is important that LINQ is a smart combination of the aforementioned underlying language features, and a huge set of predefined extension methods for various types. One of those many types is the `IEnumerable<T>` class, but LINQ has very similar extension methods for other types of datasources, like relational databases (LINQ to SQL), XML (LINQ to XML), or in a way if we want to think about it that way events (LINQ to Events), which leads us to Rx.

## LINQ vs Rx

| LINQ (to Objects) | Rx |
| ---: | :--- |
| Can work with collections and it works on enumerable types, which means that it builds on the enumerator design pattern which is represented by the `IEnumerable<T>` and the `IEnumerator<T>` interfaces | Works with observable types, which means it builds on the observer design pattern which is represented by the `IObservable<T>` and `IObserver<T>` interfaces |
| Represents a *polling* technique, as internally we keep polling the `MoveNext()` method on the `IEnumerator` for new elements in the collection. We ask the system for the next element | Represents a *push* technique, as internally the `IObservable` object keeps a reference to the subscribed `IObserver` and calls it's `OnNext()` callback method when some event occurs. The system notifies us if there is a new element available
| Takes a collection and allows you to transform, filter, order, group and do many more operations on it and returns the modified collection | Takes a source event stream and transforms, filters, orders, groups and does many more operations on it, and returns the modified event stream |
| Is a collection of extension methods for the `IEnumerable<T>` interface | Is a collection of extension methods for the `IObservable<T>` interface |

# Hello Rx

## Preparations

In this chapter you will see a more realistic but still relatively simple example, that will demonstrate how much Rx simplifies your life even after the `async`/`await` keywords.

You will build a search application that works with a simulated web service. This service will provide suggestions and results from a hard coded list of words and more importantly it will simulate varying latency and failures.

Let's build the frame of the application so you can focus on the important part of the code later in the chapter.

* Open Visual Studio (2015)
* Create a new project (`File / New / Project`)
* In the dialog window select `Windows / Universal` on the left hand pane
* Then the `Blank App (Universal Windows)` project template on the right.
* Add the Rx NuGet package to the project by typing `PM> Install-Package System.Reactive` to the Package Manager console
* Get a list of words and save them in a static class named `SampleData`, returned by a method named `GetTop100EnglishWords()`. For reference here is the sample data I used in my application:
```csharp
// Code Sample 2-1
// Sample data

public static class SampleData
{
    public static List<string> GetTop100EnglishWords()
    {
        return new List<string>
        {
            "the", "be", "to", "of", "and", "a", "in", "that", "have", "I",
            "it", "for", "not", "on", "with", "he", "as", "you", "do", "at",
            "this", "but", "his", "by", "from", "they", "we", "say", "her", "she",
            "or", "an", "will", "my", "one", "all", "would", "there", "their", "what",
            "so", "up", "out", "if", "about", "who", "get", "which", "go", "me",
            "when", "make", "can", "like", "time", "no", "just", "him", "know", "take",
            "people", "into", "year", "your", "good", "some", "could", "them", "see", "other",
            "than", "then", "now", "look", "only", "come", "its", "over", "think", "also",
            "back", "after", "use", "two", "how", "our", "work", "first", "well", "way",
            "even", "new", "want", "because", "any", "these", "give", "day", "most", "us"
        };
    }
}
```
* Build the fake web service that will work with this sample data and simulate latency and failure. As a first step create the latency and failure simulation method
```csharp
// Code Sample 2-2
// Latency and failure simulation

private async Task SimulateTimeoutAndFail()
{
    // Simulating long time execution         
    var random = new Random();
    await Task.Delay(random.Next(300));

    // Simulating failure         
    if (random.Next(100) < 10)
        throw new Exception("Error!");
}
```
* The next method should be the one that returns the result for a search, as it won't do anything but generate a `string` saying *"This was your query: YOUR_QUERY"*
```csharp
// Code Sample 2-3
// Query results

public async Task<IEnumerable<string>> GetResultsForQuery(string query)
{
    await this.SimulateTimeoutAndFail();

    return new string[] { $"This was your query: {query}" };
}
```
* Last but not least, the most important part of the service is the one that will generate the suggestions. It will take the query, split it into individual words, try to suggest possible endings for the last word using the sample data, and generate the full suggestions by taking the "head" of the query (all the words before the last one) and the possible suggestions for the last one and concatenate them into strings.
```csharp
// Code Sample 2-4
// Query suggestions

public async Task<IEnumerable<string>> GetSuggestionsForQuery(string query)
{
    await this.SimulateTimeoutAndFail();

    var words = SampleData.GetTop100EnglishWords().Select(x => x.ToLower());

    var wordsOfQuery = query.ToLower().Split(' ');
    var lastWordOfQuery = wordsOfQuery.Last();
    var suggestionsForLastWordOfQuery = words.Where(w => w.StartsWith(lastWordOfQuery));

    var headOfQuery = "";
    if (wordsOfQuery.Length > 1)
    {
        headOfQuery = String.Join(" ", wordsOfQuery.SkipLast(1));
    }
            
    return suggestionsForLastWordOfQuery.Select(s => $"{headOfQuery} {s}").Take(10);
}
```
* Now the only missing piece is the UI. It will be very simple, a `TextBox` to type the query into, a `Button` to initiate the search, a `TextBlock` for "debug" information and a `ListView` to display the suggestions
```XML
// Code Sample 2-5
// The XAML UI

<Grid Margin="100">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition />
    </Grid.RowDefinitions>

    <Grid Grid.Row="0">
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>

        <TextBox x:Name="SearchBox" />
        <Button x:Name="SearchButton" Grid.Column="1" Content="Search" />
    </Grid>

    <TextBlock x:Name="ErrorLabel" Grid.Row="1" Text="Status" />

    <ListView x:Name="Suggestions" Grid.Row="2" SelectionMode="None" IsItemClickEnabled="True" />
</Grid>
```

## Traditional approach

### Timeout

### Retry

### Throttle

### Distinct

### Race condition

## Rx approach

### Search suggestions

### Search results

## Summary