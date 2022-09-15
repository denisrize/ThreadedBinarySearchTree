Class project of multi threaded BST ( multiple clients use the same resource parallel).
I used Reader/Writer lock of type “ReaderWriterLockSlim” that fits best for this project,
you can find documentation about it here - https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=net-6.0 .
