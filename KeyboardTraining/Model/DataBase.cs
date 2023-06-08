using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTraining.Model
{
    class DataBase
    {
        List<string> Words_;
        private static Random rng = new Random();
        public DataBase()
        {
            Words_ = new List<string>();
        }
        // Get All Words in Data Base
        public List<string> GetWords()
        {
            return Words_;
        }

        // Shuffle All Words In Data Base
        public void ShuffleWords()
        {
            int n = Words_.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = Words_[k];
                Words_[k] = Words_[n];
                Words_[n] = value;
            }
        }
        // Get Easy Difficulty Words From Data Base
        public void AddEasyWords()
        {
            List<string> words = new List<string>() { "Hello", "World", "My", "First", "Text" };
            SaveToFile a = new SaveToFile();
            a.Save(words);
            LoadFromFile w = new LoadFromFile();
            Words_ = w.Load();
            ShuffleWords();
        }
        // Get Medium Difficulty Words From Data Base
        public void AddMediumWords()
        {
            List<string> words = new List<string>() { "Lorem", "ipsum", "dolor", "sit", "amet" };
            SaveToFile a = new SaveToFile();
            a.Save(words);
            LoadFromFile w = new LoadFromFile();
            Words_ = w.Load();
            ShuffleWords();
        }
        // Get Hard Difficulty Words From Data Base
        public void AddHardWords()
        {
            List<string> words = new List<string>() { "consectetur", "adipiscing", "elit", "sed", "eiusmod" };
            SaveToFile a = new SaveToFile();
            a.Save(words);
            LoadFromFile w = new LoadFromFile();
            Words_ = w.Load();
            ShuffleWords();
        }
    }
}
