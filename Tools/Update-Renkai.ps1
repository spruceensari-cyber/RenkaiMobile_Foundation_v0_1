param(
    [string]$Branch = "develop"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path ".git")) {
    Write-Error "Bu klasör bir Git deposu değil. Önce Renkai reposunu clone et veya git init yap."
}

$dirty = git status --porcelain
if ($dirty) {
    Write-Error "Çalışma ağacı temiz değil. Değişikliklerini commit/stash etmeden otomatik güncelleme yapılmadı."
}

Write-Host "Fetching origin..."
git fetch origin

Write-Host "Switching to $Branch..."
git switch $Branch

Write-Host "Pulling fast-forward only..."
git pull --ff-only origin $Branch

Write-Host "Renkai güncel. Unity açık ise dosya değişikliklerini otomatik import edecektir."
