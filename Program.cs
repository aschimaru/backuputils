using System.Linq;
using System.Text;
using System.Xml;

string[] sourcePaths = new string[1000];
string backupPath = ""; 
IEnumerable<string> allFilePaths = new List<string>();
bool searchSubdirectories = true;
char mode = 'c';
bool fallback = true;

Console.WriteLine(" _                _                      _   _ _     \r\n| |              | |                    | | (_) |    \r\n| |__   __ _  ___| | ___   _ _ __  _   _| |_ _| |___ \r\n| '_ \\ / _` |/ __| |/ / | | | '_ \\| | | | __| | / __|\r\n| |_) | (_| | (__|   <| |_| | |_) | |_| | |_| | \\__ \\\r\n|_.__/ \\__,_|\\___|_|\\_\\\\__,_| .__/ \\__,_|\\__|_|_|___/\r\n                            | |                      \r\n                            |_|                      ");
Console.WriteLine("[i] For default options press Enter.\n[i] Run parameters that were marked with ! must be set by the user, those marked with - have default values.");

bool srcErr;
do
{
    srcErr = false;
    Console.WriteLine("[i] Examples: D:\\, C:\\, or D:\\, C:\\Users\\Public");
    Console.Write("[!] Please enter the full paths of the source directories (comma-separated): ");
    string? pathsNonSeparated = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(pathsNonSeparated))
    {
        srcErr = true;
        goto ShowError;
    }

    sourcePaths = pathsNonSeparated.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    string[] drives = Environment.GetLogicalDrives();

    foreach (string path in sourcePaths)
    {
        bool pathValid = false;

        foreach (string drive in drives)
        {
            if (path.StartsWith(drive, StringComparison.OrdinalIgnoreCase) && Directory.Exists(path))
            {
                pathValid = true;
                break;
            }
        }

        if (!pathValid)
        {
            srcErr = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[!] Invalid path: \"{path}\" — either doesn't exist or doesn't start with a valid drive.");
            Console.ResetColor();
        }
    }

ShowError:
    if (srcErr)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[!] One or more entered paths are invalid. Please try again.\n");
        Console.ResetColor();
    }

} while (srcErr);

bool desErr = false;
do
{
    Console.WriteLine("[i] Examples: D:\\ or C:\\Users\\Public");
    Console.Write("[!] Please enter the destination directory for the backup: ");
    string? backupdir = Console.ReadLine();
    if (string.IsNullOrEmpty(backupdir)) desErr = true;
    string[] drives = Environment.GetLogicalDrives();
    bool driveMatch = false;
    foreach (string drive in drives)
    {
        if (!string.IsNullOrEmpty(backupdir) && backupdir.StartsWith(drive, StringComparison.OrdinalIgnoreCase))
        {
            driveMatch = true;
            break;
        }
    }

    desErr = !driveMatch;
    if(Path.Exists(backupdir) == false) desErr = true;
    if (desErr)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[!] Invalid path: It doesn't start with a valid drive.");
        Console.ResetColor();
    }
    else
    {
        backupPath = backupdir + $@"backuputils@{DateTime.Now.ToShortDateString()}\{Guid.NewGuid()}@{DateTime.Now.ToShortTimeString().Replace(":", "-")}";
    }
}
while (desErr);

Console.Write("[-] Do you want to copy the files (leave original intact) or move them (delete from source)? [copy/move]: ");
string? operation = Console.ReadLine();
if (operation == "move") mode = 'm';

Console.Write("[-] Should the program search within subdirectories as well? [true/false]: ");
string? subs = Console.ReadLine();
if (subs == "false") searchSubdirectories = false;

Console.Write("[-] Enter the file extension to process Default: .txt [*.bin, etc.]: ");
string? extension = Console.ReadLine();
if (string.IsNullOrEmpty(extension)) extension = "*.txt";
if (!extension.StartsWith("*.")) extension = "*.txt";

Console.Write("[-] Please provide a path for creating a log file: ");
string? loggingPath = Console.ReadLine();
if (string.IsNullOrEmpty(loggingPath)) loggingPath = backupPath;

