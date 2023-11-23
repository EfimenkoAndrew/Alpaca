var filenames = new string[2] { @"ARTICLES.TXT", @"USERS.TXT" };
var sBanner = new string[2]
{
    "K-Means++ Recommender Engine v.1.2.65535 (Euclidix) by Arthur V. Ratz",
    "=====================================================================\n"
};

for (var iBanner = 0; iBanner < 2; iBanner++)
    Console.WriteLine(sBanner[iBanner]);

var km = new KMeans.KMeans();

var nItemsCount = km.LoadItemsFromFile(filenames[0]);
var nUsersCount = km.LoadUsersFromFile(filenames[1]);

var nInitialUsers = 0;
int nItemsPerCluster = 0, nItemsPerClusterMax = 0;
if (nItemsCount > 0 && nUsersCount > 0)
{
    do
    {
        nItemsPerClusterMax = nItemsCount / nUsersCount;
        Console.Write("Enter the number of articles per user [2-{0}]: ", nItemsPerClusterMax);
        nItemsPerCluster = int.Parse(Console.ReadLine());
    } while (nItemsPerCluster < 2 || nItemsPerCluster > nItemsPerClusterMax);

    do
    {
        Console.Write("\nEnter the number of users (initial centroids) [1-{0}]: ", nUsersCount);
        nInitialUsers = int.Parse(Console.ReadLine());
    } while (nInitialUsers < 1 || nInitialUsers > nUsersCount);
}

km.Compute(nInitialUsers, nItemsPerCluster);