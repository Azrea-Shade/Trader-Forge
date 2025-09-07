using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Infrastructure;
using Services;
using Services.Feeds;

namespace Presentation
{
    public class BriefViewModel : ViewModelBase
    {
        private readonly BriefingService _svc;

        public string Content { get => _content; set { _content = value; OnPropertyChanged(); } }
        public string Status  { get => _status;  set { _status = value;  OnPropertyChanged(); } }

        private string _content = "Generating preview…";
        private string _status  = "";

        public ICommand GenerateNow { get; }
        public ICommand MarkDelivered { get; }

        public BriefViewModel(BriefingService svc)
        {
            _svc = svc;
            GenerateNow = new RelayCommand(async _ => await GenerateAsync(), _ => true);
            MarkDelivered = new RelayCommand(_ => { _svc.MarkDelivered(DateOnly.FromDateTime(DateTime.Today)); Status = "Delivered ✓"; }, _ => true);
            _ = GenerateAsync(); // initial preview (safe)
        }

        private async Task GenerateAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                Content = await _svc.GenerateAsync(today);
                Status = "Generated ✓";
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
        }

        // Helper factory for places that don't have DI:
        public static BriefViewModel CreateDefault()
        {
            var db = new SqliteDb();
            var feed = new DummyPriceFeed(); // CI-safe
            var svc = new BriefingService(db, feed);
            return new BriefViewModel(svc);
        }
    }
}
