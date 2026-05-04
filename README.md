<div align="center">

  <img src="assets/images/logo-biolife-1.png" alt="Biolife" width="210">

  <h1>Biolife</h1>
  <p><strong>Organic food e-commerce on ASP.NET Core MVC</strong></p>

  <img alt="Animated headline" src="https://readme-typing-svg.demolab.com?font=Nunito&weight=700&size=24&duration=2600&pause=900&color=7AC142&center=true&vCenter=true&width=720&lines=Fresh+organic+storefront;Clean+ASP.NET+Core+architecture;Secure+local+configuration">

  <p>
    <img alt=".NET" src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white">
    <img alt="ASP.NET Core" src="https://img.shields.io/badge/ASP.NET_Core-MVC-7AC142?style=for-the-badge">
    <img alt="SQL Server" src="https://img.shields.io/badge/SQL_Server-EF_Core-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white">
  </p>

  <img alt="Fresh organic marketplace" src="assets/images/home-03/green-slide-01.jpg" width="100%">

</div>

---

## ✨ About The Project

**Biolife** is an educational organic food e-commerce project with a lively storefront design: sliders, product cards, categories, authentication, an admin panel, and CRUD sections for content management.

The visual style is built around a fresh green palette, fruit banners, soft promo blocks, and dynamic UI elements from the original Biolife template.

<p align="center">
  <img src="assets/images/home-01/main-slide-01.jpg" width="31%" alt="Organic slide">
  <img src="assets/images/products/p-01.jpg" width="31%" alt="Product card">
  <img src="assets/images/home-03/product_deal_330x330.jpg" width="31%" alt="Fresh product">
</p>

## 🧩 Features

- 🛒 Storefront with a home page, promo sections, and a product showcase.
- 🔐 Cookie authentication, roles, sessions, and middleware for user validation.
- 🌐 Google external login when `Authentication:Google` is available in the local configuration.
- 🧑‍💼 Admin panel for users, roles, products, genres, authors, notes, and carousel content.
- 🗄️ Entity Framework Core + SQL Server + migrations.
- 🖼️ Image uploads through `wwwroot/uploads` as runtime storage.
- ✉️ Email service abstractions for email confirmation and password recovery.

## 🏗️ Architecture

```text
Biolife
├── .NET/Biolife
│   ├── Biolife.Web             # ASP.NET Core MVC, views, controllers, static files
│   ├── Biolife.Application     # ViewModels and application abstractions
│   ├── Biolife.Domain          # Domain entities
│   ├── Biolife.Persistence     # DbContext and EF Core migrations
│   └── Biolife.Infrastructure  # Services, email, auth/session middleware
├── assets                      # Original frontend template assets
└── index.html                  # Static source template
```

## 🚀 Getting Started

1. Install **.NET 10 SDK** and **SQL Server**.
2. Create a local `appsettings.json` inside `.NET/Biolife/Biolife.Web`.
3. Add the connection string and secrets locally only.
4. Run the application:

```bash
cd .NET/Biolife
dotnet restore
dotnet run --project Biolife.Web
```

The application automatically applies EF Core migrations on startup.

## 🔒 Security

`.gitignore` intentionally excludes local configuration and secrets:

- `appsettings.json`
- `appsettings.*.json`
- `.env`, `.env.*`
- `*.key`, `*.pem`, `*.pfx`, `secrets.json`
- build output: `bin/`, `obj/`, `artifacts/`
- IDE/runtime noise: `.vs/`, `*.log`, `devserver*.log`

For configuration examples, prefer safe files such as `appsettings.Example.json` without real keys.

## 🌿 Technologies

<p>
  <img alt="C#" src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white">
  <img alt="Razor" src="https://img.shields.io/badge/Razor_Views-512BD4?style=flat-square">
  <img alt="EF Core" src="https://img.shields.io/badge/EF_Core-6DB33F?style=flat-square">
  <img alt="Bootstrap" src="https://img.shields.io/badge/Bootstrap-7952B3?style=flat-square&logo=bootstrap&logoColor=white">
  <img alt="jQuery" src="https://img.shields.io/badge/jQuery-0769AD?style=flat-square&logo=jquery&logoColor=white">
</p>

---

<div align="center">
  <sub>Fresh code. Clean config. No leaked secrets.</sub>
</div>
