# Create an scaffolding directory and projects for one module.

param (
    # 
    [string]$definitionFile = 'ScaffoldingFramework', #prefixing with "[string]" means that is a cast
    [string]$rootNamespace = "i2", # inflexion 2
    [string]$product = "Framework", # Technical Framework
    [string]$TestFramework = "xunit"
)

$testProjectName = ".Test"

# -------------------------------------------------------------------------------------------------------------------------------------------------------
# Functions
# -------------------------------------------------------------------------------------------------------------------------------------------------------

function Get-FolderName($prefix, $namespace, $name){
  return $prefix + "." + $namespace + "." + $name
}


# Función para eliminar los archivos especificados después de crear cada proyecto
function Remove-Files($projectFolderPath, $filesToRemove) {
    foreach ($fileToRemove in $filesToRemove) {
        $filePath = Join-Path -Path $projectFolderPath -ChildPath $fileToRemove.Name
        if (Test-Path -Path $filePath) {
            Remove-Item -Path $filePath -Force
        }
    }
}

function Add-TestProject($layer, $layerFolderPath, $rootProjectNamespace) {
   
    $testProjectFolderName = $layerFolderPath + $testProjectName
    $testProjectReferenceName = $rootProjectNamespace + "." + $layer.name + $testProjectName
    $fullTestProjectPathName = Join-Path $testProjectFolderName $testProjectReferenceName

    if (!(Test-Path -Path $testProjectFolderName)) {
        md $testProjectFolderName
    }

    cd $testProjectFolderName

    if (!(Test-Path -Path ($fullTestProjectPathName + $projectExtension))) {
        Write-Host "Create test project '" $fullTestProjectPathName "' inside the folder '" $testProjectFolderName
        dotnet new $TestFramework --language "C#" --name $testProjectReferenceName --output $testProjectFolderName
    }

    Remove-Files $testProjectFolderName $filesToRemove

    cd $SolutionsFolder

    # agregar el proyecto de pruebas a la solución
    if ((Test-Path -Path $testProjectFolderName) -and (Test-Path -Path $SolutionFile)) {
        Write-Host "Add test project '" ($fullTestProjectPathName + $projectExtension) "' inside the solution folder '" $solutionFolderLayer "'" "of solution '"  $SolutionFile "'"
        dotnet sln $SolutionFile add ($fullTestProjectPathName + $projectExtension) --solution-folder $solutionFolderLayer
    }
}

# add all the layer's proyect as references to the layer test proyect.   
function Add-TestProjectReferences($layer, $layerFolderPath, $rootProjectNamespace) {
    
    $testProjectFolderName = $layerFolderPath + $testProjectName
    $testProjectReferenceName = $rootProjectNamespace + "." + $layer.name + $testProjectName
    $fullTestProjectPathName = Join-Path $testProjectFolderName $testProjectReferenceName

    foreach ($project in $layer.Projects.project) {
        $projectReferenceName = $rootProjectNamespace + "." + $layer.name + "." + $project.name
        $referencedProjectFile = Join-Path ($layerFolderPath + "." + $project.name) ($projectReferenceName + $projectExtension)

        if (!(Test-Path -Path $referencedProjectFile)) {
            Write-Host "Referenced project not found: $referencedProjectFile"
            continue
        }

        dotnet add ($fullTestProjectPathName + $projectExtension) reference $referencedProjectFile
    }
}

#function Add-ProjectReferences($projectFolderPath, $references) {
#    foreach ($reference in $references) {
#        if ($reference.type -eq "internal") {
#            $referencedProjectFile = Join-Path -Path $projectFolderPath -ChildPath ($reference.from + "." + $reference.name + $projectExtension)
#            if (Test-Path -Path $referencedProjectFile) {
#                dotnet add ($projectFolderPath + $projectExtension) reference $referencedProjectFile
#            } else {
#                Write-Host "Referenced project not found: $referencedProjectFile"
#            }
#        } elseif ($reference.type -eq "nuget") {
#            $nugetReference = '<PackageReference Include="' + $reference.name + '" Version="' + $reference.minversion + '" />'
#            $projectFilePath = Join-Path -Path $projectFolderPath -ChildPath ($reference.name + $projectExtension)
#            Add-Content -Path $projectFilePath -Value $nugetReference
#        }
#    }
#}


