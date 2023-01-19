using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Self_Version
{
    internal static class Logic
    {     
        /// <summary>
        /// lee las configuraciones del json con todas las extensiones que existen
        /// </summary>
        /// <returns>la lista de extensiones existente</returns>
        public static List<Apps> ReadJson()
        {
            List<Apps> list = new List<Apps>();

            using (StreamReader r = new StreamReader(@"E:\extensiones\Config\CheckApps.json"))
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
        public static List<string> verifyFile(string directory) 
        {
            var files =  Directory.GetFiles(directory);
            return files.ToList();
        }
        /// <summary>
        /// Funcion  de organizacion de carpetas despues de latest
        /// </summary>
        /// <param name="LatestApps">lista de archivos existentes en la carpeta</param>
        /// <param name="AllExtensions">lista de extensiones en total</param>
        /// <param name="latestPath">parametro de la carpeta de las ultimas versiones</param>
        /// <param name="PreviusPath">parametro de la carpeta de las versiones previas</param>
        public static void Organizing(List<string> LatestApps, List<Apps> AllExtensions,string latestPath,string PreviusPath)
        {
            string tempPath; 
            foreach(Apps App in AllExtensions)
            {
                var TempList = (from apps in LatestApps
                           where apps.Contains(App.name)
                           select apps).ToList();

                if (TempList.Count > 1)
                {
                    //ordena para que el mas nuevo este primero
                    TempList.Sort();
                    TempList.Reverse();

                    //verifica la creacion de carpetas de versiones previas
                    verifyDirectory(PreviusPath + @"\" + App.name);
                    tempPath = PreviusPath + @"\" + App.name;

                    //itera para poder mover las apps a un carpeta
                    for (int i = 1; i < TempList.Count; i++)
                    {
                        FileInfo mFile = new FileInfo(TempList[i]);
                        if (!new FileInfo(PreviusPath + @"\" + mFile.Name).Exists)
                            mFile.MoveTo(tempPath + @"\" + mFile.Name);
                    }
                }
            }
        }
    }
}
