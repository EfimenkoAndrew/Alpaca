using System;

namespace Alpaca.Spectral
{
    public class Spectral
    {
        public double gamma;
        public int k;

        public Spectral(int k, double gamma)
        {
            this.k = k;
            this.gamma = gamma;
        } // ctor

        public int[] Cluster(double[][] X)
        {
            var A = MakeAffinityRBF(X); // RBF
            var L = MakeLaplacian(A); // normalized
            var E = MakeEmbedding(L); // eigenvectors
            var result = ProcessEmbedding(E); // k-means

            return result;
        }

        // ------------------------------------------------------

        private double[][] MakeAffinityRBF(double[][] X)
        {
            // 1s on diagonal (x1 == x2), towards 0 dissimilar
            var n = X.Length;
            var result = MatMake(n, n);
            for (var i = 0; i < n; ++i)
            for (var j = i; j < n; ++j) // upper
            {
                var rbf = MyRBF(X[i], X[j], gamma);
                result[i][j] = rbf;
                result[j][i] = rbf;
            }

            return result;
        }

        // ------------------------------------------------------

        private static double MyRBF(double[] v1, double[] v2,
            double gamma)
        {
            // similarity. when v1 == v2, rbf = 1.0
            // less similar returns small values between 0 and 1
            var dim = v1.Length;
            var sum = 0.0;
            for (var i = 0; i < dim; ++i)
                sum += (v1[i] - v2[i]) * (v1[i] - v2[i]);
            return Math.Exp(-gamma * sum);
        }

        // ------------------------------------------------------

        private double[][] MakeAffinityRNC(double[][] X)
        {
            // radius neighbors connectivity
            // 1 if x1 and x2 are close; 0 if not close
            var n = X.Length;
            var result = MatMake(n, n);
            for (var i = 0; i < n; ++i)
            for (var j = i; j < n; ++j) // upper
            {
                var d = Distance(X[i], X[j]);
                if (d < gamma)
                {
                    result[i][j] = 1.0;
                    result[j][i] = 1.0;
                }
            }

            return result;
        }

        // ------------------------------------------------------

        private static double Distance(double[] v1,
            double[] v2)
        {
            // helper for MakeAffinityRNC()
            var dim = v1.Length;
            var sum = 0.0;
            for (var j = 0; j < dim; ++j)
                sum += (v1[j] - v2[j]) * (v1[j] - v2[j]);
            return Math.Sqrt(sum);
        }

        // ------------------------------------------------------

        private double[][] MakeLaplacian(double[][] A)
        {
            // unnormalized
            // clear but not very efficient to construct D
            // L = D - A
            // here A is an affinity-style adjaceny matrix
            var n = A.Length;

            var D = MatMake(n, n); // degree matrix
            for (var i = 0; i < n; ++i)
            {
                var rowSum = 0.0;
                for (var j = 0; j < n; ++j)
                    rowSum += A[i][j];
                D[i][i] = rowSum;
            }

            var result = MatDifference(D, A); // D-A
            return NormalizeLaplacian(result);

            // more efficient, but less clear
            //int n = A.Length;
            //double[] rowSums = new double[n];
            //for (int i = 0; i < n; ++i)
            //{
            //  double rowSum = 0.0;
            //  for (int j = 0; j < n; ++j)
            //    rowSum += A[i][j];
            //  rowSums[i] = rowSum;
            //}

            //double[][] result = MatMake(n, n);
            //for (int i = 0; i < n; ++i)
            //  result[i][i] = rowSums[i];  // degree
            //for (int i = 0; i < n; ++i)
            //  for (int j = 0; j < n; ++j)
            //    if (i == j)
            //      result[i][j] = rowSums[i] - A[i][j];
            //    else
            //      result[i][j] = -A[i][j];
            //return result;
        }

        // ------------------------------------------------------

        private double[][] NormalizeLaplacian(double[][] L)
        {
            // scipy library csgraph._laplacian technique
            var n = L.Length;
            var result = MatCopy(L);
            for (var i = 0; i < n; ++i)
                result[i][i] = 0.0; // zap away diagonal

            // sqrt of col sums: in-degree version
            var w = new double[n];
            for (var j = 0; j < n; ++j)
            {
                var colSum = 0.0;
                for (var i = 0; i < n; ++i)
                    colSum += Math.Abs(result[i][j]);
                w[j] = Math.Sqrt(colSum);
            }

            for (var i = 0; i < n; ++i)
            for (var j = 0; j < n; ++j)
                result[i][j] /= w[j] * w[i];

            // restore diagonal
            for (var i = 0; i < n; ++i)
                result[i][i] = 1.0;

            return result;
        }

        // ------------------------------------------------------

