# backuputils
🗂️ A simple, reliable backup tool written in C# for copying files with certain extensions from one or more source directories into a single flat destination folder. It includes error logging, fallback behavior, and user-friendly console prompts.

## 🚀 Features
- 📁 Copy `.txt` files from multiple paths
- ✅ Handles inaccessible folders and permission issues
- 🧠 Smart fallback with GUID renaming if filename conflicts occur
- 📝 Error-only logging with detailed timestamps
- 🛠️ Designed to run even from root-level directories (e.g. `C:\`)

## 🧑‍💻 Usage
You can run it directly as a console app:
```bash
dotnet run
```
You can also run it via Visual Studio, just open the .sln for that.
