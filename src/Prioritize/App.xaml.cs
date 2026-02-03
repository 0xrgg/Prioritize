namespace Prioritize
{
    public partial class App : Application
    {
        public App(AppDbContext db)
        {
            InitializeComponent();

            //db.Database.EnsureCreated();

            MainPage = new MainPage();
        }
    }
}
