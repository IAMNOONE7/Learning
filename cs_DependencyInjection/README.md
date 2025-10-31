# Dependency Injection in WPF (.NET 9)

This folder contains a simple **WPF app** demonstrating how to use **Dependency Injection (DI)** 
with **Views**, **ViewModels**, and **Services** using the built-in 
`Microsoft.Extensions.DependencyInjection` and `Microsoft.Extensions.Hosting`.

---

## It includes:

- HomeView and SettingsView (two screens)
- NavigationStore and NavigationService for navigation
- IGreeter with two implementations (CasualGreeter, FormalGreeter)
- MainViewModel with commands to navigate between screens

---

## How It Works

- App.xaml.cs builds the DI container
- MainWindow uses MainViewModel as DataContext.
- MainViewModel has two commands:
 - GoHome = new RelayCommand(_ => nav.Navigate<HomeViewModel>());
 - GoSettings = new RelayCommand(_ => nav.Navigate<SettingsViewModel>());
- NavigationService uses DI to create ViewModels.
- NavigationStore holds the current ViewModel, and WPF updates automatically using DataTemplates.

---

## Why DI

- No new inside ViewModels — DI handles creation.
- Easy to swap implementations (e.g., switch greeters).
- Testable — fake services can be injected.
- Centralized lifetime management.
- Cleaner code and less dependency spaghetti.
