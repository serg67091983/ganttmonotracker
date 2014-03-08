// created on 03.12.2005 at 0:19

using System;
using TaskManagerInterface;
using Arise.Logic;

namespace GanttTracker.TaskManager.ManagerException
{

	public class NotAllowedException : Exception, IManagerException
	{
		public NotAllowedException() : base ("Operation not allowed")
		{			
		}
		
		public NotAllowedException(string message) : base (message)
		{			
		}

		public int ID
		{
			get
			{
				return IdentityManager.Instance.CreateTypeIdentity(this.GetType());
			}
			set
			{
				
			}
		}
	}
}