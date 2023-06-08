using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyboardTraining;
using KeyboardTraining.Controls;

namespace KeyboardTraining
{
    // Base Class For Keyboard Button
    abstract class KeyBoardButtons
    {
        public string Content { get; private set; }
        public string Shift { get; private set; }
        public UIElement GridElement { get; private set; }
        public Button TextElement { get; private set; }

        public KeyBoardButtons(string ContentValue, string shiftValue, int row, int column, int columnSpan, string stylebtn, string name)
        {
            Content = ContentValue;
            Shift = shiftValue;

            // Connect Resource Style to Button by Style Name
            Style? style = KeyboardTraining.App.Current.FindResource($"{stylebtn}") as Style;
            Button KeyboardBtn = new Button
            {
                Content = ContentValue,
                FontSize = 24.0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(2.0),
                Style = style,
                Name = name,
            };

            // Connect Click Event to Button
            KeyboardBtn.Click += new RoutedEventHandler(MainWindow.MyButtonHandler);

            Grid.SetRow(KeyboardBtn, row);
            Grid.SetColumn(KeyboardBtn, column);
            Grid.SetColumnSpan(KeyboardBtn, columnSpan);

            TextElement = KeyboardBtn;
            GridElement = TextElement;
        }

        public virtual void RefreshText(bool shiftIsOn, bool capsIsOn)
        { }
    }

    // Control Key Button ( Backspace, Alt, Shift and other )
    class ControlKey : KeyBoardButtons
    {
        public ControlKey(string value, int row, int column, int columnSpan)
            : base(value, value, row, column, columnSpan, "GrayBtn", value)
        {
            TextElement.FontSize = 16.0;
        }
    }

    // Letter Key Button
    class LetterKey : KeyBoardButtons
    {
        public LetterKey(string value, int row, int column, string btnStyle)
            : base(value.ToLower(), value.ToUpper(), row, column, 2, btnStyle, value)
        { }

        public override void RefreshText(bool shiftIsOn, bool capsIsOn)
        {
            if (shiftIsOn ^ capsIsOn)
            {
                TextElement.Content = Shift;
            }
            else
            {
                TextElement.Content = Content;
            }
        }
    }

    // Special Key Button ( ~, -, = )
    class SpecialCharKey : KeyBoardButtons
    {
        public SpecialCharKey(string regularValue, string shiftValue, int row, int column, int columnSpan, string btnStyle, string name)
            : base(regularValue, shiftValue, row, column, columnSpan, btnStyle, name)
        { }

        public override void RefreshText(bool shiftIsOn, bool capsIsOn)
        {
            if (shiftIsOn)
                TextElement.Content = Shift;
            else
                TextElement.Content = Content;
        }
    }

    // Digit Button
    class DigitKey : KeyBoardButtons
    {
        public DigitKey(string regularValue, string shiftValue, int row, int column, string btnStyle, string name)
            : base(regularValue, shiftValue, row, column, 2, btnStyle, name)
        { }

        public override void RefreshText(bool shiftIsOn, bool capsIsOn)
        {
            if (shiftIsOn)
                TextElement.Content = Shift;
            else
                TextElement.Content = Content;
        }
    }

    // Space Button 
    class SpaceKey : KeyBoardButtons
    {
        public SpaceKey(int row, int column, int columnSpan, string btnStyle)
            : base("Space", "Space", row, column, columnSpan, btnStyle, null)
        {
            TextElement.FontSize = 16.0;
        }
    }

    // Main Window Class
    public partial class MainWindow : Window
    {
        // Dictionary For All Keyboard Buttons
        Dictionary<Key, KeyBoardButtons> allKeyboardButtons;

        // Controller
        public Controller controller_;

        // Fails Input Count
        int Failsvalue;

        // Correct Input Count
        int CorrectValue;

        int CorrectValueSelect;

        // Timer That Observe Correct Input Count And Update Display Every 1 Second
        System.Windows.Threading.DispatcherTimer CorrectInputDispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        // Timer That Observe The End of the Game, At the end of which displays Result ( Every 20 Seconds )
        System.Windows.Threading.DispatcherTimer ResultGameDispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        // Click Start Button Boolean Variable Using To Requests Data Base Words
        bool start;

