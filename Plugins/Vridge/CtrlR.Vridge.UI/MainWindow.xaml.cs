using System.Windows;
using System.Windows.Controls;

namespace CtrlR.Vridge.UI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VridgeSocketReader m_VridgeSocketReader;

        public MainWindow()
        {
            m_VridgeSocketReader = new VridgeSocketReader();

            InitializeComponent();

            var portNames = m_VridgeSocketReader.SerialPortNames;

            SerialPortName.Items.Clear();

            if (portNames.Length > 0)
            {
                foreach (var port in portNames)
                    SerialPortName.Items.Add(port);

                SerialPortName.SelectedIndex = portNames.Length - 1;
                SerialPortName.SelectionChanged += OnSerialPortNameChanged;

                m_VridgeSocketReader.SerialportName = SerialPortName.SelectedValue.ToString();
            }
        }

        private void OnSerialPortNameChanged(object sender, SelectionChangedEventArgs e)
        {
            m_VridgeSocketReader.SerialportName = SerialPortName.SelectedValue.ToString();
        }

        private void OnPositionChanged(object sender, TextChangedEventArgs e)
        {
            if (PositionX == null || PositionY == null || PositionZ == null)
                return;

            float.TryParse(PositionX.Text, out float x);
            float.TryParse(PositionY.Text, out float y);
            float.TryParse(PositionZ.Text, out float z);

            m_VridgeSocketReader.Controller.SetPosition(x, y, z);
        }

        private void OnHandTypeChanged(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo != null)
            {
                var left = combo.SelectedIndex == 0;
                m_VridgeSocketReader.Controller.SetHand(left);
            }
        }

        private void OnActionClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                if (button.Tag.ToString() == "Connect")
                    m_VridgeSocketReader.Connect();
                else
                    m_VridgeSocketReader.Shutdown();

                ConnectionStatus.Text = m_VridgeSocketReader.IsRunning ? "Connected" : "Not Connected";
            }
        }

        private void OnHeadRelationChanged(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo != null)
                m_VridgeSocketReader.Controller.SetHeadRelation(combo.SelectedIndex);
        }
    }
}
