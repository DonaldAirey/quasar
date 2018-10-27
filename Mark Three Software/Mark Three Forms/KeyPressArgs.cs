namespace MarkThree.Forms
{
	using System;

	class KeyPressArgs
	{

		private char keyChar;
		private int selectionStart;
		private int selectionLength;
		private string text;

		public char KeyChar {get {return this.keyChar;}}
		public int SelectionStart {get {return this.selectionStart;}}
		public int SelectionLength {get {return this.selectionLength;}}
		public string Text {get {return this.text;}}

		public KeyPressArgs(char keyChar, string text, int selectionStart, int selectionLength)
		{

			this.keyChar = keyChar;
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
			this.text = text;

		}

	}

}
