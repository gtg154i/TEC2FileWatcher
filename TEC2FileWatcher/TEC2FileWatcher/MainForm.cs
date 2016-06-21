/*
 * Created by SharpDevelop.
 * User: Mike2012Old
 * Date: 5/10/2016
 * Time: 9:55 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;

namespace TEC2FileWatcher
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private NotifyIcon  trayIcon;
		private ContextMenu trayMenu;
		
		private void OnExit(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Release the icon resource.
				trayIcon.Dispose();
			}
			
			base.Dispose(isDisposing);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Visible       = false; // Hide form window.
			ShowInTaskbar = false; // Remove from taskbar.
			
			base.OnLoad(e);
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			trayMenu = new ContextMenu();
			trayMenu.MenuItems.Add("Exit", OnExit);
			// Create a tray icon. In this example we use a
			// standard system icon for simplicity, but you
			// can of course use your own custom icon too.
			trayIcon      = new NotifyIcon();
			trayIcon.Text = "TEC2FileWatcher";
			//trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
			trayIcon.Icon = new Icon(SystemIcons.Shield, 40, 40);
			
			// Add menu to tray icon and show it.
			trayIcon.ContextMenu = trayMenu;
			trayIcon.Visible     = true;
			
			//FileWatcher Code here
			//FileSystemWatcher watcherGameData = new FileSystemWatcher();
			//watcherGameData.Path = @"C:\Users\Mike2012\AppData\Roaming\okshur\Enchanted Cave 2\TEC2GameData.sol";
			
			//FileSystemWatcher watcherSlot0 = new FileSystemWatcher();
			//watcherSlot0.Path = @"C:\Users\Mike2012\AppData\Roaming\okshur\Enchanted Cave 2\TEC2Slot0.sol";
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = @"C:\Users\Mike2012\AppData\Roaming\okshur\Enchanted Cave 2\";
			//watcher.Path = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\okshur\Enchanted Cave 2\";
			//this makes it go to Mike2012Old so hardcoded for now
			
			/* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			
			// Only watch text files.
			watcher.Filter = "*.sol";
			
			// Add event handlers.
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			
			// Begin watching.
			watcher.EnableRaisingEvents = true;
		}
		
		public bool IsFileClosed(string filename)
		{
			try
			{
				using (var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					return true;
				}
			}
			catch (IOException)
			{
				return false;
			}
		}
		
		//private DateTime lastRead = DateTime.MinValue;
		//private int fireCount = 0;
		//ConcurrentDictionary<string, int> firecount = new ConcurrentDictionary<string, int>();
		
		ConcurrentDictionary<string, DateTime> lastRead = new ConcurrentDictionary<string, DateTime>();
		
		// Define the event handlers.
		private void OnChanged(object source, FileSystemEventArgs e)
		{
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
			
			if(!lastRead.ContainsKey(e.FullPath))
				lastRead.TryAdd(e.FullPath, DateTime.MinValue);
				
			if (lastWriteTime != lastRead[e.FullPath])
			{
				lastRead[e.FullPath] = lastWriteTime;
				{
					while (!IsFileClosed(e.FullPath))
					{
						Thread.Sleep(1000);
					}
					
					System.IO.FileInfo file = new System.IO.FileInfo(@"C:\Users\Mike2012\AppData\Roaming\okshur\Enchanted Cave 2\Backup\");
					file.Directory.Create();
					
					//String backupname = e.FullPath + " " + String.Format("{0:yyyyMMddtHHmmss}", DateTime.Now);
					
					String backupname = e.FullPath;//  + String.Format("{0:yyyyMMddtHHmmss}", DateTime.Now) + " ";
					backupname = backupname.Replace("Enchanted Cave 2\\","Enchanted Cave 2\\Backup\\");
					backupname = backupname.Replace(e.Name, String.Format("{0:yyyyMMddtHHmmss}", DateTime.Now) + " " + e.Name);
					File.Copy(e.FullPath, backupname, true); // "20080903t160507);
				}
			}
		}
	}
}
