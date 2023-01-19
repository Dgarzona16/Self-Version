using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Self_Version
{
    internal class Program
    {
        #region ocultar consola
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static int SW_HIDE = 0;
        static int SW_SHOW = 5;
        #endregion

        static void Main(string[] args)
        {
            //ocultar consola
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            try
            {
                //declaracion de variables
                string DirFrom = @"E:\extensiones\Ext", DirTo = @"E:\extensiones\Latest", DirPrev = @"E:\extensiones\Previus";
                List<string> FilesFrom, FilesTo;

                //agarra todas las extensiones
                var applist = Logic.ReadJson();

                //verifica que exista la carpeta para colocar 
                Logic.verifyDirectory(DirFrom);

                //Verificar que la carpeta tenga algo
                FilesFrom = Logic.verifyFile(DirFrom);
                if (!FilesFrom.Any())
                    return;

                //verifica que exista la carpeta para colocar 
                Logic.verifyDirectory(DirTo);

                //ingresa las nuevas extensiones a la carpeta
                foreach (string File in FilesFrom)
                {
                    FileInfo mFile = new FileInfo(File);
                    if (!new FileInfo(DirTo + @"\" + mFile.Name).Exists)
                        mFile.MoveTo(DirTo + @"\" + mFile.Name);
                }

                //recibe todo dentro de la lista de latest
                FilesTo = Logic.verifyFile(DirTo);

                //organizar las carpetas posteriores
                Logic.Organizing(FilesTo, applist, DirTo, DirPrev);
            }
            catch (Exception ex)
            {
                ShowWindow(handle,SW_SHOW);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
