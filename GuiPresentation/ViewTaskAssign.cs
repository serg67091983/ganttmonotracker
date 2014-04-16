//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

using System;
using System.Threading;
using System.Data;
using System.IO;
using Gtk;

using GanttTracker.StateManager;
using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker.ExceptionPresentation;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{
	public class ViewTaskAssign : IGuiMessageDialog,IDisposable
	{
		private string fComment;

		private Gtk.Dialog thisDialog;

		[Glade.Widget()]
		private Gtk.Label lbAssignAction;

		[Glade.Widget()]
		private Gtk.ComboBoxEntry cbActor;

		[Glade.Widget()]
		private Gtk.TextView tvComment;
		
		public ViewTaskAssign(Window parent)
		{			
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewTaskAssign.glade");	
			Glade.XML glade = new Glade.XML(stream, "ViewTaskAssign", null);
			stream.Close();
			glade.Autoconnect(this);
			
			thisDialog = ((Gtk.Dialog)(glade.GetWidget("ViewTaskAssign")));
			thisDialog.Modal = true;
			thisDialog.TransientFor = parent;
			thisDialog.SetDefaultSize(200,450);
			
			cbActor.Entry.IsEditable = false;
			cbActor.Changed += new EventHandler(OnCbActorChanged);
			tvComment.KeyReleaseEvent += CommentKeyReleaseEvent;
		}

		void CommentKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			fComment = tvComment.Buffer.Text;
		}

		public int Run()
		{
			thisDialog.ShowAll();
			
			int result = 0;
			for (; true;)
			{
				result = thisDialog.Run();
				if ((result != ((int)(Gtk.ResponseType.None))))
				{
					break;
				}
				Thread.Sleep(500);
			}
			thisDialog.Destroy();
			return result;
		}
		
		#region IGuiMessageDialog Implementation
		
		public int ShowDialog()
		{
			return Run();
		}
		
		public string Title
		{
			get
			{
				return thisDialog.Title;
			}
			set
			{
				thisDialog.Title = value;
			}			
		}
		
		#endregion
		
		#region IDisposable Implementation
		
		public void Dispose()
		{
			this.thisDialog.Dispose();
		}
		
		#endregion	
		
		private ListStore fActorStore;

		public DataSet ActorSource { get; set; }
		
		private int fActorID;
		public int ActorID
		{
		 	get
		 	{
		 		return fActorID;
		 	}
		 	
		 	set
		 	{
		 		if (ActorSource == null)
					throw new NotAllowedException("Bind combo before with BindActor method");
				int index = 0;
				foreach(DataRow row in ActorSource.Tables["Actor"].Rows)
				{
					if ((int)row["ID"] == 	value)
					{
						cbActor.Active = index;
						fActorID = value;
						return;
					}
					index++;
				}
				throw new NotAllowedException("ActorID not found in Actor Source");
			}
		} 
		
		public void BindActor()
		{
			fActorStore = new ListStore(typeof(int),typeof(string));
			cbActor.Clear();
			foreach (DataRow row in ActorSource.Tables["Actor"].Rows)
			{
				fActorStore.AppendValues((int)row["ID"],(string)row["Name"]);
			}
			cbActor.Model = fActorStore;
			Gtk.CellRendererText actorText = new Gtk.CellRendererText();
			actorText.Style = Pango.Style.Oblique;
			//actorText.BackgroundGdk = new Gdk.Color(0x63,0,0);
			cbActor.PackStart(actorText,true);
			cbActor.AddAttribute(actorText,"text",1);
			TreeIter iter;
			if (fActorStore.GetIterFirst(out iter))
			{
				cbActor.SetActiveIter(iter);
				fActorID = (int)ActorSource.Tables["Actor"].Rows[0]["ID"];
			}
		}
		
		public string AssignAction
		{
			get
			{
				return lbAssignAction.Text;
			}
			set
			{
				lbAssignAction.Text = value;
			}
		}
		
		public string Comment
		{
			get
			{
				return tvComment.Buffer.Text;
			}
			set
			{
				tvComment.Buffer.Text = value;
			}
		}
		
		private void OnCbActorChanged(object sender, EventArgs args)
		{
			if (cbActor.Active != -1)
			{
				ActorID = (int)ActorSource.Tables["Actor"].Rows[cbActor.Active]["ID"];
				cbActor.Entry.Text = (string)ActorSource.Tables["Actor"].Rows[cbActor.Active]["Name"];				
			}
		}
		
	}
	
}