        // Initialize Buttons
        public void InitializeButtons()
        {
            allKeyboardButtons = new Dictionary<Key, KeyBoardButtons>();

            // Create all keys for the keyboard:
            allKeyboardButtons[Key.Oem3] = new SpecialCharKey("`", "~", 0, 0, 2, "RedBtn", "Tilda");
            allKeyboardButtons[Key.D1] = new DigitKey("1", "!", 0, 2, "RedBtn", "First");
            allKeyboardButtons[Key.D2] = new DigitKey("2", "@", 0, 4, "RedBtn", "Second");
            allKeyboardButtons[Key.D3] = new DigitKey("3", "#", 0, 6, "YellowBtn", "Third");
            allKeyboardButtons[Key.D4] = new DigitKey("4", "$", 0, 8, "GreenBtn", "Fourth");
            allKeyboardButtons[Key.D5] = new DigitKey("5", "%", 0, 10, "BlueBtn", "Fifth");
            allKeyboardButtons[Key.D6] = new DigitKey("6", "^", 0, 12, "BlueBtn", "Sixth");
            allKeyboardButtons[Key.D7] = new DigitKey("7", "&", 0, 14, "PurpleBtn", "Seventh");
            allKeyboardButtons[Key.D8] = new DigitKey("8", "*", 0, 16, "PurpleBtn", "Eight");
            allKeyboardButtons[Key.D9] = new DigitKey("9", "(", 0, 18, "RedBtn", "Nine");
            allKeyboardButtons[Key.D0] = new DigitKey("0", ")", 0, 20, "YellowBtn", "Zero");
            allKeyboardButtons[Key.OemMinus] = new SpecialCharKey("-", "_", 0, 22, 2, "GreenBtn", "Minus");
            allKeyboardButtons[Key.OemPlus] = new SpecialCharKey("=", "+", 0, 24, 2, "GreenBtn", "Plus");
            allKeyboardButtons[Key.Back] = new ControlKey("Backspace", 0, 26, 4);
            allKeyboardButtons[Key.Tab] = new ControlKey("Tab", 1, 0, 3);
            allKeyboardButtons[Key.Q] = new LetterKey("Q", 1, 3, "RedBtn");
            allKeyboardButtons[Key.W] = new LetterKey("W", 1, 5, "YellowBtn");
            allKeyboardButtons[Key.E] = new LetterKey("E", 1, 7, "GreenBtn");
            allKeyboardButtons[Key.R] = new LetterKey("R", 1, 9, "BlueBtn");
            allKeyboardButtons[Key.T] = new LetterKey("T", 1, 11, "BlueBtn");
            allKeyboardButtons[Key.Y] = new LetterKey("Y", 1, 13, "PurpleBtn");
            allKeyboardButtons[Key.U] = new LetterKey("U", 1, 15, "PurpleBtn");
            allKeyboardButtons[Key.I] = new LetterKey("I", 1, 17, "RedBtn");
            allKeyboardButtons[Key.O] = new LetterKey("O", 1, 19, "YellowBtn");
            allKeyboardButtons[Key.P] = new LetterKey("p", 1, 21, "GreenBtn");
            allKeyboardButtons[Key.OemOpenBrackets] = new SpecialCharKey("[", "{", 1, 23, 2, "GreenBtn", "CubeSlashLeft");
            allKeyboardButtons[Key.OemCloseBrackets] = new SpecialCharKey("]", "}", 1, 25, 2, "GreenBtn", "CubeSlash");
            allKeyboardButtons[Key.Oem5] = new SpecialCharKey("\\", "|", 1, 27, 3, "GreenBtn", "SlashSlash");
            allKeyboardButtons[Key.CapsLock] = new ControlKey("CapsLock", 2, 0, 4);
            allKeyboardButtons[Key.A] = new LetterKey("A", 2, 4, "RedBtn");
            allKeyboardButtons[Key.S] = new LetterKey("S", 2, 6, "YellowBtn");
            allKeyboardButtons[Key.D] = new LetterKey("D", 2, 8, "GreenBtn");
            allKeyboardButtons[Key.F] = new LetterKey("F", 2, 10, "BlueBtn");
            allKeyboardButtons[Key.G] = new LetterKey("G", 2, 12, "BlueBtn");
            allKeyboardButtons[Key.H] = new LetterKey("H", 2, 14, "PurpleBtn");
            allKeyboardButtons[Key.J] = new LetterKey("J", 2, 16, "PurpleBtn");
            allKeyboardButtons[Key.K] = new LetterKey("K", 2, 18, "RedBtn");
            allKeyboardButtons[Key.L] = new LetterKey("L", 2, 20, "YellowBtn");
            allKeyboardButtons[Key.OemSemicolon] = new SpecialCharKey(";", ":", 2, 22, 2, "GreenBtn", "ColumnPoint");
            allKeyboardButtons[Key.OemQuotes] = new SpecialCharKey("'", "\"", 2, 24, 2, "GreenBtn", "UpperColumn");
            allKeyboardButtons[Key.Enter] = new ControlKey("Enter", 2, 26, 4);
            allKeyboardButtons[Key.LeftShift] = new ControlKey("Shift", 3, 0, 5);
            allKeyboardButtons[Key.Z] = new LetterKey("Z", 3, 5, "RedBtn");
            allKeyboardButtons[Key.X] = new LetterKey("X", 3, 7, "YellowBtn");
            allKeyboardButtons[Key.C] = new LetterKey("C", 3, 9, "GreenBtn");
            allKeyboardButtons[Key.V] = new LetterKey("V", 3, 11, "BlueBtn");
            allKeyboardButtons[Key.B] = new LetterKey("B", 3, 13, "BlueBtn");
            allKeyboardButtons[Key.N] = new LetterKey("N", 3, 15, "PurpleBtn");
            allKeyboardButtons[Key.M] = new LetterKey("M", 3, 17, "PurpleBtn");
            allKeyboardButtons[Key.OemComma] = new SpecialCharKey(",", "<", 3, 19, 2, "RedBtn", "Comma");
            allKeyboardButtons[Key.OemPeriod] = new SpecialCharKey(".", ">", 3, 21, 2, "YellowBtn", "Bigger");
            allKeyboardButtons[Key.OemQuestion] = new SpecialCharKey("/", "?", 3, 23, 2, "GreenBtn", "Slash");
            allKeyboardButtons[Key.RightShift] = new ControlKey("Shift", 3, 25, 5);
            allKeyboardButtons[Key.LeftCtrl] = new ControlKey("Ctrl", 4, 0, 3);
            allKeyboardButtons[Key.LWin] = new ControlKey("Win", 4, 3, 3);
            allKeyboardButtons[Key.LeftAlt] = new ControlKey("Alt", 4, 6, 3);
            allKeyboardButtons[Key.Space] = new SpaceKey(4, 9, 12, "SpaceBtn");
            allKeyboardButtons[Key.RightAlt] = new ControlKey("Alt", 4, 21, 3);
            allKeyboardButtons[Key.RWin] = new ControlKey("Win", 4, 24, 3);
            allKeyboardButtons[Key.RightCtrl] = new ControlKey("Ctrl", 4, 27, 3);

            // Place all KeyboardButtons to the grid on the main window:
            foreach (KeyBoardButtons keyboardButton in allKeyboardButtons.Values)
                keyboardGrid.Children.Add(keyboardButton.GridElement);
        }

