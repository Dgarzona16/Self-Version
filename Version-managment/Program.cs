using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

namespace Version_Managment
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

        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            try
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appSettings.json")
                    .Build();

                var path = config.GetValue<string>("Configuration:path");
                //ocultar consola
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
                //log de inicio
                log.InfoFormat("Inicio de proceso: {0}", DateTime.Now);
                //declaracion de variables
                string DirFrom = path + @"\Ext", DirTo = path + @"\Latest", DirPrev = path + @"\Previus";
                List<FileInfo> FilesFrom, FilesTo;
                
                //agarra todas las extensiones
                var applist = Logic.ReadJson(path);

                //verifica que exista la carpeta para colocar 
                Logic.verifyDirectory(DirFrom);

                //Verificar que la carpeta tenga algo
                FilesFrom = Logic.ExtractFiles(DirFrom);
                if (!FilesFrom.Any())
                    return;

                //verifica que exista la carpeta para colocar 
                Logic.verifyDirectory(DirTo);

                //ingresa las nuevas extensiones a la carpeta
                foreach (var File in FilesFrom)
                {
                    if (!new FileInfo(DirTo + @"\" + File.Name).Exists)
                        File.MoveTo(DirTo + @"\" + File.Name);
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

                log.InfoFormat("Fin de proceso: {0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                //log
                log.ErrorFormat("ERROR: {0}; {1}; {2}", ex.Source, ex.Message, ex.InnerException);
            }
        }
    }
}