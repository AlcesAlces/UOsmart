// ==================================================================================================
// ServerList.cs
// ==================================================================================================
//    	1.0 	RunUO Beta 36	    Initial version
//    	1.1 	Mr Fixit	        Now automaticly detects if you are connecting localy and uses the
//				                    servers local ip in the client serverlist.
//	    1.2	    Mr Fixit	        Internet IP autodetection using www.whatismyip.com.
//  	1.3	    Mr Fixit	        If script fails to find the internet ip, keep the old one and try
//				                    again in # minutes.
//      1.4	    Mr Fixit	        You can now add AutoIP mirrors. Added whatismyip.com and myip.com.
//      1.5	    Mr Fixit	        Adjusted the AutoIP mirror engine so it supports more mirrors.
//				                    Added findmyip.com and ipaddy.com.
//      1.6	    Mr Fixit	        IP is now trimmed (Just in case). Added simflex.com, edpsciences.com,
//				                    ipid.shat.net and checkip.dyndns.org.
//      1.7     Mr Fixit            Removed www.whatismyip.com is it seems to be out of buisness.
//      1.8     Mr Fixit            Added a message to the console with ServerList.cs version when server loads.
//				                    Now detects the internet ip when the server starts.
//				                    Now checks if the ip has changed every 60 minutes instead of 30 minutes.
//      1.9     Mr Fixit            Removed noip.com mirror as it isnt working anymore.
//      2.0     Mr Fixit            Now only renews the ip every 24 hours (1440 minutes).
//      2.1     Callandor2k         Made compatible with svn 278.
//              AKA                 Changed the way it writes to console and added a few console lines. 
//              Shai'Tan Malkier    Removed findmyip.com. 3 edits are required. ServerName, Address and Local Lan IP.
//                                  Check the port address on line 208 to make sure its the one you use in SocketOptions.cs.
// ==================================================================================================

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Server;
using Server.Misc;
using Server.Network;

namespace Server.Misc
{
	public class ServerList
	{
		 
		// ==================================================================================
		// YOUR SERVERS NAME
		// ==================================================================================
        public const string ServerName = "Test Server";    // Server Name Goes Here

		// ==================================================================================
		// YOUR INTERNET IP OR DNS NAME
		// Here you can select to autodetect your internet ip, or manualy specify
		// Examples:
		// public static string Address = "12.34.56.78";
		// public static string Address = "shard.host.com";
		// ==================================================================================
		public const bool InternetIPAutodetect = true;
		public const int MinutesBetweenIPAutodetect = 14400;
        public static string Address = "";      // Server Address Goes Here
		
		// ==================================================================================
		// Here are some values stored
		// ==================================================================================
		private static LocalLanIPRange[] LocalLanIPRanges = new LocalLanIPRange[10];		
		private static UInt32 LocalLanIPRangesCount;
		private static AutoIPMirror[] AutoIPMirrors = new AutoIPMirror[10];	
		private static UInt32 AutoIPMirrorsCount;
		private static DateTime InternetIPAutodetectLast;
		
		// ==================================================================================
		// Initialization
		// ==================================================================================
		public static void Initialize()
		{
			// ----------------------------------------------------
			// Load the local LAN ip ranges. 
            // Remove the // on section you need or create your own
			// ----------------------------------------------------
			//AddLocalLANIPRange("10.0.0.0", "10.255.255.255");
			//AddLocalLANIPRange("192.168.1.100", "192.168.1.110");
			//AddLocalLANIPRange("172.16.0.0", "172.32.255.255");
			//AddLocalLANIPRange("169.254.0.0", "169.254.255.255");

			// ----------------------------------------------------
			// Load the Auto IP mirros
			// ----------------------------------------------------
            AddAutoIPMirror("http://www.ipaddy.com/", "Your IP address is: ", "<br>");
            AddAutoIPMirror("http://checkip.dyndns.org/", "Current IP Address: ", "</body>");
            AddAutoIPMirror("http://ipid.shat.net/iponly/", "<title>", "</title>");
			
			// ----------------------------------------------------
			// Create the event
			// ----------------------------------------------------
			EventSink.ServerList += new ServerListEventHandler( EventSink_ServerList );
			
			// ----------------------------------------------------
			// Lets find internet ip
			// ----------------------------------------------------
			DetectInternetIP();
		}

