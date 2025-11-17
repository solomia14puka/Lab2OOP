using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Xml.Linq;
using Lab2.Models;
using Lab2.Services;
using Lab2.Utils;
using Contact = Lab2.Models.Contact;


namespace Lab2.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly XmlSearchContext _searchContext;
        private readonly SaxSearchStrategy _sax;
        private readonly DomSearchStrategy _dom;
        private readonly LinqToXmlSearchStrategy _linq;
        private readonly XslTransformer _transformer;

        public MainViewModel(XmlSearchContext ctx, SaxSearchStrategy sax, DomSearchStrategy dom,
                             LinqToXmlSearchStrategy linq, XslTransformer transformer)
        {
            _searchContext = ctx; _sax = sax; _dom = dom; _linq = linq; _transformer = transformer;

            PickXmlCommand = new DelegateCommand(async _ => await PickXml());
            PickXslCommand = new DelegateCommand(async _ => await PickXsl());
            AnalyzeCommand = new DelegateCommand(async _ => await Analyze(), _ => CanAnalyze);
            TransformCommand = new DelegateCommand(async _ => await Transform(), _ => CanTransform);
            ClearCommand = new DelegateCommand(async _ => { Clear(); await Task.CompletedTask; });

            SelectedStrategy = Strategies.FirstOrDefault();
        }

        //Стан
        private string? _xmlPath;
        public string? XmlPath { get => _xmlPath; set { _xmlPath = value; OnPropertyChanged(); UpdateCanExecutes(); } }

        private string? _xslPath;
        public string? XslPath { get => _xslPath; set { _xslPath = value; OnPropertyChanged(); UpdateCanExecutes(); } }

        private string? _keyword;
        public string? Keyword { get => _keyword; set { _keyword = value; OnPropertyChanged(); UpdateCanExecutes(); } }

        private string? _selectedStrategy;
        public string? SelectedStrategy { get => _selectedStrategy; set { _selectedStrategy = value; OnPropertyChanged(); SelectStrategy(); UpdateCanExecutes(); } }

        public ObservableCollection<string> Strategies { get; } =
            new(new[] { "SAX (XmlReader)", "DOM (XmlDocument)", "LINQ to XML (XDocument)" });

        public ObservableCollection<string> AttributeNames { get; } = new();
        public ObservableCollection<string> AttributeValues { get; } = new();

        private string? _selectedAttributeName;
        public string? SelectedAttributeName
        {
            get => _selectedAttributeName;
            set { _selectedAttributeName = value; OnPropertyChanged(); LoadAttributeValues(); UpdateCanExecutes(); }
        }

        private string? _selectedAttributeValue;
        public string? SelectedAttributeValue { get => _selectedAttributeValue; set { _selectedAttributeValue = value; OnPropertyChanged(); UpdateCanExecutes(); } }

        public ObservableCollection<Contact> Results { get; } = new();

        //Команди
        public ICommand PickXmlCommand { get; }
        public ICommand PickXslCommand { get; }
        public ICommand AnalyzeCommand { get; }
        public ICommand TransformCommand { get; }
        public ICommand ClearCommand { get; }

        //Логіка
        private async Task PickXml()
        {
            var xmlType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                [DevicePlatform.WinUI] = new[] { ".xml" },
                [DevicePlatform.Android] = new[] { "text/xml", "application/xml" },
                [DevicePlatform.iOS] = new[] { "public.xml" },
                [DevicePlatform.MacCatalyst] = new[] { "public.xml" }
            });

            var res = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = xmlType,
                PickerTitle = "Оберіть XML-файл"
            });

            if (res != null)
            {
                XmlPath = res.FullPath;
                await LoadAttributeNames();
            }
        }

        private async Task PickXsl()
        {
            var res = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    [DevicePlatform.WinUI] = new[] { ".xsl", ".xslt" },
                    [DevicePlatform.Android] = new[] { "application/xml" },
                    [DevicePlatform.MacCatalyst] = new[] { "public.xml" },
                    [DevicePlatform.iOS] = new[] { "public.xml" }
                }),
                PickerTitle = "Оберіть XSL-файл"
            });
            if (res != null) XslPath = res.FullPath;
            await Task.CompletedTask;
        }

        private async Task LoadAttributeNames()
        {
            AttributeNames.Clear();
            AttributeValues.Clear();
            if (string.IsNullOrWhiteSpace(XmlPath)) return;
            try
            {
                var x = XDocument.Load(XmlPath);
                var names = x.Root!.Elements("contact")
                    .SelectMany(e => e.Attributes().Select(a => a.Name.LocalName))
                    .Distinct()
                    .OrderBy(n => n);
                foreach (var n in names) AttributeNames.Add(n);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void LoadAttributeValues()
        {
            AttributeValues.Clear();
            if (string.IsNullOrWhiteSpace(XmlPath) || string.IsNullOrWhiteSpace(SelectedAttributeName)) return;
            try
            {
                var x = XDocument.Load(XmlPath);
                var values = x.Root!.Elements("contact")
                    .Attributes(SelectedAttributeName)
                    .Select(a => a.Value)
                    .Distinct()
                    .OrderBy(v => v);
                foreach (var v in values) AttributeValues.Add(v);
            }
            catch (Exception ex)
            {
                _ = Application.Current.MainPage?.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void SelectStrategy()
        {
            switch (SelectedStrategy)
            {
                case "SAX (XmlReader)": _searchContext.SetStrategy(_sax); break;
                case "DOM (XmlDocument)": _searchContext.SetStrategy(_dom); break;
                default: _searchContext.SetStrategy(_linq); break;
            }
        }

        private async Task Analyze()
        {
            Results.Clear();
            if (XmlPath is null) return;
            try
            {
                var list = _searchContext.Search(XmlPath, Keyword, SelectedAttributeName, SelectedAttributeValue);
                foreach (var item in list) Results.Add(item);
                if (Results.Count == 0)
                    await Application.Current.MainPage.DisplayAlert("Результат", "Нічого не знайдено.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");

            }
        }

        private async Task Transform()
        {
            if (XmlPath is null || XslPath is null) return;
            try
            {
                var outPath = _transformer.TransformToHtml(XmlPath, XslPath);

                await Application.Current.MainPage.DisplayAlert(
                    "Готово",
                    $"HTML збережено: {outPath}",
                    "OK");

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(outPath)
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void Clear()
        {
            Keyword = string.Empty;
            SelectedAttributeName = null;
            SelectedAttributeValue = null;
            Results.Clear();
        }

        private bool CanAnalyze => !string.IsNullOrWhiteSpace(XmlPath);
        private bool CanTransform => !string.IsNullOrWhiteSpace(XmlPath) && !string.IsNullOrWhiteSpace(XslPath);

        private void UpdateCanExecutes()
        {
            (AnalyzeCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            (TransformCommand as DelegateCommand)?.RaiseCanExecuteChanged();
        }
    }

}