        // Window Constructor
        public MainWindow()
        {
            InitializeComponent();
            InitializeButtons();
            controller_ = new Controller();
            StartCorrectInputTimerWithSettings();
            StopBtn.IsEnabled = false;
        }

        // Button Click Input Handler
        static public void MyButtonHandler(object sender, EventArgs e)
        {
            Button a = (Button)sender;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    try
                    {
                        if ((window as MainWindow).TBContent.Text[(window as MainWindow).CorrectValueSelect].ToString().ToUpper() == a.Content.ToString().ToUpper())
                        {
                            (window as MainWindow).CorrectValue++;
                            (window as MainWindow).CorrectValueSelect++;
                            (window as MainWindow).TBContent.Focus();
                            (window as MainWindow).TBContent.Select(0, (window as MainWindow).CorrectValueSelect);
                        }
                        else
                        {
                            if (a.Content.ToString() == "Space" && (window as MainWindow).TBContent.Text[(window as MainWindow).CorrectValueSelect] == ' ')
                            {
                                (window as MainWindow).CorrectValue++;
                                (window as MainWindow).CorrectValueSelect++;
                                (window as MainWindow).TBContent.Select(0, (window as MainWindow).CorrectValueSelect);
                                return;
                            }
                            (window as MainWindow).Failsvalue++;
                            (window as MainWindow).LBFailsNumber.Content = (window as MainWindow).Failsvalue;
                        }
                        if ((window as MainWindow).TBContent.Text.Count() - 1 == (window as MainWindow).CorrectValueSelect && (window as MainWindow).start == true)
                        {
                            (window as MainWindow).CorrectValueSelect = 0;
                            (window as MainWindow).TBContent.Text = " ";

                            List<string> aw = (window as MainWindow).FillWords();
                            foreach (var item in aw)
                            {
                                (window as MainWindow).TBContent.Text += item + " ";
                            }
                            (window as MainWindow).TBContent.Text = (window as MainWindow).TBContent.Text.Substring(1);
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
        }
        // Button Start Click
        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            if (start == true)
            {
                return;
            }
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;
            CBCaseSensetive.IsEnabled = false;
            SliderDifficulty.IsEnabled = false;
            controller_.AddEasyWords();
            if (SliderDifficulty.Value == 1)
            {
                controller_.AddEasyWords();
            }
            else if (SliderDifficulty.Value == 2)
            {
                controller_.AddMediumWords();
            }
            else if (SliderDifficulty.Value == 3)
            {
                controller_.AddHardWords();
            }
            start = true;
            List<string> a = controller_.GetAllWords();
            foreach (var item in a)
            {
                TBContent.Text += item + " ";
            }
            StartResultGameTimerWithSettings();
        }

        // Button Stop Click
        private void StopBtnClick(object sender, RoutedEventArgs e)
        {
            start = false;
            TBContent.Text = string.Empty;
            CorrectValue = 0;
            CorrectValueSelect = 0;
            LBFailsNumber.Content = '0';
            Failsvalue = 0;
            ResultGameDispatcherTimer.Stop();
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
            CBCaseSensetive.IsEnabled = true;
            SliderDifficulty.IsEnabled = true;
        }

        // Fill Words From Data Controller
        private List<string> FillWords()
        {
            controller_.ShuffleWords();
            return controller_.GetAllWords();
        }

        // Keyboard Buttons Click ( by Mouse )
        private void InputBtnClick(object sender, RoutedEventArgs e)
        {
            Button a = (Button)sender;
            try
            {
                if (TBContent.Text[CorrectValue].ToString().ToUpper() == a.Content.ToString().ToUpper())
                {
                    CorrectValue++;
                    TBContent.Select(0, CorrectValue);
                }
                else
                {
                    if (a.Content.ToString() == "Space" && TBContent.Text[CorrectValue] == ' ')
                    {
                        CorrectValue++;
                        TBContent.Select(0, CorrectValue);
                        return;
                    }
                    Failsvalue++;
                    LBFailsNumber.Content = Failsvalue;
                }

            }
            catch (Exception)
            {
                return;
            }
        }

        // KeyDown Event
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                allKeyboardButtons[e.Key].GridElement.Effect = new DropShadowEffect();

                if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.CapsLock)
                {
                    RefreshKeyboard();
                    return;
                }