# -------------------------------------------------------------------------------------------------------------------------------------------------------
# End Functions
# -------------------------------------------------------------------------------------------------------------------------------------------------------

#cd 'C:\git\i3\080-Scripting'

$currentLocation = Get-Location

# constants for extensión and generated classes to remove
$currentfolder       	= '.\'
$projectExtension    	= '.csproj'
$solutionExtension      = '.sln'
$xmlExtension        	= '.xml'
$jsonExtension			= '.json'
$yamlExtension			= '.yml'
$cs						= ".cs"
$folderPrefix        	= ".-"

$program  				= "program"
$class 					= "class"

#Children folders
$AssembliesFolder    	= Join-Path $currentLocation  '\000-Build\'
$ScriptsFolder       	= Join-Path $currentLocation  '\000-Scripts\'
$DocumentationFolder 	= Join-Path $currentLocation  '\010-Doc\'

#Parent folders
$Wiki					= join-Path $currentLocation  '.\010-Documentation'
$HorizontalFwkFolder 	= Join-Path $currentLocation  '.\020-HorizontalFwk\'
$VerticalFwkFolder 	 	= Join-Path $currentLocation  '.\030-VerticalFwks\'
$LibrariesFolder  		= Join-Path $currentLocation  '.\040-Libraries\'
$Appsfolder        		= Join-Path $currentLocation  '.\050-Apps\'

$ModulesFolder       	= Join-Path $currentLocation  '.\090-Modules\'
$SolutionsFolder     	= Join-Path $currentLocation  '.\099-Solutions\'                      

$SolutionsFolder        = $currentLocation                     

if (!$definitionFile ){ # [string]::IsNullOrEmpty(...)
   $definitionFile = 'ScaffoldingFramework.xml'
}


if (!$definitionFile.EndsWith($xmlExtension)){
   $definitionFile = $definitionFile + $xmlExtension
}

#Register-PackageSource -Name "NuGet.org" -Location "https://api.nuget.org/v3/index.json" -ProviderName "NuGet"
Install-PackageProvider -Name NuGet


$InputXmlFilePath     = Join-Path $currentLocation  $definitionFile #'..\070-Definitions\ModuleScaffolding.xml'
$SolutionFile         = $rootNamespace + $solutionExtension
$rootProjectNamespace = $rootNamespace +"."+ $product

#Read the definition of the structure projects fromthe xml file
#
$XmlDefinition = New-Object -TypeName XML
Write-Host "input xml path" $InputXmlFilePath
$XmlDefinition.Load($InputXmlFilePath)
#Write-Host $xmlDefinition.InnerXml

# create solution folder if it dosen't exist. 
#
$solutionPath = Join-Path -Path $SolutionsFolder $SolutionFile
if (!(Test-Path -Path $SolutionsFolder)) { #test if the solution folder exist
    md $SolutionsFolder
}

# create the solution file if it dosen't exist. In this way we can add to the sln file the projects in the first iteration of the layers definition
#
cd $SolutionsFolder
if (!(Test-Path -Path $SolutionFile)) { #test if the sln file exist
    dotnet new sln -n $rootNamespace
}
cd $currentLocation

#first iteration over layers to create the folders & projects of the solution 
#
$layers = $XmlDefinition.Solution.Layers.layer
$filesToRemove = $XmlDefinition.Solution.FilesToRemoveFromProjects.file

