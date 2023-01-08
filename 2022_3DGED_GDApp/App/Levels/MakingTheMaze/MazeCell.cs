
namespace GD.App
{
    /// <summary>
    /// This sets up our maze cells which is comprised of 4 walls, we also set the visited to false here which will allow us
    /// to use our depth search algorithm in our maze class
    /// </summary>
    public class MazeCell
    {
        public bool[] Walls = new bool[4] { true, true, true, true };
        public bool Visited = false;
    }
}
