# Build the solution first, then run from the repository root:
# ./tests/PackagedContextMenus.Tests.ps1 -AssemblyPath ./ContextMenuManager/bin/Release/ContextMenuManager.exe
param([Parameter(Mandatory = $true)][string]$AssemblyPath)
$ErrorActionPreference = 'Stop'
$assembly = [Reflection.Assembly]::LoadFile((Resolve-Path -LiteralPath $AssemblyPath).Path)
$helper = $assembly.GetType('ContextMenuManager.Methods.UwpHelper', $true)
$flags = [Reflection.BindingFlags]'Static, NonPublic, Public'
$read = $helper.GetMethod('ReadContextMenus', $flags)
$match = $helper.GetMethod('MatchesScene', $flags)
$script:passed = 0
function Assert($condition, [string]$message) {
    if (!$condition) { throw $message }
    $script:passed++
}

# Use a synthetic package identity and CLSIDs: discovery must not depend on a dictionary.
[xml]$manifest = @'
<Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
 xmlns:menu="http://schemas.microsoft.com/appx/manifest/desktop/windows10/4"
 xmlns:verb="http://schemas.microsoft.com/appx/manifest/desktop/windows10/5"
 xmlns:com="http://schemas.microsoft.com/appx/manifest/com/windows10">
 <Applications><Application><Extensions>
  <com:Extension Category="windows.comServer"><com:ComServer><com:SurrogateServer>
   <com:Class Id="11111111-1111-1111-1111-111111111111"/>
  </com:SurrogateServer></com:ComServer></com:Extension>
  <menu:Extension Category="windows.fileExplorerContextMenus"><menu:FileExplorerContextMenus>
   <verb:ItemType Type="Directory">
    <verb:Verb Id="OpenProject" Clsid="{22222222-2222-2222-2222-222222222222}"/>
    <verb:Verb Id="Invalid" Clsid="not-a-guid"/>
    <verb:Verb Id="Empty" Clsid="00000000-0000-0000-0000-000000000000"/>
   </verb:ItemType>
   <menu:ItemType Type="Directory\Background"><menu:Verb Id="Background" Clsid="33333333-3333-3333-3333-333333333333"/></menu:ItemType>
   <verb:ItemType Type="*"><verb:Verb Id="File" Clsid="44444444-4444-4444-4444-444444444444"/></verb:ItemType>
   <verb:ItemType><verb:Verb Id="MissingType" Clsid="55555555-5555-5555-5555-555555555555"/></verb:ItemType>
  </menu:FileExplorerContextMenus></menu:Extension>
 </Extensions></Application></Applications>
</Package>
'@
$package = 'Example.Project_1.2.3.4_x64__publisher'
$items = $read.Invoke($null, @($manifest, $package))
Assert ($items.Count -eq 3) 'Must include desktop4/5 verbs and exclude unrelated/invalid classes.'
Assert ($items[0].Text -eq 'Example.Project (OpenProject)') 'Unknown packages need a readable fallback label.'
Assert ($items[0].PackageName -eq $package) 'Keep the exact package version.'
Assert ($items[0].RegPath -eq "HKEY_CLASSES_ROOT\PackagedCom\Package\$package\Class\{22222222-2222-2222-2222-222222222222}") 'Registry location must identify the declared class.'
Assert ($items[1].ItemType -eq 'Directory\Background') 'Keep the declared menu scene.'
Assert ($match.Invoke($null, @('Directory', 'HKEY_CLASSES_ROOT\Directory'))) 'Discover project menus on selected directories.'
Assert (!$match.Invoke($null, @('Directory', 'HKEY_CLASSES_ROOT\Directory\Background'))) 'Do not show directory verbs on background.'
Assert (!$match.Invoke($null, @('Directory', 'HKEY_CLASSES_ROOT\*'))) 'Do not show directory verbs on files.'
Assert ($match.Invoke($null, @('directory', 'HKEY_CLASSES_ROOT\Directory'))) 'Match scene names without case sensitivity.'
Assert ($match.Invoke($null, @('.txt', 'HKEY_CLASSES_ROOT\SystemFileAssociations\.txt'))) 'Match file extensions.'
Assert (!$match.Invoke($null, @('*', $null))) 'Ignore unsupported/null scenes.'
Assert (!$match.Invoke($null, @('*', 'HKEY_CURRENT_USER\*'))) 'Do not misinterpret arbitrary registry roots.'
[xml]$emptyManifest = '<Package/>'
Assert ($read.Invoke($null, @($emptyManifest, $package)).Count -eq 0) 'Packages without menus must be ignored.'

# This performs read-only discovery; it neither starts the application nor changes registry values.
$live = $helper.GetMethod('GetContextMenus', $flags).Invoke($null, @())
foreach ($item in $live) {
    Assert (Test-Path -LiteralPath "Registry::$($item.RegPath)") 'Only expose registered COM classes.'
}
Write-Output "PASS: $script:passed assertions; $($live.Count) installed menu registrations discovered."
$live | Where-Object ItemType -eq 'Directory' | Select-Object PackageName, ItemType, Text | Format-List
