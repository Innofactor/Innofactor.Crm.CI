namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class ErrorControl : ControlBase
    {
        public ErrorControl(string label, string text)
        {
            InitializeComponent();
            lblError.Text = label;
            txtError.Text = text;
        }
    }
}
