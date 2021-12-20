using System.Windows.Forms;
using AutoMelder.MeldingLogic;

namespace AutoMelder
{
    public partial class AutoMelderSettings : Form
    {
        public AutoMelderSettings()
        {
            InitializeComponent();
        }

        public MeldRequest MeldRequest = new MeldRequest();
    }
}