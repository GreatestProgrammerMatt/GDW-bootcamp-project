using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public static class JSONHandler 
{
    
    /*
 
 ___  ___      _                            _              __                        __ _ _                                        _   
 |  \/  |     (_)                          | |            / _|                      / _(_) |                                      | |  
 | .  . | __ _ _ _ __    _ __ ___  __ _  __| | ___ _ __  | |_ _ __ ___  _ __ ___   | |_ _| | ___    ___  _ __    __ _ ___ ___  ___| |_ 
 | |\/| |/ _` | | '_ \  | '__/ _ \/ _` |/ _` |/ _ \ '__| |  _| '__/ _ \| '_ ` _ \  |  _| | |/ _ \  / _ \| '__|  / _` / __/ __|/ _ \ __|
 | |  | | (_| | | | | | | | |  __/ (_| | (_| |  __/ |    | | | | | (_) | | | | | | | | | | |  __/ | (_) | |    | (_| \__ \__ \  __/ |_ 
 \_|  |_/\__,_|_|_| |_| |_|  \___|\__,_|\__,_|\___|_|    |_| |_|  \___/|_| |_| |_| |_| |_|_|\___|  \___/|_|     \__,_|___/___/\___|\__|
                                                                                                                                       
                                                                                                                                       
 
*/
    //Lee el JSONLocal y checkea si hay archivo
    public static List<T> ReadJSONLocal<T> (TextAsset t,string file)
    {
        string path = PathFinder(file);
        string content;
        Debug.Log(PathFinder(file));

        //Si existe el archivo lee de ahí, si no lee del Asset del juego la primera vez
        if (File.Exists(path))
        {
            content = Reader(PathFinder(file));
            List<T> feed = Feeder<T>(content);
            return feed;
        }
        else
        {
            content = t.text;
            List<T> feed = Feeder<T>(content);
            
            string c = JsonHelper.ToJson<T>(feed.ToArray());
            Writer(PathFinder(file),c);
            
            return feed;
        }
    }
    
    //Acción de convertir el contenido si hay
    public static List<T> Feeder<T>(string content)
    {
        if (content==null||content=="{}")
        {
            return new List<T>();
        }
        else
        {
            List<T> feed = JsonHelper.FromJson<T>(content).ToList();
            return feed;
        }
    }
    
    

    /*
 
 .|'''|                             .|';                                         ||`                     
 ||                            ''   ||    ''                                     ||                      
 `|'''|, '||''|, .|''|, .|'',  ||  '||'   ||  .|'',    '||''| .|''|,  '''|.  .|''||  .|''|, '||''| ('''' 
  .   ||  ||  || ||..|| ||     ||   ||    ||  ||        ||    ||..|| .|''||  ||  ||  ||..||  ||     `'') 
  |...|'  ||..|' `|...  `|..' .||. .||.  .||. `|..'    .||.   `|...  `|..||. `|..||. `|...  .||.   `...' 
          ||                                                                                             
         .||                                                                                             
 
*/

    //Lee el JSON del text asset textos
    public static List<T> ReadJSONTextAsset<T> (TextAsset t)
    {
            string content = t.text;
            List<T> feed = Feeder<T>(content);
            return feed;
    }
 
    //Método básico para leer JSON desde un archivo
    public static List<T> ReadJSONFile<T> (string file)
    {
        string content = Reader(PathFinder(file));
        List<T> feed = Feeder<T>(content);
        return feed;
    }


    

    /*
 
  _____                         __                                   ___ _____  _____ _   _ 
 /  ___|                       / _|                                 |_  /  ___||  _  | \ | |
 \ `--.  __ ___   _____ _ __  | |_ ___  _ __    __ _ _ __  _   _      | \ `--. | | | |  \| |
  `--. \/ _` \ \ / / _ \ '__| |  _/ _ \| '__|  / _` | '_ \| | | |     | |`--. \| | | | . ` |
 /\__/ / (_| |\ V /  __/ |    | || (_) | |    | (_| | | | | |_| | /\__/ /\__/ /\ \_/ / |\  |
 \____/ \__,_| \_/ \___|_|    |_| \___/|_|     \__,_|_| |_|\__, | \____/\____/  \___/\_| \_/
                                                            __/ |                           
                                                           |___/                            
 
*/
    //Guarda el JSON
    public static void SaveJSON<T> (List<T> savedFile, string file)
    {
        Debug.Log(PathFinder(file));
        string content = JsonHelper.ToJson<T>(savedFile.ToArray());
    
        Writer(PathFinder(file),content);
    }


    
    /*
 
 ______                _   _                   _           __ _           _               _   _                       _ _          __ _ _                             _                      _    __ _ _           
 |  ___|              | | (_)                 | |         / _(_)         | |             | | | |                     (_) |        / _(_) |                           | |                    | |  / _(_) |          
 | |_ _   _ _ __   ___| |_ _  ___  _ __  ___  | |_ ___   | |_ _ _ __   __| |  _ __   __ _| |_| |__     __      ___ __ _| |_ ___  | |_ _| | ___  ___    __ _ _ __   __| |  _ __ ___  __ _  __| | | |_ _| | ___  ___ 
 |  _| | | | '_ \ / __| __| |/ _ \| '_ \/ __| | __/ _ \  |  _| | '_ \ / _` | | '_ \ / _` | __| '_ \    \ \ /\ / / '__| | __/ _ \ |  _| | |/ _ \/ __|  / _` | '_ \ / _` | | '__/ _ \/ _` |/ _` | |  _| | |/ _ \/ __|
 | | | |_| | | | | (__| |_| | (_) | | | \__ \ | || (_) | | | | | | | | (_| | | |_) | (_| | |_| | | |_   \ V  V /| |  | | ||  __/ | | | | |  __/\__ \ | (_| | | | | (_| | | | |  __/ (_| | (_| | | | | | |  __/\__ \
 \_|  \__,_|_| |_|\___|\__|_|\___/|_| |_|___/  \__\___/  |_| |_|_| |_|\__,_| | .__/ \__,_|\__|_| |_( )   \_/\_/ |_|  |_|\__\___| |_| |_|_|\___||___/  \__,_|_| |_|\__,_| |_|  \___|\__,_|\__,_| |_| |_|_|\___||___/
                                                                             | |                   |/                                                                                                              
                                                                             |_|                                                                                                                                   
 
*/
    
    //Define la locación del .JSON
    private static string PathFinder(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
    
    
    
    
    //Pasa el contenido de string a JSON
    private static void Writer(string path, string content)
    {
        FileStream strm = new FileStream(path,FileMode.Create);
        using (StreamWriter wrt = new StreamWriter (strm))
            {
                wrt.Write(content);
            }

    }
    
    
    
    
    //Pasa el .JSON a string
    private static string Reader(string path)
    {
        if(File.Exists(path))
        {
            using (StreamReader rdr = new StreamReader (path))
            {
                string fileContent = rdr.ReadToEnd();
                return fileContent;
            }
        }
        else
        {
            return "";
        }
    }





    /*
 
  _    _                                    __                  _   _                 
 | |  | |                                  / _|                | | (_)                
 | |  | |_ __ __ _ _ __  _ __   ___ _ __  | |_ _   _ _ __   ___| |_ _  ___  _ __  ___ 
 | |/\| | '__/ _` | '_ \| '_ \ / _ \ '__| |  _| | | | '_ \ / __| __| |/ _ \| '_ \/ __|
 \  /\  / | | (_| | |_) | |_) |  __/ |    | | | |_| | | | | (__| |_| | (_) | | | \__ \
  \/  \/|_|  \__,_| .__/| .__/ \___|_|    |_|  \__,_|_| |_|\___|\__|_|\___/|_| |_|___/
                  | |   | |                                                           
                  |_|   |_|                                                           
 
*/

    //JSON Wrapper, convierte el JSON en un archivo convertible agregando el campo Items
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }




    /*
 
  _____ _             _              _     _           _       ___ _____  _____ _   _  ______               _           
 /  ___(_)           | |            | |   (_)         | |     |_  /  ___||  _  | \ | | | ___ \             | |          
 \ `--. _ _ __   __ _| | ___    ___ | |__  _  ___  ___| |_      | \ `--. | | | |  \| | | |_/ /___  __ _  __| | ___ _ __ 
  `--. \ | '_ \ / _` | |/ _ \  / _ \| '_ \| |/ _ \/ __| __|     | |`--. \| | | | . ` | |    // _ \/ _` |/ _` |/ _ \ '__|
 /\__/ / | | | | (_| | |  __/ | (_) | |_) | |  __/ (__| |_  /\__/ /\__/ /\ \_/ / |\  | | |\ \  __/ (_| | (_| |  __/ |   
 \____/|_|_| |_|\__, |_|\___|  \___/|_.__/| |\___|\___|\__| \____/\____/  \___/\_| \_/ \_| \_\___|\__,_|\__,_|\___|_|   
                 __/ |                   _/ |                                                                           
                |___/                   |__/                                                                            
 
*/
    // Convert JSON into single object from file
    public static T ReaderJSONSingle<T>(string file)
    {
        string path = PathFinder(file);
        string content;
        
        if (File.Exists(path))
        {
            content = Reader(path);
            T single = JsonUtility.FromJson<T>(content);
            return single;
        }
        return default(T);
    }

    // Convert JSON into single object from TextAssets
    public static T ReaderJSONSingle<T>(TextAsset t, string file)
    {
        string path = PathFinder(file);

        string content = t.text;
        T single = JsonUtility.FromJson<T>(content);

        string contentToSave = JsonUtility.ToJson(single);
        Writer(path,contentToSave);

        return single;
    }

}
