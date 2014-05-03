//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

using Gtk;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using TaskManagerInterface;

namespace GanttMonoTracker.ExceptionPresentation
{
	public class ExceptionViewDialog : IGuiMessageDialog
	{
		private Gtk.Dialog thisDialog;


		[Glade.Widget()]
		private Gtk.Label lbExceptionType;


		[Glade.Widget()]
		private Gtk.Label lbExceptionMessage;


		[Glade.Widget()]
		private Gtk.TextView tvExceptionDescription;


		[Glade.Widget()]
		private Gtk.TextView tvComment;


		[Glade.Widget()]
		private Gtk.TextView tvButtonDescription;


		public ExceptionViewDialog(Exception exception, Window parent)
		{
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ExceptionViewDialog.glade");
			Glade.XML glade = new Glade.XML(stream, "ExceptionViewDialog", null);
			stream.Close();
			glade.Autoconnect(this);
			thisDialog = ((Gtk.Dialog)(glade.GetWidget("ExceptionViewDialog")));
			thisDialog.TransientFor = parent;
			thisDialog.Modal = true;
			thisDialog.AllowGrow = false;
			thisDialog.AllowShrink = false;
			tvButtonDescription.Buffer.Text = 
				"Use ignore for continue, Quit for exit program.";
			ExceptionType = exception.GetType().FullName;
			ExceptionMessage = exception.Message;
			ExceptionDescription = exception.ToString();
			tvButtonDescription.Sensitive = false;
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
					switch(result)
					{
					case  1 :
						Application.Quit();
						break;
					case 2 :
						//todo : send a mail
						break;
					}
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
			thisDialog.Dispose();
		}
		
		#endregion
		
		public string ExceptionType
		{
			get
			{
				return lbExceptionType.Text; 
			}
			
			set
			{
				lbExceptionType.Text = value;
			}
		}
		
		public string ExceptionMessage
		{
			get
			{
				return lbExceptionMessage.Text; 
			}
			
			set
			{
				lbExceptionMessage.Text = value;
			}
		}
		
		public string ExceptionDescription
		{
			get
			{
				return tvExceptionDescription.Buffer.Text; 
			}
			
			set
			{
				var rows = new List<string>();
				while (value.Length > 0)
				{
					var len = value.Length > 100 ? 
						100 : 
						value.Length;
					rows.Add( value.Substring(0, len));
					value = value.Substring(len);
				}

				tvExceptionDescription.Buffer.Text = string.Join(Environment.NewLine, rows.ToArray());
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

	}
}