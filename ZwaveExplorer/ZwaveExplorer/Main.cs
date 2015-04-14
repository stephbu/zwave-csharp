using System;
using System.IO;
using System.Net;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

namespace ZwaveExplorer
{
	class MainClass
	{
		private static ManualResetEvent exitConsole;
		private static ZwaveListener listener;

		public static void Main (string[] args)
		{
			exitConsole = new ManualResetEvent (false);
			listener = new ZwaveListener ("/dev/tty.SLAB_USBtoUART");
			try
			{
				listener.OnMessage += HandleMessage;
				Console.CancelKeyPress += HandleCancelKeyPress;

				listener.Start();

				// Send discovery
				listener.Send(new Discover());

				exitConsole.WaitOne ();
			}
			finally
			{
				exitConsole.Dispose();
			}
		}

		static bool HandleMessage(Message msg)
		{
			return false;
		}

		static void HandleCancelKeyPress (object sender, ConsoleCancelEventArgs e)
		{
			exitConsole.Set ();
		}
	}	
}
