# MakeKits

MakeKits is a .NET solution for building and loading `Workshop` modules. It includes the core abstraction layer, a Webview runtime, a CLI entry point, and runnable sample hosts and workshops.

## Project Structure

- `src/MakeKits.Workshop.Abstractions`: Defines workshop interfaces, context, and descriptor metadata.
- `src/MakeKits.Workshop.Webview`: Provides Webview workshop runtime support.
- `src/MakeKits.CLI`: Command-line entry point.
- `sample/MakeKits.Workshop.Host`: WPF host application.
- `sample/MakeKits.Workshop.Webview.HelloWorld`: Webview Hello World sample.
- `sample/MakeKits.Workshop.Webview.Debugger`: Webview debugger sample.

## Features

- Organizes plugin-style workshops through `IWorkshop`, `IWorkshopDescriptor`, and `IWorkshopContext`.
- Loads workshops as separate assemblies inside a host application.
- Provides Webview2-based page rendering.
- Includes samples to help you get started quickly.

## Install

```bash
powershell -NoProfile -ExecutionPolicy Bypass -Command "iwr -useb https://example.com/install.ps1 | iex"
```

## Build

```bash
dotnet build MakeKits.slnx
```

## Run the Sample

Build the solution first, then start `sample/MakeKits.Workshop.Host`.

```bash
dotnet run --project sample/MakeKits.Workshop.Host/MakeKits.Workshop.Host.csproj
```

## Packaging

Use `build/nuget_pack.ps1` to create NuGet packages.

## License

[MIT](LICENSE).

