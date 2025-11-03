# C# Async Learning Demo

This small WPF solution is used to **learn and experiment with C# asynchronous programming concepts** — all inside separate, easy-to-run views.

Each folder (and View + ViewModel pair) demonstrates a specific topic:
| Folder | Focus | Description |
|---------|--------|-------------|
| `Task_Async_Await_Basic` | async / await basics | Simple sequential async operations using `await Task.Delay(...)` |
| `SemaphoreDemo` | SemaphoreSlim | Running multiple tasks with **controlled concurrency** (e.g., only 3 at a time) |
| `LockDemo` | lock / synchronization | Demonstrates **race conditions** and thread-safe increments using `lock` |
| `CancellationProgressDemo` | Cancellation + Progress | Shows how to cancel async work and update UI progress using `CancellationToken` and `IProgress<T>` |
| `AsyncStreamsDemo` | Async Streams | Demonstrates streaming values over time using `IAsyncEnumerable<T>` and `await foreach` |

---

## How to run a specific demo

Each demo is a standalone window with its own View and ViewModel.  
To run a specific one, change the **startup view** in `App.xaml`.

Example:

```xml
<Application x:Class="cs_Asynchronous.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:cs_Asynchronous"
             StartupUri="AsyncStreamsDemo/AsyncStreamsDemo_View.xaml">
    <!--StartupUri="CancellationProgressDemo/CancellationProgress_View.xaml-->
    <!--StartupUri="LockDemo/LockDemo_View.xaml"-->
    <!--StartupUri="SemaphoreDemo/SemaphoreDemo_View.xaml"-->
    <!--StartupUri="Task_Async_Await_Basic/Task_Async_Await_Basic_View.xaml"-->
    <Application.Resources>
         
    </Application.Resources>
</Application>