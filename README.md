# backuputils
🗂️ A simple, reliable backup tool written in C# for copying files with certain extensions from one or more source directories into a single flat destination folder. It includes error logging, fallback behavior, and user-friendly console prompts.

## 🚀 Features
- 📁 Copy files with a certain extension from multiple paths
- ✅ Handles inaccessible folders and permission issues
- 🧠 Smart fallback with GUID renaming if filename conflicts occur
- 🛠️ Designed to run even from root-level directories (e.g. `C:\`)

## 🧑‍💻 Usage
You can run it directly as a console app:
```bash
dotnet run
```
You can also run it via Visual Studio, just open the .sln for that.

## 📦 Future Ideas
CLI arguments (--dest, --source, --ext)
Support for multiple file types
Parallelized copying
Optionally recreate source structure

## 🤝 Contributing
Feel free to open issues or submit pull requests. Even small fixes or tweaks

Made with ☕ and ❤️