		// ==================================================================================
		// Add a range of local lan ips
		// ==================================================================================
		private static void AddLocalLANIPRange(string RangeFrom, string	RangeTo)
		{
			LocalLanIPRanges[LocalLanIPRangesCount] = new LocalLanIPRange();
			LocalLanIPRanges[LocalLanIPRangesCount].RangeFrom = StringIPToUInt32IP(RangeFrom);
			LocalLanIPRanges[LocalLanIPRangesCount].RangeTo = StringIPToUInt32IP(RangeTo);
			LocalLanIPRangesCount = LocalLanIPRangesCount + 1;
		}
		
		// ==================================================================================
		// Convert a ip string to a binary unsigned int
		// ==================================================================================
		private static UInt32 StringIPToUInt32IP(string addr)
		{
			byte[] byteArray1 = IPAddress.Parse(addr).GetAddressBytes();
			byte[] byteArray2 = IPAddress.Parse(addr).GetAddressBytes();
			byteArray1[0] = byteArray2[3];
			byteArray1[1] = byteArray2[2];
			byteArray1[2] = byteArray2[1];
			byteArray1[3] = byteArray2[0];
			return  BitConverter.ToUInt32( byteArray1, 0);
		}
		
		// ==================================================================================
		// Used to store the local lan ip ranges
		// ==================================================================================
		private class LocalLanIPRange
		{
			public UInt32			RangeFrom;
			public UInt32			RangeTo;
		}	

		// ==================================================================================
		// Add a AutoIP mirror
		// ==================================================================================
		private static void AddAutoIPMirror(string sURL, string	sStart, string sEnd)
		{
			AutoIPMirrors[AutoIPMirrorsCount] = new AutoIPMirror();
			AutoIPMirrors[AutoIPMirrorsCount].sURL = sURL;
			AutoIPMirrors[AutoIPMirrorsCount].sStart = sStart;
			AutoIPMirrors[AutoIPMirrorsCount].sEnd = sEnd;
			AutoIPMirrorsCount = AutoIPMirrorsCount + 1;
		}

		// ==================================================================================
		// Used to store the Auto IP mirrors
		// ==================================================================================
		private class AutoIPMirror
		{
			public string			sURL;
			public string			sStart;
			public string			sEnd;
			public UInt32			iFailures;
		}

		// ==================================================================================
		// Detect ip
		// ==================================================================================
		public static void DetectInternetIP()
		{

			// ----------------------------------------------------
			// Autodetect the Internet IP
			// ----------------------------------------------------
			if (InternetIPAutodetect) {
				DateTime UpdateTime = InternetIPAutodetectLast;
				UpdateTime = UpdateTime.AddMinutes(MinutesBetweenIPAutodetect);
				if (UpdateTime<DateTime.Now) {
					string NewAddress = null;
					NewAddress = FindInternetIP();
					InternetIPAutodetectLast = DateTime.Now;
					if (NewAddress!=null) 
					{
						Address = NewAddress;
					}
				}
			}
		}

		// ==================================================================================
		// The serverlist event
		// ==================================================================================
		public static void EventSink_ServerList( ServerListEventArgs e )
		{
			try
			{

				// ----------------------------------------------------
				// Lets find internet ip
				// ----------------------------------------------------
				DetectInternetIP();
				
				// ----------------------------------------------------
				// Find the server ip to use for this user
				// ----------------------------------------------------
				IPAddress ipAddr = FindMachineIP(e);
				
				// ----------------------------------------------------
				// Send serverlist
				// ----------------------------------------------------
				if (ipAddr != null)
				{
                    e.AddServer(ServerName, new IPEndPoint(ipAddr, 2593 ));//  Place the port that you are using here.
				} else {
					e.Rejected = true;
				}
			}
			catch
			{
				e.Rejected = true;
			}
		}