        private double[][] MakeEmbedding(double[][] L)
        {
            // eigenvectors for k-smallest eigenvalues
            // extremely deep graph theory
            double[] eigVals;
            double[][] eigVecs;
            EigenQR(L, out eigVals, out eigVecs); // QR algorithm
            var allIndices = ArgSort(eigVals);
            var indices = new int[k]; // small eigenvecs
            for (var i = 0; i < k; ++i)
                indices[i] = allIndices[i];
            var extracted =
                MatExtractCols(eigVecs, indices);
            return extracted;
        }

        // ------------------------------------------------------

        private int[] ProcessEmbedding(double[][] E)
        {
            // cluster a complex transformation of source data
            var km = new KMeans(E, k);
            var clustering = km.Cluster();
            return clustering;
        }

        // ------------------------------------------------------

        private static double[][] MatDifference(double[][] ma,
            double[][] mb)
        {
            var r = ma.Length;
            var c = ma[0].Length;
            var result = MatMake(r, c);
            for (var i = 0; i < r; ++i)
            for (var j = 0; j < c; ++j)
                result[i][j] = ma[i][j] - mb[i][j];
            return result;
        }

        // ------------------------------------------------------

        private static int[] ArgSort(double[] vec)
        {
            var n = vec.Length;
            var idxs = new int[n];
            for (var i = 0; i < n; ++i)
                idxs[i] = i;
            Array.Sort(vec, idxs); // sort idxs based on vec
            return idxs;
        }

        // ------------------------------------------------------

        private static double[][] MatExtractCols(double[][] m,
            int[] cols)
        {
            var r = m.Length;
            var c = cols.Length;
            var result = MatMake(r, c);

            for (var j = 0; j < cols.Length; ++j)
            for (var i = 0; i < r; ++i)
                result[i][j] = m[i][cols[j]];
            return result;
        }

        // === Eigen functions ==================================

        private static void EigenQR(double[][] M,
            out double[] eigenVals, out double[][] eigenVecs)
        {
            // compute eigenvalues and eigenvectors same time
            // stats.stackexchange.com/questions/20643/finding-
            //   matrix-eigenvectors-using-qr-decomposition

            var n = M.Length;
            var X = MatCopy(M); // mat must be square
            double[][] Q;
            double[][] R;
            var pq = MatIdentity(n);
            var maxCt = 10000;

            var ct = 0;
            while (ct < maxCt)
            {
                MatDecomposeQR(X, out Q, out R, false);
                pq = MatProduct(pq, Q);
                X = MatProduct(R, Q); // note order
                ++ct;

                if (MatIsUpperTri(X, 1.0e-8))
                    break;
            }

            // eigenvalues are diag elements of X
            var evals = new double[n];
            for (var i = 0; i < n; ++i)
                evals[i] = X[i][i];

            // eigenvectors are columns of pq
            var evecs = MatCopy(pq);

            eigenVals = evals;
            eigenVecs = evecs;
        }

        // ------------------------------------------------------

        private static bool MatIsUpperTri(double[][] mat,
            double tol)
        {
            var n = mat.Length;
            for (var i = 0; i < n; ++i)
            for (var j = 0; j < i; ++j)
                // check lower vals
                if (Math.Abs(mat[i][j]) > tol)
                    return false;
            return true;
        }

        // ------------------------------------------------------

        public static double[][] MatProduct(double[][] matA,
            double[][] matB)
        {
            var aRows = matA.Length;
            var aCols = matA[0].Length;
            var bRows = matB.Length;
            var bCols = matB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices");

            var result = MatMake(aRows, bCols);

            for (var i = 0; i < aRows; ++i) // each row of A
            for (var j = 0; j < bCols; ++j) // each col of B
            for (var k = 0; k < aCols; ++k)
                result[i][j] += matA[i][k] * matB[k][j];

            return result;
        }

        // === QR decomposition functions =======================

        public static void MatDecomposeQR(double[][] mat,
            out double[][] q, out double[][] r,
            bool standardize)
        {
            // QR decomposition, Householder algorithm.
            // assumes square matrix

            var n = mat.Length; // assumes mat is nxn
            var nCols = mat[0].Length;
            if (n != nCols) Console.WriteLine("M not square ");

            var Q = MatIdentity(n);
            var R = MatCopy(mat);
            for (var i = 0; i < n - 1; ++i)
            {
                var H = MatIdentity(n);
                var a = new double[n - i];
                var k = 0;
                for (var ii = i; ii < n; ++ii) // last part col [i]
                    a[k++] = R[ii][i];

                var normA = VecNorm(a);
                if (a[0] < 0.0) normA = -normA;
                var v = new double[a.Length];
                for (var j = 0; j < v.Length; ++j)
                    v[j] = a[j] / (a[0] + normA);
                v[0] = 1.0;

                var h = MatIdentity(a.Length);
                var vvDot = VecDot(v, v);
                var alpha = VecToMat(v, v.Length, 1);
                var beta = VecToMat(v, 1, v.Length);
                var aMultB = MatProduct(alpha, beta);

                for (var ii = 0; ii < h.Length; ++ii)
                for (var jj = 0; jj < h[0].Length; ++jj)
                    h[ii][jj] -= 2.0 / vvDot * aMultB[ii][jj];

                // copy h into lower right of H
                var d = n - h.Length;
                for (var ii = 0; ii < h.Length; ++ii)
                for (var jj = 0; jj < h[0].Length; ++jj)
                    H[ii + d][jj + d] = h[ii][jj];

                Q = MatProduct(Q, H);
                R = MatProduct(H, R);
            } // i

            if (standardize)
            {
                // standardize so R diagonal is all positive
                var D = MatMake(n, n);
                for (var i = 0; i < n; ++i)
                    if (R[i][i] < 0.0) D[i][i] = -1.0;
                    else D[i][i] = 1.0;
                Q = MatProduct(Q, D);
                R = MatProduct(D, R);
            }

            q = Q;
            r = R;
        } // MatDecomposeQR()

