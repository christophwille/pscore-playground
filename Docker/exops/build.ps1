# .\build.ps1 -imageversion v20211115.1
param ([Parameter(Mandatory)]$imageversion)
docker build . -t exops:$imageversion