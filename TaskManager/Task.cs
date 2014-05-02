//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 23:03

using System;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker;

namespace GanttTracker.TaskManager
{
	public class Task : IManagerEntity, ITask
	{
		private Task()
		{
		}
		
		public Task(ITaskManager parent)
		{
			Parent = parent;
		}
		
		public Task(ITaskManager parent, int id)
		{
			Id = id;
			Parent = parent;
			Parent.BindTask(this);
		}
		
		#region Data
		
		public string Description {	get;set; }
		
		public bool ActorPresent { get;set; }
		
		private int fActorID;
		public int ActorID
		{
			get
			{
				if (!ActorPresent)
					throw new ManagementException(ExceptionType.NotAllowed, "Actor not present");
					
				return fActorID;
			}
			set
			{
				ActorPresent = true;
				fActorID = value;
			}
		}
		
		public bool StatePresent {	get;set; }
		
		private int fStateID;

		public int StateID
		{
			get
			{
				if (!StatePresent)
					throw new ManagementException(ExceptionType.NotAllowed, "State not present");
				return fStateID;
			}
			
			set
			{
				StatePresent = true;
				fStateID = value;
			}
		}
		
		public DateTime StartTime {	get;set; }
		
		public DateTime EndTime	{ get;set; }
		
		public TimeSpan EstimatedTime {	get;set; }
		
		#endregion
	
	
		#region IManagerEntity Implementation
		
		public int Id {	get;set; }
		
		public bool IsNew
		{
			get
			{
				return (Id == 0);
			}
		}
		
		public bool isUpdated
		{
			get
			{
				return Parent.isUpdatedTask(this);
			}
		}
			
		public ITaskManager Parent { get;set; }


		public string Comment {	get;set; }
		
		public void BindData()
		{
			Parent.BindTask(this);
		}
			
		public void Save()
		{
			Parent.UpdateTask(this);
		}
		
		public void Delete()
		{
			Parent.DeleteTask(this.Id);
		}
			
		#endregion
	}
}