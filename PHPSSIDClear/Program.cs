using PHPSSIDClear;
using System.IO;

var Ini = new IniFile("Settings.ini");

int Deleted = 0;
string Path = Ini.Read("Path");
int Duration = Int32.Parse(Ini.Read("Duration"));

void ProcessDir(string Folder) {

	//Console.WriteLine("- Exploring " + Folder);

	DirectoryInfo Directory = new DirectoryInfo(Folder);
	DirectoryInfo[] DirList = Directory.GetDirectories();
	FileInfo[] FileList = Directory.GetFiles();

	//Console.WriteLine("-- Directories: " + DirList.Length.ToString() + " / Files: " + FileList.Length.ToString());

	if (FileList.Length > 0) {
		DateTime now = DateTime.Now;

		foreach (FileInfo file in FileList) {
			if ((now - file.LastWriteTime).TotalSeconds > Duration) {
				//Console.WriteLine("--- Delete " + file.FullName + " " + file.LastWriteTime.ToString("dd/MM/yy HH:mm:ss") + " last edit " + (now - file.LastWriteTime).TotalSeconds + " seconds");
				file.Delete();
				Deleted++;
			}
		}
	}

	if (DirList.Length > 0) {
		foreach (DirectoryInfo dir in DirList) {
			ProcessDir(dir.FullName);
		}
	}
}

Console.WriteLine("Starting PHPSIDClear on " + Path + " with " + Duration.ToString() + " duration");
ProcessDir(Path);
Console.WriteLine("End! Deleted: " + Deleted.ToString() + " files");