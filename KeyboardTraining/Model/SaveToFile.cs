using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTraining.Model
{
    public class SaveToFile
    {
        public void Save(List<string> collection)
        {
            FileStream stream = new FileStream("Vocabulary.xml", FileMode.Create);
            DataContractJsonSerializer saver = new DataContractJsonSerializer(typeof(List<string>));
            saver.WriteObject(stream, collection);
            stream.Close();
            Console.WriteLine("Json serializer OK");
        }
    }
}
