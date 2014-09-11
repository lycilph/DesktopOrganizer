$source_folder = "DesktopOrganizer\bin\release"
$target_folder = "C:\Private\Projects\Releases"
$app_name = "DesktopOrganizer"
$path = $target_folder+"\release"
$final_path = $target_folder+"\"+$app_name

# Copy application release folder and rename
Copy-Item $source_folder $target_folder -Recurse 
Rename-Item $path $app_name

# Remove unwanted directories
Get-ChildItem $final_path -Directory | Remove-Item -Force -Recurse

# Remove unwanted files
$items = Get-ChildItem $final_path\*vshost*
$items += Get-ChildItem $final_path\*.pdb
$items += Get-ChildItem $final_path\*.xml -Exclude "Startup.xml"

Remove-Item $items
