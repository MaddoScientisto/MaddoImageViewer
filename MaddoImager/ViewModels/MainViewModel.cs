﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MaddoImageLib.Async;

using MaddoServices.Data;
using MaddoServices.Services;
using Microsoft.Maui.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MaddoImager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IAsyncCommand LoadCommand { get; private set; }
        public IAsyncCommand RestartCommand { get; private set; }

        private double _number;

        public double Number
        {
            get { return _number;}
            set
            {
                if (_number != value)
                {
                    _number = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Number"));
                }
            }
        }

        private ObservableCollection<FileData> _files = new ObservableCollection<FileData>();

        public ObservableCollection<FileData> Files
        {
            get { return _files; }
            set
            {
                if (_files != value)
                {
                    _files = Files;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Files"));
                }
            }
        }

        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                if (_currentImage != value)
                {
                    _currentImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentImage"));
                }
            }
        }

        private int _imageIndex = 0;
        private string _fileName = "";

        private readonly IFisherYatesService _fss;
        private readonly IFolderPicker _folderPicker;

        public MainViewModel(IFisherYatesService fss, IFolderPicker folderPicker)
        {
            _fss = fss;
            _folderPicker = folderPicker;
            
            LoadCommand = new AsyncCommand(ExecuteLoadAsync, null, new ErrorHandler());
            RestartCommand = new AsyncCommand(ExecuteRestart, null, new ErrorHandler());
        }

        private async Task ExecuteRestart()
        {
            await Restart();
        }

        private async Task ExecuteLoadAsync()
        {
            Debug.WriteLine("It werks");
            var fol = await _folderPicker.PickFolder();
            var fileTypes = new[] { "*.jpg", "*.png", "*.jpeg", "*.jfif", "*.webp" };

            var data = new List<FileData>();
            foreach (var fileType in fileTypes)
            {
                var res = Directory.EnumerateFiles(fol, fileType, SearchOption.AllDirectories);

                data.AddRange(res.Select(x => new FileData()
                {
                    FileName = Path.GetFileName(x),
                    FullPath = x,
                    FolderPath = Path.GetDirectoryName(x),
                    Date = File.GetLastWriteTime(x)
                }));
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
            _imageIndex = GetNextIndex(_imageIndex, true);
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
           
            var v = new BitmapImage();

            using (var memStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(pic.FullPath)).AsRandomAccessStream())
            {
                await v.SetSourceAsync(memStream);
            }

            CurrentImage = v;

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
    }

    public class ErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
