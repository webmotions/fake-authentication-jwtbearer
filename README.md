# Fake Authentication Jwt Bearer for ASP.NET Core 2

This code allow to fake a Jwt Bearer and build integration test for ASP.Net Core application.  
By this way we can fake any authentication we need, without the need to really authenticate a user.  
This code is based on [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/aspnet/Security/tree/dev/src/Microsoft.AspNetCore.Authentication.JwtBearer).

 > If You need it for ASP.NET Core 1, check [Tag 1.0.4](https://github.com/GestionSystemesTelecom/fake-authentication-jwtbearer/tree/1.0.4)

## How to install it?

First add this package to your Nuget configuration file : [GST.Fake.Authentication.JwtBearer](https://www.nuget.org/packages/GST.Fake.Authentication.JwtBearer).

Let's imagine we are coding integration tests in the project `MyApp.TestsIntegration`.

This is the tree of the global solution:

```bash
+--src
| +---MyApp
| +---SecondApp
+---test
| +---MyApp.Tests
| +---MyApp.TestsIntegration
```

My integration test are based on this tutorial [Introduction to integration testing with xUnit and TestServer in ASP.NET Core](http://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/).  
So I have a `TestFixture.cs` file where I can extend configurations made in the `Startup.cs` file.

In the class `TestFixture.cs` we have to extend the configuration of our Startup.  
You have to disable you original `AddJwtBearer` in your `Startup.cs`, because the `AddFakeJwtBearer` doesno't overload the original.

```C#
public class TestFixture<TStartup> : IDisposable where TStartup : class
{
    public TestFixture()
    {
    // ...

    // We must configure the realpath of the targeted project
    string appRootPath = Path.GetFullPath(Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath
                    , "..", "..", "..", "..", "..", "..", "src", baseNamespace));

    var builder = new WebHostBuilder()
      .UseContentRoot(appRootPath)
      .UseStartup<TStartup>()
      .UseEnvironment("Test")
      .ConfigureServices(x =>
      {
          // Here we add our new configuration
          x.AddAuthentication()
		  .AddFakeJwtBearer()
          ;
      });
      // ...
    }
}
```

## How to use it?

Now all the things are tied up, how to faked a user?

I've defined tree methods :
 - A token with a custom object
 - A token with a Username
 - A token with a Username and some roles

 Let see that in a real world example.

```C#
 using GST.Fake.Authentication.JwtBearer;
 using Newtonsoft.Json;
 using Newtonsoft.Json.Linq;
 using System;
 using System.Collections.Generic;
 using System.Net.Http;
 using System.Text;
 using Xunit;

 namespace MyApp.TestsIntegration
 {
     public class SomeWeirdTest : IClassFixture<TestFixture<MyApp.Startup>>
     {
         private TestFixture<MyApp.Startup> fixture;

         public SomeWeirdTest(TestFixtureMyApp.Startup> _fixture)
         {
             fixture = _fixture;
             // Create a token with a Username and two roles
             fixture.Client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
         }

         [Fact]
         public void testCallPrivateAPI()
         {
             // We call a private API with a full authenticated user (admin)
             var response = fixture.Client.GetAsync("/api/my-account").Result;
             Assert.True(response.IsSuccessStatusCode);
         }

		 
         [Fact]
         public void testCallPrivate2API()
         {
		 dynamic data = new System.Dynamic.ExpandoObject();
            data.organism = "ACME";
            data.thing = "more things";
            fixture.Client.SetFakeBearerToken("SUperUserName", new[] { "Role1", "Role2" }, (object)data);

            // We call a private API with a full authenticated user (admin)
            var response = fixture.Client.GetAsync("/api/my-account").Result;
            Assert.True(response.IsSuccessStatusCode);
         }
     }
 }
```

# Create Nuget Package

```bash
dotnet build src/GST.Fake.Authentication.JwtBearer/GST.Fake.Authentication.JwtBearer.csproj --configuration Release --framework netcoreapp2.0 --force
dotnet pack src/GST.Fake.Authentication.JwtBearer/GST.Fake.Authentication.JwtBearer.csproj --configuration Release --include-source --include-symbols --output ../../nupkgs
dotnet nuget push src/Auth.DTO/bin/Release/Auth.DTO.[VERSION].nupkg
```