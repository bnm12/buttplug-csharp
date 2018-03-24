
$docfxVersion = "2.33.0"

# Install docfx
& nuget install docfx.console -Version $docfxVersion -Source https://api.nuget.org/v3/index.json

copy-item ..\README.md index.md
copy-item ..\*.md articles

& .\docfx.console.$docfxVersion\tools\docfx
& .\docfx.console.$docfxVersion\tools\docfx --pdf