                if (e.Key == Key.Back)
                {
                    if (CorrectValueSelect > 0)
                    {
                        CorrectValueSelect--;
                        TBContent.Select(0, CorrectValueSelect);
                    }
                }

                if (e.Key == Key.Space)
                {
                    e.Handled = true;
                    if (TBContent.Text[CorrectValueSelect] == ' ')
                    {
                        CorrectValueSelect++;
                        CorrectValue++;
                        TBContent.Select(0, CorrectValueSelect);
                        return;
                    }
                    else
                    {
                        Failsvalue++;
                        LBFailsNumber.Content = Failsvalue;
                        return;
                    }
                }

                if (CBCaseSensetive.IsChecked == false)
                {

                    if (TBContent.Text[CorrectValueSelect].ToString().ToUpper() == e.Key.ToString())
                    {
                        CorrectValue++;
                        CorrectValueSelect++;
                        TBContent.Focus();
                        TBContent.Select(0, CorrectValueSelect);
                    }
                    else
                    {
                        if (e.Key == Key.Capital)
                        {
                            return;
                        }
                        Failsvalue++;
                        LBFailsNumber.Content = Failsvalue;
                    }
                    if (TBContent.Text.Count() - 1 == CorrectValueSelect && start == true)
                    {
                        CorrectValueSelect = 0;
                        TBContent.Text = " ";

                        List<string> a = FillWords();
                        foreach (var item in a)
                        {
                            TBContent.Text += item + " ";
                        }
                        TBContent.Text = TBContent.Text.Substring(1);
                    }
                }
                else
                {

                    if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
                    {

                    }
                    else
                    {

                    }

                    if (TBContent.Text[CorrectValueSelect].ToString() == ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled ? e.Key.ToString().ToUpper() : e.Key.ToString().ToLower()))
                    {
                        CorrectValue++;
                        CorrectValueSelect++;
                        TBContent.Focus();
                        TBContent.Select(0, CorrectValueSelect);
                    }
                    else
                    {
                        if (e.Key == Key.Capital)
                        {
                            return;
                        }
                        Failsvalue++;
                        LBFailsNumber.Content = Failsvalue;
                    }
                    if (TBContent.Text.Count() - 1 == CorrectValueSelect && start == true)
                    {
                        CorrectValueSelect = 0;
                        TBContent.Text = " ";

                        List<string> a = FillWords();
                        foreach (var item in a)
                        {
                            TBContent.Text += item + " ";
                        }
                        TBContent.Text = TBContent.Text.Substring(1);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        // KeyUp Event
        private void mainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!allKeyboardButtons.ContainsKey(e.Key))
                return;

