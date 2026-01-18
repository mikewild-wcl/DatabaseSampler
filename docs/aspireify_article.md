# Aspireifying on old .NET application

My DatabaseSampler project is something I wrote a few years ago to learn about data access in SQL Server, Postgres, Cosmos DB as well as how to use Redis for caching. Over the years I've upgraded it to new .NET versions, but now it's time to modernize it again.

Aspire has changed the game for .NET applications that need to work with multiple databases. It's now easy to set up a host application that spins up SQL Server and Postgres databases in local containers, start an emulator for Cosmos DB. So let's [aspireify](https://aspireify.net/)!

## Update to .slnx
I'll change the .sln file to the new XML format .slnx. Right click on the soltion in Visual Studio, Save as... and select Save as type XML Solution File. Close Visual Studio and repoen from the new `.slnx` file. The old `.sln` file can be deleted.

![Image - save as .slnx.](./images/save_as_slnx.png)

## Update to .NET 10

First I need to update the .NET version - it's already on 9.0 so changing it to 10.0 will be easy. I previously moved everything to file-scoped namespaces. This time I'm going to 

- centrally managed nuget
- Directory.Build.props exists but I'm going to add
  - Sonar
  - etc
- .editorconfig
- Copilot instructions!

Add new `.editorconfig file`. Visual Studio will do that for us; just right click on the solotion and select Add new EditorConfig from the menu.

### Central package management

Instead of every project having it's own versions of nuget packages, it's a good practive to move all versioning to a single file. This can be a bit labour-intensive so ket's just ask Copilot (other coding assistants are available!) to do it in Agent Mode:
```
Change this solution to use centrally managed nuget packages. Add a file `Directory.Packages.props` to the repo root folder with a property group that sets ManagePackageVersionsCentrally to true. Add all nuget packages used in the solution and update all .csproj files to use those versions.
```

If you want to do it by hand, add the `Directory.Packages.props` file like this:
```
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Azure.Identity" Version="1.13.2" />
    ...other ppackages...
  </ItemGroup>
</Project>
```

And in all nuget PackageReferences in project files just remove the version, like this:
```
  <PackageReference Include="Azure.Identity" />
```

Sometimes Visual Studio builds fail after this with errors saying "PackageReference items do not define a corresponding PackageVersion item" - if this hapopens just clean and rebuild, or as a last resort clean, close and reopen Visual Studio, and rebuild. If it still happens you might have missed one of the packages so you'll need to look at the detail.


### Directory.build.props

Add a nuget reference to `SonarAnalyzer.Csharp` by adding the following (using a valid recent version) to `Directory.Packages.props`:
```
  <PackageVersion Include="SonarAnalyzer.CSharp" Version="10.18.0.131500" />
```

Update the contents `Directory.build.props` to
```
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AnalysisLevel>Latest</AnalysisLevel>
    <AnalysisMode>All</AnalysisMode>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference
      Include="SonarAnalyzer.CSharp"
      PrivateAssets="all"
      Condition="$(MSBuildProjectExtension) == '.csproj'" />
  </ItemGroup>

</Project>
```

Note that TreatWarningsAsErrors and EnforceCodeStyleInBuild are all false - I'll change them to tru later but there could be lots of build errors so let's start with false.

These lines can be removed from all `.csproj` files, since they will now be taken from `Directory.build.props` any any now-empty `<PropertyGroup>` sections removed:
```
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
```

## Upgrade to .NET 10 and upgrade nuget packages

Now we only need to change one place to upgrade to .net 10 - the FrameworkVersion in `Directory.build.props`. Change it to 
```
    <TargetFramework>net10.0</TargetFramework>
```

Now the solution won't build. This is the error:
```
Invalid combination of TargetFramework and AzureFunctionsVersion is set.
```
I double clicked on it and found the file it was coming from, which is a targets file in C:\Users\<user>\.nuget\packages\microsoft.azure.functions.worker.sdk\2.0.2\build. We're going to update to a newer version so hopefully the error will go away.

At this point we can update all nuget packages. I'm using Visual Studio so I just go to the manage nuget page and select all to update.If FluentAssertions appears don't update it because we're going to replace it soon.

Now it builds and the test all pass. There are a lot of build warnings but those can be cleaned up later.

## Tests

There aren't many tests so this won't take long. First I want to replace FluentAssertions with Shouldly, for reasons. 

After adding nuget references to Shouldly and removing FluentAssertions, the changes are mostly just things like

 - result.**Should().NotBeNull()** --> result.**ShouldNotBeNull**();
 - result.Latitude.**Should().Be**(51.50354); -> result.Latitude.**ShouldBe**(51.50354);

I tasked Copilot with doing this in agent mode, and it took ages and messed up the central package management, although it did ask if I wanted it to fix that at the end. But then it made things worse and I had to intervene...I probably need to use a higher model or add more instruction. I'll deal with that in the next section, but to be honest this would have been faster by hand than by agent.

I also want to upgrade to xUnit.net v3 - 
- install nuget package xunit.v3
- uninstall xunit
Add this to the test projects <PropertGroup>:
```
  <OutputType>Exe</OutputType>
```

In the tests I now have the following for Global usings
```
<ItemGroup>
  <Using Include="Shouldly" />
  <Using Include="Xunit" />
</ItemGroup>```

Any using statements in tests that references those can be removed.

I also added a NoWarn to the test projects to supress warnings about underscores in names.
```
<PropertyGroup>
  <!-- Allow underscores in test names -->
  <NoWarn>CA1707;</NoWarn>
</PropertyGroup>
```

All the tests run correctly so it's time to move to the next step.

## Copiiot

Since I'm using Copilot a lot thse days, I asked it to create a copilot-instructions file for me. That has an overview of the project as it is now, and some simple 

- DO NOT add using statements when the project .csproj file as a <Using include="" /> for that namespace.
- 

## Adding Aspire AppHost and ServiceDefaults


## Adding databases

## Redis

In the old version, I had a docker file to bring up Redis but it was a manaul process. Now all we need to do is include Redis in the AppHost:
```
```

The `containers` folder and any docker files can be removed.

## TODO 

- Set *TreatWarningsAsErrors and EnforceCodeStyleInBuild to true and fix errors.
- this was added by copilot in Directory.Build.props - should I put it back:
```
  <AzureCosmosDisableNewtonsoftJsonCheck>true</AzureCosmosDisableNewtonsoftJsonCheck>
```
See [Missing Newtonsoft.Json Reference](https://learn.microsoft.com/en-us/azure/cosmos-db/best-practice-dotnet#missing-newtonsoftjson-reference)
  > per Copilot "Added AzureCosmosDisableNewtonsoftJsonCheck to bypass the Azure Cosmos SDK's Newtonsoft.Json check (so build succeeds without explicitly adding Newtonsoft.Json)"

## Deploying to Azure

- Parameter to say we are running locally? Or just use IsDevelopment
- IsPublishing? What does that do?



## Links

 - Aspireify DatabaseSampler - emulators for now, next step - deploy to Azure
	[aspireify.NET](https://aspireify.net/)
	https://aspireify.net/a/241216/add-sql-server-database-with-seed-data-to-.net-aspire-during-app-startup
	https://aspireify.net/a/250709/how-to-have-gitlab-cicd-for-a-.net-aspire-project
