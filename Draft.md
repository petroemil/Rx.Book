# Table of contents

+ **Introduction**
  + [The problem - asynchrony](#the-problem---asynchrony)
  + [What is an Observable?](#what-is-an-observable)
  + [What is LINQ?](#what-is-linq)
  + [LINQ vs Rx](#linq-vs-rx)
+ **Hello Rx World**
  + [Preparations](#preparations)
  + [Traditional approach](#traditional-approach)
  + [Rx approach](#rx-approach)
  + [Summary](#summary)
+ **Rx = Observables + LINQ + Schedulers**
  + [Preparation](#preparations-1)
  + [Observable streams](#observable-streams)
  + [LINQ](#linq)
  + [Schedulers](#shedulers)
  + [Rx + Async](#rx--async)
  + [Summary](#summary-1)

# Introduction

## The problem - asynchrony

If you are developing modern applications (whether it's a mobile, desktop or server side application), at some point you will reach the limitations of synchronously running code and find yourself facing the difficulties of multi-threaded and/or event-driven development. If you want to build a performant and responsive application and handle I/O operations, web service calls or CPU intensive tasks asynchronously, your code will become much more difficult to handle. Unusual methods must be used for coordination, exception handling and you might have to deal with cancellability of long running operations and the synchronization of parallel running ones.

Reactive Extensions (hereinafter referred to as Rx) is a class library that allows you to build asynchronous and/or event-driven applications by representing the asynchronous data as "Observable" streams that you can use LINQ operations on and the execution, handling of race conditions, synchronization, marshalling of threads and more are handled by so called "Schedulers".

In short: Rx = Observables + LINQ + Schedulers

## What is an Observable?

To understand the concept of Observables, you first have to understand the basic difference between synchronous and asynchronous programming.

Depending on what kind of application you are building, there is a good chance that most of the business logic is synchronous code which means that all the lines of code you write will be executed sequentially, one after the other. The computer takes a line, executes it, waits for it to finish and only after that will it go for the next line of code to execute it. A typical console application only contains synchronous code, to the level that if it has to wait for input from the user, it will just wait there doing exactly nothing.

In contrast with that, asynchrony means you want to do things in response to events you don't necessarily know when they happen.

A good example for this is the `Click` event of a button on the UI, but also the response from a web service call. In the case of the latter it's very much possible that in terms of the execution order of the commands you know exactly that you only want to do things after you have the response, but you have to consider the chance that this service call could take seconds to return, and if it would be written in a synchronous way, the execution on the caller thread would be blocked and it couldn't do anything until the service call returns. And this can lead to very unpleasant experience for the users as they will just see the UI becoming unresponsive or "frozen". To make sure it doesn't happen you will have to do asynchronous call which means that the logic will be executed (or waited in the case of I/O operations) in the background without blocking the calling thread and you will get the result through some kind of callback mechanism. If you have been around in the developer industry for a while, you might remember the dark days of callback chains, but fortunately in C# 5.0 / .NET 4.5 (in 2012) Microsoft introduced the `async` and `await` keywords that makes it possible to write asynchronous code that looks just like a regular synchronous code. Just think about the difficulties of implementing simple language constructs like a conditional operation, a loop or a try-catch block with the traditional form (callbacks) of asynchronous programming. With these new keywords you can naturally use all of these while still having a performant, responsive and scalable asynchronous code. But even though it makes life significantly easier in many scenarios, the moment you want to do something a little more complicated, like a retry, timeout or adding paging functionality to a web service call, you have to start writing complex logic because hiding the difficulties of dealing with callbacks won't save you from implementing these custom operations.

Events (in C#) have a serious problem though, they are not objects, you can't pass them around as method parameters, and tracking the subscribers of an event is also not trivial. This is where the Observer design patter comes in handy, because with it you can implement event handling for yourself instead of using a language feature, so you have full control over the event source, the subscribers and the method of the notification.

The Observer design pattern consists of two simple interfaces.

One of them is the `IObservable<T>` which represents an "observable" data source. This interface only has a `Subscribe()` method which kind of translates to the `+=` operator of the "event world".

The other one is the `IObserver<T>` which represents an "observer" object that you can pass to the `IObservable<T>`'s `Subscribe()` method. In the original design pattern this interface has only a `Notify()` method that gets called by the event source when an event occurs.

In the case of Rx, the implementation is slightly different. The `IObserver<T>` interface defines three methods: `OnNext()`, `OnError()` and `OnCompleted()`. `OnNext()` gets called for each new event from the event source, `OnError()` if something bad happened and `OnCompleted()` if the source wants to signal that it will not produce any more events and the subscription can be disposed.

Rx is built on top of these two interfaces, and using this simple design pattern it makes dealing with asynchronous programming extremely easy (and I would even go as far as saying fun). To understand how, let's talk a little bit about LINQ.

## What is LINQ?

LINQ (Language Integrated Query) is making devs' life easier since .NET 3.5 (2007) when it comes to dealing with processing collections. This technology, or more like the language features that have been introduced with it, brought the first seeds of functional programming to C#. I won't go into details about what functional programming means because it's way out of the scope of this book, but I would like to quote Luca Bolognese's analogy from a PDC presentation from 2008.

>It's like going to a bar and telling the guy behind the counter <br/>
> \- I want a cappuccino <br/>
>or going to the bar and telling the same guy <br/>
> \- Now you grind the coffee, then get the water, warm it, at the same time prepare the milk, and I want these two thing to go in parallel...

To bring a bit better example, think about how you would solve the following problem: Given a range of numbers, get the even numbers and sum their squares.

The very first solution would probably look something like this:

```csharp
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

What could you do to make this code look prettier? How could you compress it a little bit, make it less verbose? Let's start with the initialization of the list.

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
```

Let's go through this piece of code from keyword to keyword.

It starts with a `var` keyword. When you declare (and also initialize in the same time) a variable, the compiler can infer the type of the variable based on the right side of the assignment operator. In this example if you would move the mouse cursor over the `var` keyword, it would show that the `numbers` variable has the type of `List<int>`. This is a handy little feature of the language that makes the code a bit more compact and readable, but you will see examples for situations where it has a deeper role to enable certain functionality.

Let's move on and take a look at the initialization of the list. The line doesn't end with calling the parameterless constructor, but the items of the list are defined within curly braces. This is called the *collection initializer* syntax. With this you can initialize the elements of a collection with a more lightweight syntax.

There is similar way to initialize properties of an object, which is called the *object initializer*.

Even though it doesn't belong to this specific example, but if you would combine the `var` keyword and the object initializer syntax, you would arrive to the so called anonymous types. This is really useful when you want to store temporary information in the middle of your algorithm, in a nice and structured format, without explicitly creating the class(es) that represent that structure.

```csharp
var person = new { Name = "Emil", Age = 27 };
```

Here the compiler can look at the right side of the assignment operator, it won't be able to guess the type of the variable... so it generates one behind the scene.

Let's get back to the original example, and take the refactoring of the code one step further.

```csharp
return numbers
    .Where(num => num % 2 == 0)
    .Select(num => num * num)
    .Sum();
```

If you remember, the `numbers` variable is a collection of integers. How can you call a method called `Where()` on it, when the `List<T>` type doesn't have such method? The answer for this question is extension methods. In the good old days when you wanted to add new functionality to a type, you inherited a new class from the original type and defined the new method on it. This approach has two main problems though. On one side it's possible that the type you want to extend is `sealed`, the other problem is that even if you can define your own "super type", you will have to use that throughout your application and convert back and forth between that and the original type on the edges of your library.

You don't have to think something too complicated. Just think about a function that extends the `string` with some special kind of formatting. If you want to use this logic in multiple places, you either have to move it to a static helper class or derive your `SuperString` class. The latter in this specific example is not actually possible because the string type is `sealed`. So you are left with the static class. But think about how it would look if you would have to use more of these operations after each other...

To solve these kinds of problems, Microsoft introduced the extension methods in C#, which makes it possible to extend any type (class, struct or even interface) using the following syntax:

```csharp
public static string SpecialFormat(this string s, int spaces)
```

You have to place these extension methods in a static class. The interesting part of this method signature is its very first parameter that has the `this` keyword in front of it. It means that you are extending the `string` class with this method. Whenever you have a `string` in your application, you can just call this method, the system will automatically hide this first parameter by implicitly passing the reference of the `string` there, and you only have to provide - in this example - the `int spaces` parameter. 

It's important to understand, that the extension method is just a syntactic sugar over the static methods, meaing these methods won't have any special access to the first parameter (with the `this` keyword) other than having a reference to it. You won't be able to access non-public parts of the extended object - except if the object has internal parts and you define the extension method in the same assembly.

In action it will look something like this.

```csharp
"Hello World".SpecialFormat(42);
```

The way LINQ uses these extension methods brings this to the next level. To stay at the original example, let's take a quick look at the `Where()` method's signature.

```csharp
public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
```

The thing that you should pay attention to is that it extends the `IEnumerable<T>` interface and returns an object of type `IEnumerable<T>` as well. Almost all of the LINQ operators' signature looks like this, which is good because it enables the composition of these operators, so you can use as many of these operators after each other as many you want. The current example shows this very well by using the `Where()`, `Select()` and `Sum()` operators without introducing any kind of intermediate variables.

The other typical feature of LINQ operators is that they leave the important part of the work to you. In case of the `Where()` operator, the implementation only deals with iterating the original collection and building a new one, but the predicate for the selection logic is a parameter and it's up to you to define your logic to say which element stays and which one has to go. For this it uses a `Func<T, bool>` delegate, that represents a method that receives one element of the collection (the one that the `Where()` operator is internally just iterating through) and returns a `bool` which tells the operator whether the element can stay or it has to go.

If you continue reading the sample code, you very quickly arrive to an interesting expression passed as a parameter to the Where (and later to the Select) method.

```csharp
num => num % 2 == 0
```

This is called a lambda expression. With this lightweight syntax it's possible to write a method (and pass it to some other part of the code as a parameter) without actually writing a method. With this you have the ability to write your logic in place instead of writing a method somewhere and referring to that as a delegate.

The full syntax of a lambda expression looks like this:

```csharp
(param1, param2) =>
{
    // ...
    return true;
};
```

Before the arrow operator (`=>`), between parentheses you get the parameters, and after the arrow, between curly braces you have the body of the method.</br>
The reason you don't have to provide any type information for the parameters or the return type is because you are passing this expression as a delegate, so the compiler knows already all those information. </br>
There are some shortcuts as well.</br>
If there's only one parameter, you can loose the parentheses, but if there's no parameter, you have to include an empty pair of parentheses.</br>
If the expression only contains one line of code, you also don't have to wrap it between curly braces and don't have to use the explicit `return` keyword.</br>
And of course don't forget about the case where you actually have the method that matches the delegate's signature, in which case you can provide only the name of the method, so you don't have to write something like `Method(x => F(x))`, it's enough to just write `Method(F)`.</br>
And just as a "fun fact" I'd like to mention that parallelizing the aforementioned example would only require one more operator in the beginning of the expression.

```csharp
return numbers
    .AsParallel()
    .Where(num => num % 2 == 0)
    .Select(num => num * num)
    .Sum();
```

Thanks to the `AsParallel()` operator the whole query operation will run in parallel. Just think about what you would do if this requirement would come up after only having the original implementation in place.

LINQ is actually more than these language elements, there is actual language level support to write query expressions, but that's not really important from this book's point of view. What is important that LINQ is a smart combination of the aforementioned underlying language features, and a huge set of predefined extension methods for various types. One of those many types is the `IEnumerable<T>` class, but LINQ has very similar extension methods for other types of data sources, like relational databases (LINQ to SQL), XML (LINQ to XML), or if you want to think about it that way, events (LINQ to Events), which leads to Rx.

## LINQ vs Rx

| LINQ (to Objects) | Rx |
| ---: | :--- |
| Can work with collections and it works on enumerable types, which means that it builds on the iterator design pattern which is represented by the `IEnumerable<T>` and the `IEnumerator<T>` interfaces | Works with observable types, which means it builds on the observer design pattern which is represented by the `IObservable<T>` and `IObserver<T>` interfaces |
| Represents a *pull* technique, as internally it keeps pulling out elements from the collection using the `MoveNext()` method. You ask the system to get the next element | Represents a *push* technique, as internally the `IObservable` object keeps a reference to the subscribed `IObserver` and calls its `OnNext()` callback method when some event occurs. The system notifies you if there is a new element available
| Takes a collection and allows you to transform, filter, order, group and do many more operations on it and returns the modified collection | Takes a source event stream and transforms, filters, orders, groups and does many more operations on it, and returns the modified event stream |
| Is a collection of extension methods for the `IEnumerable<T>` interface | Is a collection of extension methods for the `IObservable<T>` interface |

# Hello Rx World

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
public async Task<IEnumerable<string>> GetResultsForQuery(string query)
{
    await this.SimulateTimeoutAndFail();

    return new string[] { $"This was your query: {query}" };
}
```

* Last but not least, the most important part of the service is the one that will generate the suggestions. It will take the query, split it into individual words, try to suggest possible endings for the last word using the sample data, and generate the full suggestions by taking the "head" of the query (all the words before the last one) and the possible suggestions for the last one and combine them into strings.

```csharp
public async Task<IEnumerable<string>> GetSuggestionsForQuery(string query)
{
    await this.SimulateTimeoutAndFail();

    // Split "The Quick Brown Fox" to [ "The", "Quick", "Brown" ] and "Fox"
    var queryWords = query.ToLower().Split(' ');
    var fixedPartOfQuery = queryWords.SkipLast(1);
    var lastWordOfQuery = queryWords.Last();

    // Get all the words from the 'SampleData' 
    // that starts with the last word fragment of the query
    var suggestionsForLastWordOfQuery = SampleData
        .GetTop100EnglishWords()
        .Select(w => w.ToLower())
        .Where(w => w.StartsWith(lastWordOfQuery));

    // Combine the "fixed" part of the query 
    // with the suggestions for the last word fragment
    var suggestionsToReturn = suggestionsForLastWordOfQuery
        .Select(s => fixedPartOfQuery.Concat(new[] { s }))
        .Select(s => string.Join(" ", s))
        .Take(10);

    return suggestionsToReturn;
}
```

* Now the only missing piece is the UI. It will be very simple, a `TextBox` to type the query into, a `Button` to initiate the search, a `TextBlock` for "debug" information and a `ListView` to display the suggestions

```XML
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

Now that you have the foundation of the app, let's take a look at the high level description of the problem you are facing.

The goal is to build a search page where as the users are typing their query, they get suggestions for possible completions, and when they finally hit the Search button, show them some search results.

An interesting side fact: when you are using a real search engine (like Bing), the suggestions you are getting are just a result of some smart matching of words, but doesn't necessarily represent an actual result. It's very much possible that the search engine will help you phrase the perfect query by giving you some amazing suggestions, and then it won't find anything related to what you wanted. So long story short, suggestions and results are different things, but suggestions in real life are built based on statistical data from other users' queries and indexed websites and some intelligence to just suggest matching words to your existing query, so they can still be absolutely useful.

Let's start building the app logic with a naive implementation and find the potential problems in it.

Put the following code block in the constructor of the main page and try it in action.

```csharp
this.searchBox.TextChanged += async (s, e) => 
{
    var query = this.searchBox.Text;
    var suggestions = await this.searchService.GetSuggestionsForQuery(query);
    this.suggestions.ItemsSource = suggestions;
};

this.searchButton.Click += async (s, e) => 
{
    var query = this.searchBox.Text;
    var results = await this.searchService.GetResultsForQuery(query);
    this.suggestions.ItemsSource = results;
};
```

What will happen if you run the app?

Probably one of the first things you will notice after playing with it a little bit is that thanks to the simulated random latency of queries you will receive suggestions out of order. You might try to type something like "where", but the logic sends a request for new suggestions after every single new character in the query, and it's very much possible that you receive suggestions for the fragment "wh" by the time you already typed "whe", in a worse case even overriding the suggestions for "whe" because of the latency difference makes it possible that the response from the web service call returns the suggestions for "whe" sooner than the suggestions for "wh".

Another problem that you might notice is not necessarily a logical problem, but more like a bad user experience. Requesting suggestions for every single new character the user types is a waste of resources (CPU, battery, network - not to mention money, if the user is using a metered connection, like 4G).

Seeing these problems, let's try to collect the requirements for this app to work better.

The application should be resilient to server side errors and timeouts. It should automatically retry (let's say 3 times) every request if they fail for any reason.

It would be nice to have some throttling mechanism, so the app wouldn't issue a request for suggestions for every new character, but it would wait until the user calms down and is not typing.

A seemingly small but useful optimisation to check if two consecutive queries are the same, in which case the app shouldn't do anything. This can happen because of the throttling, if the user types something, waits, types something new but immediately deletes it returning to the previous state of the query, the app would basically see 2 events with the same request. Also if the user just types something in the search box and then starts spamming the search button, the app shouldn't send the exact same query multiple times.

Last but not least there is a race condition problem, meaning the app have to make sure that the user only gets suggestions or results for what they typed last, and no phantom responses with high latency should show up unexpectedly.

Let's take a look at the individual problems then to a solution to combine them.

### Timeout

Thanks to the `await` keyword introduced in C# 5.0, you can write short and easy to read asynchronous code. The *raw* service call is just one line of code...

```csharp
var suggestions = await this.searchService.GetSuggestionsForQuery(query);
```

... but as soon as you want something a little bit more difficult, you have to leave the comfort of this simplicity and start writing code. A possible implementation for the timeout logic can be seen below.

```csharp
private readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(500);
public async Task<IEnumerable<string>> ServiceCall_Timeout(string query)
{
    var newTask = this.searchService.GetSuggestionsForQuery(query);
    var timeoutTask = Task.Delay(this.timeoutInterval);

    var firstTaskToEnd = await Task.WhenAny(newTask, timeoutTask);

    if (newTask == firstTaskToEnd) return newTask.Result;
    else throw new Exception("Timeout");
}
```

You start the service call and a `Delay` in parallel and wait for one of them to return. Based on which one returned first you can decide if the operation finished successfully (or failed for some other reason), or timed out.

### Retry

Retry logic usually involves some kind of loop, trying an operation over and over again a number of times. Again, the `await` keyword saves quite a lot of code as you can use `await` within a `for` or `while` loop, but you have to write some code to make it happen. Here is a possible implementation.

```csharp
private readonly int numberOfRetries = 3;
public async Task<IEnumerable<string>> ServiceCall_Retry(string query)
{
    for (var tries = 0; tries < this.numberOfRetries; tries++)
    {
        try { return await this.searchService.GetSuggestionsForQuery(query); }
        catch { /* deal with the exception */ }
    }

    throw new Exception("Out of retries");
}
```

The code basically includes a `for` loop that loops 3 times and if it "successfully" manages to do that, at the end of the method an exception waits to be thrown. So loop - loop - loop - throw.

Inside the `for` loop you can find the actual service call in a `try`-`catch` block. <br/>
If the service call returns successfully, the method returns and there won't be any more looping or exception throwing at the end. <br/>
But if the service call fails, the `try`-`catch` block "swallows" it and lets the `for` loop to go to the next iteration and retry it, or exit the `for` loop and throw the exception at the end of the method.

### Throttle

Throttling, distinct check and race condition handling are more complicated tasks to implement. By their nature they will filter out some of the requests, not returning anything for them. There won't be a 1:1 connection between the requests and responses, there will be more requests than responses.

Throttling will let you request suggestions as frequently as you want, but internally it will only issue a request (and produce any kind response) after the user haven't touched the keyboard for half second.

Yet again, for the distinction check, you can request suggestions as many times as you want, but internally it will only send the request through if it's different than the previous request.

And the race condition check will throw away some of the old service calls in case there is a newer one requested.

This leads to a pattern where the service call wrapping method will no longer directly return anything, it will turn into a `void` method, and instead you will be able to get the most recent results through a CallBack event.

Throttling works in a simple way: You save the current `DateTime` into an instance level field, wait some time (half second) and check if the time difference between the saved and the new current `DateTime` is equals or more than the specified throttling interval. If it is, it means that no one called this method while it was waiting (otherwise the difference between the saved and current `DateTime` would be less than the specified throttle interval), so it can advance forward and do the service call, and send the result of the call through the CallBack event.

```csharp
private readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(500);
private DateTime lastThrottledParameterDate;
public event Action<IEnumerable<string>> CallBack_Throttle;
public async void ServiceCall_Throttle(string query)
{
    this.lastThrottledParameterDate = DateTime.Now;
    await Task.Delay(this.throttleInterval);

    if (DateTime.Now - this.lastThrottledParameterDate < this.throttleInterval) return;

    var suggestions = await this.searchService.GetSuggestionsForQuery(query);
    this.CallBack_Throttle?.Invoke(suggestions);
}
```

### Distinct

As described before, distinction check has a very simple logic: only do the service call if the query is different from the previous one.

```csharp
private string lastDistinctParameter;
public event Action<IEnumerable<string>> CallBack_Distinct;
public async void ServiceCall_Distinct(string query)
{
    if (this.lastDistinctParameter != query)
        this.lastDistinctParameter = query;
    else
        return;

    var suggestions = await this.searchService.GetSuggestionsForQuery(query);

    this.CallBack_Distinct?.Invoke(suggestions);
}
```

### Race condition

Dealing with race condition will happen with a typical optimistic concurrency approach. Tag the service call with a unique identifier save it in a shared variable, wait for it to return and check if it this time the saved identifier is still the same as the identifier of the just returned service call. <br/>
If they are the same, it means that no other service calls were issued during the time the original service call was being processed, and it's still the latest, most recent result. <br/>
If they don't match though, it means that another service call has been made while waiting for the first one, invalidating the first service call's result as it's no longer the most recent one.

In this specific example you won't have to actually explicitly tag the service calls with some kind of `Guid`, you can just use the `Task` object's reference that represents the service call.

```csharp
private Task<IEnumerable<string>> lastCall;
public event Action<IEnumerable<string>> CallBack_RaceCondition;
public async void ServiceCall_RaceCondition(string query)
{
    var newCall = this.searchService.GetSuggestionsForQuery(query);
    this.lastCall = newCall;

    var result = await newCall;

    if (this.lastCall == newCall)
        this.CallBack_RaceCondition?.Invoke(result);
}
```

### All together

Now, that you have an idea about the individual pieces, let's try to put them together and also see how the resulting implementation can actually be used.

```
public class ServiceCallWrapper<TParam, TResult>
{
    private readonly Func<TParam, Task<TResult>> wrappedServiceCall;
    public ServiceCallWrapper(Func<TParam, Task<TResult>> wrappedServiceCall)
    {
        this.wrappedServiceCall = wrappedServiceCall;
    }

    // Throttle global variables
    private readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(500);
    private DateTime lastThrottledParameterDate;

    // Distinct global variables
    private TParam lastDistinctParameter;

    // Retry global variables 
    private readonly int numberOfRetries = 3;

    // Timeout global variables 
    private readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(500);

    // Switch global variables 
    private Task<TResult> lastCall;

    // Callback events 
    public event Action<TResult> CallBack;
    public event Action<Exception> ErrorCallBack;

    public async void ServiceCall(TParam query)
    {
        try
        {
            // Throttle logic
            this.lastThrottledParameterDate = DateTime.Now;
            await Task.Delay(this.throttleInterval);

            if (DateTime.Now - this.lastThrottledParameterDate < this.throttleInterval) return;

            // Distinct logic
            if (this.lastDistinctParameter?.Equals(query) != true)
                this.lastDistinctParameter = query;
            else
                return;

            var newCall = Task.Run(async () =>
            {
                // Retry logic
                for (var tries = 0; tries < this.numberOfRetries; tries++)
                {
                    try
                    {
                        // Timeout logic
```
```csharp
                        var newTask = this.wrappedServiceCall(query);
```
```
                        var timeoutTask = Task.Delay(this.timeoutInterval);

                        var firstTaskToEnd = await Task.WhenAny(newTask, timeoutTask);

                        if (newTask == firstTaskToEnd) return newTask.Result;
                        else throw new Exception("Timeout");
                    }
                    catch { /* deal with the exception */ }
                }

                throw new Exception("Out of retries");
            });

            // Switch logic
            this.lastCall = newCall;

            var result = await newCall;

            if (this.lastCall == newCall)
                this.CallBack?.Invoke(result);
        }

        catch (Exception ex)
        {
            this.ErrorCallBack?.Invoke(ex);
        }
    }
}
```

Even though this implementation is not quite pretty on the inside, using it is fairly simple.

```csharp
var suggestionsServiceHelper = new ServiceCallWrapper<string, IEnumerable<string>>(this.searchService.GetSuggestionsForQuery);

// Subscribing to events to trigger service call
this.searchBox.TextChanged += (s, e) => suggestionsServiceHelper.ServiceCall(this.searchBox.Text);

// Registering callback methods
suggestionsServiceHelper.CallBack += this.CallBack;
suggestionsServiceHelper.ErrorCallBack += this.ErrorCallBack;
```

```csharp
var resultsServiceHelper = new ServiceCallWrapper<string, IEnumerable<string>>(this.searchService.GetResultsForQuery);
            
// Subscribing to events to trigger service call
this.searchButton.Click += (s, e) => resultsServiceHelper.ServiceCall(this.searchBox.Text);
this.suggestions.ItemClick += (s, e) => resultsServiceHelper.ServiceCall(e.ClickedItem as string);
this.searchBox.KeyDown += (s, e) =>
{
    if (e.Key == VirtualKey.Enter)
        resultsServiceHelper.ServiceCall(this.searchBox.Text);
};

// Registering callback methods
resultsServiceHelper.CallBack += this.CallBack;
resultsServiceHelper.ErrorCallBack += this.ErrorCallBack;
```

Just for the record the two event handlers for `CallBack` and `ErrorCallBack` have the following implementation in the sample code.

```csharp
private void CallBack(IEnumerable<string> items)
{
    this.errorLabel.Visibility = Visibility.Collapsed;
    this.suggestions.ItemsSource = items;
}

private void ErrorCallBack(Exception exception)
{
    this.errorLabel.Visibility = Visibility.Visible;
    this.errorLabel.Text = exception.Message;
}
```

## Rx approach

When you think about Rx, you have to think about it as a stream or pipeline. You put a message into the pipeline and it will go through various steps of transformation, filtering, grouping, aggregating, delaying, throttling, etc.

After you defined your logic as a series of steps, you can subscribe to this stream of events and act on anything coming out of it.

I don't want to waste your time too much, because the concepts and a lot of the operators are going to be explained in depth in the next chapter, so let me just show you the final implementation of the "Rx-style" `ServiceCallWrapper` class.

```csharp
public static class ServiceCallWrapper
{
    private static readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(250);
    private static readonly int numberOfRetries = 3;

    public static IObservable<TOut> WrapServiceCall<TIn, TOut>(IObservable<TIn> source, Func<TIn, Task<TOut>> serviceCall)
    {
        return source
            .Throttle(throttleInterval)
            .DistinctUntilChanged()
            .Select(param => Observable
                .FromAsync(() => serviceCall(param))
                .Timeout(timeoutInterval)
                .Retry(numberOfRetries))
            .Switch();
    }
}
```

As you can see the code is *significantly* simpler and more readable than the "traditional approach".

There are two main differences that I would like to point out. You won't call this method directly each time you have a new value, you just provide "some source" that will provide the input strings for the service call.<br/>
The other one is that as an output you didn't have to define events, it will be just another `IObservable` stream that the consumer logic can subscribe to.

If you remember what I wrote about LINQ, you might notice a similarity of the method signature: receiving some `IObservable<TIn>` as a parameter and returning an `IObservable<TOut>`. This is a perfect candidate to be turned into an extension method - and by the way a great example to show one easy way to build custom operators for Rx.

```csharp
public static IObservable<TOut> CallService<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> serviceCall)
{
    return ServiceCallWrapper.WrapServiceCall(source, serviceCall);
}
```

The remaining bit is to show the code to use this implementation.

The code to get suggestions:

```csharp
// Define source event (observable)
var queryTextChanged = Observable
    .FromEventPattern(this.searchBox, nameof(this.searchBox.TextChanged))
    .Select(e => this.searchBox.Text);

// Subscribe to the prepared event stream
queryTextChanged
    .CallService(this.searchService.GetSuggestionsForQuery)
    .ObserveOnDispatcher()
    .Do(this.OnNext, this.OnError)
    .Retry()
    .Subscribe();
```

And the code to get results:

```csharp
// Define source events (observables)
var searchButtonClicked = Observable
    .FromEventPattern(this.searchButton, nameof(this.searchButton.Click))
    .Select(_ => this.searchBox.Text);

var suggestionSelected = Observable
    .FromEventPattern<ItemClickEventArgs>(this.suggestions, nameof(this.suggestions.ItemClick))
    .Select(e => e.EventArgs.ClickedItem as string);

var enterKeyPressed = Observable
    .FromEventPattern<KeyRoutedEventArgs>(this.searchBox, nameof(this.searchBox.KeyDown))
    .Where(e => e.EventArgs.Key == VirtualKey.Enter)
    .Select(_ => this.searchBox.Text);
            
// Merge the source events into a single stream
var mergedInput = Observable
    .Merge(searchButtonClicked, enterKeyPressed, suggestionSelected);

// Subscribe to the prepared event stream
mergedInput
    .CallService(this.searchService.GetResultsForQuery)
    .ObserveOnDispatcher()
    .Do(this.OnNext, this.OnError)
    .Retry()
    .Subscribe();
```

What you can see here is a bunch of example to transform a traditional .NET `event` into an `IObservable` stream. To do this you can use the `Observable.FromEventPattern` static method. EventPattern in this context refers to the typical `(object sender, EventArgs args)` signature, so that's what this method expects. You just pass the event source object and the name of the event, and optionally the more specific types for the `EventArgs` and the sender object.

Once you converted the event into an observable stream, you can start doing various operations on it, like extracting the useful information, transforming this weird `EventPattern<T>` object into something more meaningful, in this case the `string` that you want to send to the service calls. 

You can also do some filtering as you can see with the `enterKeyPressed` example. You subscribe to the event, do the filtering based on the pressed key and extract the useful information. It means it will produce a new event containing the content of the `SearchBox` every time the user hits the Enter key.

You should notice the creation of the `mergedInput` observable. You defined 3 separate observables, but you actually want to do the exact same thing with them, so it would be better to have all of those merged in just one stream. That's what you can use the `Merge()` operator for.

Once you have your source in place, you can finally use your prepared extension method for the `ServiceCallWrapper` class and just naturally use it as part of the pipeline definition, just like any other operator.

The rest of the pipeline goes like this: <br/>
The `ObserveOnDispatcher()` operator is used to marshall the flow of the observable stream back to the UI thread. <br/>
The `Do()` operator lets you to inspect the stream at specific positions - this is where the actual CallBack methods are called. <br/>
The `Retry()` operator is necessary to make sure the stream never gets terminated by some unhandled exception. <br/>
And lastly the `Subscribe()` method is the method that actually activates this whole pipeline. Until you don't call subscribe, the stream is just a definition of steps, but the steps won't be subscribed to each other, the stream won't be active.

## Summary

In this chapter you saw a step-by-step example to compare the pain of dealing with asynchronous programming in a traditional way, and the ease of doing the same thing using Rx.

In the next chapter you will learn about the depths of Rx, the concepts behind it, a number of commonly used operators and more.

# Rx = Observables + LINQ + Schedulers

## Preparations

Just for the sake of having an app that you can use to play with and run the code samples, let's create a UWP "Console" application. The reason for it is that most of the time a traditional console where you can print lines is more than enough, but for some of the examples you will need things like a `TextBox` or pointer events.

Just like in the previous chapter, let's create an empty UWP app
* Open Visual Studio (2015)
* Create a new project (`File / New / Project`)
* In the dialog window select `Windows / Universal` on the left hand pane
* Then the `Blank App (Universal Windows)` project template on the right.
* Add the Rx NuGet package to the project by typing `PM> Install-Package System.Reactive` to the Package Manager console
* Add the Ix NuGet package to the project by typing `PM> Install-Package System.Interactive` to the Package Manager console - this is an extension for regular collections that adds a few more useful operators to the catalogue of LINQ operators.
* Add a new class file to the project that will contain a little extension method for the `TextBlock` control <br/>
This extension method will allow you to write on the screen line-by-line and it will always keep only the last 25 lines.

```csharp
public static class TextBlockExtensions
{
    public static void WriteLine(this TextBlock textBlock, object obj)
    {
        var lines = textBlock.Text
            .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
            .TakeLast(24)
            .Concat(new[] { obj.ToString() });

        textBlock.Text = string.Join(Environment.NewLine, lines);
    }
}
```

* The next step is to create the UI. It will be very simple just a `TextBlock` and some styling to make it look like a console

```XML
<Grid Background="Black">
    <TextBlock x:Name="Console" Margin="25" FontFamily="Consolas" FontSize="24" Foreground="LightGray" IsTextSelectionEnabled="True" />
</Grid>
```

* With the `TextBlock` and the extension method defined, you can now write things like this in the page's code behind file

```csharp
Console.WriteLine("Hello Rx");
```

## Observable streams

As you could already see it in the previous chapters, Rx at its core gives you the ability to handle asynchronous data streams and perform various operations by applying any number of operators.

Just as a recap, the core interfaces are the `IObservable<T>` and `IObserver<T>`.

The `IObservalbe<T>` is an observable object gives you the ability to subscribe to it and "observe it" through its only method, the `Subscribe()`.

The `IObserver<T>` interface defines how the observer object looks like. It has an `OnNext()` method that will be called every time the observable emits a new event (so 0 or more times), and `OnError()` and `OnCompleted()` methods that will be called when the observer terminates, either naturally or due to an error. These latter two methods are terminating methods and they can be called 0 or 1 time during the lifecycle of an observable object. They are also mutually exclusive, meaning you can't see both of them emitted by the same observable.

As it's usually "phrased": `OnNext* (OnError | OnCompleted)?`

As a last point before you'd dive into the details of how to create and how to transform existing asynchronous data sources (like an `event` or a `Task`) to observable streams, let's take a quick look at the subscription and the terminology you will see for the rest of this book.

There are 3 main groups of overloads for the `Subscribe()` method.
* Subscribing by passing an `IObserver` object
* Subscribing by defining the `OnNext()`, and optionally the `OnCompleted()` and/or `OnError()` callback actions as lambda expressions
  * OnNext
  * OnNext + OnError
  * OnNext + OnCompleted
  * OnNext + OnError + OnCompleted
* Subscribing without passing any parameter - it will make sense later
* And all of the above with an optional `CancellationToken` parameter

Throughout this book you will mostly use the second one (passing the lambda expressions) as that's the fastest to implement. Obviously in a real application you would want to build your `Observable` object and build a unit test suite around it, something that you can't quite do with inline defined lambda functions.

The pattern that you should follow for the rest of the examples is the following:

```csharp
private void Subscribe<T>(IObservable<T> source, string subscribtionName)
{
    source
        .ObserveOnDispatcher()
        .Subscribe(
            next => Console.WriteLine($"[{subscribtionName}] [OnNext]: {next}"),
            error => Console.WriteLine($"[{subscribtionName}] [OnError]: {error.Message}"),
            () => Console.WriteLine($"[{subscribtionName}] [OnCompleted]"));
}
```

The `ObserveOnDispatcher()` is required to make sure no matter which thread the stream is coming from, it's definitely marshalled to the UI thread.

And the `Subscribe()` method is fairly trivial, you just handle all possible notification types by providing all three of the callbacks for `OnNext`, `OnError` and `OnCompleted`.

Even though you now have this console application, you will also have a more visual marble diagram at every operator to describe what they are doing.

They will look something like this:

![](Marble%20Diagrams/Example.png)

### Generator operators

There are a bunch of "primitive streams" that can be easily generated by one of the built-in operators. Some of these represent the most basic variations of streams. These operators can be looked at like `int.MinValue` / `int.MaxValue`, `Task.FromResult()` / `Task.FromException()` / `Task.FromCancelled()`, `Enumerable.Range()`, etc.

#### Never

The `Never()` operator represents the simplest possible stream, a stream that doesn't do anything, has no events and never ends.

```csharp
var source = Observable.Never<string>();
```

![](Marble%20Diagrams/Never.png)

#### Empty

The empty stream is a stream that yields no `OnNext`, just an `OnCompleted` event immediately.

```csharp
var source = Observable.Empty<string>();
```

![](Marble%20Diagrams/Empty.png)

#### Return

Similarly to the `Task.FromResult()` you can use the `Observable.Return()` operator to produce a stream that has one `OnNext` and an `OnCompleted` event.

```csharp
var source = Observable.Return("A");
```

![](Marble%20Diagrams/Return.png)

#### Throw

Yet again, bringing the analogy from the `Task` world, just like you can use to construct a failed `Task` object using the `Task.FromException()` static method, you can use the `Observable.Throw()` operator to construct a stream that has only one `OnError` event.

```csharp
var source = Observable.Throw<string>(new Exception("X"));
```

![](Marble%20Diagrams/Throw.png)

#### Range

The same way you can use the `Enumerable.Range()` operator to generate a(n `IEnumerable`) range of numbers by providing a start value and the number of values you want to generate, you can use the `Observable.Range()` operator with the same parameters to generate an `IObservable` stream.

```csharp
var source = Observable.Range(0, 10);
```

![](Marble%20Diagrams/Range.png)

#### Generate

Generate works in a very similar way to a traditional `for` loop. You have to provide an initial value, a condition to be checked and an iterator logic.

```csharp
var source = Observable.Generate(
    initialState: 0,
    condition: i => i < 10,
    iterate: i => i + 1,
    resultSelector: i => i * i);
```

![](Marble%20Diagrams/Generate.png)

#### Interval

With the `Interval()` operator you get two things:
* a generated sequence of numbers starting from 0
* and the values with a time delay between them specified by you

It also worth mentioning that this stream never ends.

If you had some kind of `Timer` based logic in your application before, where you had to do something in every second or minute and you want to convert that to an Rx observable based implementation, this operator is a good starting point.

```csharp
var source = Observable.Interval(TimeSpan.FromMilliseconds(100));
```

![](Marble%20Diagrams/Interval.png)

#### Timer

The `Timer()` operator can work in two different ways.

On one hand you can use it to produce one event with either by providing a `TimeSpan` to produce that event with some delay after subscribing to the observable; or by providing a `DateTime` as a parameter in which case the value will appear in the stream at that given time.

```csharp
var sourceRelative = Observable.Timer(TimeSpan.FromMilliseconds(500));
var sourceAbsolute = Observable.Timer(new DateTime(2063, 4, 4));
```

![](Marble%20Diagrams/Timer.png)

On the other hand you can also provide a second `TimeSpan` parameter in which case the stream won't terminate after just one element, but it will keep generating subsequent elements with the specified time between them.

```csharp
var source = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
```

![](Marble%20Diagrams/TimerWithDelay.png)

### Converter operators

Even though you learned a couple of ways to construct "observable primitives", in reality it's more likely that you will want to convert some kind of existing data source to an observable stream. This data source can be an existing collection, an asynchronous operation represented by a `Task` object, an `event`, etc.

#### ToObservable

Two very common scenarios are collections and `Task` objects. For these you can just use the `ToObservable()` extension method to turn them into an observable stream.

If you have some kind of `IEnumerable` datasource, you can just call the extension method on it and turn it into an `IObservable` stream.

```csharp 
var sourceFromEnumerable = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }.ToObservable();
```

![](Marble%20Diagrams/ToObservable.png)

Also if you have a `Task` object, you can just call the extension method to turn it into a stream.

```csharp
var sourceFromTask = Task.FromResult("A").ToObservable();
```

![](Marble%20Diagrams/FromAsync.png)

Even though the extension method is present and you are free to use it and in some cases it's perfectly fine to use it, I would personally suggest to use the `FromAsync()` operator, because it comes with a few important behaviour differences compared to the `ToObservable()` operator.

#### FromAsync

If you have an asynchronous operation that is represented by a `Task`, you should use the `FromAsync()` operator to convert it to an observable stream. The reason for that are 2 words: lazy evaluation. 

The `ToObservable()` operator can only act on an existing `Task` object. If you have some kind of retry logic defined in your stream description, in case of using the `ToObservable()` operator, it will always go back to the same `Task` object and query its state. There is no way to somehow rerun the function that produced that `Task`.

But if you use the `FromAsync()` operator that wraps the function itself that produces the `Task`, if you re-subscribe (for example because of a failure), it will actually re-execute the function, producing a new `Task` with that, so at least you have the chance to recover from a transient error.

```csharp
var goodExample = Observable.FromAsync(SomeAsyncOperation);
```

![](Marble%20Diagrams/FromAsync.png)

#### FromEventPattern

Another good candidate to convert to Rx stream is the .NET event.

Most cases when you have to deal with events they have a typical signature that looks something like `(object sender, EventArgs args)`. As long as the event follows this signature, you can use a simplified way to convert these events to observables, like this:

```csharp
var source = Observable.FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown));
```

You can choose not to specify any generic type parameter in which case it will default to the most generic `(object, EventArgs)` combination. 

You can choose to specify only the actual type of the `args` parameter (as you can see in the example above), or to explicitly specify both the type of the `sender` and the `args` parameters.

![](Marble%20Diagrams/FromEvent.png)

#### FromEvent

In case you have an event that doesn't follow this pattern, your life will be slightly more complicated.

Let's assume you have an event like this:

``` csharp
public event Action<string, int, double> MySpecialEvent;
```

In this case your converter operator will look like this:

```csharp
var source = Observable.FromEvent<Action<string, int, double>, Tuple<string, int, double>>(
    rxOnNext => (s, i, d) => rxOnNext(Tuple.Create(s, i, d)),
    eventHandler => MySpecialEvent += eventHandler, 
    eventHandler => MySpecialEvent -= eventHandler);
```

So let's see what is happening here.

First of all, you have to call the `FromEvent` operator, which requires 2 generic type parameters, the type of the event and the type of the event arguments. The second one is necessary because in the world of events when an event happens, a subscribed method gets called potentially with many parameters, but in Rx world when an event happens, a new object is placed into the stream. So you have to wrap the received parameters into one object.

`MySpecialEventHandler(string s, int i, double d)` VS `OnNext(Tuple<string, int, double> values)`

Let's see the method parameters. After providing the type of the `event` and the type of the `args`, Rx internally prepares and exposes an `OnNext<TArgs>` method to push new elements into the stream. So here's what's happening:

The operator gives a reference to this `OnNext` function to you, and expects you to return a method that matches the signature of the original event, so it can be used as an event handler, and because it has a reference to the `OnNext` method, it should do the conversion from the method parameters to the `Tuple` object and push it into the stream.

Once you got your head around this rather complicated line, the last 2 parameters of the method are fairly simple, you just get a reference to the event handler (prepared by the system) that you have to subscribe to and unsubscribe from the original `event`.

It's worth mentioning that this example is the worst case scenario. If your event doesn't have any parameters or only has one, you don't have to bother with this complicated conversion logic, you just have to provide the subscribe / unsubscribe functions.

### Hot and Cold observables

Even though so far I didn't talk about it explicitly, you might have noticed that one of the main characteristic of observables is that they are only getting activated when you subscribe to them (and this is something that you should pay attention to if/when you design a custom observable).

There are real-time data sources, like .NET events (`PointerMoved`, `Click`, `KeyDown`, etc.), data sources that you can observe, but can't really control when they emit new events, and they've likely been virtually active before you subscribed to them. These are called *Hot Observables*, and I think the best analogy to these kind of observables is reading the value of a globally accessible field or property... at any given time if multiple "observers" try to read the value they will be pointed to the same reference, to the same source and they will see the same value.

And there are data sources, like an asynchronous method call, that you can still treat as an Rx stream, but you know that the service call will be triggered by your subscription to the (`FromAsync()`) observable stream. You know that any kind of notification will only appear in the stream after you subscribed to it, because the subscription triggered the execution of the underlying logic that pushes notifications in the stream. These are called *Cold Observables*, and a good analogy to them is when you retrieve a value through some kind of getter or generator logic, every "observer" will retrieve the value by executing that piece of logic that returns that value, so there is a good chance that they will see different values.

Based on your needs, you can easily switch between these behaviours and turn a Cold observable into a Hot observable, or the other way around by using the `Publish()` or the `Replay()` operators.

#### Creating hot observables

To turn a Cold observable into a Hot one, you will have to use the combination of the `Publish()` and the `Connect()` methods. The `Publish()` will prepare you an observable object that wraps your original observable stream and broadcasts its values to all the subscribers. And in this case instead of the `Subscribe()` call, calling the `Connect()` method will activate the stream and trigger the subscription chain in the wrapped observable, and with that the execution/activation of the underlying data source that will put events into the stream.

To demonstrate this let's create a simple Cold observable using the `Interval()` operator. It will generate a new stream for each of its subscribers instead of sharing the same one. You can easily see it in action with the following little sample:

```csharp
var source = Observable.Interval(TimeSpan.FromSeconds(1));

// Subscribe with the 1st observer
this.Subscribe(source, "Cold Observable - #1");

// Wait 3 seconds
await Task.Delay(TimeSpan.FromSeconds(3));

// Subscribe with the 2nd observer
this.Subscribe(source, "Cold Observable - #2");
```

If you run this, you will see events popping up on your screen from the two subscriptions like this:

![](Marble%20Diagrams/ColdObservableSample.png)

Now it's time to turn this Cold observable into a Hot one by using the combination of the `Publish()` and `Connect()` methods.

```csharp
var originalSource = Observable.Interval(TimeSpan.FromSeconds(1));
var publishedSource = originalSource.Publish();

// Call Connect to activate the source and subscribe immediately with the 1st observer
publishedSource.Connect();
this.Subscribe(publishedSource, "Publish - #1");

// Wait 3 seconds
await Task.Delay(TimeSpan.FromSeconds(3));

// Subscribe with the 2nd observer
this.Subscribe(publishedSource, "Publish - #2");
```

The code above shows you how to publish a stream and turn it into a Hot observable. As you can see you are subscribing to the `publishedSource` and because you immediately call `Connect()` and subscribe with the 1st observable, it will immediately start producing new values. And you can also see that this is a Hot observable because after waiting 3 seconds and subscribing the 2nd observable, it will only receive values that have been emitted from the source after the subscription, meaning it will never see the values the source produced before the subscription happened.

![](Marble%20Diagrams/PublishSample1.png)

And last but not least, try to put the `Connect()` call after the 3 second delay to demonstrate that a source is activated by the `Connect()` call and not the `Subscribe()` as it is the case for regular type of observables. Even though you immediately subscribe to the `publishedSource` with the 1st observer, it only gets activated 3 seconds later when you call the `Connect()` and subscribe with the 2nd observer. In this case both observers will see the exact same events.

```csharp
var originalSource = Observable.Interval(TimeSpan.FromSeconds(1));
var publishedSource = originalSource.Publish();

// Subscribe to the not-yet-activated source stream with the 1st observer
this.Subscribe(publishedSource, "Publish - #1");

// Wait 3 seconds
await Task.Delay(TimeSpan.FromSeconds(3));

// Call Connect to activate the source and subscribe with the 2nd observer
publishedSource.Connect();
this.Subscribe(publishedSource, "Publish - #2");
```

The marble diagram for this case looks something like this:

![](Marble%20Diagrams/PublishSample2.png)

#### Creating cold observables

The logic behind "cooling down" an observable is similar to the logic discussed about Hot observables, but obviously it will go the other way around. In this case you will have to use the combination of `Replay()` and `Connect()` methods. The `Replay()` method will wrap the original (Hot) observable into a caching stream, but it will only start recording and emitting the values after the `Connect()` method has been called.

As a demonstration let's just create a Hot observable and make 2 subscriptions to it to prove that it's Hot.

```csharp
var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
timer.Start();

var source = Observable
    .FromEventPattern(timer, nameof(timer.Tick))
    .Select(e => DateTime.Now);

Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
this.Subscribe(source, "Hot Observable - #1");

await Task.Delay(TimeSpan.FromSeconds(3));

Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
this.Subscribe(source, "Hot Observable - #2");
```

After running this example, you can clearly see that you only receive events with dates after the date of the subscription.

![](Marble%20Diagrams/HotObservableSample.png)

Now, just like in the previous example, let's try to "cool it down" by using the `Replay()` and `Connect()` methods. In the first example call the `Connect()` immediately, meaning this Cold observable will start caching the events of the underlying Hot observable immediately, and every time someone subscribes to this cold observable, they will receive the whole history of events since the activation of the stream.

```csharp
var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
timer.Start();

var originalSource = Observable
    .FromEventPattern(timer, nameof(timer.Tick))
    .Select(e => DateTime.Now);

var replayedSource = originalSource.Replay();

Console.WriteLine($"Cold stream activated at {DateTime.Now}");
replayedSource.Connect();

Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
this.Subscribe(replayedSource, "Replay - #1");

await Task.Delay(TimeSpan.FromSeconds(3));

Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
this.Subscribe(replayedSource, "Replay - #2");
```

The event timelines will look something like this:

![](Marble%20Diagrams/ReplaySample1.png)

And just to keep the tradition, check what happens if you call `Connect()` after the 3 second delay.

```csharp
var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
timer.Start();

var originalSource = Observable
    .FromEventPattern(timer, nameof(timer.Tick))
    .Select(e => DateTime.Now);

var replayedSource = originalSource.Replay();

Console.WriteLine($"Subscribing with #1 observer at {DateTime.Now}");
this.Subscribe(replayedSource, "Replay - #1");

await Task.Delay(TimeSpan.FromSeconds(3));

Console.WriteLine($"Cold stream activated at {DateTime.Now}");
replayedSource.Connect();

Console.WriteLine($"Subscribing with #2 observer at {DateTime.Now}");
this.Subscribe(replayedSource, "Replay - #2");
```

Events will only get recorded (and emitted) after the activation of the cold observable.

![](Marble%20Diagrams/ReplaySample2.png)

#### RefCount

As you can see it's a little bit cumbersome to deal with these `IConnectableObservable`s, you have to manually call the `Connect()` when you want it to start doing its job, and normally you also have to keep track of the subscribers and make sure you dispose the underlying subscription if it's no longer needed.

To deal with these complications, you can use the `RefCount()` operator that does all of these for you. It will wrap the `IConnectableObservable` and activate it after the first subscriber, keeps track of the number of subscribers, and when there are no more, it disposes the underlying observable.

To demonstrate lets do the following.
* Create a Cold observable
* Make it Hot using the combination of `Publish()` and `RefCount()`
* Make a subscription (#1), notice that it immediately gets activated, no need to call `Connect()` explicitly
* Wait a couple of seconds
* Make an other subscription (#2), to verify it's Hot
* Dispose subscription #1 and #2, to demonstrate that it will dispose the underlying stream as well
* Make a third subscription to show that it will activate a new subscription to the underlying stream

```csharp
var hotInterval = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .Publish()
    .RefCount();

var subscription1 = this.Subscribe(hotInterval, "RefCount #1");

await Task.Delay(TimeSpan.FromSeconds(3));

var subscription2 = this.Subscribe(hotInterval, "RefCount #2");

await Task.Delay(TimeSpan.FromSeconds(3));

subscription1.Dispose();
subscription2.Dispose();

var subscription3 = this.Subscribe(hotInterval, "RefCount #3");
```

The timeline for this will look something like this:

![](Marble%20Diagrams/RefCount.png)

### Subjects

Subjects are special kind of types that implements both the `IObservable` and `IObserver` interfaces. They will usually sit somewhere in the middle of the stream. Or you can use them as a source that you can use as an entry point to the stream, and you can manually call the `OnNext()`, `OnError()` and `OnCompleted()` methods on it. In some samples later you will see this kind of usage.

#### Subject

The `Subject` class is kind of an alternative to the `Publish()` + `Connect()` method pair. Basically once you subscribe it to some observable, it will turn it into a hot observable, maintaining only 1 connection to the original source and broadcasting everything that comes through to its own subscribers.

```csharp
// Create the source (cold) observable
var interval = Observable.Interval(TimeSpan.FromSeconds(1));

var subject = new Subject<long>();

// Subscribe the subject to the source observable
// With this you activate the source observable
interval.Subscribe(subject);

this.Subscribe(subject, "Subject #1");
await Task.Delay(TimeSpan.FromSeconds(3));
this.Subscribe(subject, "Subject #2");
```

As mentioned above, you can also use subjects as sources to manually push events into the stream by calling the `OnNext()`, `OnCompleted()` or `OnError()` methods.

```csharp
var subject = new Subject<string>();
subject.OnNext("1");
this.Subscribe(subject, "Subject");
subject.OnNext("2");
subject.OnNext("3");
subject.OnCompleted();
subject.OnNext("4");
```

In this example you will receive the events "2" and "3". You won't receive "1" because `Subject` is a hot observable and it happened before the subscription, and you also won't receive "4" because it happened after the `OnCompleted` event, which is a terminating event and implicitly disposes the whole pipeline (or more precisely the connections between the pieces of the pipeline).

#### ReplaySubject

The `ReplaySubject` class is sort of an alternative to the `Replay()` + `Connect()` method pair. Once you subscribe it to an observable stream, it will remember everything that flows through it, and will "replay" all the events for each of its new subscribers.

```csharp
var replaySubject = new ReplaySubject<string>();
replaySubject.OnNext("1");
this.Subscribe(replaySubject, "ReplaySubject #1");
replaySubject.OnNext("2");
this.Subscribe(replaySubject, "ReplaySubject #2");
replaySubject.OnNext("3");
```

In this example you can see that the `ReplaySubject` replays every event it flew through it to each of its subscribers.

The 1st subscription will immediately receive "1" after subscription, and later "2" and "3" as they appear. <br/>
The 2nd subscription will immediately receive "1" and "2" after subscription, and later "3" as it appears.

#### BehaviorSubject

The `BehaviorSubject` is very similar to the regular `Subject`, so it's a hot observable, but it does an extra trick by also replaying the last element before the occurrence of a subscription. To make sure it can always provide a "last element" immediately on subscription, you have to provide a default value to it, so even if technically there was no event flowing through it, it can still provide this default value on subscription.

```csharp
var behaviorSubject = new BehaviorSubject<string>("0");
this.Subscribe(behaviorSubject, "BehaviorSubject #1");
behaviorSubject.OnNext("1");
behaviorSubject.OnNext("2");
this.Subscribe(behaviorSubject, "BehaviorSubject #2");
behaviorSubject.OnNext("3");
behaviorSubject.OnCompleted();
this.Subscribe(behaviorSubject, "BehaviorSubject #3");
```

The 1st subscription will see "0", "1", "2", "3" and "OnCompleted". <br/>
The 2nd subscription will see "2", "3" and "OnCompleted". <br/>
But the 3rd subscription, that happened after the stream have been terminated by the `OnCompleted()` call, will only see the "OnCompleted" event but not the last event before it.

#### AsyncSubject

`AsyncSubject` caches the last element that flows through it and publishes it once it's source observable is terminated. Any future subscriptions to the subject will receive the same cached value. It's like keeping a reference to an asynchronous operation's `Task` object. This is also the kind of behaviour you can see when you `await` an `IObservable` stream, but more on that later.

```csharp
var asyncSubject = new AsyncSubject<string>();
asyncSubject.OnNext("1");
this.Subscribe(asyncSubject, "AsyncSubject #1");
asyncSubject.OnNext("2");
asyncSubject.OnNext("3");
asyncSubject.OnCompleted();
this.Subscribe(asyncSubject, "AsyncSubject #2");
```

The `AsyncSubject` only yields anything after it's been terminated, meaning no matter when or where do you subscribe to it, you will always get the last event from the stream before its termination. In this example both the 1st and 2nd subscription will see the events "3" and "OnCompleted" right after `OnCompeted()` have been called on the subject.

## LINQ

Up until this point in this chapter you were learning about the basics of observables. How to create them, convert existing event sources or use subjects to manually push events into a stream, what kind of characteristics they have, etc.

The more interesting part though are the operators, all the things that you can do with these streams. Throughout the remaining part of this chapter you will learn about quite a few operators that you can use in your everyday work/life for simple tasks like doing some data transformation on the events, filter them, or more advanced tasks like error handling, combining multiple streams, etc.

I believe the absolute top two standard LINQ operators are the `Select()` and `Where()`, so let's start with those.

### Projection

#### Select

You can use `Select()` to transform (or "project") the data that is travelling through the stream in each event.<br/>
You could see an example of it when you were extracting some useful information from the `EventArgs` after converting a traditional .NET event to an Rx stream.

```csharp
var source = Observable
    .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
    .Select(e => e.EventArgs.Key.ToString());
```

### Filtering

#### Where

The ability to do basic filtering on the events is incredibly useful especially in cases when your source is quite "noisy". What if you want to detect if your users are moving their pointer (mouse/touch/stylus/gaze/whatever) over a certain part of your UI? Your naive solution would be to subscribe to the `PointerMoved` event and in the event handler make a big `if` statement because you only care about certain positions. Let's assume you want to divide your UI to a grid of 100x100 pixel rectangles, and you only care about the cases when the user is moving his/her pointer within the diagonal rectangles, so where the rectangle's position is like x=y.

```csharp
var source = Observable
    .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
    .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
    .Select(p => new Point((int)(p.X / 100), (int)(p.Y / 100)))
    .Where(p => p.X == p.Y)
    .Select(p => $"{p.X}, {p.Y}");
```

![](Marble%20Diagrams/Where.png)

#### Distinct, DistinctUntilChanged

If you try the previous code example, you will notice that it produces many events with the exact same data. 

Let's modify the requirements a little bit. Keep the grid, but let's say you want to be notified each time the user enters into one of these rectangles and you don't want the remaining couple of hundred events stating "the user is in this area", "the user is still in the same area"...

For this you can use the `DistinctUntilChanged()` operator, that will only let an event through if it's different from the previous one.

```csharp
var source = Observable
    .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
    .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
    .Select(p => new Point((int)(p.X / 100), (int)(p.Y / 100)))
    .DistinctUntilChanged()
    .Select(p => $"{p.X}, {p.Y}");
```

![](Marble%20Diagrams/DistinctUntilChanged.png)

There is a stronger version of this operator that you would typically use in a more traditional data query, the `Distinct()`, which guarantees that a given value can only occur once in the lifetime of the stream and every other occurrences will be filtered out.

#### Skip

While the `Where()` operator works on the whole stream, checks each element against a predicate and decides whether to keep it or not, it's possible that you might just want to skip elements at the beginning or end of the stream. And this is exactly what you can do with the different variations of the `Skip()` operator. The difference is similar to `foreach` + `if` vs `while` loops.

The most simple version lets you specify the number of elements or the duration of time you want to skip at the beginning (with the `Skip()`) or end (with the `SkipLast()`) of the stream.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .Skip(5);
```

![](Marble%20Diagrams/Skip.png)

It's worth mentioning that by its nature the `SkipLast()` will only produce elements after its source stream has completed. It makes sense, you can only tell which elements were the last 3 (for example) after the source has terminated.

```csharp
var source = Observable
    .Range(0, 4)
    .SkipLast(2);
```

![](Marble%20Diagrams/SkipLast.png)

While with the `Skip()` operator you can specify a timespan, a duration of time you would like to skip, it's also possible to specify an absolute time (in the future) saying you are not interested in any event until that specific time in the future.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .SkipUntil(DateTime.Now + TimeSpan.FromSeconds(5));
```

![](Marble%20Diagrams/SkipUntil.png)

And last but not least it's also possible to provide a predicate saying you want to skip every element until the predicate is true. After the first time the predicate evaluates to false, it will start letting events through and won't evaluate the predicate any more (otherwise it would be equivalent to the `Where()` operator).

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .SkipWhile(num => num % 2 == 0);
```

![](Marble%20Diagrams/SkipWhile.png)

#### Take

The `Take()` operator is the opposite of `Skip()`.

You can say you want to *take* the first *n* elements or the first *n* second of the stream.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .Take(5);
```

![](Marble%20Diagrams/Take.png)

Or you can say the same about the last *n* elements or *n* seconds.

```csharp
var source = Observable
    .Range(0, 3)
    .TakeLast(2);
```

![](Marble%20Diagrams/TakeLast.png)

It also has the version to specify an absolute time in the future, saying you want everything until that point and nothing after.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .TakeUntil(DateTime.Now + TimeSpan.FromSeconds(5));
```

![](Marble%20Diagrams/TakeUntil.png)

And it also works with a predicate, saying you want everything while the predicate evaluates to true and nothing after.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .TakeWhile(num => num % 2 == 0);
```

![](Marble%20Diagrams/TakeWhile.png)

Just like any other two operators, you can combine the `Skip()` and `Take()` operators if that's what your business logic requires.

In the sample below you can see a stream producing a new event every second.<br/>
First you say you want to skip 3 elements, so it will skip 0, 1 and 2,<br/>
and after that you say you want to take 4 elements, so it will take 3, 4, 5 and 6 and after the 4th element, it will terminate the stream.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .Skip(3)
    .Take(4);
```

![](Marble%20Diagrams/SkipAndTake.png)

### Selectors

In this section you will see some of the operators that you can use to select a single element from the stream.

All of these operators have 2x2 versions.

The normal version requires the operator to successfully execute, otherwise it throws an exception (produces an OnError event). So the `First()` or `Last()` operators will fail if there is no element in the stream, the `ElementAt()` will fail if the specified index doesn't exist before the stream is terminated and the `Single()` will fail if there is either more or less element in the stream than one.

There is a more tolerant version for these operators which adds an `OrDefault` ending to their name. It means if it would normally fail, it will rather return the `default` value for the given type (`null` for reference types, `0` for `int`, `false` for `bool`, etc.).

These operators also support an additional overload where you can specify a predicate. For example in case of the `First()` operator it would mean that it wouldn't just blindly return the very first element in the source stream, but it would wait for the first element that evaluates the provided predicate to `true`.

It also worth mentioning that technically you will see 4 version for each of the operators:
* Operator
* OperatorAsync
* OperatorOrDefault
* OperatorOrDefaultAsync

... but you should NOT use the non-async versions as they are marked obsolete. The problem with them is that they wait for the given element to appear in the stream synchronously, blocking the execution thread, and that would be quite counterproductive considering Rx is a library thats purpose is to make asynchronous programming easier.

#### First

So the `FirstAsync()` operator, as its name suggests, will return the very first element in the stream right after it appears.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .FirstAsync();
```

![](Marble%20Diagrams/First.png)

#### Last

The `LastAsync()` operator will have to wait for the source stream to terminate and then it can return the last element before the termination.

```csharp
var source = Observable
    .Range(0, 4)
    .LastAsync();
```

![](Marble%20Diagrams/Last.png)

#### ElementAt

The `ElementAt()` operator will keep track of the index of the events and once the stream grows big enough to reach the specified index, it returns with that event.

Pay attention to the subtle detail that compared to `FirstAsync()` and `LastAsync()`, the `ElementAt()` doesn't have the "Async" ending in its name (just to confuse things a little).

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .ElementAt(2);
```

![](Marble%20Diagrams/ElementAt.png)

#### Single

The `SingleAsync()` operator (yes, it has the "Async" ending) is kind of a condition check instead of selection logic. It only returns successfully if there is exactly one element in the stream, no more, no less.

It worth mentioning that the `SingleOrDefaultAsync()` operator will fail if there are more than one elements in the stream, it only "protects" against the case when there is no element in the stream.

```csharp
var source = Observable
    .Return(42)
    .SingleAsync();
```

![](Marble%20Diagrams/Single.png)

### Maths

Maths operators cover a really specific area: doing some basic kind of range operations on numerical streams. All of these operators will produce one element right after the source stream has terminated.

There are `Max()`, `Min()`, `Average()`, `Sum()` and `Count()`.

```csharp
var source = Observable
    .Range(0, 5)
    .Max();
```

![](Marble%20Diagrams/Max.png)

### Default values

In your application you might need to handle cases if an event source didn't (yet) produce any events. 

#### StartWith

There are basically two ways to "use" Rx. One of them is when you are subscribing to a stream with a long runtime and just handle the events. It's similar kind of use case to traditional .NET events. You just subscribe and the stream might never terminate. <br/>
In this case you might have a requirement to receive an event immediately after subscribing to the stream even if the source didn't *actually* produce an event. In this scenario you can use the `StartWith()` operator to proactively inject one or more elements to the beginning of the stream that the subscriber will definitely receive right after subscription.

```csharp
var source = Observable
    .Range(0, 5)
    .StartWith(42);
```

![](Marble%20Diagrams/StartWith.png)

#### DefaultIfEmpty

The other use is when you are replacing a traditional `Task` based asynchronous operation with a fancier Rx pipeline (add timeout, retry, parallel execution of multiple tasks, etc.) and then you `await` it. <br/>
In this case you can use the `DefaultIfEmpty()` operator which will retroactively check the stream and if the source stream terminates as an empty stream, it will fill in that void with a predefined default value (or the `default` value of the type).

```csharp
var source = Observable
    .Range(0, 5)
    .Where(x => x > 10)
    .DefaultIfEmpty(42);
```

![](Marble%20Diagrams/DefaultIfEmpty.png)

### Conditional operators

These operators can be used to test the stream for some kind of condition or against some predicate.

#### SequenceEqual

This is a fairly naive way to compare two streams or a stream with a traditional .NET collection. It will only return `true` if the two streams/collections has the same elements in the same order, not more, not less. They have to match exactly.

By default the operator will compare the elements using the default equality check mechanism (`GetHasValue() + Equals()`), but you can also explicitly pass an `IEqualityComparer<T>` object to do the comparison.

#### Contains

Checks if a given event occurred in the stream. It will return `true` as soon as the element-in-question appears in the stream.

#### IsEmpty

Checks if the stream is empty.

#### All

The `All()` and `Any()` operators take a predicate as their argument.

The `All()` will return `true` if the predicate returns `true` for all of the events in the stream.

#### Any

The `Any()` will return `true` if the predicate returns `true` for any of the elements in the stream. It will return `true` as soon as an event appears where the predicate returns `true`.

### Timing

#### Delay

The `Delay()` operator will do exactly what you would expect it to do. It takes the source stream, preserving the relative time between its events, and delays its elements by some specified amount of time. You can specify this delay by providing a `TimeSpan` or a `DateTime` value. The `TimeSpan` will of course just delay the occurrence of the events in the stream with the specified amount of time, while the `DateTime` will delay all the events to appear after that absolute time (again, the relative time between the events are preserved).

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .Delay(TimeSpan.FromSeconds(2));
```

![](Marble%20Diagrams/Delay.png)

There is also a way to provide some logic that will be applied to all individual elements in the original stream and calculate the delay for them. The interesting bit here is that you don't get an overload where you can provide a function that receives the actual event and returns a `TimeSpan` or `DateTime`, instead you have to return an `IObservable<object>` that will be used as a "signalling stream": when it produces its first element, the `Delay()` operator will emit the associated event on its output stream.

In this case it's also possible that your logic will delay the event "n" to a later point in time than the event "n+1", meaning this might mess up the order of your events.

There are some other operators where you can use this kind of pattern of providing an `IObservable<object>` (the actual data travelling on it doesn't matter, hence the `object`) as a "signalling stream".

#### Throttle

You could already read a little bit about the `Throttle()` operator in the Introduction chapter. Its purpose is to throttle the stream to not produce events too fast. You might want to build an auto-complete TextBox that is hooked to some web service to provide its suggestions, and you would probably not want to send a web request every single time the user hits a key. You want to wait a little bit for the user to calm down and stop typing, that's the moment when the user runs out of ideas to type and is waiting for some suggestions. It's also very useful little trick to reduce the network traffic and processing required to provide this service.

Another example could be some kind of mouse position tracking system, that tracks where the user is "resting" the cursor, so you only want to receive some notification if the user didn't move the mouse for like one second.

The operators' behaviour is that it will always emit the very last element before the "rest time" and it will just simply ignore the other events (events that were followed by another event "too soon").

The following example shows the throttling of `KeyDown` event:

```csharp
var source = Observable
    .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
    .Select(e => e.EventArgs.Key)
    .Throttle(TimeSpan.FromSeconds(1));
```

![](Marble%20Diagrams/Throttle.png)

#### Sample

If you know how analog-to-digital conversion works for video and audio recording (or recording pretty much any signal over time to be honest), then you know what sampling is. You have some kind of event source that emits events continuously, but you don't have the capacity to process all of that, so you choose to "sample" that event every millisecond or every second or so. When you see the metadata for an audio file saying it's sample rate is 44kHz, it means that the original recording was done by recording the actual air pressure with a microphone 44.000 times a second.

Or to turn back to the mouse tracking example, you might not want to just record the "resting" positions, you want more, but not MUCH more, like recording the mouse position with a 125Hz refresh rate (that is the default for USB mice), but more like the position in every 500ms or so.

Similarly to the `Throttle()` operator, the `Sample()` operator only captures the preceding event before the sampling and ignores the rest that occurs between.

```csharp
var source = Observable
    .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
    .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
    .Sample(TimeSpan.FromMilliseconds(500));
```

![](Marble%20Diagrams/Sample.png)

As with the `Delay()` operator, you can also provide another `IObservable<object>` as a "signalling stream", meaning whenever the signalling stream emits an event, it takes a sample from the original event source. Again the actual data flowing on this signalling stream doesn't matter, hence the `object` generic type parameter.

### Error handling

#### Timeout

With the `Timeout()` operator you can force the stream to fail if something doesn't happen within a specified time.

You can give it a `TimeSpan` as an argument, which means events must occur quicker in the stream than the specified amount of time. If the first event doesn't appear in the stream within that time, or if there is more time between any two consecutive events, the stream will fail.

You can also specify a `DateTime`, in which case the original stream has to terminate (successfully, with an OnCompleted event) before the specified date.

```csharp
var source = Observable
    .Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
    .Timeout(TimeSpan.FromSeconds(3));
```

![](Marble%20Diagrams/Timeout.png)

As with a couple of other operators previously, it's possible to provide a function that will have to return an `IObservable<object>` meaning the next element has to appear in the original stream before that generated `IObservable<object>` signalling stream yields an event.

#### Retry

The `Retry()` operator is really simple. If the source stream fails (produces an OnError event), it will re-subscribe infinite or a given amount of times.

Let's assume you have some failing service call (`RandomlyFailingService()` in the example) that you would like to retry 3 times before completely giving up. It would look something like this:

```csharp
var source = Observable
    .FromAsync(() => this.RandomlyFailingService())
    .Retry(3);
```

Very important piece of detail (once again) is that you should never ever convert a `Task`-returning async call to an Rx stream by calling the `ToObservable()` extension method on it. The reason is, if you properly build a stream using the `FromAsync()` method, it will re-evaluate that little function every time you subscribe to it, re-executing the actual async call. If you call `ToObservable()` on a `Task` it will just have a reference to that `Task` object but if it fails it won't be able to re-execute the operation that returned that `Task`. No matter how many times you (or the `Retry()` operator internally) subscribe to this stream, you will always get the same result or error that the original `Task` holds.

#### OnErrorResumeNext

With the `OnErrorResumeNext()` operator you can silently swallow an error and seamlessly redirect the subscription to another stream. It could be a good example for this to try to download something from multiple mirrors. If one fails, try the next one.

To demonstrate this you can set up two observables (of the same type) and then connect them with this operator.

```csharp
var source1 = Observable.Create<int>(observer => () =>
{
    observer.OnNext(0);
    observer.OnNext(1);
    observer.OnError(new Exception(""));
});

var source2 = Observable.Create<int>(observer => () =>
{
    observer.OnNext(10);
    observer.OnNext(20);
    observer.OnNext(30);
});

var source = source1.OnErrorResumeNext(source2);
```

![](Marble%20Diagrams/OnErrorResumeNext.png)

#### Catch

The `Catch()` operator can work in the exact same way as the `OnErrorResumeNext()`, but it can also be used to do more precise exception handling, building up multiple catch cases similarly as you would do it in a traditional `try-catch` expression. You can handle specific types of exceptions and provide the continuation stream based on the exception details.

```csharp
var source1 = Observable.Throw<int>(new KeyNotFoundException("Problem"));
var source2 = Observable.Return(42);
var source3 = Observable.Return(0);

var source = source1
    .Catch((KeyNotFoundException ex) => source2)
    .Catch((Exception ex) => source3);
```

#### Finally

This operator gives you the ability to do things after the source stream terminates (whether it's an OnComplete or an OnError termination). It won't alter the stream, just appends one last step before it finishes. It takes a simple `Action` as the parameter, unlike with the `Do()` operator, you don't get to see the content of the stream.

```csharp
var source = Observable
    .Return(42)
    .Finally(() => { /* ... */ });
```

### Loops

#### Repeat

The `Repeat()` operator is very similar to the `Retry()` with the minor difference that it re-subscribes to the underlying stream automatically once it *successfully* finished. Just like with `Retry()` you can choose to repeat in an infinite loop or to only repeat just a given number of times.

#### DoWhile

The `DoWhile()` operator is similar to the `Repeat()`, but it takes a predicate as an argument and it will only keep re-subscribing to the source stream while the predicate evaluates to `true`.

### Monitoring

#### Do

With the `Do()` operator you have the chance to inspect the stream at any point without modifying it. It's like writing a `Select()` logic that takes an element, doesn't do anything with it but sends some messages to a log and then returns the original element untouched. But actually, it's better than that because with the `Do()` operator you can also inspect the OnError and OnCompleted channels, which comes extremely handy as most of the time what you are really interested in is the `Exception` on the OnError channel.

#### Materialize / Dematerialize

With `Materialize()` you can capture all channels into a single (and by the way serializable) object. This is great for monitoring purposes, or when you have different parts of your Rx pipeline running on different machines and somewhere in the middle of the pipeline you have to move data over the internet.

It packs the whole stream, with all of its channels into a `Notification<T>` object. It has a couple of properties; none of them should be too surprising though. 
* a property named `Kind` of type `NotificationKind` enumeration with the possible values of `OnNext`, `OnError` and `OnCompleted`
* a pair of properties, `HasValue` (`bool`) and `Value` (`T`)
* and an `Exception` property of type `Exception`

Using this operator will come with some "side-effects".

* OnNext will work as expected

![](Marble%20Diagrams/Materialize_OnNext.png)

* OnCompleted will produce an event containing a `Notification` with `Kind` being `OnCompleted`, and then it will actually terminate the stream with a real OnCompleted event

![](Marble%20Diagrams/Materialize_OnCompleted.png)

* OnError will produce an event containing a `Notification` with `Kind` being `OnError`, and then it will terminate the stream with an OnCompleted (not OnError) event

![](Marble%20Diagrams/Materialize_OnError.png)

This operator has a corresponding `Dematerialize()` pair, to unwrap these notifications to "real" events on the appropriate channels.

#### Timestamp

This operator attaches a `DateTimeOffset` timestamp to the events, producing a `Timestamped<T>` object that has a `Timestamp` property of type `DateTimeOffset`, and a `Value` property of type `T`. Unlike the `Materialize()` operator, this one does not come with some kind of "RemoveTimestamp" operator to unwrap this object, though you can easily write your own operator to achieve this functionality.

#### TimeInterval

While the `Timestamp()` operator attaches a `DateTimeOffset` to each event, the `TimeInterval()` operator attaches a `TimeSpan` to the events, representing the time difference between the current and the previous event. It produces a `TimeInterval<T>` object that has an `Interval` property of type `TimeSpan` and a `Value` property of type `T`. Just like the `Timestamp()` operator, the `TimeInterval()` also lacks a "RemoveTimeInterval" unwrapping operator, but again, it's very easy to write one for yourself.

It also worth mentioning that while you are absolutely free to use these operators in your production environment, they also serve as a great guideline for this kind of functionality and you can also build your very own metadata object that can hold even more data than these built-in ones.

### Combiners

#### SelectMany, Merge and Concat

These operators are merging multiple streams with different strategies.

##### SelectMany

The `SelectMany()` operator might sound familiar from the traditional LINQ world. Somehow you end up having a collection of collections and you want to flatten this two dimensional hierarchy to just one dimension by appending each sub-collection after each other.

The difference in the Rx world compared to the LINQ world is that things are happening asynchronously and in parallel, so what you will get on the resulting stream is a mix of all the sub-streams that you generated (or retrieved) from the events.

This particular operator offers a huge amount of overloads but the basic idea is always the same: have an original "one dimensional" stream, for each event generate/extract some kind of sub-stream (or collection or `Task`) (at this point you have a "two dimensional" stream) and "merge" all these sub-streams into a single resulting stream.

##### Merge

A simplified version of this is the `Merge()` operator that has only 2 overloads and it will only do the "merging" part of the `SelectMany()` operator. It will just take a couple of already existing (but not necessarily active) streams and merge them (in contrary to the `SelectMany()` that creates the many streams as part of its logic).

The `Merge()` operator has an overload where you can specify a number that will limit the maximum number of concurrent subscriptions to the underlying sub-streams.

You can also think about the connection between `SelectMany()` and `Merge()` in a way that you can represent the functionality of `SelectMany()` with the combination of the `Select()` and `Merge()` operators.

![](Marble%20Diagrams/Merge.png)

##### Concat

And last but not least, in this group, comes the `Concat()` operator, which is basically the same as `Merge(1)`, meaning it will subscribe to the sub-streams sequentially, one-by-one, concatenating them after each other instead of subscribing to many of them and merging their events.

This example shows two hot observables using the `Concat()` operator, and as you can see it only starts looking at the second stream after the first one completed.

![](Marble%20Diagrams/Concat.png)

#### Zip and CombineLatest

Until this point you have been reading about simple flattening operators that were combining multiple streams into one, but what if you don't just want to blindly merge those events on the different streams but you want to match (or join) them?

Doing traditional LINQ/SQL style join on streams might not be the best idea (though it's obviously not impossible to do). Instead you might be interested in "order based" joins like combining events with the same index from multiple streams (first element on one stream with the first element on the other stream, second with second, third with third, etc.), or always having an up-to-date combination of the latest elements on all streams.

Doing traditional "equality based" join would mean that you subscribe to multiple streams, store all the events in a collection that ever appeared in them, and whenever any of them emits an event, you try to join it with the stored collections and if successful, produce an event in the result stream containing the joined events.

##### Zip

So the first version of "order based" joining is when you join events from multiple streams based on their indexes. This can be achieved with the `Zip()` operator.

To demonstrate it I will use two subjects and manually send events to them in mixed up order to then see the result being nicely "zipped" by the operator.

```csharp
var source1 = new Subject<int>();
var source2 = new Subject<char>();

var source = Observable.Zip(source1, source2, (i, c) => $"{i}:{c}");

source1.OnNext(0);
source1.OnNext(1);
source2.OnNext('A');
source2.OnNext('B');
source2.OnNext('C');
source1.OnNext(2);
source1.OnNext(3);
```

![](Marble%20Diagrams/Zip.png)

#### CombineLatest

The second version of the "order based" join is when you are interested in the combination of the latest events on all streams every time any of the streams produces a new event.

One example for this would be to keep track of the key (or keys) pressed on the keyboard at the time the user clicks with their mouse. Depending on whether they pressed the CTRL, ALT, SHIFT or none of them (or a combination of those), you might have to do different things (selection, scaling, panning, etc.). I will not show such a complicated logic here, I will just reach for some subjects again to manually push events to demonstrate how the operator works.

```csharp
var source1 = new Subject<int>();
var source2 = new Subject<char>();

var source = Observable.CombineLatest(source1, source2, (i, c) => $"{i}:{c}");

source1.OnNext(0);
source1.OnNext(1);
source2.OnNext('A');
source1.OnNext(2);
source1.OnNext(3);
source2.OnNext('B');
```

![](Marble%20Diagrams/CombineLatest.png)

#### Amb and Switch

The `Amb()` and `Switch()` operators are designed to receive multiple streams and keep alive only one of them.

##### Amb

The `Amb()` will receive many streams, subscribe to them and wait for the first one to emit an event and keep only that stream and throw away the rest of the streams.

```csharp
var source1 = Observable
    .Timer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));

var source2 = Observable
    .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
    .Select(x => x * 10);

var source = Observable.Amb(source1, source2);
```

![](Marble%20Diagrams/Amb.png)

##### Switch

In contrary to `Amb()`, `Switch()` always switches to the most recent stream. It doesn't work with a collection of streams, it works on a stream of streams and always only listens to the latest sub-stream. It's a little bit like `Concat()` but it forcibly closes the previous sub-stream if a new one appears instead of waiting for it to naturally complete and switching to the next one only after that.

To demonstrate this I will do the following:
* Create a base stream that will provide powers of 10 (10, 100, 1000, etc.) every 5 seconds
* And on each event generate a sub-stream that will produce a sequence of numbers multiplied by the number from the base stream's event, so (1, 2, 3 or 10, 20, 30 or 100, 200, 300, etc.)
* And as a last step, apply the `Switch()` operator on the resulting stream of streams

```csharp
var baseStream = Observable
    .Interval(TimeSpan.FromSeconds(5))
    .Select(i => Math.Pow(10, i));

var subStreams = baseStream
    .Select(i => Observable
        .Interval(TimeSpan.FromSeconds(1))
        .Select(j => i * j));

var source = subStreams.Switch();
```

The result will look something like this:

![](Marble%20Diagrams/Switch.png)

### Windowing

#### Window and Buffer

The `Window()` operator will take the source stream and based on the various parameterisation it will open windows over the original stream that you can subscribe to. It can be parameterised in a way to create windows of n elements, or windows of x seconds, or windows of x seconds with a maximum of n elements, but you can also take more control over the opening and closing of windows by providing other streams as "signal streams" to close the existing and open a new window or to have overlapping windows opened by some event and closed by a connecting event. Like opening windows by pressing A, B or C and closing them by pressing 1, 2 or 3 but A will be closed by 1, B by 2 and C by 3.

As Window produces an `IObservable<IObservable<T>>`, you will have to have some kind of strategy to flatten this nested hierarchy of observables by using `Merge()`, `Concat()`, `Switch()` or something custom.

The `Buffer()` operator is very similar to the `Window()` but it won't make the individual windows available as observable streams, instead it waits for them to close and returns their content as a collection, so you will get an `IObservable<IEnumerable<T>>`.

As long as you are using these operators with their basic overloads and cover predictable windows over the original data source, I think it can be very useful especially for analytical/statistical purposes. Though an important note to mention is that there is no metadata (like a reference to the opening event for example) stored with either the sub-streams in case of the `Window()` or the collections in case of the `Buffer()` that would identify that particular window. So if you go crazy with opening and closing overlapping windows, well, good luck figuring out which window belongs to which pair of events.

#### Scan and Aggregate

These operators let you go through each event of a stream, holding some accumulator object and letting you modify it for each event.

For example for a numerical stream you can choose to collect all elements into a list. In this case a list is the accumulator and you just add the new numbers to it as they appear in the stream. Or you can do something more useful and do min or max selection, or sum all the numbers (though you have explicit operators for all of that).

The difference between the `Scan()` and `Aggregate()` operators is similar to the difference between the `Window()` and `Buffer()` in a way that `Scan()` is kind of a real time operator that emits all the subsequent results of the aggregation as new events appear in the source stream, while the `Aggregate()` will only emit the final result of the aggregation right after the original stream completes.

Here you can see two examples to sum numbers using the `Scan()` and `Aggregate()` operators.

```csharp
var source = Observable
    .Range(0, 5)
    .Scan((accumulator, current) => accumulator += current);
```

![](Marble%20Diagrams/Scan.png)

```csharp
var source = Observable
    .Range(0, 5)
    .Aggregate((accumulator, current) => accumulator += current);
```

![](Marble%20Diagrams/Aggregate.png)

#### GroupBy

With the `GroupBy()` operator you can specify a key selector function that would extract some kind of partitioning information from the events and create sub-streams based on that key or just send new events into an already existing one.

You will end up with an `IObservable<IGroupedObservable<T>>`, where the `IGroupedObservble<T>`'s only difference compared to the basic `IObservable<T>` is that is has a `Key` property, so you know which group it is.

To demonstrate it with a very simple example you will divide numbers to 2 partitions: odd and even.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .GroupBy(x => x % 2 == 0);
```

![](Marble%20Diagrams/GroupBy.png)

As you can see the output of this operator will only have 2 events, one for creating the group "odd" and one for "even". If you are interested in the actual events that go into those groups, you have to subscribe to those sub-streams or use one of the flattening techniques to simplify this nested hierarchy.

And here I would like to stop and make you think. If you have a `GroupBy()` operator that you later merge back into a single stream using the `Merge()` or `SelectMany()` operator while appending some metadata to the events (like their group key), what is the difference between using this technique compared to just using one simple `Select()` statement to generate that metadata? Probably nothing. 

This operator is useful if you don't just do a simple grouping but you also apply some kind of logic to the inner streams (or groups or windows) and potentially merge them back only after that.

For example you can "amplify" the previous example and say you want to have a "scan" (real time aggregation) of each group, but you also don't want to deal with this nested observable nonsense so you want all of these in a single stream, tagged by the key of the group.

```csharp
var source = Observable
    .Interval(TimeSpan.FromSeconds(1))
    .GroupBy(x => x % 2 == 0)
    .SelectMany(x => x
        .Scan((accumulator, current) => accumulator += current)
        .Select(y => Tuple.Create(x.Key, y)));
```

![](Marble%20Diagrams/GroupByAdvanced.png)

## Schedulers

Schedulers are a core part of Rx. When you are building complicated pipelines, in special cases you might want greater control over the execution of parts of the pipeline. By default Rx is single threaded and it won't start spawning threads for you, though it's very much likely that the actual asynchronous code you wrap and use as part of your pipeline will execute its logic on background thread(s).

If you have a piece of code that is resource intensive and you want to parallelise it, you can explicitly specify this need using a scheduler. Or if it's the opposite way and you want that piece of code to be executed on the same thread (queued or immediately), again, schedulers are going to be your good friends. Or another great example for being explicit about the execution policy of your code is when you have to manipulate the UI and you have to marshall the execution to the UI thread.

Schedulers are not just about threading though. In Rx you could see many examples for timing. Schedulers give the whole Rx system the sense of time. Every single Rx operator depends on its scheduler's clock and not on the system provided static `DateTime.Now`. Thanks to this you can be in explicit control of the time which can come handy when you are writing tests for your pipeline, you can speed things up and make a pipeline with lots of timings unit testable.

### The high level anatomy of schedulers

I won't go into great details about schedulers because this is not really an in-depth kind of book, I would rather just cover the high level concept and give you some good idea about schedulers, how they work, how to use them.

Schedulers are fairly simple and they just give an extra level of control over the execution of the piece of logic you pass to the various operators. They implement the `IScheduler` interface that defines the following contract:

```csharp
public interface IScheduler
{
    // Gets the scheduler's notion of current time.
    DateTimeOffset Now { get; }

    // Schedules an action to be executed.
    IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action);
    
    // Schedules an action to be executed after (realtive) dueTime.
    IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action);
    
    // Schedules an action to be executed at (absolute) dueTime.
    IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action);
}
```

With the `Now` property you can get some insight on the scheduler's notion of current time (which is mostly going to be interesting when you are writing unit tests), and three overloads of the `Schedule()` method for scheduling the execution of some action "immediately" or sometime later in the future specified either by a relative (`TimeSpan`) or an absolute (`DateTimeOffset`) time.

### Types of schedulers

You will likely never have to actually implement a scheduler, but you should be aware the ones provided by the library.

#### ImmediateScheduler

With the `ImmediateScheduler` you can execute your logic immediately (synchronously) on the currently executing thread. This is basically forcing the execution to happen right now. Usually considering Rx has a lightweight "thread-free" execution model, it's not really advised to be used. You can find it by referring to the `ImmediateScheduler.Instance` static property.

#### CurrentThreadScheduler

The next scheduler in the line is the `CurrentThreadScheduler` which can be found under the `CurrentThreadScheduler.Instance` property. This will queue the execution of the action on the currently executing thread, but it won't force it to execute immediately.

#### TaskPoolScheduler and NewThreadScheduler

If you want to make sure the execution will happen on a background thread, you can use the `TaskPoolScheduler` (`TaskPoolScheduler.Default`) which uses the TPL `TaskFactory` to schedule operations on the thread pool (very important that this is going to use a thread **pool** behind the scenes), or the `NewThreadScheduler` (`NewThreadScheduler.Default`) which will forcibly start a new thread for execution (which is not advised to use because creating threads is expensive and creating too much will actually have a negative effect on the performance).

#### ThreadPoolScheduler and CoreDispatcherScheduler (UWP)

If you are building an UWP application, you can also use the WinRT `ThreadPool` with `ThreadPoolScheduler.Default`, and you will also have a special scheduler to marshall work to the UI thread, the `CoreDispatcherScheduler` (`CoreDispatcherScheduler.Current`), which is used so commonly that it even has its own extension method: `ObserveOnDispatcher()`.

#### DefaultScheduler (will use the default thread pool for the platform)

There's also a helper scheduler to schedule work on the available system `ThreadPool`, the `DefaultScheduler` which will choose between `System.Threading.ThreadPool` and `Windows.System.Threading.ThreadPool`.

### Using schedulers

Using schedulers can either be done by just simply passing an instance of it to the operator if it supports that kind of overload, or to make it more explicit without messing up the parameterisation of the operators, you can use the `ObserveOn()` or the `SubscribeOn()` extension methods explicitly.

With the `SubscribeOn()` you can define where the *subscription* should happen, which would likely mean the execution of some asynchronous operation that you encapsulate with the `Observable.FromAsync()` operator or one of the other generator operators.

On the other hand the `ObserveOn()` operator will define where the notifications (OnNext, OnCompleted, OnError) are executed going forward in the pipeline.

It would be a good example to consider a client side application where you have some resource intensive operation that will yield some result but you want to display that result once you receive it. You would use the `SubscribeOn()` operator to make sure the execution will happen on a background thread, and the `ObserveOn()` (or more likely the `ObserveOnDispatcher()`) to marshall the result to the UI thread so you can display it.

### Testing

It's great that you have Rx in your disposal to handle all kinds of asynchronous and timed workflows and declaratively specify your threading strategy, but at some point you have to test what you built, to make sure it does what you think it should do. And I'm pretty sure you don't want to wait a minute in a unit test to figure out whether your timeout/retry logic works or not. Do you remember when I said Rx's notion of time relies on the scheduler it is using? This is another point where the concept of schedulers comes extremely handy.

There is a special type of scheduler, the `HistoricalScheduler` that basically gives you the ability to speed up time, so that all the timed events will happen immediately.

You can either use the absolutely manual way of adjusting the scheduler's clock by using the `AdvanceBy()` or `AdvanceTo()` methods to advance the clock by a relative amount of time or advance it to an absolute time, or you can just call the `Start()` method to run the scheduler while it has any scheduled elements.

To demonstrate this here is an example to test a piece of pipeline that would normally take a minute to run, but in a unit test it will finish in the fraction of a second.

```csharp
// Arrange
var baseTime = DateTimeOffset.Now;

var scheduler = new HistoricalScheduler(baseTime);

var expectedValues = new[]
{
    Timestamped.Create(0L, baseTime + TimeSpan.FromSeconds(10)),
    Timestamped.Create(1L, baseTime + TimeSpan.FromSeconds(20)),
    Timestamped.Create(4L, baseTime + TimeSpan.FromSeconds(30)),
    Timestamped.Create(9L, baseTime + TimeSpan.FromSeconds(40)),
    Timestamped.Create(16L, baseTime + TimeSpan.FromSeconds(50)),
    Timestamped.Create(25L, baseTime + TimeSpan.FromSeconds(60))
};

var actualValues = new List<Timestamped<long>>();

var source = Observable
    .Interval(TimeSpan.FromSeconds(10), scheduler)
    .Select(x => x * x)
    .Take(6);

var testSource = source
    .Timestamp(scheduler)
    .Do(x => actualValues.Add(x));

// Act
testSource.Subscribe();
scheduler.Start();

// Assert
if (expectedValues.SequenceEqual(actualValues, TestDataEqualityComparer.Instance))
    Console.WriteLine("The test was successful");
else
    Console.WriteLine("The test failed");
```

Plus the helper class to compare the expected and real values:

```csharp
private class TestDataEqualityComparer : IEqualityComparer<Timestamped<long>>
{
    // Singleton object
    private TestDataEqualityComparer() { }
    private static TestDataEqualityComparer instance;
    public static TestDataEqualityComparer Instance => instance ?? (instance = new TestDataEqualityComparer());

    // Interface implementation
    public int GetHashCode(Timestamped<long> obj) => 0;
    public bool Equals(Timestamped<long> x, Timestamped<long> y)
        => x.Value == y.Value && AreDateTimeOffsetsClose(x.Timestamp, y.Timestamp, TimeSpan.FromMilliseconds(10));

    // Helper method to compare DateTimes
    private static bool AreDateTimeOffsetsClose(DateTimeOffset a, DateTimeOffset b, TimeSpan treshold)
        => Math.Abs((a - b).Ticks) <= treshold.Ticks;
}
```

This code requires some quick remarks.

As you could notice, all the schedulers are singletons, and so should your `TestScheduler` or `HistoricalScheduler` be. Always share the same instance of the scheduler between each operator in your pipeline. In a real-world scenario you would just use some dependency-injected `IScheduler` objects in your logic, and in production you would set real schedulers for those, and in test you would set a `TestScheduler` or `HistoricalScheduler`.

When it makes sense (it's not always the case) you should do assertions on events by specifying their expected value and time of occurrence in the stream. To achieve this it's worth saving the date and time at the beginning of the test (and set this as the initial state of the test scheduler's clock) and use the `TimeStamp()` operator to tag each event with its "birth date" (according to the scheduler of course) and make the assertion on these `TimeStamped<T>` objects. When you make assertion on time stamps, you should make "close to" assertions instead of equality checks, because those dates will never be exactly the same. It worth mentioning that this example is a very naive (or verbose) approach to do the setup and the assertion, you could write some generic helper methods to help you with the "boilerplate" so your test code can be more compact and can focus on the important things.

## Rx + Async

Even though I've been talking about C#'s `async` and `await` keywords a lot throughout the book, I think it worth spending a couple of sentences to talk about the relationship between those keywords and Rx's `IObservable` streams.

Long story short, they are "awaitable". In C# any object can be "awaitable" that meets a certain contract, it's not limited to the `Task` type. And the fun part is that these required methods are not enforced by some kind of interface, they just have to be there and the compiler will realise that the object is "awaitable". Even more fun is that these methods not even have to be actually on the object, you can provide them using extension methods, so you can actually make *any* type awaitable by just defining a bunch of extension methods for it.

But what does it mean to `await` an `IObservable`? A stream can have many events in it during its lifetime, but the `await` operator will only capture one thing. Is it going to be the first or last element, or is it going to be a list of all events that appeared in the stream during its lifetime?

The answer is: it's going to be the last element. It's like implicitly calling the `LastAsync()` operator on the stream. 

Let's see a couple of simple examples to give you ideas of what can you achieve with Rx and the `await` keyword.

If you have been converting a `Task` into an Rx stream for the sake of adding timeout and retry logic to it, at the end it will still return you one event or an exception, so it's directly suitable to just `await` as is. 

```csharp
var result = await Observable
    .FromAsync(async () => "Hello World")
    .Timeout(TimeSpan.FromSeconds(5))
    .Retry(3);
```

If you would like to collect events but you are actually only interested in the result collection and not the real time events themselves, you can call `ToList()` on the stream which will do exactly what you need, collect all the events in a `List` and return that collection when the stream completes.

```csharp
var result = await Observable
    .FromEventPattern<PointerRoutedEventArgs>(this, nameof(this.PointerMoved))
    .Select(e => e.EventArgs.GetCurrentPoint(this).Position)
    .Take(TimeSpan.FromSeconds(5))
    .ToList();
```

Or you can even do more interesting things, like popping up a dialogue and waiting for the user to press "Ok" or "Cancel" (or in the following example, hitting "Enter" or "ESC").

```csharp
var enter = Observable
    .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
    .Select(e => e.EventArgs.Key)
    .Where(k => k == VirtualKey.Enter)
    .FirstAsync();

var esc = Observable
    .FromEventPattern<KeyRoutedEventArgs>(this, nameof(this.KeyDown))
    .Select(e => e.EventArgs.Key)
    .Where(k => k == VirtualKey.Escape)
    .FirstAsync();

var dialogResult = await Observable.Amb(enter, esc);

Console.WriteLine($"The user pressed the {dialogResult} key");
```

When you are using the `await` keyword, you don't need to explicitly call the `Subscribe()` method on the stream to activate it, it happens implicitly by the `await` keyword. It also catches the `Exception` flowing on the OnError channel and throws it as an exception, so you can catch it in a traditional, imperative `try-catch` block as you can see it in the following example.

```csharp
try
{
    await Observable.Throw<int>(new Exception("Some problem happened in the stream"));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

Another thing to consider is to use Rx schedulers in your imperative code.

For example when you would normally write something like 

```csharp
await Task.Delay(TimeSpan.FromSeconds(5));
```

you could write instead

```csharp
var scheduler = CurrentThreadScheduler.Instance;
await Observable.Timer(TimeSpan.FromSeconds(5), scheduler);
```

which means that if you get that scheduler from outside, your otherwise imperative code with timing inside will become really quick during unit tests. Of course you have to build on this assumption and always use the scheduler's clock instead of `DateTime.Now`, but it's always a good idea to not use `DateTime.Now` directly, because it makes unit testing complicated.

## Summary

In this very long chapter you learned about working with Rx streams. Generating/converting existing event sources, building complex query pipelines to modify, filter and join streams, taking explicit control over the threading policy with schedulers and testing all of these in unit tests using special schedulers where you are in control of the time. And last but not least you could learn a little about the relationship between the `IObservable<T>` type and the `await` keyword.
