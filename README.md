**Class project** of multi threaded BST ( multiple clients use the same resource parallel).
I used Reader/Writer lock of type “ReaderWriterLockSlim” that fits best for this project,
you can find documentation about it here - [ReaderWriterLockSlim Info ](https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=net-6.0) .

### Quick Summry
The idea here is that each Node contains a lock and after the first lock is obtained on a node, the traversal will additionally lock the desired child before releasing the lock on the parent node. This ensures that the traversal always has a lock on a valid node, which would not be the case if the parent lock was released prematurely and the child node was removed before a lock could be obtained on it.
