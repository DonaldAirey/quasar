namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public delegate void ActionHandler(object[] parameters);

	public struct Action
	{

		// Public Members
		private ActionHandler actionHandler;
		private object[] parameters;

		public Action(ActionHandler actionHandler, params object[] parameters)
		{

			// Initialize the object
			this.actionHandler = actionHandler;
			this.parameters = parameters;

		}

		public void DoAction()
		{
			this.actionHandler(this.parameters);
		}

	}

}
