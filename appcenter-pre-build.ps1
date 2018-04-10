$version = $env:BUILD_VERSION + $env:APPCENTER_BUILD_ID;

$path = ".\src\Sannel.House.BackgroundTasks.SensorCapture\Package.appxmanifest"
[xml]$xml = Get-Content $path
$xml.Package.Identity.Version = $version + ".0"
$xml.Save($path);