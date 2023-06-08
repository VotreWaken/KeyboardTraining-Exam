using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTraining.Model
{
    class LoadFromFile
    {
        public List<string> Load()
        {
            FileStream stream = new FileStream("Vocabulary.xml", FileMode.Open);
            DataContractJsonSerializer downloader = new DataContractJsonSerializer(typeof(List<string>));
            List<string> collectionVocabulary = (List<string>)downloader.ReadObject(stream);
            stream.Close();
            return collectionVocabulary;
        }

    }
}
