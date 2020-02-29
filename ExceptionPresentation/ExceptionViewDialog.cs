﻿//----------------------------------------------------------------------------------------------
// <copyright file="ExceptionViewDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

namespace GanttMonoTracker.ExceptionPresentation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using GanttTracker.TaskManager.ManagerException;

    using Gtk;

    using TaskManagerInterface;

    public class ExceptionViewDialog : IGuiMessageDialog
    {
		#pragma warning disable 0649

        [Glade.Widget]
        private Gtk.Label lbExceptionMessage;
        [Glade.Widget]
        private Gtk.Label lbExceptionType;
        private Gtk.Dialog thisDialog;
        [Glade.Widget]
        private Gtk.TextView tvButtonDescription;
        [Glade.Widget]
        private Gtk.TextView tvComment;
        [Glade.Widget]
        private Gtk.TextView tvExceptionDescription;

		#pragma warning restore 0649

        public ExceptionViewDialog(Exception exception, Window parent)
        {
			//TODO: Looks like builder pattern needed here
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ExceptionViewDialog.glade");
            Glade.XML glade = new Glade.XML(stream, "ExceptionViewDialog", null);
            stream.Close();
            glade.Autoconnect(this);
            thisDialog = ((Gtk.Dialog)(glade.GetWidget("ExceptionViewDialog")));
            thisDialog.TransientFor = parent;
            thisDialog.Modal = true;
            thisDialog.AllowGrow = false;
            thisDialog.AllowShrink = false;
            thisDialog.WindowPosition = WindowPosition.Center;

            ExceptionMessage = exception.Message;

            tvButtonDescription.Sensitive = false;
            if (!(exception is ManagementException))
            {
                ExceptionType = exception.GetType ().FullName;
                ExceptionDescription = exception.ToString ();
                tvButtonDescription.Buffer.Text =
                    "Use ignore for continue, Quit for exit program.";
            }
            else
            {
                ExceptionType = string.Empty;
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

        public void Dispose()
        {
            thisDialog.Dispose();
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

        public int ShowDialog()
        {
            return Run();
        }
    }
}