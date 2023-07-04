# Create an scaffolding directory and projects for one module.

param (
    # 
    [string]$definitionFile = 'ModuleScaffolding', #prefixing with "[string]" means that is a cast
    [string]$rootNamespace = "i2", # inflexion 2
    [string]$product = "TF" # Technical Framework
)


# -------------------------------------------------------------------------------------------------------------------------------------------------------
# Functions
# -------------------------------------------------------------------------------------------------------------------------------------------------------

function Get-FolderName($prefix, $namespace, $name){
  return $prefix + "." + $namespace + "." + $name
}


# -------------------------------------------------------------------------------------------------------------------------------------------------------
# End Functions
# -------------------------------------------------------------------------------------------------------------------------------------------------------

#cd 'C:\git\i3\080-Scripting'

$currentLocation = Get-Location

$currentfolder       = '.\'
$projectExtension    = '.csproj'
$xmlExtension        = '.xml'
$folderPrefix        = ".- "

$AssembliesFolder    = Join-Path $currentLocation  '.\000-Assemblies\'
$DocumentationFolder = Join-Path $currentLocation  '.\010-Documentation\'
$TchFwkFolder        = Join-Path $currentLocation  '.\020-TchFwk\'
$3rdLibrariesFolder  = Join-Path $currentLocation  '.\030-3rdLibraries\'
$SPFolder            = Join-Path $currentLocation  '.\040-SP\'
$SSFolder            = Join-Path $currentLocation  '.\050-SS\'
$KernelFolder        = Join-Path $currentLocation    '.\060-Kernel\'
$DefinitionsFolder   = Join-Path $currentLocation  '.\070-Definitions\'
$ScriptingFolder     = Join-Path $currentLocation  '.\080-Scripting\'
$ModulesFolder       = Join-Path $currentLocation  '.\090-Modules\'
$SolutionsFolder     = Join-Path $currentLocation  '.\099-Solutions\'                      

if (!$definitionFile ){ # [string]::IsNullOrEmpty(...)
   $definitionFile = 'ModuleScaffolding.xml'
}


if (!$definitionFile.EndsWith($xmlExtension)){
   $definitionFile = $definitionFile + $xmlExtension
}


$InputXmlFilePath     = Join-Path $currentLocation  $definitionFile #'..\070-Definitions\ModuleScaffolding.xml'
$SolutionFile         = $rootNamespace + '.sln'
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
            dotnet new $project.template --language "C#" --name $referenceProjectName  --output $projectfolderName
        }

        cd $SolutionsFolder

        #add the project to the solution
        if ((Test-Path -Path $projectfolderName) -and (Test-Path -Path $SolutionFile)) { 
            Write-Host "Add project '" ($fullProjectPathName + $projectExtension) "' inside the solution folder '" $solutionFolderLayer "'" "of solution '"  $SolutionFile "'"
            dotnet sln $SolutionFile add ($fullProjectPathName + $projectExtension) --solution-folder $solutionFolderLayer
        }
    }

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
            $referencedLayer = $layers | Where-Object -property name -eq $projectReference.fromLayer
            $referencedLayerfolder = $referencedLayer.folderPrefix + $folderPrefix + $rootProjectNamespace + "." + $referencedLayer.name + "." + $projectReference.name
            $referencedPathFolder  = Join-Path $currentLocation $referencedLayerfolder
            
            $referencedProjectName  = $rootProjectNamespace +"." + $projectReference.fromLayer + "." + $projectReference.name + $projectExtension 
            $referencedFile = Join-Path $referencedPathFolder $referencedProjectName

                if ($chekReferences -or !($referenceArray.Contains([System.IO.Path]::GetFileName($referencedFile)))){
                    dotnet add ($fullProjectPathName + $projectExtension) reference $referencedFile
            }
        }
    }

    Write-Host 
}

cd $currentLocation




#dotnet sln o3.sln add ..\020-TchFwk\04.Domain\O3.TchFwk.Domain.csproj --solution-folder 04-Domain 

#end of script restore location
cd $currentLocation


