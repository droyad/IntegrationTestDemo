Param(
    [Parameter(Mandatory=$True)] [string]$tenant,
    [Parameter(Mandatory=$True)] [string]$tenantName
)

$ErrorActionPreference = "Stop"

$stackName = "IntegrationTests-$tenantName"
$completedFormationName = ".\CompletedCloudFormation.json"

if (-not $env:AWS_ACCESS_KEY_ID) {
    throw "AWS_ACCESS_KEY_ID has not been set"
}
if (-not $env:AWS_SECRET_ACCESS_KEY) {
    throw "AWS_SECRET_ACCESS_KEY has not been set"
}

$registerRequest = Get-Content ".\Register.json" | ConvertFrom-Json
$formation = Get-Content ".\CloudFormation.json" | ConvertFrom-Json
$registerRequest.TenantIds = @($tenant)

function GetUserData($roles) {
    $registerRequest.Roles = $roles
    $registerRequestJson = ConvertTo-Json $registerRequest -Depth 100;
    $registerRequestJson = $registerRequestJson.Replace('"', '\"')
    $userData = [String]::Join("`n", (Get-Content ".\UserData.sh"))
    $userData = $userData.Replace("%%registerRequest%%", $registerRequestJson)
    return [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($userData))
}
$formation.Resources.AppServerInstance.Properties.UserData = GetUserData(@("App"))
$formation.Resources.WebServerInstance.Properties.UserData = GetUserData(@("Web", "App"))

ConvertTo-Json $formation -Depth 100 | Set-Content -Path $completedFormationName

function StackExists() {
    $stacks = aws cloudformation list-stacks --region ap-southeast-2 | ConvertFrom-Json
    $matching = $stacks.StackSummaries | Where-Object { $_.StackName -eq $stackName -and $_.StackStatus -ne "DELETE_COMPLETE" }
    return $matching.length -eq $null
}

if (StackExists) {
    "Deleting stack $stackName"
    aws cloudformation delete-stack --stack-name $stackName --region ap-southeast-2
}

while (StackExists) {
    "Waiting for $stackName stack to be deleted"
    start-sleep 1
}

"Deploying stack $stackName"
aws cloudformation deploy --template-file $completedFormationName --stack-name $stackName --region ap-southeast-2
Remove-Item $completedFormationName