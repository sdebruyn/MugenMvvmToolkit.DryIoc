## MugenMvvmToolkit.DryIoc

Adapter for [DryIoc](https://bitbucket.org/dadhi/dryioc) to integrate with [Mugen MVVM Toolkit](https://github.com/MugenMvvmToolkit/MugenMvvmToolkit)

[![Build status](https://ci.appveyor.com/api/projects/status/s3tb7r3oupirw6vq/branch/master?svg=true)](https://ci.appveyor.com/project/SamuelDebruyn/mugenmvvmtoolkit-dryioc/branch/master) [![NuGet](https://img.shields.io/nuget/v/MugenMvvmToolkit.DryIoc.svg?label=NuGet)](https://www.nuget.org/packages/MugenMvvmToolkit.DryIoc/)

```
Install-Package MugenMvvmToolkit.DryIoc
```

### Parameters

At the moment there are no supported IIocParameters as I haven't needed them myself. Please create an issue or a PR if you're missing functionality.

### Release notes

#### 1.0.1

* [FIXED] Child containers didn't have the parent's registrations
* [FIXED] Resolve unregistered types

#### 1.0.0

* Initial release
