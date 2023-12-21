using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KMeans
{
    public class KMeans : IKMeans
    {
        private readonly IItemsList _mItems;
        private readonly IItemsList _mUsers;
        private readonly Random _rnd = new Random();
        private ClustersList _mClusters;
        private double _mMaxVal;

        private double _mMinVal;
        private ClustersList _mTargetClusters;

        public KMeans()
        {
            _mItems = new ItemsList();
            _mUsers = new ItemsList();
        }

        public void Swap(ref AttribList attribs, int indexA, int indexB)
        {
            (attribs[indexA], attribs[indexB]) = (attribs[indexB], attribs[indexA]);
        }

        public void Normalize(
            IItemsList itemsList,
            int nAttribs,
            bool isUsers,
            ref double minVal,
            ref double maxVal)
        {
            // Performing a check if the minimum and maximum value are equal to 0
            if (minVal == 0 && maxVal == 0)
            {
                // Assigning the initial values to min_val and max_val variable,
                // which represent the boundaries at which the value of each attribute is normalized
                minVal = (double)1 / nAttribs;
                maxVal = (double)nAttribs / (nAttribs + 1);
            }

            // Iterating through the array of items and for each item items_list[iItem]
            // performing normalization by distributing the value of each attribute in
            // the range of [0;1] using local extremum formula
            for (var iItem = 0; iItem < itemsList.Count(); iItem++)
            {
                // For the current item items_list[iItem].AttribList retriving the array of attributes
                var attribsTarget = itemsList[iItem].AttribList;
                // Iterating through the array of attributes and for each attribute perform normalization
                // by converting its value into the value from the range [0;1] using the following formula
                for (var iAttrib = 0; iAttrib < attribsTarget.Count(); iAttrib++)
                    // Performing a check if the value of the current attribute AttribsTarget[iAttrib].Value
                    // exceeding the [0;1] range and this is not the user's attribute
                    if (attribsTarget[iAttrib].Value > 1 || isUsers == false)
                        // If so, applying the following formula to normalize the current attribute value
                        attribsTarget[iAttrib].Value = (attribsTarget[iAttrib].Value /
                            (nAttribs + 1) - minVal) / (maxVal - minVal) + 0.01;
            }
        }

        public int LoadItemsFromFile(string filename)
        {
            // Intializing the file stream object and opening the file with the name being specified
            using (var fsFile = new FileStream(filename,
                       FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Initializing the stream reader object
                using (var fsStream = new StreamReader(
                           fsFile, Encoding.UTF8, true, 128))
                {
                    var iItem = 0;
                    var nAttrib = 1;
                    var textBuf = "\0";
                    // Retrieving each line from the file until we reach the end-of-file
                    while ((textBuf = fsStream.ReadLine()) != null)
                        // Performing if the line is not empty and contains the data on a specific item
                        if (!string.IsNullOrEmpty(textBuf))
                        {
                            // If so, initializing the array of attributes TargetAttribList for the current item
                            var targetAttribList = new AttribList();
                            // Tokenizing the string according to the regular expression pattern assigned to the sItemPatern variable
                            var sItemPattern = " => ";
                            string[] sItemTokens;
                            if ((sItemTokens = Regex.Split(textBuf, sItemPattern)) != null)
                                // Iterating through the array of tokens and for each string
                                // perform another tokenization to obtain the set of attributes name for the current item
                                for (var iToken = 0; iToken < 2; iToken++)
                                {
                                    // For each string sItemTokens[iToken] we're performing tokenization to obtain the set of attributes names
                                    var sPattern = " ";
                                    string[] sTokens;
                                    if ((sTokens = Regex.Split(sItemTokens[iToken], sPattern)) != null)
                                        // For each particular attribute name token we're performing encoding
                                        // to obtain each attribute value associated with its name
                                        foreach (var token in sTokens)
                                        {
                                            // At this point, we're performing a check if the attribute with similar name
                                            // for a specific item has not been already indexed into the array of attributes
                                            var bExists = false;
                                            var nToken = 0;
                                            var nIndex = iItem;
                                            double nCoeff = 0;
                                            // Iterating the array of items to find those items that have
                                            // the attribute with the name which is equal to the name of
                                            // the current attribute attribs[nToken].Name.Equals(token) being processed
                                            while (--nIndex >= 0 && bExists == false)
                                            {
                                                nToken = 0;
                                                // Iterating through the array of attributes of the current item m_Items[nIndex]
                                                // and performing a check if a certain atribute's name of the current item is not equal
                                                // the name of the current attributed being retrieved from the file. If so, we're ending
                                                // the loop execution by assinging the the bExists variable value of true
                                                var attribs = _mItems[nIndex].AttribList;
                                                while (nToken < attribs.Count() && bExists == false)
                                                    bExists = attribs[nToken++].Name.Equals(token) ? true : false;
                                            }

                                            // Computing the coefficient value for the current attribute retrieved from the file.
                                            // If an item with the similar name of attribute has already been indexed into the array
                                            // of items we're assigning the its attribute's value to the nCoeff variable, which is
                                            // actually the value of the attribute for the current item fetched from the file, otherwise
                                            // we're assigning the actual index value for the current attributes using nAttrib counter
                                            // variable value
                                            nCoeff = bExists
                                                ? _mItems[nIndex + 1].AttribList[nToken - 1].Value
                                                : nAttrib;

                                            var bInAttribList = false;
                                            var iAttrib = 0;
                                            // Iterating through the array of target attributes and performing a check if the
                                            // attribute with the similar name has not yet been indexed to the following array for current item
                                            while (iAttrib < targetAttribList.Count() && !bInAttribList)
                                                bInAttribList = token.Equals(targetAttribList[iAttrib++].Name)
                                                    ? true
                                                    : false;

                                            // If the current attribute has not yet been indexed, inserting the new attribute
                                            // represented as a pair of two value of either token name or coefficient nCoeff into
                                            // the array of attributes for the current item being retrieved from the file
                                            if (bInAttribList == false)
                                                targetAttribList.Add(new Attrib(token, nCoeff));

                                            nAttrib++; // Incrementing the value of the attributes loop counter variable by 1
                                        }
                                }

                            // Inserting the current item retrieved from the file into the array of items m_Items
                            _mItems.Add(new Item(textBuf, targetAttribList, 0, false, false));

                            iItem++; // Incrementing the value of the items loop counter variable by 1
                        }

                    // Performing normalization of the attributes values for each item the array of items
                    Normalize(_mItems, nAttrib, false, ref _mMinVal, ref _mMaxVal);

                    // Deallocating the stream reader object
                    fsStream.Close();
                }

                // Deallocating the file stream object
                fsFile.Close();
            }

            // Returning the actual value of the number of items retrieved from the file
            return _mItems.Count();
        }

        public int LoadUsersFromFile(string filename)
        {
            // Intializing the file stream object and opening the file with the name being specified
            using (var fsFile = new FileStream(filename,
                       FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Initializing the stream reader object
                using (var fsStream = new StreamReader(
                           fsFile, Encoding.UTF8, true, 128))
                {
                    var iItem = 0;
                    var nAttrib = 1;
                    var textBuf = "\0";
                    // Retrieving each line from the file until we reach the end-of-file
                    while ((textBuf = fsStream.ReadLine()) != null)
                        // Performing if the line is not empty and contains the data on a specific user
                        if (!string.IsNullOrEmpty(textBuf))
                        {
                            // If so, initializing the array of attributes TargetAttribList for the current user
                            var sPattern = " => ";
                            string[] sTokens;
                            var targetAttribList = new AttribList();
                            // Tokenizing the string according to the regular expression pattern assigned to the sItemPatern variable
                            if ((sTokens = Regex.Split(textBuf, sPattern)) != null)
                            {
                                // For each particular attribute name token we're performing encoding
                                // to obtain each attribute value associated with its name
                                foreach (var token in sTokens[1].Split(' '))
                                {
                                    // At this point, we're performing a check if the attribute with similar name
                                    // for a specific user has not been already indexed into the array of attributes
                                    var bExists = false;
                                    var nToken = 0;
                                    var nIndex = 0;
                                    double nCoeff = 0;
                                    // Iterating the array of users to find those users that have
                                    // the attribute with the name which is equal to the name of
                                    // the current attribute attribs[nToken].Name.Equals(token) being processed
                                    while (nIndex < _mItems.Count() && bExists == false)
                                    {
                                        nToken = 0;
                                        // Iterating through the array of attributes of the current user m_Users[nIndex]
                                        // and performing a check if a certain atribute's name of the current user is not equal
                                        // the name of the current attributed being retrieved from the file. If so, we're ending
                                        // the loop execution by assinging the the bExists variable value of true
                                        while (nToken < _mItems[nIndex].AttribList.Count() && bExists == false)
                                            bExists = _mItems[nIndex].AttribList[nToken++].Name.Equals(token)
                                                ? true
                                                : false;

                                        nIndex++;
                                    }

                                    // If the users with hat have the attribute with the name which is equal to the name of
                                    // the current attribute attribs[nToken].Name.Equals(token) don't exists, then we're
                                    // iterating through the array of users performing a check if the attribute with similar
                                    // has already been indexed for a particular user into the array m_Users.
                                    if (bExists == false)
                                    {
                                        var nItem = iItem - 1;
                                        var bUserAttrib = false;
                                        // Iterating through the set of previous users in the array of users m_Users
                                        while (nItem >= 0 && bUserAttrib == false)
                                        {
                                            nToken = 0;
                                            // For each user, iterating through the array of attributes, and for each attribute's
                                            // name we're performing a check if the name of the current attribute of the current user
                                            // is not equal to the name of the current token retrieved from the file.
                                            while (nToken < _mUsers[nItem].AttribList.Count() && !bUserAttrib)
                                                bUserAttrib = _mUsers[nItem].AttribList[nToken++].Name.Equals(token)
                                                    ? true
                                                    : false;

                                            nItem--;
                                        }

                                        // Computing the coefficient value for the current attribute retrieved from the file.
                                        // If a user with the similar name of attribute has already been indexed into the array
                                        // of users we're assigning the its attribute's value to the nCoeff variable, which is
                                        // actually the value of the attribute for the current user fetched from the file, otherwise
                                        // we're assigning the actual index value for the current attributes using nAttrib counter
                                        // variable value
                                        nCoeff = bUserAttrib
                                            ? _mUsers[nItem + 1].AttribList[nToken - 1].Value
                                            : nAttrib;
                                    }

                                    // Otherwise, assigning the nCoeff variable to the value of the attribute of a specific user that
                                    // has already been indexed into the array of users
                                    else
                                    {
                                        nCoeff = _mItems[nIndex - 1].AttribList[nToken - 1].Value;
                                    }

                                    // Inserting the new attribute represented as a pair of two value of either token name
                                    // or coefficient nCoeff into the array of attributes for the current item being retrieved from the file
                                    targetAttribList.Add(new Attrib(token, nCoeff));

                                    nAttrib++; // Incrementing the value of the attributes loop counter variable by 1
                                }

                                // Inserting the current user retrieved from the file into the array of users m_Users
                                _mUsers.Add(new Item(textBuf, targetAttribList, 0, true, false));

                                iItem++; // Incrementing the value of the users loop counter variable by 1
                            }
                        }

                    // Performing normalization of the attributes values for each user the array of users
                    Normalize(_mUsers, nAttrib, true, ref _mMinVal, ref _mMaxVal);

                    // Deallocating the stream reader object
                    fsStream.Close();
                }

                // Deallocating the file stream object
                fsFile.Close();
            }

            // Returning the actual value of the number of users retrieved from the file
            return _mUsers.Count();
        }

        public void Compute(int nInitialCentroids, int nItemsPerCluster)
        {
            // Initializing the array of target clusters for which we'll produce the new clusters
            _mTargetClusters = new ClustersList();
            // Performing the iteration until we've obtained the array of target clusters
            while (_mTargetClusters.Count() < _mUsers.Count())
            {
                // Initializing the array of clusters
                _mClusters = new ClustersList();
                // Performing a check if the number of centroids within the initial cluster is not equal to the number of users
                if (nInitialCentroids != _mUsers.Count())
                {
                    // Obtaining the array of initial centroids based on the values
                    // retrieved from the array of users by performing k-means++ procedure

                    // Initializing the array of centroid indexes
                    var centroidIndexes = new List<int>();
                    // Randomly generate the index of the first intial centroid
                    var nInitialCentroid = _rnd.Next(0, _mUsers.Count());
                    // Performing iteration until we've obtained the n-initial centroids
                    while (centroidIndexes.Count() < nInitialCentroids)
                    {
                        double nDistance = 0, nDistanceSum = 0;
                        double nDistanceMin = 0;
                        var nCntMin = -1;
                        // Iterating through the array of users and for each user compute the distance
                        // to the initial centroid being previously selected
                        for (var nItem = 0; nItem < _mUsers.Count(); nItem++)
                            // Performing a check if the index of the current user is not equal to
                            // the index of the intial centroid (i.e. user) in the array of users
                            if (nItem != nInitialCentroid)
                                // If so, computing the actual distance between the two vectors
                                // of either the current user m_Users[nItem] or initial centroid's m_Users[nInitialCentroid] attributes.
                                if ((nDistance = Math.Pow(_mUsers[nItem].EuclDw(_mUsers[nInitialCentroid]), 2.0)) >= 0)
                                {
                                    // If the following distance is less than the smallest distance to the initial centroid
                                    // m_Users[nInitialCentroid], then we're performing a check if the index of the current
                                    // user has not yet been inserted into the array of the centroids indexes.
                                    if (nDistance < nDistanceMin || nCntMin == -1)
                                    {
                                        var bFound = false;
                                        var iCntIndex = 0;
                                        // Iterating through the array of centroids indexes and for each index CentroidIndexes[iCntIndex]
                                        // in the array we're performing a check if it's not equal to the index of the current user nItem,
                                        // if so, we're ending the loop execution by assigning the value true to the variable bFound.
                                        while (iCntIndex < centroidIndexes.Count() && bFound == false)
                                            bFound = centroidIndexes[iCntIndex++] == nItem ? true : false;

                                        // If the current user's index is not in the array of the centroids indexes, then
                                        // we're assigning the variable nDistanceMin the value of the previously computed
                                        // distance nDistance. Also, we're assigning the index value of the current user to nCntMin variable
                                        if (bFound == false)
                                        {
                                            nDistanceMin = nDistance;
                                            nCntMin = nItem;
                                        }
                                    }

                                    // Computing the sum of the distances to the initial centroid for each user
                                    nDistanceSum += nDistance;
                                }

                        // Modify the value of nDistanceSum variable multiplying it by the randomly generate number
                        nDistanceSum = _rnd.NextDouble() * nDistanceSum;

                        var nIndex = 0;
                        double nSum = 0;
                        // Iterating through the array of users until the sum of distances between the vectors of attributes of
                        // each user and the initial centroid doesn't exceed the total value on the sum of distances for all users
                        while (nIndex < _mUsers.Count() && nSum < nDistanceSum)
                        {
                            var iTargetIndex = 0;
                            var bFound = false;
                            // For the current user m_Users[nIndex] computing the distance
                            // to the users that has been previously selected as an initial centroid
                            var nDist = Math.Pow(_mUsers[nIndex++].EuclDw(_mUsers[nCntMin]), 2.0);
                            // Performing a check if the index of the current user m_Users[nIndex] is not in the array CentroidIndexes.
                            while (iTargetIndex < centroidIndexes.Count() && !bFound)
                                bFound = centroidIndexes[iTargetIndex++] == nIndex ? true : false;

                            // If not, summing the distance value for the current user nDist with the nSum variable
                            if (bFound == false)
                                nSum += nDist;
                        }

                        // Performing a check if the value of the nCntMin variable representing the actual index
                        // of the user with the smallest distance to initial centroid is not equal to -1
                        if (nCntMin != -1)
                            // If not, inserting the index nCntMin to the array of centroids indexes
                            centroidIndexes.Add(nCntMin);
                    }

                    // Initializing the array of initial centroids
                    var centroidItems = new ItemsList();
                    // Iterating through the array of users and inserting each user
                    // m_Users[CentroidIndexes[iIndex] with index CentroidIndexes[iIndex] to the array of centroids
                    for (var iIndex = 0; iIndex < centroidIndexes.Count(); iIndex++)
                        centroidItems.Add(_mUsers[centroidIndexes[iIndex]]);

                    // Inserting the new current initial cluster to the array of clusters.
                    _mClusters.Add(new Cluster(centroidItems, _mItems));
                }

                // Inserting the initial cluster into the array of clusters
                else
                {
                    _mClusters.Add(new Cluster(_mUsers, _mItems));
                }

                // Iterating through the array of clusters, retrieving each cluster
                // to obtain the new clusters by performing k-means procedure
                for (var iCluster = 0; iCluster < _mClusters.Count(); iCluster++)
                {
                    // Clonning the array of items belonging to the current
                    // cluster m_Clusters[iCluster] by copying them into array Items
                    var items = (IItemsList)_mClusters[iCluster].Items.Clone();
                    // Clonning the array of centroids belonging to the current
                    // cluster m_Clusters[iCluster] by copying them into array Centroids
                    var centroids = (IItemsList)_mClusters[iCluster].Centroids.Clone();
                    // Iterating through the array of centroids (i.e. users) of the current cluster m_Clusters[iCluster]
                    for (var iCentroid = 0; iCentroid < centroids.Count(); iCentroid++)
                    {
                        // For each centroid Centroids[iCentroid] of the current cluster
                        // m_Clusters[iCluster] retriving the set of attributes and copy it into array attribsA
                        var attribsA = centroids[iCentroid].AttribList;
                        // Normalizing the set of attributes of the current centroid Centroids[iCentroid]
                        // Iterating through the array of attributes of the current centroid Centroids[iCentroid]
                        for (var iAttrib = 0; iAttrib < attribsA.Count(); iAttrib++)
                            // For each attribute retrieved from the set of attributes of the current centroid
                            // Centroids[iCentroid], we're performing a linear search in the array of items Items
                            // to find those items that have one or more attributes with similar name (e.g. which name
                            // is lexicographically equal to the name of the current attribute attribsA[iAttrib] of the
                            // centroid Centroids[iCentroid]).
                            // Iterating through the array of items and for each item Items[iItem] retrieving a set of attributes
                        for (var iItem = 0; iItem < items.Count(); iItem++)
                        {
                            // Copying the array of attributes for the current item Items[iItem] into the array attribsB
                            var attribsB = items[iItem].AttribList;
                            // Iterating through the array of attributes attribB of the current item Items[iItem] and perform a check
                            // if the name of the current attribute attribsB[nAttrib] is lexicographically equal to the name of the
                            // current attribute attribsA[iAttrib] of the current centroid (e.g. user)
                            for (var nAttrib = 0; nAttrib < attribsB.Count(); nAttrib++)
                                // If the name of current item's attribute attribsB[nAttrib] is lexicographically equal to
                                // the name of the current centroid's attribute attribsA[iAttrib], then we're performing a
                                // swap to ensure that the particular attributes of either the centroid (user) or item are
                                // located at the same position in the arrays representing the vectors of either user or
                                // items attributes, relative to the position of the centroid's (user) of a particular attributes
                                // in the array of attributes of the current centroid Centroids[iCentroid]
                                if (attribsB[nAttrib].Name.Equals(attribsA[iAttrib].Name))
                                    if (iAttrib < attribsB.Count())
                                        Swap(ref attribsB, iAttrib, nAttrib);
                        }

                        // Initializing the variable nDistanceAvg to store the value of the average distance
                        // between the current centroid Centroids[iCentroid] and each item within the current cluster m_Clusters[iCluster]
                        double nDistanceAvg = 0;
                        // Initializing the list of indexes of those items that have the smallest distance
                        // to the current centroid Centroids[iCentroid]
                        var itemsIndexes = new List<int>();
                        var nNeighbors = items.Count() < nItemsPerCluster ? items.Count() : nItemsPerCluster;

                        var bDone = false;
                        // Performing the linear search to find all items in the array Items that have the smallest
                        // distance (e.g. the most similar items) to the current centroid Centroids[iCentroid] until
                        // we've exactly found nNeightbors items that exactly meet the following creteria
                        while (itemsIndexes.Count() < nNeighbors && !bDone)
                        {
                            // Initializing the variable nDistanceAvg to store the average distance within a cluster
                            nDistanceAvg = 0;
                            // Initializing the nDistanceMin variable used to store the value of the smallest distance
                            // between an item and the current centroid Centroids[iCentroid]
                            // Initializing the nItemMin variable to store the index value of the item from the array Items
                            // having the smallest distance to the current centroid Centroids[iCentroid]
                            double nDistanceMin = 0;
                            var nItemMin = -1;
                            // Iterating through the array of items of the current cluster m_Clusters[iCluster]
                            for (var iItem = 0; iItem < items.Count(); iItem++)
                            {
                                double nDistance = 0;
                                // For each item being retrieved we're performing a check if the distance for the current item
                                // has not already been computed and the current item is not the item having the smallest distance
                                // to the current centroid Centroids[iCentroid]
                                if (_mClusters[iCluster].Items[iItem].Exists == false)
                                {
                                    // If not, computing the distance between two vectors of attributes of either
                                    // the current centroid attribA or the current item Items[iItem]
                                    // Initializing the temporary array of the current centroid's attributes by copying
                                    // the array attribsA to the temporary array item
                                    var temp = new Item(null, attribsA, 0, false, false);
                                    // Computing the actual distance between the vector of the currentr centroid's attributes
                                    // stored to the array of attributes item, and the vector of attributes for the current item Items[iItem]
                                    if ((nDistance = items[iItem].EuclDw(temp)) >= 0)
                                        // If the value of distance between the either the current centroid's or item's vectors of attributes
                                        // is less than the smallest distance to the current centroid's vector of attributes, then we're performing
                                        // a check if the index of the current item's Items[iItem] has not already been stored into the array ItemsIndexes
                                        if ((nDistance < nDistanceMin ||
                                             nItemMin == -1) && items[iItem].ItemText != null && nDistance <= 1.0)
                                        {
                                            var bExists = false;
                                            var nItem = itemsIndexes.Count() - 1;
                                            // Performing the linear search to check if the index of the current item for which we've previously
                                            // computed the distance value, doesn't exists in the array of indexes.
                                            while (nItem >= 0 && bExists == false)
                                                bExists = itemsIndexes[nItem--] == iItem ? true : false;

                                            // If not, assigning the variable nDistanceMin the distance value between the vector of attributes
                                            // of the current centroid Centroids[iCentroid] and the vector of attributes of the current item Items[iItem]
                                            // Also, we're assigning the index value iItem of the current item Items[iItem] to the nItemMin variable
                                            if (bExists == false)
                                            {
                                                nDistanceMin = nDistance;
                                                nItemMin = iItem;
                                            }
                                        }
                                }

                                // Computing the avarage distance between the vector of attributes of each item
                                // Items[iItem] and the vector of attributes of the current centroid Centroids[iCentroid]
                                nDistanceAvg += nDistance / items.Count();
                            }

                            // If nItemMin variable is not equal to -1 and the appropriate value of the item with the smallest distance
                            // has been found, inserting the value of index nItemMin into the array ItemsIndexes.
                            if (nItemMin > -1)
                                itemsIndexes.Add(nItemMin);
                            // Otherwise, terminating the process of finding the items with the
                            // smallest distance to the current centroid Centroids[iCentroid]
                            else bDone = true;
                        }

                        // Iterating through the array of items of the current cluster m_Clusters[iCluster]
                        // and for each item Items[ItemsIndexes[iIndex] with the index value ItemsIndexes[iIndex]
                        // stored in the array ItemsIndexes, we're assigning the variable Exists to true, which
                        // means that the actual distance value for the current item has already been computed
                        // and the following item has already been included into the current the newly built cluster
                        for (var iIndex = 0; iIndex < itemsIndexes.Count(); iIndex++)
                            _mClusters[iCluster].Items[itemsIndexes[iIndex]].Exists = true;

                        // Updating the value of the variable Centroids[iCentroid].Distance by assigning it
                        // the value of the current averaged distance nDistanceAvg being computed
                        centroids[iCentroid].Distance = nDistanceAvg;

                        // Initializing the array of target items for the current newly built cluster
                        var targetItems = new ItemsList();
                        // Iterating through the array of indexes ItemsIndexes being obtained
                        for (var iItem = 0; iItem < itemsIndexes.Count(); iItem++)
                        {
                            // For the current cluster we're performing a check if the current cluster
                            // is not the initial cluster in the array of clusters (e.g. the following cluster
                            // has no more than one centroid).
                            if (_mClusters[iCluster].Centroids.Count() <= 1)
                                // If not, re-compute the distance between the current item with index ItemsIndexes[iItem]
                                // to the only centroid of the current non-initial cluster and assign this value to the
                                // Items[ItemsIndexes[iItem]].Distance variable for the current item
                                items[itemsIndexes[iItem]].Distance =
                                    items[itemsIndexes[iItem]].EuclDw(_mClusters[iCluster].Centroids[0]);

                            // Resetting the value of the exists variable for the current item in the newly built cluster
                            // by assigning it the variable of false.
                            items[itemsIndexes[iItem]].Exists = false;
                            // Inserting the current item with index ItemsIndexes[iItem] into the array of target items for the newly built cluster
                            targetItems.Add(items[itemsIndexes[iItem]]);
                        }

                        var nMinAttribs = -1;
                        // Iterating through the array of target items for the newly built cluster to
                        // find the item with smallest number of attributes
                        for (var iItem = 0; iItem < targetItems.Count(); iItem++)
                        {
                            // Obtaining the value of the actual number of attributes count of the current item TargetItems[iItem]
                            var nAttribCount = targetItems[iItem].AttribList.Count();
                            // If the value of attributes count nAttribCount is less than the smallest value of the attributes count for
                            // the entire array TargetItems, then assigning the value of nAttribCount to the nMinAttribs variable
                            if (nAttribCount < nMinAttribs || nMinAttribs == -1)
                                nMinAttribs = nAttribCount;
                        }

                        // Initializing the attribs list to store the computed values of attributes
                        // for the centroid of the newly built cluster
                        var attribs = new AttribList();
                        // Computing the new value for each attribute of the centroid for the newly build cluster by iterating
                        // through each attrbutes in each item, inserting each new value of attribute into the array attrib
                        for (var nAttrib = 0;
                             nAttrib < nMinAttribs &&
                             nAttrib < centroids[iCentroid].AttribList.Count();
                             nAttrib++)
                        {
                            // For each attribute of the current centroid Centroids[iCentroid] of the existing cluster
                            // obtaining the new value by computing the sum of each attribute AttribList[nAttrib] at the
                            // position nAttrib of the vector of attributes AttribList[nAttrib] for each target item being previously obtained
                            double nAttribAvg = 0;
                            var nCount = 0;
                            // Iterating through the array of target items TargetItems
                            for (var iItem = 0; iItem < targetItems.Count(); iItem++)
                                // For each item performing a check if the value of the attribute
                                // located at the current position nAttrib is greater than zero
                                if (targetItems[iItem].AttribList[nAttrib].Value > 0)
                                {
                                    // If so, adding up the value of the attribute located at the position nAttrib
                                    // with the value of the nAttribAvg variable. Also computing the count of the target
                                    // items that exactly meet the following condition by incrementing the count variable by 1
                                    nAttribAvg += targetItems[iItem].AttribList[nAttrib].Value;
                                    nCount++;
                                }

                            // Since we've obtained the new value of attribute AttribList[nAttrib].Value with index nAttrib,
                            // for the current centroid Centroids[iCentroid] we're computing the average value by performing
                            // the division of the nAttribAvg variable's value by the actual number of target items that
                            // satisfied the criteria commented above. After that, we're compacting the following value by
                            // performing normalization. Finally we're inserting the average value  into the array of
                            // atributes attrib along with the name of the new attribute which value has been obtained
                            attribs.Add(new Attrib(centroids[iCentroid].AttribList[nAttrib].Name,
                                (nAttribAvg / (nCount + 1) - _mMinVal) / (_mMaxVal - _mMinVal) + 0.01));
                        }

                        var bDiff = false;
                        var nIndex = 0;
                        // Iterating through the new vector of attributes attribs to determine if the following vector
                        // of attributes being obtained has the different values of attributes than the vector of attributes attribsA
                        // of the current centroid Centroids[iCentroid]
                        while (nIndex < attribs.Count() && nIndex < attribsA.Count() && bDiff == false)
                            bDiff = attribs[nIndex].Value != attribsA[nIndex++].Value ? true : false;

                        if (bDiff)
                        {
                            // If so, initializing the array of new centroids
                            var targetCentroids = new ItemsList();
                            // Inserting the new centroid with vector of attributes
                            // attribs into the array of centroids for the new cluster
                            targetCentroids.Add(new Item(centroids[iCentroid].ItemText,
                                attribs, nDistanceAvg, centroids[iCentroid].IsUser, false));

                            // Inserting newly built cluster represented as a pair
                            // of sets of either TargetCentroids or TargetItems to the array of clusters
                            _mClusters.Add(new Cluster(targetCentroids, targetItems));
                        }

                        else
                        {
                            // If not, iterating through the array of newly computed target clusters
                            // and for each cluster perfoming a check if there's a cluster with centroid which name
                            // value is equal to the name value of the current centroid Centroids[iCentroid]
                            var bExists = false;
                            var iTargetCluster = 0;
                            while (iTargetCluster < _mTargetClusters.Count() && !bExists)
                                bExists = _mTargetClusters[iTargetCluster++].Centroids[0].ItemText
                                    .Equals(centroids[iCentroid].ItemText)
                                    ? true
                                    : false;

                            if (bExists == false)
                            {
                                // If so, initializing the array of centroids TargetUsers and insert the
                                // new centroid into the following array.
                                var targetUsers = new ItemsList();
                                targetUsers.Add(new Item(centroids[iCentroid].ItemText,
                                    centroids[iCentroid].AttribList, centroids[iCentroid].Distance, true, true));
                                // Inserting the cluster into the array of target clusters for which we'll further
                                // be producing the new clusters during the next iteration of the outermost loop
                                // This fragment of code actually performs filtering to avoid the existance
                                // of the clusters with similar centroid's vectors of attributes in the array of clusters.
                                _mTargetClusters.Add(new Cluster((IItemsList)targetUsers.Clone(),
                                    (IItemsList)targetItems.Clone()));
                            }
                        }
                    }
                }
            }

            double nDiAvg = 0;
            // Computing the sum of the distances values for each target cluster
            for (var iCluster = 1; iCluster < _mTargetClusters.Count(); iCluster++)
                nDiAvg += _mTargetClusters[iCluster].Centroids[0].Distance / (_mTargetClusters.Count() - 1);

            double nD0Avg = 0;
            var nClustersCount = 0;
            // Computing the average distance value between each newly built cluster
            for (var iCluster = 1; iCluster < _mTargetClusters.Count(); iCluster++)
            for (var nCluster = iCluster + 1; nCluster < _mTargetClusters.Count(); nCluster++)
            {
                nD0Avg += _mTargetClusters[iCluster].Centroids[0].EuclDw(
                    _mTargetClusters[nCluster].Centroids[0]);

                nClustersCount++;
            }

            nD0Avg /= nClustersCount; // Computing the average distance between clusters
            // Computing the prediction quality value of the array of the newly built clusters.
            var nQ = (_mTargetClusters.Count() - 1) * nDiAvg / nD0Avg;

            // Performing the k-means clustering results verbose output
            for (var iCluster = 0; iCluster < _mTargetClusters.Count(); iCluster++)
            {
                var itemsList = _mTargetClusters[iCluster].Items;
                var centroids = _mTargetClusters[iCluster].Centroids;
                Console.WriteLine("\nCluster={0}, Centroid=\"{1}\"", iCluster, centroids[0].ItemText);
                Console.WriteLine("-----------------------------------------------------------");

                for (var iAttrib = 0; iAttrib < centroids[0].AttribList.Count(); iAttrib++)
                    Console.WriteLine("\"{0,-30}\" => u({1},{2}) = {3}", centroids[0].AttribList[iAttrib].Name,
                        iCluster, iAttrib, centroids[0].AttribList[iAttrib].Value);

                Console.WriteLine("-----------------------------------------------------------");

                for (var iItem = 0; iItem < itemsList.Count(); iItem++)
                {
                    Console.WriteLine("\n(cluster={0}, item={1}, distance={2})\n{3}",
                        iCluster, iItem, itemsList[iItem].Distance, itemsList[iItem].ItemText);
                    Console.WriteLine("-----------------------------------------------------------");
                    for (var iAttrib = 0; iAttrib < itemsList[iItem].AttribList.Count(); iAttrib++)
                        Console.WriteLine("\"{0,-30}\" => i({1},{2}) = {3}", itemsList[iItem].AttribList[iAttrib].Name,
                            iCluster, iAttrib, itemsList[iItem].AttribList[iAttrib].Value);

                    Console.WriteLine("-----------------------------------------------------------");
                }
            }

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("Recommendations:");
            Console.WriteLine("===========================================================\n");

            for (var iCluster = 0; iCluster < _mTargetClusters.Count(); iCluster++)
            {
                var itemsList = _mTargetClusters[iCluster].Items;
                Console.WriteLine("{0}", _mTargetClusters[iCluster].Centroids[0].ItemText);
                Console.WriteLine("======================================================");

                for (var iItem = 0; iItem < itemsList.Count(); iItem++)
                    Console.WriteLine("{0}", itemsList[iItem].ItemText);

                Console.WriteLine();
            }

            Console.WriteLine("KMeans Statistics:");
            Console.WriteLine("===========================================================");
            Console.WriteLine("The total number of clusters nClustersCount = {0}\n", _mTargetClusters.Count());
            Console.WriteLine("Average distance between clusters nDiAvg = {0}", nDiAvg);
            Console.WriteLine("Average distance within a cluster nD0Avg = {0}\n", nD0Avg);
            Console.WriteLine("Average quality of KMeans clustering nQ = {0}\n", nQ);
        }
    }
}