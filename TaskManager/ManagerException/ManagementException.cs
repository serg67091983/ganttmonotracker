﻿//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 03.12.2005 at 0:19

using System;

using TaskManagerInterface;
using Arise.Logic;
using GanttMonoTracker;

namespace GanttTracker.TaskManager.ManagerException
{
	public class ManagementException : Exception
	{
		public ManagementException(ExceptionType type) : base ("Operation not allowed") 
		{
			Type = type;
		}

		public ManagementException(ExceptionType type, string message) : base (message) 
		{
			Type = type;
		}

		public ExceptionType Type { get; private set; }
	}
}