            allKeyboardButtons[e.Key].GridElement.Effect = null;

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                RefreshKeyboard();

            e.Handled = true;


        }

        // Refresh Keyboard After Clicking Tab or Shift
        private void RefreshKeyboard()
        {
            bool shiftIsOn = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool capsIsOn = Keyboard.IsKeyToggled(Key.CapsLock);
            foreach (KeyBoardButtons keyboardButton in allKeyboardButtons.Values)
            {
                keyboardButton.RefreshText(shiftIsOn, capsIsOn);
                keyboardButton.GridElement.Effect = null;
            }
        }

        // Timer Correct Input Dispatcher Timer Tick
        private void CorrectInputDispatcherTimerTick(object sender, EventArgs e)
        {
            LBSpeedNumber.Content = (CorrectValue);
        }
        // Timer Result Game Dispatcher Timer Tick
        private void ResultGameDispatcherTimerTick(object sender, EventArgs e)
        {
            start = false;
            MessageBox.Show($"Congratulations, Chars/ Min: {CorrectValue}");
            ResultGameDispatcherTimer.Stop();
            TBContent.Text = string.Empty;
            CorrectValue = 0;
            CorrectValueSelect = 0;
            LBFailsNumber.Content = '0';
            Failsvalue = 0;
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
            CBCaseSensetive.IsEnabled = true;
            SliderDifficulty.IsEnabled = true;
        }

        // Start Correct Input Timer With Settings
        private void StartCorrectInputTimerWithSettings()
        {
            CorrectInputDispatcherTimer.Tick += new EventHandler(CorrectInputDispatcherTimerTick);
            CorrectInputDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            CorrectInputDispatcherTimer.Start();
        }

        // Start Result Game Timer With Settings
        private void StartResultGameTimerWithSettings()
        {
            ResultGameDispatcherTimer.Tick += new EventHandler(ResultGameDispatcherTimerTick);
            ResultGameDispatcherTimer.Interval = new TimeSpan(0, 0, 20);
            ResultGameDispatcherTimer.Start();
        }

        // Slider Value Changed 
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = e.OriginalSource as Slider;

            if (slider != null)
            {
                DifficultyValue.Content = slider.Value.ToString();
            }
        }
    }
}
