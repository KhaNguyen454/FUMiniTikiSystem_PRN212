# FUMiniTikiSystem - PRN212 Group Assignment

A basic e-commerce WPF application developed using 3-Layer Architecture and Entity Framework Core (Code First), as part of PRN212 Group Assignment at FPT University.

## 📐 Architecture

### ✔️ 3-Layer Architecture:
1. **BusinessObjects (BO)** - Entity models
2. **DataAccess (DAO)** - DbContext & Repositories
3. **Presentation (WPF)** - UI for users and admin

## 🧰 Technologies

- .NET 6 / .NET 7
- WPF (Windows Presentation Foundation)
- Entity Framework Core (Code First)
- MSSQL Server
- Visual Studio 2022+

## 📁 Folder Structure

📦 FUMiniTikiSystem_PRN212/
├── 📁 docs/                         # Tài liệu: mô hình, hướng dẫn, sơ đồ, báo cáo
│   ├── ERD.png                     # Sơ đồ ERD (nếu có)
│   └── AssignmentGuide.md          # Tóm tắt yêu cầu đề bài
│
├── 📁 FUMiniTikiSystem/            # Thư mục solution .sln (Visual Studio)
│   ├── StudentName_ClassCode_GASM.sln
│
│   ├── 📁 BusinessLogicLayer/         # ✅ LAYER 1 - Entities (Product, Order, Category...)
│   │   └── Models/
│   │       ├── Product.cs
│   │       └── ...
│
│   ├── 📁 DataAccessLayer/              # ✅ LAYER 2 - Database + Repositories
│   │   ├── FUMiniTikiSystemContext.cs
│   │   ├── Repositories/
│   │   │   ├── IProductRepository.cs
│   │   │   └── ProductRepository.cs
│   │   │   └── ...
│   │   └── Migrations/            # Chứa các migration EF Core
│
│   ├── 📁 StudentNameWPF/          # ✅ LAYER 3 - Giao diện người dùng WPF
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   └── App.xaml.cs, MainWindow.xaml.cs ...
│
│   └── appsettings.json           # Kết nối DB
│
├── .gitignore
├── README.md
└── LICENSE (MIT)
