using System;
using System.Collections;
using System.Reflection;

namespace Shadows.Quasar.Rule
{
	/// <summary>
	/// Summary description for TypeMethodList.
	/// </summary>
	public class TypeMethod
	{

		private object instance;
		private ArrayList methodList;

		public object Instance {get {return this.instance;}}

		public ArrayList MethodList {get {return this.methodList;}}

		public TypeMethod(object instance)
		{
			this.instance = instance;
			this.methodList = new ArrayList();
		}

	}

}
