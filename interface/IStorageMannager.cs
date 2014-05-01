//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 22.01.2006 at 2:49

namespace TaskManagerInterface
{
	public interface IStorageManager
	{
		void Update(IStorageDealer updateDealer);


		void Save();
	}
}