Console.Write("[-] Should the program try a fallback when an error occurs? [true/false]: ");
string? fallbackAllowed = Console.ReadLine();
if (fallbackAllowed == "false") fallback = false;

Console.WriteLine($"[>] Finding all paths that lead to files with the {extension} extension");
foreach (var path in sourcePaths)
{
    var ps = Directory.EnumerateFiles(path, extension, new EnumerationOptions
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = searchSubdirectories
    });

    foreach (var file in ps)
    {
        allFilePaths = allFilePaths.Append(file);
    }
}

Console.WriteLine($"[>] Trying to create folder {backupPath.Split(@"\")[1]} in the {backupPath.Split(@"\")[0]} directory.");
System.IO.Directory.CreateDirectory( backupPath );

StringBuilder sb = new StringBuilder();
string logName = $"{DateTime.Now.ToString().Replace(" ", "@").Replace(":", "-")}.log";
sb.AppendLine(" _                _                      _   _ _     \r\n| |              | |                    | | (_) |    \r\n| |__   __ _  ___| | ___   _ _ __  _   _| |_ _| |___ \r\n| '_ \\ / _` |/ __| |/ / | | | '_ \\| | | | __| | / __|\r\n| |_) | (_| | (__|   <| |_| | |_) | |_| | |_| | \\__ \\\r\n|_.__/ \\__,_|\\___|_|\\_\\\\__,_| .__/ \\__,_|\\__|_|_|___/\r\n                            | |                      \r\n                            |_|                      ");
sb.AppendLine($"Tool: backuputils v1.0");
sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
sb.AppendLine($"Machine: {Environment.MachineName}");
sb.AppendLine($"User: {Environment.UserName}");
sb.AppendLine($"OS: {Environment.OSVersion}");
sb.AppendLine();
sb.AppendLine("Source Paths:");
foreach (string src in sourcePaths)
    sb.AppendLine($"  - {src}");
sb.AppendLine();
sb.AppendLine($"Destination: {backupPath}");
sb.AppendLine();
sb.AppendLine("Parameters:");
sb.AppendLine($"  - Search Subdirectories: {(searchSubdirectories == true ? "Yes" : "No")}");
sb.AppendLine($"  - Extension: {extension}");
sb.AppendLine($"  - Log-File path: {loggingPath}");
sb.AppendLine($"  - Fallbacks: {(fallback == true ? "On" : "Off")}");
sb.AppendLine($"  - Mode: {(mode == 'c' ? "Copy" : "Move")}");
sb.AppendLine();
sb.AppendLine("════════════════════════════════════════════════════════════════════════════════════════════════════");
File.AppendAllText(loggingPath + logName, sb.ToString());

foreach (var file in allFilePaths)
{
    try
    {
        Console.Write((mode == 'c' ? "Copying " : "Moving ") + file + " >>> ");
        Console.WriteLine(backupPath + "\\" + @$"{file.Split("\\")[file.Split("\\").Length - 1]}");
        if (mode == 'c') File.Copy(file, backupPath + "\\" + @$"{file.Split("\\")[file.Split("\\").Length - 1]}");
        else File.Move(file, backupPath + "\\" + @$"{file.Split("\\")[file.Split("\\").Length - 1]}");
    } 
    catch (Exception ex)
    {
        sb.AppendLine("[Error] Using fallback.\n" + ex.ToString());
        File.AppendAllText(loggingPath + logName, sb.ToString());
        if (fallback)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] Error occured. >>> Trying Fallback");
                Console.Write(file + " >>> ");
                Guid g = Guid.NewGuid();
                Console.WriteLine(backupPath + "\\" + @$"{g}-{file.Split("\\")[file.Split("\\").Length - 1]}");
                if (mode == 'c') File.Copy(file, backupPath + "\\" + @$"{g}-{file.Split("\\")[file.Split("\\").Length - 1]}");
                else File.Move(file, backupPath + "\\" + @$"{g}-{file.Split("\\")[file.Split("\\").Length - 1]}");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                sb.AppendLine("[Error] Fallback failed.\n" + e.ToString());
                File.AppendAllText(loggingPath + logName, sb.ToString());
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Error occured. >>> Fallback failed");
                Console.ResetColor();
            }
        }
    }
    sb.Clear();
}