# WpfAppSchoolDataBase
# 🏫 Full-Stack WPF School Admission & Database Management System

Welcome to the **WPF School Database Administration Dashboard**! This open-source repository is specifically designed as a step-by-step learning guide for students and developers who want to master **WPF (Windows Presentation Foundation)**, **MVVM architecture**, and **Entity Framework Core** with **SQL Server Express (LocalDB)**.

This project features a clean, responsive layout optimized for 14-inch laptop screens, robust pre-save validation rules to handle unique constraints safely, and data-bound textboxes using a modern Microsoft Learn-inspired attached property placeholder engine.

---

## 📈 Learning Journey: The Step-by-Step Versions

This repository contains a full historical timeline of how the application was built. Instead of just looking at the finished project, you can use Git to explore the application at three distinct stages of development:

### 🔹 Version 1.0 — UI Wireframe & Read-Only Context Connection
* **Focus**: Foundational architecture.
* **Features**: Setting up the custom 15-column `DataGrid` workspace matrix, linking it to the local `(localdb)\ProjectModels` server instance, and populating tables with read-only master data records.
* **How to study**: Use this version to understand core XAML grid layouts, row spanning, and basic entity fetching mechanics before adding modification buttons.

### 🔹 Version 2.0 — Interactive CRUD Operations via RelayCommands
* **Focus**: Data manipulation and behavior.
* **Features**: Adding operational command buttons inside the toolbar and details form card. Implemented full Create, Read, Update, and Delete logic using `[RelayCommand]` hooks from the Microsoft Community Toolkit.
* **How to study**: Look at this version to learn how to change UI records dynamically and commit changes back to SQL Server seamlessly.

### 🔹 Version 3.0 — Forms with Course ComboBox Selection & Validation Engine
* **Focus**: Data integrity and premium User Experience (UX).
* **Features**: 
  * Swapped manual text input fields for an automated **Course Name Dropdown ComboBox** that dynamically maps numeric foreign keys behind the scenes.
  * Implemented an advanced **Pre-Save Defensive Validation Engine** that intercepts duplicate phone numbers or email addresses locally, notifying the user via explicit dialog alerts instead of letting the application crash on database constraint checks.
  * Integrated **WPF Style Triggers** for buttons so they display with clear, dimmed text properties when disabled instead of rendering as solid gray blocks.

---

## 🛠️ Core Technical Features Applied

* **WPF Split UI Grid Pattern**: Organized using a root two-row, two-column grid layout where Column 0 (Sidebar) handles dual row spanning.
* **Localized Scrolling**: The top `DataGrid` stays locked to the window boundaries with active independent internal scrollbars, while *only* the bottom input form card utilizes a localized `ScrollViewer` container. This allows full operation on compact laptop screens without page layout deformation.
* **Modern Attached Properties**: Created a native `WatermarkService` dependency layout that draws placeholder instructions inside empty textboxes using pure XAML graphics rendering layer triggers.
* **Fluent API Mapping**: Kept data classes clean by stripping out messy data annotation string decorations and configuring database constraints cleanly inside the `SchoolDBContext` workspace.

---

## 🚀 How to Run and Study This Project

### 1. Prerequisites
Ensure you have the following installed on your developer machine:
* Visual Studio (with .NET Desktop Development workload enabled)
* SQL Server Express LocalDB instance installed

### 2. Clone the Repository
Open a terminal or command prompt and clone this repository down onto your local machine:
```bash
git clone https://github.com
```

### 3. Initialize the Database
Open **SQL Server Object Explorer** inside Visual Studio, connect to `(localdb)\ProjectModels`, and run the primary initialization script to generate the database, master validation constraints, and tables:
```sql
CREATE DATABASE SchoolDB;
-- (Include your full table creation script here for learners to copy/paste)
```

### 4. Switch Between Versions Locally
If you want to step backward in time to study **Version 1.0** directly inside your IDE, open your project terminal and run:
```bash
git checkout <commit-hash-of-v1>
```
To return back to the complete modern **Version 3.0** engine, simply type:
```bash
git checkout main
```

---

## 📄 License
This repository is completely public and free for educational use. Feel free to download it, fork it, and use it as a structural skeleton for your own database projects!
