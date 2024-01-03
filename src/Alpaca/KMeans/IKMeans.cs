namespace Alpaca.KMeans
{
    public interface IKMeans
    {
        void Swap(ref AttribList attribs, int indexA, int indexB);

        void Normalize(IItemsList itemsList, int nAttribs,
            bool isUsers, ref double minVal, ref double maxVal);

        int LoadItemsFromFile(string filename);
        int LoadUsersFromFile(string filename);
        void Compute(int nInitialCentroids, int nItemsPerCluster);
    }
}