Param(
    [Parameter(Mandatory=$True)] [string]$stackName
)

$ErrorActionPreference = "Stop"

$stackName = "IntegrationTests-$stackName"

if (-not $env:AWS_ACCESS_KEY_ID) {
    $env:AWS_ACCESS_KEY_ID = $OctopusParameters["AWS_ACCESS_KEY_ID"]
}
if (-not $env:AWS_SECRET_ACCESS_KEY) {
    $env:AWS_SECRET_ACCESS_KEY = $OctopusParameters["AWS_SECRET_ACCESS_KEY"]
}

$stacks = aws cloudformation list-stacks --region ap-southeast-2 | ConvertFrom-Json
$matching = $stacks.StackSummaries | Where-Object { $_.StackName -eq $stackName -and $_.StackStatus -ne "DELETE_COMPLETE" }
if($matching -eq $null)
{
    "Deleting stack $stackName"
    aws cloudformation delete-stack --stack-name $stackName --region ap-southeast-2
}