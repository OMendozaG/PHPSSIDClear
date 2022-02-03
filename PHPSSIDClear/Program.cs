using PHPSSIDClear;
using System.IO;

string ExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
var Ini = new IniFile(ExePath + "Settings.ini");

int Deleted = 0;
int DirError = 0;
int FileError = 0;

string Path = Ini.Read("Path");
int Duration = Int32.Parse(Ini.Read("Duration"));
bool Log = ((Ini.KeyExists("Log")) && (Ini.Read("Log") == "1")) ? true : false;

if (File.Exists(ExePath + "Log.txt")) { File.Delete(ExePath + "Log.txt"); }

Write("Starting PHPSIDClear on " + Path + " with " + Duration.ToString() + " duration");
//Thread.CurrentThread.Priority = ThreadPriority.Lowest;
ProcessDir(Path);
Write("End! Deleted: " + Deleted.ToString() + " files, DirError: " + DirError + ", FileError: " + FileError);

void Write(string text, bool write = false) {
	Console.WriteLine(text);
	if (!Log) { return; }

	File.AppendAllText(ExePath + "Log.txt", "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + Environment.NewLine);
}

void ProcessDir(string Folder) {

	Write("- Exploring " + Folder);

	try {
		DirectoryInfo Directory = new DirectoryInfo(Folder);
		DirectoryInfo[] DirList = Directory.GetDirectories();
		FileInfo[] FileList = Directory.GetFiles();

		Write("-- Directories: " + DirList.Length.ToString() + " / Files: " + FileList.Length.ToString());

		if (FileList.Length > 0) {
			DateTime now = DateTime.Now;
			int DeletedIn = 0;

			try {
				foreach (FileInfo file in FileList) {
					try {
						if ((now - file.LastWriteTime).TotalSeconds > Duration) {
							//Console.WriteLine("--- Delete " + file.FullName + " " + file.LastWriteTime.ToString("dd/MM/yy HH:mm:ss") + " last edit " + (now - file.LastWriteTime).TotalSeconds + " seconds");
							try {
								file.Delete();
								Deleted++;
								DeletedIn++;
							} catch (Exception e) {
								FileError++;
								Write("File Error 1: " + e.ToString());
							}
						}
					} catch (Exception e) {
						FileError++;
						Write("File Error 2: " + e.ToString());
					}
				}
			} catch (Exception e) {
				DirError++;
				Write("Dir Error 1: " + e.ToString());
			}

			Write("-- Deleted: " + DeletedIn);
		}

		if (DirList.Length > 0) {
			foreach (DirectoryInfo dir in DirList) {
				ProcessDir(dir.FullName);
			}
		}

	} catch (Exception e) {
		DirError++;
		Write("Dir Error 2: " + e.ToString());
	}
}
