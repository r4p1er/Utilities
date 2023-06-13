namespace MemoryManager
{
    class Program
    {
        static void Main(string[] args)
        {
            int number = 1;
            var driveDict = new Dictionary<int, DriveInfo?>();
            
            foreach (var drive in DriveInfo.GetDrives())
            {
                Console.WriteLine($"{number}) {drive.Name}");
                driveDict[number] = drive;

                if (drive.IsReady)
                {
                    Console.WriteLine($"Size:{drive.TotalSize / (1024.0 * 1024.0 * 1024.0):f2} GB");
                    Console.WriteLine($"Occupied space:{(drive.TotalSize - drive.TotalFreeSpace) / (1024.0 * 1024.0 * 1024.0):f2} GB");
                    Console.WriteLine($"Occupied percent:{(drive.TotalSize - drive.TotalFreeSpace) * 100 / drive.TotalSize:d}%");
                }
                else
                {
                    Console.WriteLine("Drive is not ready.");
                }
                
                ++number;
            }

            Console.WriteLine();
            Console.Write("Choose the drive:");
            int chosenDrive;

            while (!int.TryParse(Console.ReadLine(), out chosenDrive) || chosenDrive < 1 || chosenDrive >= number || (!driveDict[chosenDrive]?.IsReady ?? true))
            {
                Console.WriteLine("Incorrect input!");
                Console.Write("Choose the drive:");
            }
            
            var parent = driveDict[chosenDrive]!.RootDirectory;
            long parentSize = driveDict[chosenDrive]!.TotalSize;
            var dirDict = new Dictionary<int, DirectoryInfo>();
            number = 1;
            string cmd = "";

            while (cmd != "stop")
            {
                foreach (var dir in parent.EnumerateDirectories())
                {
                    dirDict[number] = dir;
                    long dirSize = DirSize(dir);
                    Console.WriteLine($"{number}) {dir.Name}    {dirSize / (1024.0 * 1024.0 * 1024.0):f2}GB    {dirSize * 100 / parentSize:d}%");
                    ++number;
                }

                Console.Write("cmd:");
                cmd = Console.ReadLine()!;
                int chosenDir;
                
                while ((!int.TryParse(cmd, out chosenDir) && cmd != "stop" && cmd != "..") || (int.TryParse(cmd, out chosenDir) && (chosenDir < 1 || chosenDir >= number)))
                {
                    Console.WriteLine("Incorrect input!");
                    Console.Write("cmd:");
                }

                if (cmd == "..")
                {
                    parent = parent.Root;
                    parentSize = DirSize(parent);
                }
                else if (cmd != "stop")
                {
                    parent = dirDict[chosenDir];
                    parentSize = DirSize(parent);
                }

                number = 1;
            }
        }

        static long DirSize(DirectoryInfo dir)
        {
            long size = 0;

            try
            {
                foreach (var file in dir.EnumerateFiles())
                {
                    size += file.Length;
                }

                foreach (var subDir in dir.EnumerateDirectories())
                {
                    size += DirSize(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return size;
            }

            return size;
        }
    }
}

