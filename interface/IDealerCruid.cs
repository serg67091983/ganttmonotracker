//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.11.2005 at 23:29

namespace TaskManagerInterface
{
	public interface IDealerCruid
	{
		void SetDealer(IStorageDealer daler);


		IStorageCommand GetSelectCommand(string entityName);


		IStorageCommand GetInsertCommand(string entityName);


		IStorageCommand GetUpdateCommand(string entityName);


		IStorageCommand GetDeleteCommand(string entityName);
	}
}
