using log4net;
using Newtonsoft.Json;

namespace Version_Managment
{
    internal static class Logic
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// lee las configuraciones del json con todas las extensiones que existen
        /// </summary>
        /// <returns>la lista de extensiones existente</returns>
        public static List<Apps> ReadJson(string path)
        {
            var list = new List<Apps>();

            using (StreamReader r = new StreamReader(path + @"\Config\CheckApps.json"))
            {
                string json = r.ReadToEnd();
                list = JsonConvert.DeserializeObject<List<Apps>>(json);
            }
            return list;
        }
        /// <summary>
        /// verifica si existe el directorio, sino, lo crea
        /// </summary>
        /// <param name="directory">path a consultar</param>
        public static void verifyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        /// <summary>
        /// recibe las rutas para crear una  lista de todos los archivos
        /// </summary>
        /// <param name="directory">direccion a extraer archivos</param>
        /// <returns>lista de archivos en el directorio</returns>
        public static List<FileInfo> ExtractFiles(string directory)
        {
            var list = Directory.GetFiles(directory).ToList();
            return (from f in list select new FileInfo(f)).ToList();
        }
        /// <summary>
        /// Funcion  de organizacion de carpetas despues de latest
        /// </summary>
        /// <param name="LatestApps">lista de archivos existentes en la carpeta</param>
        /// <param name="AllExtensions">lista de extensiones en total</param>
        /// <param name="latestPath">parametro de la carpeta de las ultimas versiones</param>
        /// <param name="PreviusPath">parametro de la carpeta de las versiones previas</param>
        public static void Organizing(List<FileInfo> LatestApps, List<Apps> AllExtensions, string latestPath, string PreviusPath)
        {
            string tempPath;

            foreach (var App in AllExtensions)
            {
                var TempList = (from apps in LatestApps
                                where apps.Name.Contains(App.name)
                                orderby apps.LastWriteTime descending
                                select apps).ToList();

                if (TempList.Count <= 1)
                    continue;

                //ordena para que el mas nuevo este primero
                TempList.RemoveAt(0);

                //verifica la creacion de carpetas de versiones previas
                tempPath = PreviusPath + @"\" + App.name;
                verifyDirectory(tempPath);

                //itera para poder mover las apps a un carpeta
                foreach (var app in TempList)
                {
                    try
                    {
                        if (!new FileInfo(PreviusPath + @"\" + app.Name).Exists)
                            app.MoveTo(tempPath + @"\" + app.Name);
                    }
                    catch (Exception ex)
                    {
                        app.Delete();
                        log.ErrorFormat("Error al mover el archivo: {0}", ex.Message);
                    }
                }
            }
        }
    }
}