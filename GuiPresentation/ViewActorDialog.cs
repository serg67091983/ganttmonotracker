﻿//----------------------------------------------------------------------------------------------
// <copyright file="ViewActorDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

namespace GanttMonoTracker.GuiPresentation
{
    using System;
    using System.IO;

    using Glade;

    using Gtk;

    using TaskManagerInterface;

    public class ViewActorDialog : IGuiActorView
    {
		#pragma warning disable 0649
        [Widget]
        private Gtk.Entry entEmail;
        [Widget]
        private Gtk.Entry entName;
		#pragma warning disable 0649

        private string fActorEmail;
        private string fActorName;
        private Gtk.Dialog thisDialog;

        public ViewActorDialog(Window parent)
        {
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewActorDialog.glade");
            Glade.XML glade = Glade.XML.FromAssembly("ViewActorDialog.glade","ViewActorDialog", null);
            stream.Close();
            glade.Autoconnect(this);
            entName.Changed += OnChangeName;
            entEmail.Changed += OnChangeEmail;
            thisDialog = ((Gtk.Dialog)(glade.GetWidget("ViewActorDialog")));
            thisDialog.Modal = true;
            thisDialog.TransientFor = parent;
            thisDialog.WindowPosition = WindowPosition.Center;
        }

        public string ActorEmail
        {
            get
            {
                return fActorEmail;
            }

            set
            {
                fActorEmail = value;
                entEmail.Text = value;
            }
        }

        public string ActorName
        {
            get
            {
                return fActorName;
            }

            set
            {
                fActorName = value;
                entName.Text = value;
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
            entName.Changed -= OnChangeName;
            entEmail.Changed -= OnChangeEmail;
            this.thisDialog.Dispose();
        }

        public int Run()
        {
            thisDialog.Show();

            int result = 0;
            for (
                ; true;
            )
            {
                result = thisDialog.Run();
                if ((result != ((int)(Gtk.ResponseType.None))))
                {
                    break;
                }
            }

            thisDialog.Destroy();
            return result;
        }

        public int ShowDialog()
        {
            return Run();
        }

        private void OnChangeEmail(object sender, EventArgs e)
        {
            ActorEmail = (sender as Entry).Text;
        }

        private void OnChangeName(object sender, EventArgs e)
        {
            ActorName = (sender as Entry).Text;
        }
    }
}