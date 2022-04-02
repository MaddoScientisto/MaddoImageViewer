This is a blazor app hoster in .net MAUI, it's still in early in development as I'm figuring out the API, I don't havea Minimum Viable Product yet but I hope I can have it very soon.

The intent of the app is to have a simple image viewer for folder slideshows, the images are going to be randomized through a Fisher-Yates algorithm at first and the viewed images will be kept track of.

On subsequent viewings of the folder the program will try to show the least viewed images first (I still have to determine what to do with the algorithm but I'll probably run Fisher-Yates on the images that have yet to be seen first and then run it on the already seen ones and append them to the end of the list)
