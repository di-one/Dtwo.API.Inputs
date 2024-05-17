namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Informations about a click
    /// </summary>
    public struct ClickInfo
    {
        /// <summary>
        /// X position of the click
        /// </summary>
        public readonly int PosX;

        /// <summary>
        /// Y position of the click
        /// </summary>
        public readonly int PosY;

        /// <summary>
        /// Is a right click
        /// </summary>
        public readonly bool IsrightClick;

        /// <summary>
        /// Create a new instance of ClickInfo
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="isRightClick"></param>
        public ClickInfo(int posX, int posY, bool isRightClick)
        {
            PosX = posX;
            PosY = posY;
            IsrightClick = isRightClick;
        }
    }
}
