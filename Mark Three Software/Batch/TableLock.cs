namespace MarkThree
{

	using System;
	using System.Threading;

	[System.Diagnostics.DebuggerStepThrough()]
	public class TableLock
	{
        
		private string name;
        
		private ReaderWriterLock readerWriterLock;
        
		public TableLock(string name)
		{
			this.name = name;
			this.readerWriterLock = new ReaderWriterLock();
		}
        
		public string Name
		{
			get
			{
				return this.name;
			}
		}
        
		public bool IsReaderLockHeld
		{
			get
			{
				return this.readerWriterLock.IsReaderLockHeld;
			}
		}
        
		public bool IsWriterLockHeld
		{
			get
			{
				return this.readerWriterLock.IsWriterLockHeld;
			}
		}
        
		public int WriterSeqNum
		{
			get
			{
				return this.readerWriterLock.WriterSeqNum;
			}
		}
        
		public void AcquireReaderLock(int millisecondsTimeout)
		{
			this.readerWriterLock.AcquireReaderLock(millisecondsTimeout);
		}
        
		public void AcquireReaderLock(System.TimeSpan timeout)
		{
			this.readerWriterLock.AcquireReaderLock(timeout);
		}
        
		public void AcquireWriterLock(int millisecondsTimeout)
		{
			this.readerWriterLock.AcquireWriterLock(millisecondsTimeout);
		}
        
		public void AcquireWriterLock(System.TimeSpan timeout)
		{
			this.readerWriterLock.AcquireWriterLock(timeout);
		}
        
		public void AnyWritersSince(int seqNum)
		{
			this.readerWriterLock.AnyWritersSince(seqNum);
		}
        
		public void DowngradeFromWriterLock(ref LockCookie lockCookie)
		{
			this.readerWriterLock.DowngradeFromWriterLock(ref lockCookie);
		}
        
		public LockCookie ReleaseLock()
		{
			return this.readerWriterLock.ReleaseLock();
		}
        
		public void ReleaseReaderLock()
		{
			this.readerWriterLock.ReleaseReaderLock();
		}
        
		public void ReleaseWriterLock()
		{
			this.readerWriterLock.ReleaseWriterLock();
		}
        
		public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
		{
			return this.readerWriterLock.UpgradeToWriterLock(millisecondsTimeout);
		}
        
		public LockCookie UpgradeToWriterLock(System.TimeSpan timeout)
		{
			return this.readerWriterLock.UpgradeToWriterLock(timeout);
		}
	}
    
}
