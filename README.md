## MugenMvvmToolkit.DryIoc

Adapter for [DryIoc](https://bitbucket.org/dadhi/dryioc) to integrate with [Mugen MVVM Toolkit](https://github.com/MugenMvvmToolkit/MugenMvvmToolkit)

[![Build status](https://ci.appveyor.com/api/projects/status/s3tb7r3oupirw6vq/branch/master?svg=true)](https://ci.appveyor.com/project/SamuelDebruyn/mugenmvvmtoolkit-dryioc/branch/master) [![NuGet](https://img.shields.io/nuget/v/MugenMvvmToolkit.DryIoc.svg?label=NuGet)](https://www.nuget.org/packages/MugenMvvmToolkit.DryIoc/)

```
Install-Package MugenMvvmToolkit.DryIoc
```

### Features & usage

DryIoc is [one of the **fastest**](http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison) IoC containers. This is a wrapper for DryIoc to work with Mugen MVVM Toolkit so you can use all the default Mugen MVVM APIs (`IModule`, `IIocContainer` etc.)

* Supported default Mugen MVVM dependency lifecycles: `TransientInstance`, `SingleInstance`
* Extra dependency lifecycle `SingleInstancePerThread`
* Extra dpendency lifecycle `SingleInstancePerResolution`
* Use the fluent API of `DryIocContainer.Builder` to configure and build the container

### Parameters

At the moment there are no supported IIocParameters as I haven't needed them myself. Please create an issue or a PR if you're missing functionality.

### Release notes

#### 1.1.1

* [FEATURE] Code documentation and unit tests
* [FEATURE] Throw an `ArgumentException` when parameters are passed to the container that the container doesn't implement (yet)

#### 1.1.0 (breaking changes)

* [IMPROVEMENT] Use built-in DryIoc feature to resolve unregistered types
* [FEATURE - BREAKING CHANGE] Configure container options using fluent API
* [FEATURE] Dependency lifecycle `SingleInstancePerThread`
* [FEATURE] Dependency lifecycle `SingleInstancePerResolution`

#### 1.0.1

* [FIXED] Child containers didn't have the parent's registrations
* [FIXED] Resolve unregistered types

#### 1.0.0

* Initial release
