using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MaddoImageLib.Async;

using MaddoServices.Data;
using MaddoServices.Services;


namespace MaddoImager.ViewModels
{
    public class MainViewModel : ObservableViewModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;



        private double _number;

        public double Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        private ObservableCollection<FileData> _files = new ObservableCollection<FileData>();

        public ObservableCollection<FileData> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
           
        }

        private ImageSource _currentImage;
        public ImageSource CurrentImage
        {
            get => _currentImage; set => SetProperty(ref _currentImage, value);
        }

        private FileData _currentFileData = new FileData();

        public FileData CurrentFileData
        {
            get => _currentFileData; set => SetProperty(ref _currentFileData, value);
        }

        //private int _viewedCount;
        public int ViewedCount
        {
            get => CurrentFileData?.ViewdCount ?? 0;
            //set => SetProperty(ref CurrentFileData.ViewdCount, value);
        }

        //public void OnPropertyChanged([CallerMemberName] string name = "") =>
        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _imageIndex = 0;
        private string _fileName = "";

        private string _dbPath = "";

        private readonly IFisherYatesService _fss;
        private readonly IFolderPicker _folderPicker;
        private readonly IDbService _dbService;

        public MainViewModel(IFisherYatesService fss, IFolderPicker folderPicker, IDbService dbService)
        {
            _fss = fss;
            _folderPicker = folderPicker;
            _dbService = dbService;

            _dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "images.db");

            InitCommands();
        }

        #region Commands

        public IAsyncCommand LoadCommand { get; private set; }
        public IAsyncCommand RestartCommand { get; private set; }
        public IAsyncCommand PreviousCommand { get; private set; }
        public IAsyncCommand NextCommand { get; private set; }
        public IAsyncCommand ShuffleCommand { get; private set; }

        private void InitCommands()
        {
            LoadCommand = new AsyncCommand(ExecuteLoadAsync, null, new ErrorHandler());
            RestartCommand = new AsyncCommand(ExecuteRestart, null, new ErrorHandler());
            NextCommand = new AsyncCommand(ExecuteNextAsync, null, new ErrorHandler());
            PreviousCommand = new AsyncCommand(ExecutePreviousAsync, null, new ErrorHandler());

            ShuffleCommand = new AsyncCommand(ExecuteShuffleAsync, null, new ErrorHandler());


        }

        private async Task ExecuteRestart()
        {
            await Restart();
        }

        private async Task ExecuteLoadAsync()
        {
            await Load();            
        }

        private async Task ExecuteNextAsync()
        {
            await ShowNext();
        }

        private async Task ExecutePreviousAsync()
        {
            await ShowPrevious();
        }

        private async Task ExecuteShuffleAsync()
        {
            await Shuffle();
        }
        #endregion

        private async Task Load()
        {
            Debug.WriteLine("It werks");
            var fol = await _folderPicker.PickFolder();
            var fileTypes = new[] { "*.jpg", "*.png", "*.jpeg", "*.jfif", "*.webp" };

            var data = new List<FileData>();
            foreach (var fileType in fileTypes)
            {
                var res = Directory.EnumerateFiles(fol, fileType, SearchOption.AllDirectories);

                data.AddRange(res.Select(x => {

                    var fd = _dbService.GetFileData(_dbPath, x);

                    var asd = new FileData()
                    {
                        FileName = Path.GetFileName(x),
                        FullPath = x,
                        FolderPath = Path.GetDirectoryName(x),
                        Date = File.GetLastWriteTime(x)
                    };

                    if (fd != null) {

                        asd.ViewdCount = fd.ViewdCount;
                        asd.LastSeen = fd.LastSeen;
                    }
                    else
                    {
                        asd.ViewdCount = 0;
                        Debug.WriteLine($"New pic found: {x}");
                    }

                    return asd;
                    }
                ));
            }
            Files.Clear();

            foreach (var file in data.OrderBy(x => x.Date))
            {
                Files.Add(file);
            }

            await Restart();
        }

        private async Task Restart()
        {
            _imageIndex = -1;
            await ShowNext();
        }

        private async Task ShowNext()
        {
            if (_imageIndex != -1 && Files[_imageIndex] != null) {

                Save();
            }

            _imageIndex = GetNextIndex(_imageIndex, true);
            await ShowImage(Files[_imageIndex]);
        }

        private async Task ShowPrevious()
        {

            if (_imageIndex != -1 && Files[_imageIndex] != null)
            {
                Save();
            }

            _imageIndex = GetNextIndex(_imageIndex, false);
            await ShowImage(Files[_imageIndex]);
        }

        private int GetNextIndex(int currentIndex, bool isForward)
        {
            if (isForward)
            {
                if (currentIndex + 1 >= Files.Count())
                {
                    return 0;
                }
                else
                {
                    return currentIndex += 1;
                }
            }
            else
            {
                if (currentIndex - 1 < 0)
                {
                    return Files.Count() - 1;
                }
                else
                {
                    return currentIndex - 1;
                }
            }
        }

        private async Task ShowImage(FileData pic)
        {
            _fileName = pic.FileName;
            //CurrentImage = CurrentImage;


            CurrentImage = ImageSource.FromStream(() => new MemoryStream(System.IO.File.ReadAllBytes(pic.FullPath)));
            CurrentFileData = pic;


            
            pic.ViewdCount++;
            PropertyChanged?.Invoke(ViewedCount, new PropertyChangedEventArgs("ViewedCount"));
            pic.LastSeen = DateTime.Now;


            //var v = new BitmapImage();

            //using (var memStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(pic.FullPath)).AsRandomAccessStream())
            //{
            //    await v.SetSourceAsync(memStream);
            //}

            //CurrentImage = v;

            //var imageBytes = await System.IO.File.ReadAllBytesAsync(pic.FullPath);
            //var base64String = Convert.ToBase64String(imageBytes);

            //if (pic.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase))
            //{
            //    imageSource = $"data:image/jpg;base64,{base64String}";
            //}
            //else if (pic.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            //{
            //    imageSource = $"data:image/png;base64,{base64String}";
            //}
        }

        private async Task Shuffle()
        {
            var resArray = Files.ToArray();
            _fss.Shuffle(resArray);

            Files = new ObservableCollection<FileData>(resArray);
            
            await Restart();

        }

        private void Save()
        {
            var cur = Files[_imageIndex];
            
            _dbService.Save(_dbPath, cur);

        }
    }

    public class ErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
