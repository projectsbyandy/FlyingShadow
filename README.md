# Flying Shadow
Stats based battle simulator between Shadows.

- Examples of Results and Railway pattern in the FlyingShadow API and MockDataGenerator.
- Unit, Integration and System testing
- Console App execution on API build
- DTO and Domain object seperation in API layers (controller, service and repo).
- OpenAPI doc generator and Scalar for rendering / testing documents.

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

## Json Consumption
To use the generated data, create an `appsettings.local.json` in the same location as appsettings.json (if not already present) and configure the mock data flag and source. For example

```
  "mockData": {
    "isEnabled": true,
    "source": "json"
  }
```
      