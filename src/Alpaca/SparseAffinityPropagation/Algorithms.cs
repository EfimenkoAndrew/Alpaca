namespace Alpaca.SparseAffinityPropagation
{
    public static class Algorithms
    {
        public static void Swap<T>(ref T x, ref T y)
        {
            var tempswap = x;
            x = y;
            y = tempswap;
        }

        private static int qpartition(Edge[] arr, int low, int high)
        {
            Edge temp;
            var pivot = arr[high];
            var i = low - 1;
            for (var j = low; j <= high - 1; j++)
                if (arr[j].Similarity <= pivot.Similarity)
                {
                    i++;
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }

            temp = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = temp;

            return i + 1;
        }

        // Implementation of QuickSelect
        public static float[] k2thSmallest(ref Edge[] a, int left, int right, int k)
        {
            var s = new float[2];
            var temp = 0;
            while (left <= right)
            {
                // Partition a[left..right] around a pivot
                // and find the position of the pivot
                var pivotIndex = qpartition(a, left, right);

                // If pivot itself is the k-th smallest element
                if (pivotIndex == k - 1)
                {
                    s[1] = a[pivotIndex].Similarity;
                    s[0] = a[temp].Similarity;

                    return s;
                }

                // If there are more than k-1 elements on
                // left of pivot, then k-th smallest must be
                // on left side.

                if (pivotIndex > k - 1)
                    right = pivotIndex - 1;

                // Else k-th smallest is on right side.
                else
                    left = pivotIndex + 1;

                temp = pivotIndex;
            }

            return null;
        }
    }
}