namespace Dtwo.API.Inputs
{
    public struct ClickInfo
    {
        public int PosX;
        public int PosY;
        public bool IsrightClick;

        public ClickInfo(int posX, int posY, bool isRightClick)
        {
            PosX = posX;
            PosY = posY;
            IsrightClick = isRightClick;
        }
    }
}
