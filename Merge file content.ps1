# Встановити директорію-джерело як поточну директорію, звідки запущено скрипт
$sourceDirectory = (Get-Location).Path

# Встановити шлях до кінцевого файлу в поточній директорії
$outputFile = Join-Path -Path $sourceDirectory -ChildPath "merged.txt"

# Отримати повний шлях до самого скрипта
$selfScriptPath = $MyInvocation.MyCommand.Path

# Видалення кінцевого файлу, якщо він уже існує
if (Test-Path $outputFile) {
    Remove-Item $outputFile -Force
}

# Рекурсивно знайти всі файли, зчитати їх вміст, і додати до кінцевого файлу
Get-ChildItem -Path $sourceDirectory -Recurse -File | Where-Object {
    $_.FullName -ne $selfScriptPath
} | ForEach-Object {
    # Отримання відносного шляху
    $relativePath = $_.FullName.Substring($sourceDirectory.Length).TrimStart("\\")

    # Додати заголовок із відносним шляхом та розділювач
    Add-Content -Path $outputFile -Value "`n=== $relativePath ===`n"

    # Додати вміст файлу
    Get-Content $_.FullName | Add-Content -Path $outputFile
}
