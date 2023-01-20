using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
                string DirFrom = @"\\10.0.40.2\extensiones\Ext", DirTo = @"\\10.0.40.2\extensiones\Latest", DirPrev = @"\\10.0.40.2\extensiones\Previus";
                List<string> FilesFrom, FilesTo;

                //agarra todas las extensiones
                var applist = Logic.ReadJson();

                //verifica que exista la carpeta para colocar 
                Logic.verifyDirectory(DirFrom);

                //Verificar que la carpeta tenga algo
                FilesFrom = Logic.ExtractFiles(DirFrom);
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

                //eliminar los repetidos
                foreach (FileInfo file in new DirectoryInfo(DirFrom).GetFiles())
                {
                    file.Delete();
                }

                //recibe todo dentro de la lista de latest
                FilesTo = Logic.ExtractFiles(DirTo);

                //organizar las carpetas posteriores
                Logic.Organizing(FilesTo, applist, DirTo, DirPrev);
            }
            catch (Exception ex)
            {
                if (!File.Exists(@"\\10.0.40.2\extensioness\Error.txt"))
                    File.Create(@"\\10.0.40.2\extensioness\Error.txt");
                File.AppendAllText(@"\\10.0.40.2\extensioness\Error.txt", ex.Message + Environment.NewLine);
            }
        }
    }
}
