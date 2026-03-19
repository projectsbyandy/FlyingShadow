# Flying Shadow


# Mock Data Generation
The solution supports the generation of mock data to enable the testing of the Flying Shadow API without having to setup a database.

The mock data generation is handled by the `FlyingShadow.Api.MockDataGenerator` project.
- This will generate the following data in the '_GeneratedData' folder
  - Registered Users from backend
  - Users for auth requests
    - Kept seperate from Registered Users for security
  - Shadow characters and skills

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
      