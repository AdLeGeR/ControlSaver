
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace ControlSaver.Model
{
    internal class Saves
    {
        private ObservableCollection<save> collection = new ObservableCollection<save>();

        public ReadOnlyObservableCollection<save> Collection;
        public int count { get { return collection.Count; } }
        private string savefile;
        private string saveDir;
        private string activeDir;
        public int activeSave = -1;

        public Saves()
        {
            savefile = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Remedy\Control\ControlSaver\ControlSave.txt";
            saveDir = Path.GetDirectoryName(savefile);
            if (!Directory.Exists(saveDir)) 
            { 
                Directory.CreateDirectory(saveDir);
                File.Create(savefile).Close();
                EmptyDir();
                
            }
            if (!File.Exists(savefile)) { File.Create(savefile).Close(); EmptyDir(); }
            string maindir = Path.GetDirectoryName(saveDir);
            foreach (string d in Directory.GetDirectories(maindir))
            {
                if (d != saveDir)
                {
                    activeDir = d;
                }
            }
            int count = File.ReadAllLines(savefile).Length;
            string[] lines = File.ReadAllLines(savefile);
            for (int i = 0; i < count; i++)
            {
                string name = lines[i];
                if (name.EndsWith(" is active"))
                {
                    string newname = name.Substring(0, name.Length - 9);
                    collection.Add(new save(newname, true));
                    activeSave = i;
                }
                else
                {
                    Console.WriteLine(name);
                    collection.Add(new save(name, false));
                }

            }
            Collection = new ReadOnlyObservableCollection<save>(collection);
            Console.WriteLine(Collection.Count);
        }

        //выдаёт папку сохранения исходя из номера сохранения
        private string GetDir(int num)
        {
            return Path.Combine(saveDir, "save" + num.ToString());
        }

        //выдаёт номер сохранения исходя из имени
        private int GetNum(string name)
        {
            foreach (save save in collection)
            {
                if (save.Name == name)
                {
                    return collection.IndexOf(save);
                }
            }
            return -1;
        }

        //переписывает информацию в текстовом файле
        private void ReWrite()
        {
            StreamWriter file = new StreamWriter(savefile, false);
            foreach (save save in collection)
            {
                if (GetNum(save.Name) == activeSave)
                {
                    file.WriteLine(save.Name + " is active");
                }
                else
                {
                    file.WriteLine(save.Name);
                }
            }
            file.Close();
        }

        public void AddSave(string name)
        {
            int len = collection.Count;
            collection.Add(new save(name, false));
            StreamWriter file = new StreamWriter(savefile, true);
            file.WriteLine(name);
            file.Close();
            string dir = GetDir(len);
            Directory.CreateDirectory(dir);
        }

        public void AddSave(string name, string old_dir)
        {
            AddSave(name);
            int num = collection.Count - 1;
            string new_dir = GetDir(num);
            CopyDirectory(old_dir, new_dir);

        }

        public void CopySave(string Name, string oldSave)
        {
            int num = GetNum(Name);
            string old_path = GetDir(num);
            AddSave(Name, old_path);
        }

        public void Del(int num)
        {
            if ( activeSave == num)
            {
                activeSave = -1;
            }
            string dir = GetDir(num);
            Directory.Delete(dir, true);
            collection.RemoveAt(num);
            ReWrite();
            for (int i = num+1; i <= collection.Count; i++)
            {
                Directory.Move(GetDir(i), GetDir(i - 1));
            }
        }

        public void Rename(int num, string newName)
        {
            string oldName = collection[num].Name;
            collection[num].Name = newName;
            ReWrite();
        }

        public void Activate(int num)
        {
            if (activeSave != -1) 
            { 
                int num2 = activeSave;
                CopyDirectory(activeDir, GetDir(num2));
                collection[num2].activated = false;
            }
            activeSave = num;
            collection[num].activated = true;
            CopyDirectory(GetDir(num), activeDir);
            ReWrite();
        }

        //копирует файлы из одной директории в другую
        static private void CopyDirectory(string from, string to)
        {
            Directory.Delete(to, true);
            Directory.CreateDirectory(to);
            string[] dirs = Directory.GetDirectories(from);
            string[] files = Directory.GetFiles(from);
            foreach (string s in files)
            {
                string file = Path.GetFileName(s);
                string destFile = Path.Combine(to, file);

                File.Copy(s, destFile);
            }
            foreach (string s in dirs)
            {
                string dir = new DirectoryInfo(s).Name;
                string last_dir = Path.Combine(from, dir);
                string new_dir = Path.Combine(to, dir);
                Directory.CreateDirectory(new_dir);
                CopyDirectory(last_dir, new_dir);
            }
        }

        //эта функция вызывается, если приложение запустилось в первыё раз
        public void EmptyDir()
        {
            string name = "save1";
            activeSave = 0;
            AddSave(name, activeDir);
            Activate(0);
        }

        // копирует папку с охранениями
        public void CopyToDesktop(int num)
        {
            string DesktopDir = @"C:\Users\" + Environment.UserName + @"\Desktop\Save from Control";
            Directory.CreateDirectory(DesktopDir);
            CopyDirectory(GetDir(num), DesktopDir);
        }

        public void SavingActiveSave(int num)
        {
            CopyDirectory(activeDir, GetDir(num));
        }
    }

    //класс для коллекции
    class save
    {
        public string Name;
        public bool activated;

        public save(string name, bool active)
        {
            Name = name;
            activated = active;
        }
    }

}

