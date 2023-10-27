using WebUI.Models;

namespace WebUI.ViewModels
{
    public class HomeVM
    {
        public List<Article> Articles { get; set; }
        public Article FirstSlot { get; set; }
        public List<Article> AllSlot { get; set; }
    }
}
