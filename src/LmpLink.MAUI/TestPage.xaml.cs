namespace LmpLink.MAUI;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    private void OnTestButtonClicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Status: Button clicked! ✅";
        StatusLabel.TextColor = Colors.Green;
    }
}