        // ------------------------------------------------------

        private static double VecDot(double[] v1,
            double[] v2)
        {
            var result = 0.0;
            var n = v1.Length;
            for (var i = 0; i < n; ++i)
                result += v1[i] * v2[i];
            return result;
        }

        // ------------------------------------------------------

        private static double VecNorm(double[] vec)
        {
            var n = vec.Length;
            var sum = 0.0;
            for (var i = 0; i < n; ++i)
                sum += vec[i] * vec[i];
            return Math.Sqrt(sum);
        }

        // ------------------------------------------------------

        private static double[][] VecToMat(double[] vec,
            int nRows, int nCols)
        {
            var result = MatMake(nRows, nCols);
            var k = 0;
            for (var i = 0; i < nRows; ++i)
            for (var j = 0; j < nCols; ++j)
                result[i][j] = vec[k++];
            return result;
        }

        // === common ===========================================

        private static double[][] MatMake(int rows, int cols)
        {
            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        // ------------------------------------------------------

        private static double[][] MatCopy(double[][] mat)
        {
            var r = mat.Length;
            var c = mat[0].Length;
            var result = MatMake(r, c);
            for (var i = 0; i < r; ++i)
            for (var j = 0; j < c; ++j)
                result[i][j] = mat[i][j];
            return result;
        }

        // ------------------------------------------------------

        private static double[][] MatIdentity(int n)
        {
            var result = MatMake(n, n);
            for (var i = 0; i < n; ++i)
                result[i][i] = 1.0;
            return result;
        }

        // === nested KMeans ====================================

        private class KMeans
        {
            public int[] clustering;
            public readonly double[][] data;
            public readonly int dim;
            public readonly int k;
            public readonly int maxIter; // inner loop
            public double[][] means;
            public readonly int N;
            public Random rnd;
            public readonly int trials; // to find best

            // ----------------------------------------------------

            public KMeans(double[][] data, int k)
            {
                this.data = data; // by ref
                this.k = k;
                N = data.Length;
                dim = data[0].Length;
                trials = N; // for Cluster()
                maxIter = N * 2; // for ClusterOnce()
                Initialize(0); // seed, means, clustering
            }

            // ----------------------------------------------------

            public void Initialize(int seed)
            {
                rnd = new Random(seed);
                clustering = new int[N];
                means = new double[k][];
                for (var i = 0; i < k; ++i)
                    means[i] = new double[dim];
                // Random Partition (not Forgy)
                var indices = new int[N];
                for (var i = 0; i < N; ++i)
                    indices[i] = i;
                Shuffle(indices);
                for (var i = 0; i < k; ++i) // first k items
                    clustering[indices[i]] = i;
                for (var i = k; i < N; ++i)
                    clustering[indices[i]] =
                        rnd.Next(0, k); // remaining items

                UpdateMeans();
            }

            // ----------------------------------------------------

            private void Shuffle(int[] indices)
            {
                var n = indices.Length;
                for (var i = 0; i < n; ++i)
                {
                    var r = rnd.Next(i, n);
                    var tmp = indices[i];
                    indices[i] = indices[r];
                    indices[r] = tmp;
                }
            }

            // ----------------------------------------------------

            private static double SumSquared(double[] v1,
                double[] v2)
            {
                var dim = v1.Length;
                var sum = 0.0;
                for (var i = 0; i < dim; ++i)
                    sum += (v1[i] - v2[i]) * (v1[i] - v2[i]);
                return sum;
            }

            // ----------------------------------------------------

            private static double Distance(double[] item,
                double[] mean)
            {
                var ss = SumSquared(item, mean);
                return Math.Sqrt(ss);
            }

            // ----------------------------------------------------

            private static int ArgMin(double[] v)
            {
                var dim = v.Length;
                var minIdx = 0;
                var minVal = v[0];
                for (var i = 0; i < v.Length; ++i)
                    if (v[i] < minVal)
                    {
                        minVal = v[i];
                        minIdx = i;
                    }

                return minIdx;
            }

            // ----------------------------------------------------

            private static bool AreEqual(int[] a1, int[] a2)
            {
                var dim = a1.Length;
                for (var i = 0; i < dim; ++i)
                    if (a1[i] != a2[i])
                        return false;
                return true;
            }

            // ----------------------------------------------------

            private static int[] Copy(int[] arr)
            {
                var dim = arr.Length;
                var result = new int[dim];
                for (var i = 0; i < dim; ++i)
                    result[i] = arr[i];
                return result;
            }

            // ----------------------------------------------------

            public bool UpdateMeans()
            {
                // verify no zero-counts
                var counts = new int[k];
                for (var i = 0; i < N; ++i)
                {
                    var cid = clustering[i];
                    ++counts[cid];
                }

                for (var kk = 0; kk < k; ++kk)
                    if (counts[kk] == 0)
                        throw
                            new Exception("0-count in UpdateMeans()");

                // compute proposed new means
                for (var kk = 0; kk < k; ++kk)
                    counts[kk] = 0; // reset
                var newMeans = new double[k][];
                for (var i = 0; i < k; ++i)
                    newMeans[i] = new double[dim];
                for (var i = 0; i < N; ++i)
                {
                    var cid = clustering[i];
                    ++counts[cid];
                    for (var j = 0; j < dim; ++j)
                        newMeans[cid][j] += data[i][j];
                }

                for (var kk = 0; kk < k; ++kk)
                    if (counts[kk] == 0)
                        return false; // bad attempt to update

                for (var kk = 0; kk < k; ++kk)
                for (var j = 0; j < dim; ++j)
                    newMeans[kk][j] /= counts[kk];

                // copy new means
                for (var kk = 0; kk < k; ++kk)
                for (var j = 0; j < dim; ++j)
                    means[kk][j] = newMeans[kk][j];

                return true;
            } // UpdateMeans()

            // ----------------------------------------------------

            public bool UpdateClustering()
            {
                // verify no zero-counts
                var counts = new int[k];
                for (var i = 0; i < N; ++i)
                {
                    var cid = clustering[i];
                    ++counts[cid];
                }

                for (var kk = 0; kk < k; ++kk)
                    if (counts[kk] == 0)
                        throw new
                            Exception("0-count in UpdateClustering()");

                // proposed new clustering
                var newClustering = new int[N];
                for (var i = 0; i < N; ++i)
                    newClustering[i] = clustering[i];

                var distances = new double[k];
                for (var i = 0; i < N; ++i)
                for (var kk = 0; kk < k; ++kk)
                {
                    distances[kk] =
                        Distance(data[i], means[kk]);
                    var newID = ArgMin(distances);
                    newClustering[i] = newID;
                }

                if (AreEqual(clustering, newClustering))
                    return false; // no change; short-circuit

                // make sure no count went to 0
                for (var i = 0; i < k; ++i)
                    counts[i] = 0; // reset
                for (var i = 0; i < N; ++i)
                {
                    var cid = newClustering[i];
                    ++counts[cid];
                }

                for (var kk = 0; kk < k; ++kk)
                    if (counts[kk] == 0)
                        return false; // bad update attempt

                // no 0 counts so update
                for (var i = 0; i < N; ++i)
                    clustering[i] = newClustering[i];

                return true;
            } // UpdateClustering()

            // ----------------------------------------------------

            public int[] ClusterOnce()
            {
                var ok = true;
                var sanityCt = 1;
                while (sanityCt <= maxIter)
                {
                    if (ok = UpdateClustering() == false) break;
                    if (ok = UpdateMeans() == false) break;
                    ++sanityCt;
                }

                return clustering;
            } // ClusterOnce()

            // ----------------------------------------------------

            public double WCSS()
            {
                // within-cluster sum of squares
                var sum = 0.0;
                for (var i = 0; i < N; ++i)
                {
                    var cid = clustering[i];
                    var mean = means[cid];
                    var ss = SumSquared(data[i], mean);
                    sum += ss;
                }

                return sum;
            }

            // ----------------------------------------------------

            public int[] Cluster()
            {
                var bestWCSS = WCSS(); // initial clustering
                var bestClustering = Copy(this.clustering);

                for (var i = 0; i < trials; ++i)
                {
                    Initialize(i); // new seed, means, clustering
                    var clustering = ClusterOnce();
                    var wcss = WCSS();
                    if (wcss < bestWCSS)
                    {
                        bestWCSS = wcss;
                        bestClustering = Copy(clustering);
                    }
                }

                return bestClustering;
            } // Cluster()
        } // class KMeans

        // ======================================================
    }
}