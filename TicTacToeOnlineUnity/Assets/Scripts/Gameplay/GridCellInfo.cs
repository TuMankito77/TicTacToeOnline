namespace TicTacToeOnline.Gameplay
{
    using UnityEngine;

    public struct GridCellInfo
    {
        //The canvas position gets assign when a player has marked one cell on the grid.
        //It's not available from the begining of the game. 
        public Vector2 canvasPosition;
        public PlayerType playerTypeOwner;
    }
}
