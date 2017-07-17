Param(
    $tenantName = "MyTenant"
)

$ErrorActionPreference = "Stop"

$stacks = aws cloudformation list-stacks --region ap-southeast-2 | ConvertFrom-Json
$matching = $stacks.StackSummaries | Where-Object { $_.StackName -eq $stackName -and $_.StackStatus -ne "DELETE_COMPLETE" }
if($matching.length -eq $null)
{
    "Deleting stack $stackName"
    aws cloudformation delete-stack --stack-name $stackName --region ap-southeast-2
}