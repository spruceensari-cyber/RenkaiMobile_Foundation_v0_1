# Renkai GitHub bağlantısı

Renkai ayrı repo olmalıdır. Aura-of-Gods ile karıştırmayın.

## İlk kurulum

1. GitHub üzerinde `Renkai` adlı boş repository oluştur.
2. Unity projesinin kök klasöründe terminal aç.
3. Aşağıdaki komutları çalıştır:

```bash
git init
git branch -M main
git remote add origin <RENKAI_REPO_URL>
git add .
git commit -m "chore: bootstrap Renkai mobile foundation"
git push -u origin main

git switch -c develop
git push -u origin develop
```

## Günlük çalışma

Her büyük özellik ayrı branch:
- feat/mobile-controls
- feat/gunplay
- feat/round-system
- feat/spirit-core
- feat/networking
- feat/graphics-urp

Tamamlanan özellik `develop` branch'e PR ile birleşir.
Oynanabilir milestone test edildikten sonra `main` branch'e alınır.

## Güvenli güncelleme

Windows PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File .\Tools\Update-Renkai.ps1 -Branch develop
```

Script çalışma ağacı kirliyse pull yapmaz. Böylece yerel değişikliklerin üzerine yazılmasını engeller.
