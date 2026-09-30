using CursaAqui.App.UI;

namespace CursaAqui.App;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormLogin());
    }    
}