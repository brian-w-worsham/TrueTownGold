<#
.SYNOPSIS
    Build and deploy TrueTownGold mod to Bannerlord Modules folder.
.DESCRIPTION
    Builds the project and copies all module files to the game's Modules directory.
.PARAMETER GameFolder
    Path to your Bannerlord installation. Defaults to Steam's default location.
.PARAMETER Configuration
    Build configuration. Default: Release.
#>
param(
    [string]$GameFolder = "C:\Games\steamapps\common\Mount & Blade II Bannerlord",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$ModuleName = "TrueTownGold"
$TargetModuleDir = Join-Path $GameFolder "Modules\$ModuleName"

function Test-IsLegacyDefaultSettingsFile {
    param(
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        return $false
    }

    try {
        [xml]$settings = Get-Content $Path
        [double]$multiplier = [double]::Parse(
            $settings.TrueTownGoldSettings.GlobalTownGoldMultiplier,
            [System.Globalization.CultureInfo]::InvariantCulture)
        [int]$minimumTownGold = [int]::Parse(
            $settings.TrueTownGoldSettings.MinimumTownGold,
            [System.Globalization.CultureInfo]::InvariantCulture)
        [int]$maximumTownGold = [int]::Parse(
            $settings.TrueTownGoldSettings.MaximumTownGold,
            [System.Globalization.CultureInfo]::InvariantCulture)

        return $multiplier -eq 2.0 -and $minimumTownGold -eq 15000 -and $maximumTownGold -eq 500000
    }
    catch {
        return $false
    }
}

Write-Host "=== Building $ModuleName ===" -ForegroundColor Cyan

dotnet build "src\TrueTownGold\TrueTownGold.csproj" `
    -c $Configuration `
    -p:GameFolder="$GameFolder"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

Write-Host "=== Deploying to $TargetModuleDir ===" -ForegroundColor Cyan

$binDir = Join-Path $TargetModuleDir "bin\Win64_Shipping_Client"

New-Item -ItemType Directory -Path $binDir -Force | Out-Null

Copy-Item "Module\SubModule.xml" -Destination $TargetModuleDir -Force

$settingsFile = "Module\TrueTownGold.settings.xml"
$targetSettingsFile = Join-Path $TargetModuleDir "TrueTownGold.settings.xml"
if (Test-Path $settingsFile) {
    if (Test-Path $targetSettingsFile) {
        if (Test-IsLegacyDefaultSettingsFile -Path $targetSettingsFile) {
            Copy-Item $settingsFile -Destination $targetSettingsFile -Force
            Write-Host "Upgraded legacy TrueTownGold settings to the current recommended defaults." -ForegroundColor Yellow
        }
        else {
            try {
                [xml]$templateSettings = Get-Content $settingsFile
                [xml]$existingSettings = Get-Content $targetSettingsFile

                foreach ($templateNode in $templateSettings.DocumentElement.ChildNodes) {
                    if ($templateNode.NodeType -ne [System.Xml.XmlNodeType]::Element) {
                        continue
                    }

                    if (-not $existingSettings.DocumentElement.SelectSingleNode($templateNode.Name)) {
                        $importedNode = $existingSettings.ImportNode($templateNode, $true)
                        [void]$existingSettings.DocumentElement.AppendChild($importedNode)
                    }
                }

                $existingSettings.Save($targetSettingsFile)
            }
            catch {
                Write-Warning "Could not merge missing settings keys into $targetSettingsFile. Existing file left unchanged."
            }
        }
    }
    else {
        Copy-Item $settingsFile -Destination $targetSettingsFile -Force
    }
}

$builtDll = "src\TrueTownGold\bin\$Configuration\net472\TrueTownGold.dll"
if (Test-Path $builtDll) {
    Copy-Item $builtDll -Destination $binDir -Force
}

$harmonyDll = "src\TrueTownGold\bin\$Configuration\net472\0Harmony.dll"
if (Test-Path $harmonyDll) {
    Copy-Item $harmonyDll -Destination $binDir -Force
}

Write-Host "=== Done! ===" -ForegroundColor Green
Write-Host "Module deployed to: $TargetModuleDir"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Launch Bannerlord"
Write-Host "  2. Go to Mods and enable 'True Town Gold'"
Write-Host "  3. Start or load a campaign"
Write-Host "  4. Edit Modules\TrueTownGold\TrueTownGold.settings.xml if you want a different multiplier"
Write-Host "  5. Visit a high-prosperity town and confirm merchants can afford larger sales"