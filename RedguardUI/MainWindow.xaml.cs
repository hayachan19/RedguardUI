using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RedguardUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleIniParser parser = null;
        readonly MilesParser milesParser = new MilesParser("sound/DIG.INI");

        //might add friendly names to resolutions here
        readonly string[] softwareResolutionList = new string[] { "0", "2", "1" };
        readonly string[] glideResolutionList = new string[] { "3", "7", "8", "9", "12", "13", "14" };
        readonly string[] glideTexScaleList = new string[] { "1, 1", "2, 1", "2, 2", "3, 2" };

        public MainWindow()
        {
            InitializeComponent();
            parser = new SimpleIniParser("SYSTEM.INI");

            SoftwareResolutionSlider.Value = Array.FindIndex(softwareResolutionList, x => x.Equals(parser.Content["screen"]["resolution"]));
            GlideResolutionSlider.Value = Array.FindIndex(glideResolutionList, x => x.Equals(parser.Content["3dfx"]["resolution"]));
            GlideTextureSlider.Value = Array.FindIndex(glideTexScaleList, x => x.Equals(parser.Content["3dfx"]["text_scale"]));

            JoystickSlider.Value = double.Parse(parser.Content["cyrus"]["joy_tolerance"]);
            CamHSlider.Value = double.Parse(parser.Content["camera"]["glide_angle_x"], CultureInfo.InvariantCulture);
            CamVSlider.Value = double.Parse(parser.Content["camera"]["glide_angle_y"], CultureInfo.InvariantCulture);
            CombatHSlider.Value = double.Parse(parser.Content["camera"]["camera_combat_angle_offset_x"]);
            CombatVSlider.Value = double.Parse(parser.Content["camera"]["camera_combat_angle_offset_y"]);

            FastSoundCheckBox.IsChecked = BooleanHelper(parser.Content["system"]["fast_sound"]);
            SubtitlesCheckBox.IsChecked = BooleanHelper(parser.Content["dialog"]["dialog_print_text"]);
            AutoDefendCheckBox.IsChecked = BooleanHelper(parser.Content["cyrus"]["auto_defend"]);
            SMKInterlaceCheckbox.IsChecked = BooleanHelper(parser.Content["screen"]["smk_interlace"]);

            TbDeviceName.Text = milesParser.Content["DEVICE"];
            TbIOAddress.Text = milesParser.Content["IO_ADDR"];
            TbIRQ.Text = milesParser.Content["IRQ"];
            TbDMA8.Text = milesParser.Content["DMA_8_BIT"];
            TbDMA16.Text = milesParser.Content["DMA_16_BIT"];
        }

        private bool BooleanHelper(string value) //bool.Parse didn't work
        {
            if (value.Equals("0")) return false;
            return true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sender as Slider).Tag is null) return;
            double value = (sender as Slider).Value;
            string[] split = ((sender as Slider).Tag as string).Split(':');

            string[] paramList = null;
            switch (split[0] + ':' + split[1])
            {
                case "screen:resolution": paramList = softwareResolutionList; goto case "!:paramOption";
                case "3dfx:resolution": paramList = glideResolutionList; goto case "!:paramOption";
                case "3dfx:text_scale": paramList = glideTexScaleList; goto case "!:paramOption";
                case "camera:glide_angle_x": goto case "!:camera:glide_angle_z";
                case "camera:glide_angle_y": goto case "!:floatValue";
                case "!:camera:glide_angle_z": parser.Content["camera"]["glide_angle_z"] = value.ToString("G1", CultureInfo.InvariantCulture); goto case "!:floatValue";
                case "!:paramOption": parser.Content[split[0]][split[1]] = paramList[(int)value]; break;
                case "!:floatValue": parser.Content[split[0]][split[1]] = value.ToString("G1", CultureInfo.InvariantCulture); break;

                default: parser.Content[split[0]][split[1]] = ((int)value).ToString(); break;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int value = (bool)(sender as CheckBox).IsChecked ? 1 : 0;
            string[] split = ((sender as CheckBox).Tag as string).Split(':');
            parser.Content[split[0]][split[1]] = value.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            parser.Save("SYSTEM.INI");
            MessageBox.Show("Config saved.", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class GlideResolutionConverter : IValueConverter
    {
        readonly string[] resolutions = { "512x384", "640x480", "800x600", "960x720", "1024x768", "1280x1024", "1600x1200" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => resolutions[int.Parse(value.ToString())];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class SoftwareResolutionConverter : IValueConverter
    {
        readonly string[] resolutions = { "320x200", "320x400", "640x480" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => resolutions[int.Parse(value.ToString())];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}


