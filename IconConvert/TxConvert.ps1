$SourcePath = "D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Video\Textures\Icons"
$DestPath = "C:\work\GraphicsConvert\Out"

$Files = dir -Recurse "$SourcePath\*.dds"

$nsp = [System.IO.Path]

#$Files | Foreach-Object -ThrottleLimit 5 -Parallel {
$Files | %{
    $sourceFile = $_
    
    $DestDirectory =$nsp::Join($DestPath, $nsp::GetRelativePath($SourcePath, $sourceFile.DirectoryName))
#    $DestFile = [System.IO.Path]::Join($Destdirectory,$sourceFile.Name)

    if([System.IO.Directory]::Exists($DestDirectory) -eq $false)
    {
        mkdir $DestDirectory
    }

    .\texconv.exe -nologo -ft png -o "$DestDirectory" "$sourceFile"
} | Tee-Object Log.txt

