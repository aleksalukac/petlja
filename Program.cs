using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ZTrainingToPetlja
{
    class Solution
    {
        public String Name;
        public String ProgLang;
        public String Tags;
        public String Description;
    }

    class PetljaTask
    {
        public String Title;
        public Int64 Id;
        public String RefId;
        public double TimeLimit;
        public double MemLimit;
        public String Owner;
        public String Origin;
        public String[] Tags;
        public Solution[] Solutions;
        public String Status;
        public String StatusOd;
        public String Lang;

        public String Text;
    }

    class ZTrainingTask
    {
        public Int64 Id;
        public String Name;
        public String FullName;
        public String LinuxName;
        public String Type_id;
        public int TaskGroup_id;
        public String Date;
        public int Year;
        public String Text;
        public int nTests;
        public int TimeLimit;
        public int MemoryLimit;
        public Int64 AuthorId;
        public String Author;
        public int isZml; //no idea what this tag is for
        public Int64 nSubmissions;
        public Int64 nAttempts;
        public Int64 nSolved;
        public int IsPublic;
        public int WasEverPublic;
        public String Source;
        public String SourceUrl;
        public int HasGrader;
        public int UploadedTests;
        public double Stars;
        public int Locked;
        public int Category;
        public int SubCategory;
    }

    class Program
    {

        public static string ToMdTextFormat(string taskStatement)
        {
            if (taskStatement == null) return null;
            string readText = taskStatement;

            readText = readText.Replace("[c]", "~~~\n");
            readText = readText.Replace("[/c]", "\n~~~");

            readText = readText.Replace("[p]", "");
            readText = readText.Replace("[/p]", "\n");

            readText = readText.Replace("[in]", "## OPIS ULAZA\n");
            readText = readText.Replace("[/in]", "");

            readText = readText.Replace("[out]", "## OPIS IZLAZA\n");
            readText = readText.Replace("Input:", "### ULAZ\n");
            readText = readText.Replace("Output:", "### IZLAZ\n");

            readText = readText.Replace("[ex]", "## PRIMER\n");
            readText = readText.Replace("[/ex]", "\n");
            readText = readText.Replace("[/out]", "");

            readText = readText.Replace("<br>", "\n");

            readText = readText.Replace("[[", "$$");
            readText = readText.Replace("]]", "$$");

            readText = readText.Replace("\\r\\n", "\n");
            readText = readText.Replace("\\r", "\n");
            readText = readText.Replace("\\n", "\n");


            readText = readText.Replace("{{", "_");
            readText = readText.Replace("}}", "");

            readText = readText.Replace("[img]", "![](");
            readText = readText.Replace("[/img]", ")");

            var resultString = Regex.Replace(readText, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline); //izbacuje visestruke prazne linije

            //Console.WriteLine(resultString);
            return resultString;
        }

        static public void SaveAsMd(PetljaTask pt, string rootFolder, string testsFolder)
        {
            string folderName;

            string fileName = "\\text";

            if (pt.Id > 50000)
            {
                folderName = pt.Id.ToString();
            }
            else
            {
                folderName = pt.RefId.ToString();
                fileName += pt.Lang;
            }

            fileName += ".md";

            string cleanFileName = String.Join("", folderName.Split(Path.GetInvalidFileNameChars()));

            string path = Path.Combine(rootFolder, cleanFileName);

            bool exists = System.IO.Directory.Exists(path);
            if (!exists)
                System.IO.Directory.CreateDirectory(path);

            string destFolder = path;
            path += fileName;

            String TextMd = "---\ntitle: " + pt.Title +
                "\ntimelimit: " + pt.TimeLimit.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) +
                "\nmemlimit: " + pt.MemLimit.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) +
                "\nowner: " + pt.Owner +
                "\norigin: " + pt.Origin +
                "\ntags: " + pt.Tags +
                "\nsolutions: " + pt.Solutions +
                "\nstatus: " + pt.Status +
                "\nstatus-od: " + pt.StatusOd + "\n---\n";
            TextMd += pt.Text;

            if (!File.Exists(path))
            {
                File.WriteAllText(path, TextMd);
            }

            CopyTestFiles(pt, destFolder, testsFolder);
        }

        static public void CopyTestFiles(PetljaTask pt, string destFolder, string testsFolder)
        {
            if (Directory.Exists(testsFolder + "\\" + pt.Title))
            {
                foreach (string newPath in Directory.GetFiles((testsFolder + "\\" + pt.Title), "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace((testsFolder + "\\" + pt.Title), destFolder), true);
            }
            else if (Directory.Exists(testsFolder + "\\"  + "t" + pt.Id))
            {
                foreach (string newPath in Directory.GetFiles((testsFolder + "\\t" + pt.Id), "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace((testsFolder + "\\t" + pt.Id), destFolder), true);
            }

            
        }

        private static void ChangeCheckerFiles(string path)
        {
            string[] allCPFiles = Directory.GetFiles(path, "*.checker.p", SearchOption.AllDirectories);
            foreach (string checkerFile in allCPFiles)
            {
                //Console.WriteLine(checkerFile);

                string text = File.ReadAllText(checkerFile);
                text = text.Replace("writeln", "exit");
                File.WriteAllText("test.txt", text);
            }
        }

        static public void ChangeOutputExtensions(string path)
        {
            //changing extension of .sol filest to .out
            string[] allSolFiles = Directory.GetFiles(path, "*.sol", SearchOption.AllDirectories);
            foreach (string solFile in allSolFiles)
            {
                if(!File.Exists(Path.ChangeExtension(solFile, ".out")))
                {

                    File.Move(solFile, Path.ChangeExtension(solFile, ".out"));
                }
            }
        }

        static public PetljaTask ToPetlja(ZTrainingTask ztTask)
        {
            PetljaTask pt = new PetljaTask();
            pt.Id = ztTask.Id;
            pt.RefId = ztTask.FullName;
            pt.Title = ztTask.Name;
            pt.TimeLimit = ((double)ztTask.TimeLimit) / 1000; //miliS to S
            pt.MemLimit = ((double)ztTask.MemoryLimit) / 1048576; //B to MB
            //pt.Owner = 
            pt.Origin = "Z Trening " + ztTask.Source + " URL:" + ztTask.SourceUrl;
            //pt.Tags = 
            //pt.Solutions =
            pt.Status = "KANDIDAT";
            pt.StatusOd = "Year:" + ztTask.Year;

            pt.Lang = ztTask.Type_id;
            pt.Text = ToMdTextFormat(ztTask.Text);

            return pt;
        }



        static public List<String> SerialzedTaskToListOfItems(String taskTupleAsString)
        {
            List<String> taskItems = new List<String>();

            bool isOutsideQuotes = true;
            int i = 0;
            int itemStart = 0;
            char prevChar = ' '; // any value
            foreach (char c in taskTupleAsString)
            {
                if (c == ',' && isOutsideQuotes)
                {
                    taskItems.Add(taskTupleAsString.Substring(itemStart, i - itemStart));
                    itemStart = i + 1;
                    i++;
                }
                else
                {
                    // Apostrophe in word (e.g. can't) is written with backslash (can\'t) 
                    // and should not be counted as quotation mark
                    if (c == '\'' && prevChar != '\\')
                        isOutsideQuotes = !isOutsideQuotes;
                    i++;
                }
                prevChar = c;
            }
            taskItems.Add(taskTupleAsString.Substring(itemStart, i - itemStart));

            return taskItems;
        }

        static ZTrainingTask CreateZTrainingTask(List<String> taskItems)
        {
            ZTrainingTask task = new ZTrainingTask();
            task.Id = Int64.Parse(taskItems[0]);

            try
            {
                task.Name = taskItems[1].Substring(1, taskItems[1].Length - 2);
                task.FullName = taskItems[1];
                task.LinuxName = taskItems[2].Substring(1, taskItems[2].Length - 2);
                if (task.TimeLimit == null)
                    task.Type_id = taskItems[3].Substring(1, taskItems[3].Length - 2);
                else
                    task.Type_id = taskItems[3];
                task.TaskGroup_id = int.Parse(taskItems[4]);
                task.Date = taskItems[5].Substring(1, taskItems[5].Length - 2);
                task.Year = int.Parse(taskItems[6]);
                task.Text = taskItems[7].Substring(1, taskItems[7].Length - 2);


                task.nTests = int.Parse(taskItems[8]);
                task.TimeLimit = int.Parse(taskItems[9]);
                task.MemoryLimit = int.Parse(taskItems[10]);
                task.AuthorId = Int64.Parse(taskItems[11]);
                task.Author = taskItems[12].Substring(1, taskItems[12].Length - 2);
                task.isZml = int.Parse(taskItems[13]);
                task.nSubmissions = Int64.Parse(taskItems[14]);
                task.nAttempts = Int64.Parse(taskItems[15]);
                task.nSolved = Int64.Parse(taskItems[16]);
                task.IsPublic = int.Parse(taskItems[17]);
                task.WasEverPublic = int.Parse(taskItems[18]);
                task.Source = taskItems[19].Substring(1, taskItems[19].Length - 2);
                task.SourceUrl = taskItems[20].Substring(1, taskItems[20].Length - 2);
                task.HasGrader = int.Parse(taskItems[21]);
                task.UploadedTests = int.Parse(taskItems[22]);
                task.Stars = double.Parse(taskItems[23]);
                task.Locked = int.Parse(taskItems[24]);
                task.Category = int.Parse(taskItems[25]);
                task.SubCategory = int.Parse(taskItems[25]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (task.Id < 50000) task.Text = task.LinuxName;


            return task;
        }

        static void Main(string[] args)
        {
            //Ovde treba staviti odgovarajuce putanje
            string problemStatemenstFile = @"D:\Petlja\ZdbTasks.sql"; //   @"D:\Petlja\Jezerca.sql" @"c:\PetljaUns\ZTraining\ProblemStatements_Metadata\ZdbTasks_TasksOnly.sql"; 
            string testFilesAndCheckersFolder = "D:\\Petlja\\___tasks___\\tasks"; // Treba povezati testove i checkere sa tekstovima zadataka
            string destinationRootFolder = @"D:\Petlja\PetljaTask\"; //  @"c:\PetljaUns\ZTraining\ZTrainingInPetljaFormat2\";


            ChangeCheckerFiles(destinationRootFolder);
                return;


            StreamReader st = new StreamReader(problemStatemenstFile); //ostavljen je samo rezultat query-ja u fajlu         petljaSqlTest ZdbTasks.sql

            List<ZTrainingTask> Tasks = new List<ZTrainingTask>();
            var tasksAsStrings = st.ReadLine().Split(new string[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var taskString in tasksAsStrings)
            {
                List<String> L = new List<String>();
                String trimmedTaskString;
                trimmedTaskString = taskString.TrimStart('(');
                trimmedTaskString = trimmedTaskString.TrimEnd(',');
                trimmedTaskString.TrimEnd(')');

                //Tasks.Add(ReadTask(SplitStringParts(trimmedPart)));
                SaveAsMd(ToPetlja(CreateZTrainingTask(SerialzedTaskToListOfItems(trimmedTaskString))), destinationRootFolder, testFilesAndCheckersFolder);
            }

            ChangeOutputExtensions(destinationRootFolder);
            ChangeCheckerFiles(destinationRootFolder);
        }

        
    }
}