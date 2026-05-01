# Flying Shadow
Stats based battle simulator between Shadows.

- Examples of Results and Railway pattern in the FlyingShadow API and MockDataGenerator.
- Unit, Integration and System testing
- Console App execution on API build
- DTO and Domain object seperation in API layers (controller, service and repo).
- OpenAPI doc generator and Scalar for rendering / testing documents.

# Getting Started

1. Navigate to `FlyingShadow.Api` properties folder, create a `launchSettings.json` file with the contents copied from `launchSettings.template.json`

1. By default, Flying Shadow is configured to use mock data. To make this optional and only configured when running the API locally:

 - Create an `appsettings.local.json` in the same location as appsettings.json (if not already present) and configure the mock data flag and source. For example

```
  "mockData": {
    "isEnabled": true,
    "source": "json"
  }
```
- Delete the `mockData` entry from appsettings.json

1. Run the unit and integration tests either via IDE or via commandline (from the directory with the solution file) using `dotnet test`.

## Start the service
#### From the IDE 
launch the profile from 'launchSettings.json' or Select the FlyingShadown.Api https from the project dropdown and click Run

#### From commandline
Navigate to the `FlyingShadow.Api` folder and enter dotnet run.

The Scalar UI can be accessed at `http://localhost:<configured port>scalar/v1`

# Mock Data
## Json Generation
The solution supports the generation of mock data to enable the testing of the Flying Shadow API without having to setup a database.

The mock data generation is handled by the `FlyingShadow.Api.MockDataGenerator` project.
- This will generate the following data in the '_GeneratedData' folder
  - Registered Users from backend
  - Users for auth requests
    - Kept seperate from Registered Users for security
  - A copy of static Shadow characters and stealth metrics.

  Generation is controlled by the *GenerateMockData* property in the `Directory.Build.props` file.

  By default the `FlyingShadow.Api.MockDataGenerator` is executed when the `FlyingShadow.Api` project is built.

  - Mock Data is generated
    - Scenario A - *GenerateMockData* is set to `true` AND the mock files *do not* exist.
      - This will be the case during the first build of the solution.
    - Scenario B - *GenerateMockData* is set to `true` AND *one of* the mock files *do not* exist.
  - Mock Data is not generated
    - Scenario A - *GenerateMockData* is set to `false`
    - Scenario B - *GenerateMockData* is not in the `Directory.Build.props` file
    - Scenario C - `Directory.Build.props` file is missing
    - Scenario D - *GenerateMockData* is set to `true` AND the mock files exist.

# Open API generation
The Flying Shadow open api spec will be generated here `FlyingShadow.Api/OpenApi` in json format.

To view the spec from the Scalar UI browse to `<localhost with port>/openapi/FlyingShadow.Api.json`

## Open API Tests
### Initial Setup
#### Local — scoped to the repo, version-pinned in a manifest
`dotnet new tool-manifest`
`dotnet tool install Microsoft.OpenApi.Kiota`

#### Generate Client and DTOs
A build target in the FlyingShadow.Api.Spec.Test project will generate the assets pre-compilation of the test library. This means any breaking changes to the Open Api spec should reflect in the generatated assets and break consuming tests.

```
dotnet kiota generate \
  -l CSharp \
  -d ./openapi.json \
  -c FlyingShadowClient \
  -n FlyingShadow.Client \
  -o ./_Generated/FlyingShadow.Client
```

### Tests
The tests are built using
- Kiota generated a wrapper client and DTOs
- Microsoft.AspNetCore.Mvc.Testing which spins up the API, creates a httpclient for use in wrapper client.

NOTE - Error status codes throw a ProblemDetails (Kiota Api Exception).

# Troubleshooting
The folder and spec may not regenerate if the build system thinks nothing has changed (normally when the folder has been manaully deleted)

```
dotnet build --no-incremental
```