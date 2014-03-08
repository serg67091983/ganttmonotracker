// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Glade;
using Gtk;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{
	public class ViewSingleStateDialog : IStateView, IGuiMessageDialog,IDisposable
	{
		private Gtk.Dialog thisDialog;
		[Widget]
		private Gtk.Label lbStateAction;
		[Widget]
		private Gtk.Entry entName;
		[Widget]
		private Gtk.ColorButton cbtnColor;
		private string fName;

		public ViewSingleStateDialog (Window parent)
		{
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("ViewSingleStateDialog.glade");
			Glade.XML glade = Glade.XML.FromAssembly ("ViewSingleStateDialog.glade", "ViewSingleStateDialog", null);
			stream.Close ();
			glade.Autoconnect (this);
			thisDialog = ((Gtk.Dialog)(glade.GetWidget ("ViewSingleStateDialog")));
			thisDialog.TransientFor = parent;
			thisDialog.Modal = true;
			entName.Changed += OnChangeName;
			
			lbStateAction.Text = "Create State";
		}

		public int Run ()
		{
			thisDialog.ShowAll ();
			
			int result = 0;
			for (
			; true;) {
				result = thisDialog.Run ();
				if ((result != ((int)(Gtk.ResponseType.None)))) {
					break;
				}
			}
			thisDialog.Destroy ();
			return result;
		}

		#region IStateView Implementation

		public string Name {
			get {
				return fName;
			}
			set {
				fName = value;
				entName.Text = value;
			}
		}

		private byte fColorRed;

		public byte ColorRed {
			get {
				fColorRed = (byte)(cbtnColor.Color.Red >> 8);
				return (byte)(cbtnColor.Color.Red >> 8);
			}
			
			set {
				fColorRed = value;
			}
		}

		private byte fColorGreen;

		public byte ColorGreen {
			get {
				fColorGreen = (byte)(cbtnColor.Color.Green >> 8);
				return (byte)(cbtnColor.Color.Green >> 8);
			}
			
			set {
				fColorGreen = value;
			}
		}

		private byte fColorBlue;

		public byte ColorBlue {
			get {
				fColorBlue = (byte)(cbtnColor.Color.Blue >> 8);
				return (byte)(cbtnColor.Color.Blue >> 8);
			}
			
			set {
				fColorBlue = value;
			}
		}

		public void BindStateColor ()
		{
			cbtnColor.Color = new Gdk.Color (fColorRed, fColorGreen, fColorBlue);
		}

		private int fMappingID;

		public int MappingID {
			get {
				if (!IsMapped)
					throw new NotAllowedException ("State not mapped");
				return fMappingID;
			}
			
			set {
				IsMapped = true;
				fMappingID = value;
			}
		}

		public bool IsMapped {
			get;
			set;
		}

		public Hashtable Connections {
			get;
			set;
		}

		public void Connect (IManagerEntity stateEntry, string connectionName)
		{
			throw new ImplementationException ();
		}

		public void Disconnect (IManagerEntity stateEntry)
		{
			throw new ImplementationException ();
		}

		public bool IsConnected (IManagerEntity stateEntry)
		{
			throw new ImplementationException ();
		}

		public void ClearConnections ()
		{
			throw new ImplementationException ();
		}

		#endregion

		#region IGuiMessageDialog Implementation

		public int ShowDialog ()
		{
			return Run ();
		}

		public string Title {
			get {
				return thisDialog.Title;
			}
			set {
				thisDialog.Title = value;
			}			
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose ()
		{
			this.thisDialog.Dispose ();
		}

		#endregion

		private void OnChangeName (object sender, EventArgs e)
		{
			Name = (sender as Entry).Text;
		}
	}
}
