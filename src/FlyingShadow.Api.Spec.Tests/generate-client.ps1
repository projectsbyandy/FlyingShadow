$ErrorActionPreference = "Stop"

$openApiPath = if ($IsWindows) {
    "..\FlyingShadow.Api\OpenApi\FlyingShadow.Api.json"
} else {
    "../FlyingShadow.Api/OpenApi/FlyingShadow.Api.json"
}

dotnet kiota generate `
  -l CSharp `
  -d $openApiPath `
  -c FlyingDaggersClient `
  -n FlyingDaggers.Client `
  -o ./_Generated/FlyingDaggers.Client
