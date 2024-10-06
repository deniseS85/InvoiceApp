using System.ComponentModel;

namespace RechnungsApp.Views {
    public partial class Popup : ContentView, INotifyPropertyChanged {
        public event EventHandler? PopupClosed;
        public Popup() {
            InitializeComponent();
            this.BindingContext = this;
        }

        private string? _title;
        public string? Title {
            get => _title;
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string? _message;
        public string? Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected virtual new void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ShowPopup(string title, string message) {
            Title = title;
            Message = message;
            IsVisible = true;
            Opacity = 0;
            await this.FadeTo(1, 250);
        }

        public async void OnClose(object sender, EventArgs e) {
            await this.FadeTo(0, 250);
            IsVisible = false;
            PopupClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}

