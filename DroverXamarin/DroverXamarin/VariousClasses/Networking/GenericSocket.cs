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
		public static byte[] bAray = { 104,200,31,123 };
		string linode = "2600:3c03::f03c:91ff:febb:f7fe";
		static IPAddress ipAddress = IPAddress.Parse("104.200.31.123");
		static IPEndPoint remoteEP = new IPEndPoint(ipAddress, 16999);

		public GenericSocket()
		{
			
			Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
			s.Connect(remoteEP);


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

		public static Task GET_DROVER(double lat, double lng)
		{
			return Task.Factory.StartNew(() =>
			{
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "GET_DROVER:" + lat + "," + lng + "," + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public static Task CHANGE_DROP_OFF_LOCATION(double lat, double lng)
		{
			return Task.Factory.StartNew(() =>
			{
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "CHANGE_DROP_OFF_LOCATION:" + lat + "," + lng + "," + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public static Task PROPOSAL_RESPONSE(bool respo)
		{
			return Task.Factory.StartNew(() =>
			{
				int resp = respo ? 1 : 0;
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "PROPOSAL_RESPONSE:" + resp + "," + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public static Task CONFIRM_ARRIVAL(string rideID)
		{
			return Task.Factory.StartNew(() =>
			{
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "CONFIRM_ARRIVAL:" + rideID + "," + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public static Task CONFIRM_DROP_OFF(string rideID)
		{
			return Task.Factory.StartNew(() =>
			{
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "CONFIRM_DROP_OFF:" + rideID + "," + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}

		public static Task REQUEST_DRIVER_MODE()
		{
			return Task.Factory.StartNew(() =>
			{
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				string message = "REQUEST_DRIVER_MODE:" + FireAuth.getFirebaseToken() + "!";
				s.Send(getBytes(message));
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			});
		}



		public Task<string> GET_BRAINTREE_TOKEN()
		{
			return Task.Factory.StartNew(() =>
			{
				string message = "GET_BRAINTREE_CLIENT_TOKEN:" + FireAuth.getFirebaseToken() + "!";
				return sendMessageForResponse(message).Result;
			});
		}

		public async Task PCLStorageSample(byte[] bytes)
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder folder = await rootFolder.CreateFolderAsync("e45Ds",
				CreationCollisionOption.OpenIfExists);
			IFile file = await folder.CreateFileAsync("myfile.jpg", CreationCollisionOption.ReplaceExisting);
			using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		public Task<string> sendMessageForResponse(string message)
		{
			return Task.Factory.StartNew(() =>
			{
				string response = "";
				Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
				s.Connect(remoteEP);
				s.Send(getBytes(message));

				byte[] b = new byte[10000];
				int k = s.Receive(b);
				response = Encoding.ASCII.GetString(b, 0, k);

				return response;
			});
		}

		public static byte[] getBytes(string val)
		{
			return Encoding.ASCII.GetBytes(val);
		}
	}
}
