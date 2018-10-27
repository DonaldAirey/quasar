namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Text;

	public interface IViewerSite : ISite
	{

		void OpenObject(object parameter);

	}

}