# create the project for every single layer
#
foreach($layer in $layers){
    $layerfolder = $layer.folderPrefix + $folderPrefix  + $rootProjectNamespace + "." + $layer.name# Get-FolderName($layer.folderPrefix, $rootProjectNamespace, $layer.name) 
    $solutionFolderLayer = $layer.folderPrefix + " " + $layer.name
    Write-Host "Creating projects for layer '" $layer.name "'"
    $pathFolder = Join-Path $currentLocation $layerfolder

    foreach($project in $layer.Projects.project){

        $projectfolderName     = $pathFolder + "." + $project.name
        $referenceProjectName  = $rootProjectNamespace +"." + $layer.name +"." + $project.name 
        $fullProjectPathName   = Join-Path $projectfolderName $referenceProjectName
        
        if (!(Test-Path -Path $projectfolderName )) { #test if the project file exist
            md $projectfolderName
        }

        cd $projectfolderName 
          
        if (!(Test-Path -Path  ($fullProjectPathName + $projectExtension))) { 
            Write-Host "Create project '" $fullProjectPathName "' inside the folder '" $projectfolderName 
            dotnet new $project.template --language "C#" --name $referenceProjectName  --output '..\..\Build\$(Configuration)\'
        }

        Remove-Files $projectfolderName $filesToRemove # remove the default files from the templates

        cd $SolutionsFolder

        #add the project to the solution
        if ((Test-Path -Path $projectfolderName) -and (Test-Path -Path $SolutionFile)) { 
            Write-Host "Add project '" ($fullProjectPathName + $projectExtension) "' inside the solution folder '" $solutionFolderLayer "'" "of solution '"  $SolutionFile "'"
            dotnet sln $SolutionFile add ($fullProjectPathName + $projectExtension) --solution-folder $solutionFolderLayer
        }
    }

    Add-TestProject $layer $pathFolder $rootProjectNamespace

    Write-Host 
}

# create the references for every single project
foreach($layer in $layers){
    $layerfolder = $layerfolder = $layer.folderPrefix + $folderPrefix + $rootProjectNamespace + "." + $layer.name # Get-FolderName($layer.folderPrefix, $rootProjectNamespace, $layer.name) 
    Write-Host "Adding references to the projects for layer '" $layer.name "'"
    $pathFolder = Join-Path $currentLocation $layerfolder

    foreach($project in $layer.Projects.project){

        $projectfolderName     = $pathFolder + "." + $project.name
        $referenceProjectName  = $rootProjectNamespace +"." + $layer.name +"." + $project.name 
        $fullProjectPathName   = Join-Path $projectfolderName $referenceProjectName
        
        cd $projectfolderName 

        $referenceArray = dotnet list $projectPath reference
        $chekReferences = ($referenceArray.GetType().Name -ne 'String') -or ($referenceArray[0] -eq '  List all project-to-project references of the project.')

        if ($chekReferences){
            for($i = 0; $i -lt $referenceArray.Count; $i++){
                $referenceArray[$i] = [System.IO.Path]::GetFileName($referenceArray[$i])
            }
        }

        foreach($projectReference in $project.References.reference){
            $referencedLayer = $layers | Where-Object -property name -eq $projectReference.from
            $referencedLayerfolder = $referencedLayer.folderPrefix + $folderPrefix + $rootProjectNamespace + "." + $referencedLayer.name + "." + $projectReference.name
            $referencedPathFolder  = Join-Path $currentLocation $referencedLayerfolder
            
            $referencedProjectName  = $rootProjectNamespace +"." + $projectReference.from + "." + $projectReference.name + $projectExtension 
            $referencedFile = Join-Path $referencedPathFolder $referencedProjectName

            if ($projectReference.type -eq "internal") {
                    if ($chekReferences -or !($referenceArray.Contains([System.IO.Path]::GetFileName($referencedFile)))){
                        dotnet add ($fullProjectPathName + $projectExtension) reference $referencedFile
                }
            } elseif ($projectReference.type -eq "nuget") {
                dotnet add package $projectReference.name --version $projectReference.minversion
                #Install-Package -Name $projectReference.name -MinimumVersion $projectReference.minversion -Force
                #$projectFolderPath
                #$nugetReference = '<PackageReference Include="' + $reference.name + '" Version="' + $reference.minversion + '" />'
                #$projectFilePath = Join-Path -Path $projectFolderPath -ChildPath ($projectReference.name + $projectExtension)
                #Add-Content -Path $projectFilePath -Value $nugetReference
            }
        }
    }

    Add-TestProjectReferences $layer $pathFolder $rootProjectNamespace

    Write-Host 
}

cd $currentLocation




#dotnet sln o3.sln add ..\020-TchFwk\04.Domain\O3.TchFwk.Domain.csproj --solution-folder 04-Domain 

#end of script restore location
cd $currentLocation


