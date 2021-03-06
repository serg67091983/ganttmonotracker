﻿//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 27.11.2005 at 21:21

using System;
using System.IO;
using System.Data;
using System.Collections;

using GanttTracker.TaskManager.TaskStorage;
using GanttTracker.TaskManager.ManagerException;
using GanttTracker.StateManager;
using TaskManagerInterface;
using GanttMonoTracker;

namespace GanttTracker.TaskManager
{
	//TODO: Create methods for Tasks, Actors, States e.t.s need to be moved to Abstract factories for unified presentaion in future.
	public class XmlTaskManager : ITaskManager
	{
		string fConnectionString;

		DataSet fGanttSource;

		DataSet fAssigmentSource;

		IStorageRepository fRepository;

		public XmlTaskManager(string connectionString)
		{
			fConnectionString  = connectionString;
			Initialize();
		}

		private void Initialize()
		{
			fRepository = new StorageRepository(fConnectionString,new CommandFactory());
			if (!fRepository.CheckConnection())
			{
				throw new ManagementException(ExceptionType.NotAllowed,"Check connection failed for connection string " + fConnectionString);
			}

			fRepository.Load(); 
		}
	
	   #region Tasks
		
		public DataSet TaskSource
		{
			get
			{
				DataSet taskSource = new DataSet("TaskSource");
				taskSource.Tables.Add(fRepository.Storage.Tables["Task"].Copy());
				return taskSource;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"No changes in source");
			}
		}

		public IManagerEntity GetTask(int id)
		{
			Task task = new Task(this, id);
			BindTask(task);
			
			return task; 
		}

		public IManagerEntity CreateTask()
		{
			IStorageCommand createTaskCommand = fRepository.CommandFactory.GetInsertCommand("Task");
			
			Hashtable values = new Hashtable();
			
			values.Add("ActorID",DBNull.Value);
			values.Add("Description", string.Empty);			
			values.Add("StartTime",DateTime.Now);
			values.Add("EndTime",DateTime.Now);
			values.Add("StateID",DBNull.Value);
            values.Add("Priority", 10);

            createTaskCommand.SetParam("Values",values);
			
			int id = (int)fRepository.ExecuteScalar(createTaskCommand);		
			 
			Task task = new Task(this, id);
			BindTask(task);
			
			return task; 
		}

		public void BindTask(IManagerEntity taskEntity)
		{
			Task task = (Task)taskEntity;
			
			IStorageCommand taskCommand = fRepository.CommandFactory.GetSelectCommand("Task");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + task.Id);
			
			taskCommand.SetParam("Rules",rules);
			
			DataTable taskTable = fRepository.ExecuteDataSet(taskCommand).Tables[0];
			if (taskTable.Rows.Count > 1)
			{
				throw new ManagementException(ExceptionType.ValidationFailed,"more when one task for id " + task.Id);
			}
			
			if (taskTable.Rows.Count == 0)
				throw new ManagementException(ExceptionType.ValidationFailed,"task not found for id " + task.Id);
			
			DataRow taskRow = taskTable.Rows[0]; 
			
			if (!(taskRow["ActorID"] is DBNull))
				task.ActorID = (int)taskRow["ActorID"];
			else
				task.ActorPresent = false;

			task.Description = taskRow["Description"] as string;		
			if(task.Description == null)
				task.Description = string.Empty;

			task.StartTime = (DateTime)taskRow["StartTime"];
			task.EndTime = (DateTime)taskRow["EndTime"];
            if(taskRow["Priority"] is DBNull)
            {
                task.Priority = 10;
            }
            else
            {
                task.Priority = (int)taskRow["Priority"];
            }