		// ==================================================================================
		// Connects to a webserver that gives you your internet ip
		// ==================================================================================
                public static string FindInternetIP( )
		{

			// ----------------------------------------------------
			// Pick a random mirror
			// ----------------------------------------------------
			Random rnd = new Random();
			int UseMirror = (int)( rnd.NextDouble() * AutoIPMirrorsCount);
			string MyIP = "";
			
			// ----------------------------------------------------
			// Catch if the mirror is down
			// ----------------------------------------------------
			try
			{
				// ----------------------------------------------------
				// Get the webpage
				// ----------------------------------------------------
        	    		WebClient client = new WebClient();
            			byte[] pageData = client.DownloadData(AutoIPMirrors[UseMirror].sURL);
            			MyIP = System.Text.Encoding.ASCII.GetString(pageData);
            		
				// ----------------------------------------------------
				// Find the string
				// ----------------------------------------------------
                        	int iStart = MyIP.LastIndexOf(AutoIPMirrors[UseMirror].sStart);   
                        	int iEnd = MyIP.IndexOf(AutoIPMirrors[UseMirror].sEnd, iStart+AutoIPMirrors[UseMirror].sStart.Length);
                        	MyIP = MyIP.Substring(iStart+AutoIPMirrors[UseMirror].sStart.Length, iEnd-iStart-AutoIPMirrors[UseMirror].sStart.Length );
                        	MyIP = MyIP.Trim();
                        	
				// ----------------------------------------------------
				// Return value
				// ----------------------------------------------------
                            Console.WriteLine("-------------------------------------------------------------------------------");
                            Console.WriteLine("Serverlist.cs: 2.1");
                            Console.WriteLine("{0} ({1})", ServerName, Address);
                            Console.WriteLine("You are using Internet IP : {0}", MyIP);
                            Console.WriteLine("IP found by: ({0})", AutoIPMirrors[UseMirror].sURL);
                            Console.WriteLine("-------------------------------------------------------------------------------");
        	                return MyIP;
			}
			catch
			{
				Console.WriteLine("Unable to autoupdate the Internet IP from {0}!", AutoIPMirrors[UseMirror].sURL);
				Console.WriteLine("----------------------------------------------------------------------");
				Console.WriteLine(MyIP);
				Console.WriteLine("----------------------------------------------------------------------");
				return null;
			}
        	}

		// ==================================================================================
		// Calculates what server IP to use
		// ==================================================================================
                public static IPAddress FindMachineIP( ServerListEventArgs e )
		{
			// ----------------------------------------------------
			// Find the IP of the connecting user
			// ----------------------------------------------------
			Socket sock = e.State.Socket;
			IPAddress theirAddress = ((IPEndPoint)sock.RemoteEndPoint).Address;				
			IPAddress serverAddress;

			// ----------------------------------------------------
			// Is it Loopback?
			// ----------------------------------------------------
			if ( IPAddress.IsLoopback( theirAddress ) )
			{
				return IPAddress.Parse( "127.0.0.1" );
			}

			// ----------------------------------------------------
			// Local
			// ----------------------------------------------------
			UInt32 uint32Address = StringIPToUInt32IP(theirAddress.ToString());
			for (UInt32 LocalLanIPRangesLoop = 0 ; LocalLanIPRangesLoop < LocalLanIPRangesCount; LocalLanIPRangesLoop++)
			{	
				if ( (LocalLanIPRanges[LocalLanIPRangesLoop].RangeFrom <= uint32Address) && (LocalLanIPRanges[LocalLanIPRangesLoop].RangeTo >= uint32Address) )
				{
					Resolve(Dns.GetHostName(), out serverAddress);
					
					Console.WriteLine("Player is reconnecting to " + serverAddress.ToString());
					return serverAddress;
				}
			}

			// ----------------------------------------------------
			// Internet addresses
			// ----------------------------------------------------
			if (Address!=null)
			{
				Resolve(Address, out serverAddress);	
			} else {
				Resolve(Dns.GetHostName(), out serverAddress);	
			}
			
			Console.WriteLine("Player is reconnecting to " + serverAddress.ToString());
			return serverAddress;
		}
		
		// ==================================================================================
		// Resolves dns names
		// ==================================================================================
		public static bool Resolve( string addr, out IPAddress outValue )
		{
			try
			{
				outValue = IPAddress.Parse( addr );
				return true;
			}
			catch
			{
				try
				{
				IPHostEntry iphe = Dns.GetHostEntry( addr );

					if ( iphe.AddressList.Length > 0 )
					{
						outValue = iphe.AddressList[iphe.AddressList.Length - 1];
						return true;
					}
				}
				catch
				{
				}
			}
			outValue = IPAddress.None;
			return false;
		}
	}
}