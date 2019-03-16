public class Flipper : ISetup 
{
    public bool[,,] getSetup(int x, int y, int z)
    {
        bool[,,] arr = new bool[x, y, z];

        arr[5,5,0] = true;
        arr[6,5,0] = true;
        arr[7,5,0] = true;

        return arr;
    }
}