            if (!(taskRow["StateID"] is DBNull))
				task.StateID = (int)taskRow["StateID"];		
			else
				task.StatePresent = false;
			task.EstimatedTime = task.EndTime.Subtract(task.StartTime);		
		}

		public void UpdateTask(IManagerEntity taskEntity)
		{
			Task task = (Task)taskEntity;
			
			IStorageCommand updateTaskCommand = fRepository.CommandFactory.GetUpdateCommand("Task");

			Hashtable values = new Hashtable();
			
			if (task.ActorPresent)
				values.Add("ActorID",task.ActorID);
			values.Add("Description", task.Description);			
			values.Add("StartTime",task.StartTime);
			values.Add("EndTime",task.EndTime);
            values.Add("Priority", task.Priority);
            if (task.StatePresent)
				values.Add("StateID",task.StateID);
					
			updateTaskCommand.SetParam("Values",values);
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + task.Id);
			
			updateTaskCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery(updateTaskCommand);			
		}

		public bool IsUpdatedTask(IManagerEntity taskEntity)
		{
			Task newTask = (Task)taskEntity;
			Task oldTask = new Task(this, newTask.Id);
			
			BindTask(oldTask);
			
			bool result = newTask.ActorID == oldTask.ActorID;
			result = result && 	newTask.Description == oldTask.Description;
			result = result && 	newTask.EndTime == oldTask.EndTime;
			result = result && 	newTask.StartTime == oldTask.StartTime;
			result = result && 	newTask.StateID == oldTask.StateID;
            result = result && newTask.Priority == oldTask.Priority;

            return result;
		}

		public void DeleteTask(int id)
		{
			IStorageCommand deleteTaskCommand = fRepository.CommandFactory.GetDeleteCommand("Task");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + id);
			
			deleteTaskCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery (deleteTaskCommand);
		}	
		
		#endregion
				
		#region Actors
		
		public DataSet ActorSource
		{
			get
			{
				DataSet actorSource = new DataSet("ActorSource");
				actorSource.Tables.Add(fRepository.Storage.Tables["Actor"].Copy());				
				return actorSource;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"No changes in source");
			}		
		}

		public IManagerEntity GetActor(int id)
		{			
			Actor actor = new Actor(this, id);
			BindActor(actor);
			
			return actor;
		}

		public IManagerEntity CreateActor()
		{
			IStorageCommand createActorCommand = fRepository.CommandFactory.GetInsertCommand("Actor");

			Hashtable values = new Hashtable();
			
			values.Add("Name","");
			values.Add("Email","");		
					
			createActorCommand.SetParam("Values",values);
			
			int id = (int)fRepository.ExecuteScalar(createActorCommand); 
			Actor actor = new Actor(this, id);
			BindActor(actor);
			
			return actor;
		}

		public void BindActor(IManagerEntity actorEntity)
		{
			Actor actor = (Actor)actorEntity;
			
			IStorageCommand actorCommand = fRepository.CommandFactory.GetSelectCommand("Actor");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + actor.Id);
			
			actorCommand.SetParam("Rules",rules);
			
			DataTable actorTable = fRepository.ExecuteDataSet(actorCommand).Tables[0];
			if (actorTable.Rows.Count > 1)
			{
				throw new ManagementException(ExceptionType.ValidationFailed,"more when one actor for id " + actor.Id);
			}
			
			if (actorTable.Rows.Count == 0)
				throw new ManagementException(ExceptionType.ValidationFailed,"Actor not found for id " + actor.Id);
			
			DataRow actorRow = actorTable.Rows[0]; 
			
			actor.Name = actorRow["Name"].ToString();
			actor.Email = actorRow["Email"].ToString();		
		}

		public void UpdateActor(IManagerEntity actorEntity)
		{
			Actor actor = (Actor)actorEntity;
			
			IStorageCommand updateActorCommand = fRepository.CommandFactory.GetUpdateCommand("Actor");

			Hashtable values = new Hashtable();
			
			values.Add("Name",actor.Name);
			values.Add("Email",actor.Email);
					
			updateActorCommand.SetParam("Values",values);
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + actor.Id);
			
			updateActorCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery(updateActorCommand);
		}

		public bool IsUpdatedActor(IManagerEntity actorEntity)
		{
			Actor newActor = (Actor)actorEntity;
			Actor oldActor = new Actor(this, newActor.Id);
			
			BindActor(oldActor);
			
			bool result = newActor.Id == oldActor.Id;
			result = result && 	newActor.Name == oldActor.Name;
			result = result && 	newActor.Email == oldActor.Email;
						
			return result;
		}

		public void DeleteActor(int id)
		{
			IStorageCommand deleteActorCommand = fRepository.CommandFactory.GetDeleteCommand("Actor");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + id);
			
			deleteActorCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery (deleteActorCommand);
		}		
		
		#endregion
		
		#region Gantt

		public DataSet GanttSource
		{
			get
			{
				CalculateGanttSource();	
				return fGanttSource; 
			}
		}

		void CalculateGanttSource()
		{
			ActorSource.Tables ["Actor"].AcceptChanges ();
			TaskSource.Tables["Task"].AcceptChanges ();
			TaskStateSource.Tables["TaskState"].AcceptChanges ();

			fGanttSource = new DataSet("GanttSource");
			fGanttSource.Tables.Add(ActorSource.Tables["Actor"].Copy());
			fGanttSource.Tables.Add(TaskSource.Tables["Task"].Copy());
			fGanttSource.Tables.Add(TaskStateSource.Tables["TaskState"].Copy());
			
			fGanttSource.Tables.Add("DataRange");			
			fGanttSource.Tables["DataRange"].Columns.Add("MinDate",typeof(DateTime));
			fGanttSource.Tables["DataRange"].Columns.Add("MaxDate",typeof(DateTime));
			
			DataRow rangeRow = fGanttSource.Tables["DataRange"].NewRow();
			rangeRow["MinDate"] = GanttFirstDate;
			rangeRow["MaxDate"] = GanttLastDate;
			
			fGanttSource.Tables["DataRange"].Rows.Add(rangeRow);						
		}

		public DateTime GanttFirstDate
		{
			get
			{
				IStorageCommand taskCommand = fRepository.CommandFactory.GetSelectCommand("Task");
			
				Hashtable rules = new Hashtable();				
				
				taskCommand.SetParam("Rules",rules);
				
				DataTable taskTable = fRepository.ExecuteDataSet(taskCommand).Tables[0];
				
				DateTime firstDate;
				if (taskTable == null)
					throw new ManagementException(ExceptionType.NotAllowed,"Task table not set to instance");
				if (taskTable.Rows.Count == 0)
					return DateTime.Now;
				Task task = (Task)GetTask((int)taskTable.Rows[0]["ID"]);
				if(task == null) return DateTime.Now;
				firstDate = task.StartTime; 
				foreach(DataRow row in taskTable.Rows)
				{
					task = (Task)GetTask((int)row["ID"]);
					if(task == null) return DateTime.Now;
					if (firstDate > task.StartTime)
						firstDate = task.StartTime;
				}

				return firstDate; 
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"Update for gantt min date not implemented");
			}
		}

		public DateTime GanttLastDate
		{
			get
			{
				IStorageCommand taskCommand = fRepository.CommandFactory.GetSelectCommand("Task");
			
				Hashtable rules = new Hashtable();
				
				taskCommand.SetParam("Rules",rules);
				
				DataTable taskTable = fRepository.ExecuteDataSet(taskCommand).Tables[0];
				
				DateTime lastDate;
				if (taskTable == null)
					throw new ManagementException(ExceptionType.NotAllowed,"Task table not set to instance");
				if (taskTable.Rows.Count == 0)
					return DateTime.Now;
				Task task = (Task)GetTask((int)taskTable.Rows[0]["ID"]);
				if(task == null) return DateTime.Now;
				lastDate = task.EndTime; 
				foreach(DataRow row in taskTable.Rows)
				{
					task = (Task)GetTask((int)row["ID"]);
					if(task == null) return DateTime.Now;
					if (lastDate < task.EndTime)
						lastDate = task.EndTime;
				}

				return lastDate;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"Update for gantt max date not implemented");
			}		
		}
		
		#endregion
		
		#region Assigment

		public DataSet AssigmentSource
		{
			get
			{
				CalculateAssigmentSource();
				return fAssigmentSource;
			}
		}

		void CalculateAssigmentSource()
		{
			fAssigmentSource = new DataSet("AssigmentSource");
			fAssigmentSource.Tables.Add("AssigmentSource");
			fAssigmentSource.Tables.Add(ActorSource.Tables["Actor"].Copy());			
			fAssigmentSource.Tables.Add("DataRange");		
			fAssigmentSource.Tables.Add ("StateRange");
			
			fAssigmentSource.Tables["DataRange"].Columns.Add("MinDate",typeof(DateTime));
			fAssigmentSource.Tables["DataRange"].Columns.Add("MaxDate",typeof(DateTime));
			
			DataRow rangeRow = fAssigmentSource.Tables["DataRange"].NewRow();
			rangeRow["MinDate"] = GanttFirstDate;
			rangeRow["MaxDate"] = GanttLastDate;
			
			fAssigmentSource.Tables["DataRange"].Rows.Add(rangeRow);
			
			fAssigmentSource.Tables["AssigmentSource"].Columns.Add("ID",typeof(int));
			fAssigmentSource.Tables["AssigmentSource"].Columns.Add("ActorID",typeof(int));			
			fAssigmentSource.Tables["AssigmentSource"].Columns.Add("Date",typeof(DateTime));
			fAssigmentSource.Tables["AssigmentSource"].Columns.Add("TaskCount",typeof(int));

			fAssigmentSource.Tables["StateRange"].Columns.Add("AssigmentID",typeof(int));			
			fAssigmentSource.Tables["StateRange"].Columns.Add("StateID",typeof(int));
			fAssigmentSource.Tables["StateRange"].Columns.Add("TaskCount",typeof(int));
			fAssigmentSource.Tables["StateRange"].Columns.Add("ColorBlue",typeof(int));
			fAssigmentSource.Tables["StateRange"].Columns.Add("ColorGreen",typeof(int));
			fAssigmentSource.Tables["StateRange"].Columns.Add("ColorRed",typeof(int));

			int assignmentId = 0;
			foreach(DataRow actorRow in ActorSource.Tables["Actor"].Rows)
			{
				Actor actor = (Actor)GetActor((int)actorRow["ID"]);
				if(actor == null) return;
							
				foreach(DataRow taskRow in TaskSource.Tables["Task"].Rows)
				{								 
					Task task = (Task)GetTask((int)taskRow["ID"]);
					if(task == null) return;
					if (actor.Id == task.ActorID)
					{
						for(DateTime day =  task.StartTime.Date; day <= task.EndTime.Date; day = day.AddDays(1))
						{
							int currentAssignmentId = -1;

							if ( fAssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + actor.Id + " and Date = '" + day.ToShortDateString() + "'" ).Length > 0)
							{
								DataRow existAssigmentRow = fAssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + actor.Id + " and Date = '" + day.ToShortDateString()+"'")[0];
								existAssigmentRow["TaskCount"] = (int)existAssigmentRow["TaskCount"] + 1;								 
								currentAssignmentId = (int)existAssigmentRow ["ID"];
							}
							else
							{
								DataRow assigmentRow = fAssigmentSource.Tables["AssigmentSource"].NewRow();								
								currentAssignmentId = assignmentId;
								assigmentRow["ID"] = assignmentId++; 
								assigmentRow["ActorID"] = actor.Id; 
								assigmentRow["TaskCount"] = 1;
								assigmentRow["Date"] = day;
								fAssigmentSource.Tables["AssigmentSource"].Rows.Add(assigmentRow);
							}

							var stateRows = fAssigmentSource.Tables ["StateRange"].Select ("AssigmentID = " + currentAssignmentId + " and StateID = " + task.StateID);
							if (stateRows.Length > 0) {
								stateRows [0] ["TaskCount"] = (int)stateRows [0] ["TaskCount"] + 1;
							} else {
								DataRow stateRow = fAssigmentSource.Tables ["StateRange"].NewRow ();
								stateRow ["AssigmentID"] = currentAssignmentId;
								stateRow ["StateID"] = task.StateID;
								var state = (IState)GetTaskState (task.StateID);
								stateRow ["ColorBlue"]  = state.ColorBlue;
								stateRow ["ColorGreen"]  = state.ColorGreen;
								stateRow ["ColorRed"]  = state.ColorRed;

								stateRow ["TaskCount"] = 1;
								fAssigmentSource.Tables["StateRange"].Rows.Add(stateRow);
							}
						}
					}
				}
			}

