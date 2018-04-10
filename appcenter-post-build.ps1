$version = $env:BUILD_VERSION + $env:APPCENTER_BUILD_ID;
if($env:APPCENTER_BRANCH -ne "master")
{
	$version = $version + "-alpha";
}
$path = ".\nuspec\Sannel.House.SensorCaptureSDK.nuspec"
[xml]$xml = Get-Content $path
$xml.package.metadata.version = $version
$xml.Save($path);

nuget pack $path
nuget setapikey $env:MYGET_APIKEY -Source "https://www.myget.org/F/sannel-house/api/v3/index.json"
$package = ".\Sannel.House.SensorCaptureSDK." + $version + ".nupkg"
nuget push $package -Source "https://www.myget.org/F/sannel-house/api/v3/index.json"