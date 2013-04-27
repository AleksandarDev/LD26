using System.ComponentModel;
using Microsoft.Xna.Framework;
using SquaredEngine.PathEditor.Library;
using System;

namespace SquaredEngine.PathEditor
{
    class GameViewModel
    {
		public GameViewModel()
        {
            Game = new GameStart();
        }

        public GameStart Game { get; private set; }

		public void ChangeTool(PathEditorTools tool) {
			Game.ChangeTool(tool);
		}
    }
}
