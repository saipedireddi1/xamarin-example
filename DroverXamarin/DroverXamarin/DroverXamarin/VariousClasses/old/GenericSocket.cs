/*
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace DroverXamarin
{
	public class GenericSocket
	{
		public static byte[] bAray = { 127, 0, 0, 1 };
		static IPAddress ipAddress = new IPAddress(bAray);
		static IPEndPoint remoteEP = new IPEndPoint(ipAddress, 16999);

		public GenericSocket()
		{
			/*
			Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
			s.Connect(remoteEP);
			s.Send(bits);
			s.Shutdown(SocketShutdown.Both);
			s.Close();
			*/
		}


		public void downloadPicture(string uid)
		{
			Task t = Task.Factory.StartNew(() =>
			{
				//Console.WriteLine(pic.Length);
				byte[] pic = Encoding.ASCII.GetBytes(uid);
				//Console.WriteLine(pic.Length);
				string length = FireAuth.auth.FirebaseToken.Length.ToString();
				//Console.WriteLine(length);
				int padding = 10 - length.Length;
				string message = "";
				for (int x = 0; x < padding; x++)
				{
					message += "0";
				}
				message += length;
				message += FireAuth.auth.FirebaseToken;
				length = pic.Length.ToString();
				padding = 10 - length.Length;
				for (int x = 0; x < padding; x++)
				{
					message += "0";
				}
				message += length;
				byte[] toBytes = Encoding.ASCII.GetBytes(message);
				byte[] rv = new byte[toBytes.Length + pic.Length + 1];
				rv[0] = (byte)1;
				System.Buffer.BlockCopy(toBytes, 0, rv, 1, toBytes.Length);
				System.Buffer.BlockCopy(pic, 0, rv, toBytes.Length + 1, pic.Length);


				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				s.Send(rv);
				byte[] size = new byte[10];
				s.Receive(size);
				string something = Encoding.ASCII.GetString(size);
				Console.WriteLine(something);
				int sizeInt = Int32.Parse(something);
				byte[] pictureData = new byte[sizeInt];
				s.Receive(pictureData);
				PCLStorageSample(pictureData);
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public async Task PCLStorageSample(byte[] bytes)
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder folder = await rootFolder.CreateFolderAsync("MySubFolder",
				CreationCollisionOption.OpenIfExists);
			IFile file = await folder.CreateFileAsync("myfile.jpg", CreationCollisionOption.ReplaceExisting);
			using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}
*/