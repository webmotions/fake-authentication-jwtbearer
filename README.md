# Fake Authentication Jwt Bearer

This code allow to fake a Jwt Bearer and build integration test for ASP.Net Core application.  
By this way we can fake any authentication we need, without the need to really authenticate a user.  
This code is based on [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/aspnet/Security/tree/dev/src/Microsoft.AspNetCore.Authentication.JwtBearer).

## How to install it?

Let's imagine we are coding integration tests in the project `MyApp.TestsIntegration`.

This is the tree of the global solution:

```bash
+--src
| +---MyApp
| +---SecondApp
+---test
| +---MyApp.Tests
| +---MyApp.TestsIntegration
| \---GST.Fake.Authentication.JwtBearer
```

My integration test are based on this tutorial [Introduction to integration testing with xUnit and TestServer in ASP.NET Core](http://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/).  
So I have a `TestFixture.cs` file where I can extend configurations made in the `Startup.cs` file.

First, Clone this repository in your test folder.

Next add a class called `AddConfiguration` in the root of `MyApp.TestsIntegration`.  
This class will make the link between our application and the Jwt Bearer faker

```C#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using GST.Fake.Builder;

namespace MyApp.TestsIntegration
{
    public class AddConfiguration : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseFakeJwtBearerAuthentication();
                next(builder);
            };
        }
    }
}
```

in the class `TestFixture.cs` we have to extend the configuration of our Startup.

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
          x.AddTransient<IStartupFilter, AddConfiguration>()
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

 ````C#
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
     }
 }
 ```
