using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace FTP_ProgressBar
{
	/// <summary>
	/// The FtpProgress class is derived from background worker, so it has the built-in functions of 
	/// run, report progress, and completed. 
	/// It only does upload, as a demonstration of how to transfer a file by FTP and report the progress. 
	/// It is left as an exercise for the reader :) to implement download if you want...
	/// </summary>
	public partial class FtpProgress : BackgroundWorker
	{
		public FtpProgress()
		{
			InitializeComponent();
		}

		public FtpProgress(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
		}

		private void FtpProgress_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = sender as BackgroundWorker;
			FtpSettings f = e.Argument as FtpSettings;
			// set up the host string to request.  this includes the target folder and the target file name (based on the source filename)
			string UploadPath = String.Format("{0}/{1}{2}", f.Host, f.TargetFolder == "" ? "" : f.TargetFolder + "/", Path.GetFileName(f.SourceFile));
			if(!UploadPath.ToLower().StartsWith("ftp://"))
				UploadPath = "ftp://" + UploadPath;
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(UploadPath);
			request.UseBinary = true;
			request.UsePassive = f.Passive;
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential(f.Username, f.Password);

			// Copy the contents of the file to the request stream.
			long FileSize = new FileInfo(f.SourceFile).Length;
			string FileSizeDescription = GetFileSize(FileSize); // e.g. "2.4 Gb" instead of 240000000000000 bytes etc...			
			int ChunkSize = 4096, NumRetries = 0, MaxRetries = 50;
			long SentBytes = 0;
			byte[] Buffer = new byte[ChunkSize];    // this buffer stores each chunk, for sending to the web service via MTOM
			using(Stream requestStream = request.GetRequestStream())
			{
				using(FileStream fs = File.Open(f.SourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					int BytesRead = fs.Read(Buffer, 0, ChunkSize);	// read the first chunk in the buffer
					// send the chunks to the web service one by one, until FileStream.Read() returns 0, meaning the entire file has been read.
					while(BytesRead > 0)
					{
						try
						{
							if(bw.CancellationPending)
								return;

							// send this chunk to the server.  it is sent as a byte[] parameter, but the client and server have been configured to encode byte[] using MTOM. 
							requestStream.Write(Buffer, 0, BytesRead);

							// sentBytes is only updated AFTER a successful send of the bytes. so it would be possible to build in 'retry' code, to resume the upload from the current SentBytes position if AppendChunk fails.
							SentBytes += BytesRead;

							// update the user interface
							string SummaryText = String.Format("Transferred {0} / {1}", GetFileSize(SentBytes), FileSizeDescription);
							bw.ReportProgress((int)(((decimal)SentBytes / (decimal)FileSize) * 100), SummaryText);
						}
						catch(Exception ex)
						{
							Debug.WriteLine("Exception: " + ex.ToString());
							if(NumRetries++ < MaxRetries)
							{
								// rewind the filestream and keep trying
								fs.Position -= BytesRead;
							}
							else
							{
								throw new Exception(String.Format("Error occurred during upload, too many retries. \n{0}", ex.ToString()));
							}
						}
						BytesRead = fs.Read(Buffer, 0, ChunkSize);	// read the next chunk (if it exists) into the buffer.  the while loop will terminate if there is nothing left to read
					}
				}
			}
			using(FtpWebResponse response = (FtpWebResponse)request.GetResponse())
				System.Diagnostics.Debug.WriteLine(String.Format("Upload File Complete, status {0}", response.StatusDescription));
		}



		/// <summary>
		/// Returns a description of a number of bytes, in appropriate units.
		/// e.g. 
		///		passing in 1024 will return a string "1 Kb"
		///		passing in 1230000 will return "1.23 Mb"
		/// Megabytes and Gigabytes are formatted to 2 decimal places.
		/// Kilobytes are rounded to whole numbers.
		/// If the rounding results in 0 Kb, "1 Kb" is returned, because Windows behaves like this also.
		/// </summary>
		public static string GetFileSize(long numBytes)
		{
			string fileSize = "";

			if(numBytes > 1073741824)
				fileSize = String.Format("{0:0.00} Gb", (double)numBytes / 1073741824);
			else if(numBytes > 1048576)
				fileSize = String.Format("{0:0.00} Mb", (double)numBytes / 1048576);
			else
				fileSize = String.Format("{0:0} Kb", (double)numBytes / 1024);

			if(fileSize == "0 Kb")
				fileSize = "1 Kb";	// min.							
			return fileSize;
		}
	}

	public class FtpSettings
	{
		public string Host, Username, Password, TargetFolder, SourceFile;
		public bool Passive;
		public int Port = 21;
	}
}