//			test Me
//			foreach (DataRow row in fAssigmentSource.Tables["AssigmentSource"].Rows)
//			{
//				Console.WriteLine("{0} {1} {2} ",row["ActorID"],row["TaskCount"],row["Date"]);
//			}
//			throw new ImplementationException();
		}
		
		#endregion
		
		#region TaskState
				
		public DataSet TaskStateSource
		{
			get
			{
				DataSet stateSource = new DataSet("TaskStateSource");
				stateSource.Tables.Add(fRepository.Storage.Tables["TaskState"].Copy());				
				return stateSource;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"No changes in source");
			}
		}

		public DataSet GetInitialTaskStateSource()
		{
			DataSet stateSource = new DataSet("TaskStateSource");
			stateSource.Tables.Add(fRepository.Storage.Tables["TaskState"].Copy());				
			return stateSource;
		}

		public DataSet GetTaskStateSource(IManagerEntity state)
		{
			DataSet stateSource = new DataSet("TaskStateSource");
			stateSource.Tables.Add(fRepository.Storage.Tables["TaskState"].Copy());				
			return stateSource;
		}

		public IManagerEntity GetTaskState(int id)
		{
			State state = new State(this, id);
			BindTaskState(state);
			
			return state;
		}

		public IManagerEntity CreateTaskState()
		{
			IStorageCommand createTaskStateCommand = fRepository.CommandFactory.GetInsertCommand("TaskState");

			Hashtable values = new Hashtable();
			
			values.Add("Name","");
			
			values.Add("ColorRed",0);
			values.Add("ColorGreen",0);
			values.Add("ColorBlue",0);
						
			int mappintID = -1;
			foreach (DataRow row in TaskStateSource.Tables["TaskState"].Rows)
			{
				if ( !(row["MappingID"] is DBNull) && (int)row["MappingID"] > mappintID) mappintID = (int)row["MappingID"]; 
			}

			mappintID++;
			values.Add("MappingID",mappintID);
					
			createTaskStateCommand.SetParam("Values",values);			
			int id = (int)fRepository.ExecuteScalar(createTaskStateCommand);
			Console.WriteLine(id);
			State state = new State(this, id);
			BindTaskState(state);
			return state;
		}

		public void BindTaskState(IManagerEntity stateEntity)
		{
			State state = (State)stateEntity;
			
			IStorageCommand stateCommand = fRepository.CommandFactory.GetSelectCommand("TaskState");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + state.Id);
			
			stateCommand.SetParam("Rules",rules);
			
			DataTable stateTable = fRepository.ExecuteDataSet(stateCommand).Tables[0];
			if (stateTable.Rows.Count > 1)
			{
				throw new ManagementException(ExceptionType.ValidationFailed,"more when one task state for id " + state.Id);
			}
			
			if (stateTable.Rows.Count == 0)
				throw new ManagementException(ExceptionType.ValidationFailed,"Task state not found for id " + state.Id);
			
			DataRow stateRow = stateTable.Rows[0];
			state.Name = (string)stateRow["Name"];
			
			state.ColorRed = (byte)stateRow["ColorRed"];
			state.ColorGreen = (byte)stateRow["ColorGreen"];
			state.ColorBlue = (byte)stateRow["ColorBlue"];
			
			bool connectionsPresent = true;
			try
			{
				state.MappingID = (int)stateRow["MappingID"];
				state.IsMapped = true;
			}
			catch(InvalidCastException)
			{
				connectionsPresent = false;
				state.IsMapped = false;
			}

			if (connectionsPresent)
			{
				IStorageCommand stateConnectionCommand = fRepository.CommandFactory.GetSelectCommand("TaskStateConnection");

				Hashtable connectionRules = new Hashtable();
				rules.Add("ReferenceID","ID = " + state.MappingID);
				
				stateConnectionCommand.SetParam("Rules",connectionRules);
				
				DataTable stateConnectionTable = fRepository.ExecuteDataSet(stateConnectionCommand).Tables[0];
							
				foreach(DataRow row in stateConnectionTable.Rows)
				{
					State connectedState = (State)GetTaskStateConnection((int)row["ID"]);
					state.Connect(connectedState,connectedState.Name);
				}
			}
		}

		/*
		public void BindTaskComment(IManagerEntity parent)
		{
			IStorageCommand commentCommand = fDealer.CommandFactory.CreateSelectCommand(fDealer);

			commentCommand.SetParam("EntityName","Comment");

			Hashtable rules = new Hashtable();
			rules.Add("TaskID","ID = " + parent.ID);

			commentCommand.SetParam("Rules",rules);

			DataTable commentTable = fDealer.ExecuteDataSet(commentCommand).Tables[0];
			if (commentTable.Rows.Count > 1)
			{
				throw new ValidationException("more when one comment for task id " + parent.ID);
			}

			if (commentTable.Rows.Count == 0)
			{
				//todo create comment on the fly
			}
			else
			{
				//todo: fill comment
				DataRow commentRow = commentTable.Rows[0];
				Comment comment = new Comment 
				{
					Description = (string)commentRow["Description"],
					Date = (DateTime)commentRow["Description"],
					ID = commentRow["ID"],

				};
			}

		}
		*/

		public void UpdateTaskState(IManagerEntity stateEntity)
		{
			State state = (State)stateEntity;
			
			IStorageCommand updateStateCommand = fRepository.CommandFactory.GetUpdateCommand("TaskState");

			Hashtable values = new Hashtable();
			
			values.Add("Name",state.Name);
						 			
			values.Add("ColorRed",state.ColorRed);
			values.Add("ColorGreen",state.ColorGreen);
			values.Add("ColorBlue",state.ColorBlue);
			
			try
			{
				values.Add("MappingID",state.MappingID);
			}
			catch(ManagementException)
			{
				values.Add("MappingID",DBNull.Value);
			}
					
			updateStateCommand.SetParam("Values",values);
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + state.Id);
			
			updateStateCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery(updateStateCommand);
			
			//store fConnections
		}

		public bool IsUpdatedTaskState(IManagerEntity stateEntity)
		{
			State newState = (State)stateEntity;
			State oldState = new State(this, newState.Id);
			
			BindTaskState(oldState);
			
			bool result = newState.Id == oldState.Id;
			result = result && 	newState.Name == oldState.Name;
			try
			{
				result = result && 	newState.MappingID == oldState.MappingID;
			}
			catch(ManagementException)
			{
				result = result && 	newState.IsMapped == oldState.IsMapped;
			}

			foreach(int stateID in newState.Connections.Keys)
			{
				result = result && oldState.Connections.ContainsKey(stateID) && oldState.Connections[stateID] == newState.Connections[stateID]; 
			}
						
			return result;
		}

		public void DeleteTaskState(int id)
		{
			IStorageCommand deleteStateCommand = fRepository.CommandFactory.GetDeleteCommand("TaskState");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + id);
			
			deleteStateCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery (deleteStateCommand);
		}
		
		#endregion
		
		#region Task State Connection
		
		public DataSet TaskStateConnectionsSource
		{
			get
			{
				DataSet connectionSource = new DataSet("TaskStateConnectionSource");
				connectionSource.Tables.Add(fRepository.Storage.Tables["TaskStateConnection"].Copy());				
				return connectionSource;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"No changes in source");
			}
		}

		public IManagerEntity GetTaskStateConnection(int id)
		{
			Connection connection = new Connection(this, id);
			BindTaskStateConnection(connection);
			
			return connection;
		}

		public IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity)
		{
			IStorageCommand createTaskStateConnectionCommand = fRepository.CommandFactory.GetInsertCommand("TaskStateConnection");

			Hashtable values = new Hashtable();
			
			State state = (State)stateEntity;
			State connectedState = (State)connectedStateEntity;
			
			values.Add("Name","");
			if (!state.IsMapped)
			{
				// create mappintID
				int mappintID = -1;
				foreach (DataRow row in TaskStateSource.Tables["TaskState"].Rows)
				{
					if ( !(row["MappingID"] is DBNull) && (int)row["MappingID"] > mappintID) mappintID = (int)row["MappingID"]; 
				}

				mappintID++;
				state.MappingID = mappintID;
				state.Save();
			}	

			values.Add("MappingID",state.MappingID);
			values.Add("StateID",connectedState.Id);
			
			createTaskStateConnectionCommand.SetParam("Values",values);			
			int id = (int)fRepository.ExecuteScalar(createTaskStateConnectionCommand);					 
			Connection connection = new Connection(this, id);
			BindTaskStateConnection(connection);
			return connection;
		}

		public void BindTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			Connection connection = (Connection)stateConnectionEntity;
			
			IStorageCommand connectionCommand = fRepository.CommandFactory.GetSelectCommand("TaskStateConnection");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + connection.Id);
			
			connectionCommand.SetParam("Rules",rules);
			
			DataTable connectionTable = fRepository.ExecuteDataSet(connectionCommand).Tables[0];
			if (connectionTable.Rows.Count > 1)
			{
				throw new ManagementException(ExceptionType.ValidationFailed,"more when one task state connection for id " + connection.Id);
			}
			
			if (connectionTable.Rows.Count == 0)
				throw new ManagementException(ExceptionType.ValidationFailed,"Task state Connection not found for id " + connection.Id);
			
			DataRow connectionRow = connectionTable.Rows[0];
			connection.Name = (string)connectionRow["Name"];		
			connection.MappingID = (int)connectionRow["MappingID"];
			connection.StateID = (int)connectionRow["StateID"];			
		}

		public void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			Connection connection = (Connection)stateConnectionEntity;
			
			IStorageCommand updateStateConnectionCommand = fRepository.CommandFactory.GetUpdateCommand("TaskStateConnection");

			Hashtable values = new Hashtable();
			
			values.Add("Name",connection.Name);
			values.Add("MappingID",connection.MappingID);
			values.Add("StateID",connection.StateID);			
					
			updateStateConnectionCommand.SetParam("Values",values);
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + connection.Id);
			
			updateStateConnectionCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery(updateStateConnectionCommand);
			
			//store fConnections
		}

		public bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			Connection newConnection = (Connection)stateConnectionEntity;
			Connection oldConnection = new Connection(this, newConnection.Id);
			
			BindTaskStateConnection(oldConnection);
			
			bool result = 			newConnection.Id == oldConnection.Id;
			result = result && 	newConnection.Name == oldConnection.Name;
			result = result && 	newConnection.MappingID == oldConnection.MappingID;
			result = result && 	newConnection.StateID == oldConnection.StateID;
						
			return result;
		}

		public void DeleteTaskStateConnection(int id)
		{
			IStorageCommand deleteStateConnectionCommand = fRepository.CommandFactory.GetDeleteCommand("TaskStateConnection");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + id);
			
			deleteStateConnectionCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery (deleteStateConnectionCommand);
		}
		
		#endregion
		
		#region Comment
		
		public DataSet CommentSource
		{
			get
			{
				DataSet commentSource = new DataSet("CommentSource");				
				commentSource.Tables.Add(fRepository.Storage.Tables["Comment"].Copy());				
				return commentSource;
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed,"No changes in source");
			}
		}

		/*
		public IManagerEntity GetComment(int id)
		{
			Comment comment = new Comment(this, id);
			BindComment(comment);			
			return comment;
		}*/

		public IManagerEntity CreateComment(IManagerEntity commentedEntity)
		{
			IStorageCommand createCommentCommand = fRepository.CommandFactory.GetInsertCommand("Comment");

			Hashtable values = new Hashtable();
			
			values.Add("Description","");
			values.Add("Date",DateTime.Now);
			if (commentedEntity != null)
			{
				if (commentedEntity is Task)
					values.Add("EntryID",commentedEntity.Id);				
			}
			else
				values.Add("EntryID",DBNull.Value);
			
			createCommentCommand.SetParam("Values",values);			

			//int id = (int)fDealer.ExecuteScalar(createCommentCommand);					 
			//Comment comment = new Comment(this, id);
			//BindComment(comment);
			return null;
		}
				
		/*
		public void BindComment(IManagerEntity commentEntity)
		{
			Comment comment = (Comment)commentEntity;
			
			IStorageCommand commentCommand = fDealer.CommandFactory.CreateSelectCommand(fDealer);
			
			commentCommand.SetParam("EntityName","Comment");
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + comment.ID);
			
			commentCommand.SetParam("Rules",rules);
			
			DataTable commentTable = fDealer.ExecuteDataSet(commentCommand).Tables[0];
			if (commentTable.Rows.Count > 1)
			{
				throw new ValidationException("more when one task state comment for id " + comment.ID);
			}
			
			if (commentTable.Rows.Count == 0)
					throw new ValidationException("Comment not found for id " + comment.ID);
			
			DataRow commentRow = commentTable.Rows[0];
			comment.Description = (string)commentRow["Description"];		
			comment.Date = (DateTime)commentRow["Date"];
			if (!(commentRow["EntryID"] is DBNull))
			{
				comment.CommentedEntryID = (int)commentRow["EntryID"];
				comment.CommentedEntity = this.GetTask(comment.CommentedEntryID);
			}
			else
				comment.CommentedEntryPresent = false;
		}
		*/
		/*
		public void UpdateComment(IManagerEntity commentEntity)
		{
			Comment comment = (Comment)commentEntity;
			
			IStorageCommand updateCommentCommand = fDealer.CommandFactory.CreateUpdateCommand(fDealer);			
			
			updateCommentCommand.SetParam("EntityName","Comment");
			
			Hashtable values = new Hashtable();
			
			values.Add("Description",comment.Description);
			values.Add("Date",comment.Date);
			if (comment.CommentedEntryPresent)
				values.Add("EntryID",comment.CommentedEntryID);
			else			
				values.Add("EntryID",DBNull.Value);
					
			updateCommentCommand.SetParam("Values",values);
			
			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + comment.ID);
			
			updateCommentCommand.SetParam("Rules",rules);
			
			fDealer.ExecuteNonQuery(updateCommentCommand);
		}
		*/

		/*
		public bool isUpdatedComment(IManagerEntity commentEntity)
		{
			Comment newComment = (Comment)commentEntity;
			Comment oldComment = new Comment(this, newComment.ID);
			
			BindComment(oldComment);
			
			bool result = 			newComment.ID == oldComment.ID;
			result = result && 	newComment.CommentedEntryPresent == oldComment.CommentedEntryPresent;
			if (newComment.CommentedEntryPresent && oldComment.CommentedEntryPresent)
				result = result && 	newComment.CommentedEntryID == oldComment.CommentedEntryID;
			result = result && 	newComment.Date == oldComment.Date;
			result = result && 	newComment.Description == oldComment.Description;		
						
			return result;
		}
		*/

		public void DeleteComment(int id)
		{
			IStorageCommand deleteCommentCommand = fRepository.CommandFactory.GetDeleteCommand("Comment");

			Hashtable rules = new Hashtable();
			rules.Add("UniqueID","ID = " + id);
			
			deleteCommentCommand.SetParam("Rules",rules);
			
			fRepository.ExecuteNonQuery (deleteCommentCommand);
		}
		
		#endregion
		
		#region IStorageManager Implementation
		
		public void Save()
		{
			fRepository.Save();
		}

		public void Update(IStorageRepository updateDealer)
		{
			if (!updateDealer.CheckConnection())
				throw new ManagementException(ExceptionType.ValidationFailed,"Update storage not allowed for ConnectionString " + fRepository.ConnectionString);
			updateDealer.Load();
						
			fRepository.Storage.Merge(updateDealer.Storage,true,System.Data.MissingSchemaAction.Error);			
		}

		public void BindProject(IManagerEntity taskEntity)
		{
			throw new NotImplementedException();
		}

		public async System.Threading.Tasks.Task SyncronizeAsync()
		{
			var exists = await fRepository.RestoreAsync(Path.GetFileName(fRepository.ConnectionString));
			await fRepository.BackupAsync(Path.GetFileName(fRepository.ConnectionString), exists);
			//fRepository.Revoke();
        }

		#endregion
	}
}