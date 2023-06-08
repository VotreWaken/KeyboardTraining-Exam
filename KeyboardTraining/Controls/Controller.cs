using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyboardTraining.Model;
namespace KeyboardTraining.Controls
{
    public class Controller
    {
        DataBase database_ = new DataBase();

        // Get Easy Difficulty Words From Data Base
        public void AddEasyWords()
        {
            database_.AddEasyWords();
        }
        // Get Medium Difficulty Words From Data Base
        public void AddMediumWords()
        {
            database_.AddMediumWords();
        }
        // Shuffle All Words In Data Base
        public void ShuffleWords()
        {
            database_.ShuffleWords();
        }
        // Get Hard Difficulty Words From Data Base
        public void AddHardWords()
        {
            database_.AddHardWords();
        }
        // Get All Words From Data Base
        public List<string> GetAllWords()
        {
            return database_.GetWords();
        }
    }
}
