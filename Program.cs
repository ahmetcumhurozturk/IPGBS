using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IPGBS
{
    class Program
    {
        private Dictionary<string, int> vertexIndexes;
        private Dictionary<string, int> countOfSensitiveItems;
        private List<SensitiveTransactionTable> sensitiveTransactionTable;
        private Dictionary<string, List<int>> sanitizationTable;
        private List<string> longestPath;
        private List<string> discovered;
        private CreateLists list;
        private Dictionary<string, double> sensitive_data;
        private int databaseLenght;
        private Dictionary<string, int> coverDegrees;
        private List<string> data;
        private int itemsRemoved = 0;
        private void sortAdjacencyList()
        {
            foreach(string key in list.AdjacencyList.Keys)
            {
                
                foreach (string key2 in list.AdjacencyList[key].Keys)
                {
                   if(vertexIndexes[key2]>vertexIndexes[key])
                    {
                        if(!list.AdjacencyList.ContainsKey(key2))
                        {
                            list.AdjacencyList[key2] = new Dictionary<string, List<int>>();

                        }
                        if(!list.AdjacencyList[key2].ContainsKey(key))
                        {
                            list.AdjacencyList[key2][key] = new List<int>();
                        }
                        foreach(int i in list.AdjacencyList[key][key2])
                        {
                            list.AdjacencyList[key2][key].Add(i);
                        }
                        list.AdjacencyList[key].Remove(key2);
                        if(list.AdjacencyList[key].Count==0)
                        {
                            list.AdjacencyList.Remove(key);
                        }
                    }
                }
            }
        }

        private void readSensitiveDataFromTxt()
        {
            string line;
            string[] values;
            string[] dataSplit = new string[2];
            // Read the file and display it line by line.C:\Users\cumhur\Desktop\databaseForDegreePGBS\chess
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\cs\Desktop\datasets\sensitive.txt");
            sensitive_data = new Dictionary<string, double>();
            coverDegrees = new Dictionary<string, int>();
            int id = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (line != "")
                {
                    id++;
                    values = line.Split('.');
                    sensitive_data.Add(values[0], Convert.ToDouble(values[1]));
                    string[] splittedValues = values[0].Split(' ');
                    for (int i = 0; i < splittedValues.Length; i++)
                    {
                        if (coverDegrees.ContainsKey(splittedValues[i]))
                        {
                            coverDegrees[splittedValues[i]] = coverDegrees[splittedValues[i]] + 1;
                        }
                        else
                        {
                            coverDegrees.Add(splittedValues[i], 1);
                        }
                    }
                }
            }
            file.Close();

        }
        private bool sensitiveTransaction(string transaction, string sensitiveItemset)
        {

            string[] splittedA = transaction.Split(' ');
            string[] splittedB = sensitiveItemset.Split(' ');
            int lenghtOfA = splittedA.Length;
            int lenghtofB = splittedB.Length;

            int indexofB = 0;
            int result = 0;
            bool contains = true;
            while (indexofB < splittedB.Length && contains)
            {
                contains = false;
                for (int i = 0; i < splittedA.Length; i++)
                {
                    if (splittedA[i] == splittedB[indexofB])
                    {

                        indexofB++;
                        result++;
                        contains = true;
                        break;
                    }
                }



            }
            if (result == splittedB.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private CreateLists readDataFromTxt()
        {
            list = new CreateLists();
            List<int> sensitiveTransactionsIds = new List<int>();
            int index = 0;
            string[] dataSplit = new string[2];
            string line;
            data = new List<string>();
            //bool sensiztiveTransaction;
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(@"\Users\cs\Desktop\datasets\dataset.txt");
            List<string> added = new List<string>();
            int transactionId = 0;
            countOfSensitiveItems = new Dictionary<string, int>();
            string[] splittedLine;
            foreach(string itemset in sensitive_data.Keys)
            {
                splittedLine = itemset.Split(' ');
                for (int i = 0; i < splittedLine.Length; i++)
                {
                    countOfSensitiveItems[splittedLine[i]] = 0;
                }
            }
           
             while ((line = file.ReadLine()) != null)
            {
                // sensitiveTransaction = false;
                if (line[line.Length - 1].ToString() == " ")
                    line = line.Remove(line.Length - 1);
                data.Add(line);
                //yeni data.Add(line);
                List<string> transactions = new List<string>();
                added = new List<string>();
                foreach (string v1 in sensitive_data.Keys)
                {
                    if (sensitiveTransaction(line, v1))
                    {
                        splittedLine = v1.Split(' ');
                        for (int k = 0; k < splittedLine.Length; k++)
                        {
                            if (!added.Contains(splittedLine[k]))
                            {
                                countOfSensitiveItems[splittedLine[k]] = countOfSensitiveItems[splittedLine[k]] + 1;
                                added.Add(splittedLine[k]);
                            }
                        }
                        transactions.Add(v1);
                    }
                }
                if(transactions.Count>0)
                {
                    list.CreateAdjacencyList(transactions, transactionId);
                }
                //sadece sensitive itemları tutmak için değiştirildi
                /*    if (transactions.Count > 0)
                    {
                        List<string> itemsToBeRemoved = new List<string>();
                        List<string> splittedLine = line.Split(' ').ToList();
                        bool containsSensitiveItem = true;
                        foreach (string sensitiveItem in transactions)
                        {
                            List<string> splittedSensitiveItem = sensitiveItem.Split(' ').ToList();
                            foreach (string s in splittedSensitiveItem)
                            {
                                splittedLine.Remove(s);
                            }
                        }
                         if(splittedLine.Count>0)
                        {
                            foreach(string s in splittedLine)
                            {
                                transactions.Add(s);
                            }
                        }  
                        list.CreateAdjacencyList(transactions, transactionId);
                    }*/
                transactionId++;
                index++;
            }
            file.Close();
            databaseLenght = index;
            return list;
        }
        private List<int> findTransacitionsOfASensitiveItemset(string sensitiveItemset)
        {
            int[] arr2 = new int[databaseLenght];

            List<int> tmpTransactions = new List<int>();
            List<int> transactions = new List<int>();
            List<int> transactions2 = new List<int>();

            int[] arr = new int[databaseLenght];
            if (list.InverseAdjacencyList.ContainsKey(sensitiveItemset))
            {
                foreach (string k in list.InverseAdjacencyList[sensitiveItemset].Keys)
                {
                    foreach (int val2 in list.InverseAdjacencyList[sensitiveItemset][k])
                    {
                        arr[val2] = 1;
                    }
                }

            }


            if (list.AdjacencyList.ContainsKey(sensitiveItemset))
            {
                foreach (string key2 in list.AdjacencyList[sensitiveItemset].Keys)
                {
                    foreach (int val in list.AdjacencyList[sensitiveItemset][key2])
                    {

                        arr[val] = 1;
                    }

                }
            }
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 1)
                    transactions.Add(i);
            }
            return transactions;
        }
        private List<int> findAllTransactions(string pattern)// finds transactions of a pattern given in any lenght
        {
            string[] splittedPattern;
            splittedPattern = pattern.Split(' ');
            List<int> transactions = new List<int>();
            transactions = findTransacitionsOfASensitiveItemset(pattern);
           // transactions = findTwoOfTheTransactions(splittedPattern[0], splittedPattern[1]);
            if (splittedPattern.Length > 2)
            {
                for (int i = 1; i < splittedPattern.Length - 1; i++)
                {
                    //transactions=intersectTrsansactions(transactions,findTwoOfTheTransactions(splittedPattern[i],splittedPattern[i+1]));
                    // transactions = transactions.Intersect(findTwoOfTheTransactions(splittedPattern[i], splittedPattern[i + 1])).ToList();
                    transactions = intersectTransactions(transactions, findTwoOfTheTransactions(splittedPattern[i], splittedPattern[i + 1]));
                }
            }

            return transactions;
        }
        
        private void createSensitiveTransactionTable(int databaseSize)
        {
            sensitiveTransactionTable = new List<SensitiveTransactionTable>();
            int sid = 0;
            int spCount;
            List<int> transactions;
            foreach (string sensitiveItemset in sensitive_data.Keys)
            {
                if(sensitiveItemset== "11 90 148 218 521")
                {
                    
                }
                transactions = findTransacitionsOfASensitiveItemset(sensitiveItemset);

                if (sensitive_data[sensitiveItemset] == 0)
                {
                    spCount = transactions.Count;
                }
                else
                {
                    spCount = Convert.ToInt32(Math.Floor(transactions.Count - (sensitive_data[sensitiveItemset] * databaseSize) + 1));
                }
                sensitiveTransactionTable.Add(new SensitiveTransactionTable(sid, sensitiveItemset, spCount));
                sid++;
            }

            

        }
        private List<int> intersectTransactions(List<int> listA, List<int> listB)
        {

            // worst case size for the intersection array  


            int[] c = new int[Math.Min(listA.Count, listB.Count)];
            int ai = 0, bi = 0, ci = 0;

            while (ai < listA.Count && bi < listB.Count)
            {
                if (listA[ai] == listB[bi])
                {
                    // check if element already exists  
                    if (ci == 0 || listA[ai] != c[ci - 1])
                    {
                        c[ci++] = listA[ai];
                    }
                    ai++;
                    bi++;
                }
                else if (listA[ai] > listB[bi])
                {
                    bi++;
                }
                else if (listA[ai] < listB[bi])
                {
                    ai++;
                }
            }
            List<int> listC = new List<int>();
            for (int i = 0; i < ci; i++)
            {
                listC.Add(c[i]);
            }
            return listC;
        }
        private List<int> findTwoOfTheTransactions(string item1, string item2)//finds pattern with length 2
        {

            List<int> tmpTransactions = new List<int>();
            List<int> transactions = new List<int>();
            List<int> transactions2 = new List<int>();

            int[] arr = new int[databaseLenght];
            if (list.InverseAdjacencyList.ContainsKey(item2))
            {
                foreach (string k in list.InverseAdjacencyList[item2].Keys)
                {
                    tmpTransactions = list.InverseAdjacencyList[item2][k];
                    foreach (int val2 in tmpTransactions)
                    {
                        transactions2.Add(val2);
                    }
                }

            }



            foreach (string key2 in list.AdjacencyList[item1].Keys)
            {


                tmpTransactions = list.AdjacencyList[item1][key2];


                foreach (int val in tmpTransactions)
                {

                    arr[val] = 1;


                }

            }
            int[] arr2 = new int[databaseLenght];
            foreach (int x in transactions2)
            {
                if (arr[x] == 1)
                {
                    arr2[x] = 1;
                }
            }

            for (int i = 0; i < arr2.Length; i++)
            {
                if (arr2[i] == 1)
                    transactions.Add(i);
            }
            return transactions;
        }
        private void DFS(string v)
        {
            discovered.Add(v);
            if (list.AdjacencyList[v].Keys.Count > 0)
            {
                foreach (string w in list.AdjacencyList[v].Keys)
                {
                    if (!discovered.Contains(w) && w.Length > 1)
                    {
                        longestPath.Add(w);
                        DFS(w);
                    }
                }
            }
            

        }
       
        private void findDeepestTransaction(string sensitiveItemset)
        {
            List<int> transactions = new List<int>();
           foreach(string itemset in list.AdjacencyList[sensitiveItemset].Keys)
            {

            }
        }
        private string selectVictim(List<string> candidateTransactions)
        {
            List<int> indexOfItemsetsNotContainingVictim = new List<int>(); 
            bool contains = false;
            Dictionary<string, int> candidateVictims = new Dictionary<string, int>();
            string[] splittedSensitiveItemset = candidateTransactions[0].Split(' ');
            for (int i = 0; i < splittedSensitiveItemset.Length; i++)
            {
                candidateVictims[splittedSensitiveItemset[i]] = 1;
            }
            for(int j=1;j<candidateTransactions.Count;j++)
            {
                contains = false;
                string[] splittedCandidate = candidateTransactions[j].Split(' ');
                for(int a=0;a<splittedCandidate.Length;a++)
                {
                    
                    if (candidateVictims.ContainsKey(splittedCandidate[a]))
                    {
                        contains = true;
                        candidateVictims[splittedCandidate[a]] = candidateVictims[splittedCandidate[a]] + 1;
                    }
                }
                if(!contains)
                {
                    indexOfItemsetsNotContainingVictim.Add(j);

                }
            }
            //tek bir en büyük cover degree olabilir
            int highestCoverDegree=candidateVictims.ElementAt(0).Value;
            for(int i=1;i<candidateVictims.Count;i++)
            {
                if(candidateVictims.ElementAt(i).Value>highestCoverDegree)
                {
                    highestCoverDegree = candidateVictims.ElementAt(i).Value;
                }
            }
            for(int s=0;s<candidateVictims.Count;s++)
            {
                if(candidateVictims.ElementAt(s).Value!=highestCoverDegree)
                {
                    candidateVictims.Remove(candidateVictims.ElementAt(s).Key);
                }
            }
            //eğer birden çok victim var ise aynı cover degree'ye sahip o zaman support'u en büyük olanı seç.
            if(candidateVictims.Count>1)
            {
                int highestSupport = countOfSensitiveItems[candidateVictims.ElementAt(0).Key];
                for(int i=0;i<candidateVictims.Count;i++)
                {
                    if(highestSupport!=countOfSensitiveItems[candidateVictims.ElementAt(i).Key])
                    {
                        candidateVictims.Remove(candidateVictims.ElementAt(i).Key);
                    }
                }
            }
            return candidateVictims.ElementAt(0).Key;
        }
        private void deleteTransactions(int transactionID,List<string> longestPattern)
        {
            int i = 0;
            int j = 1;
            while(i<longestPattern.Count-1)
            {
                if (list.AdjacencyList[longestPattern[i]].ContainsKey(longestPattern[j]))
                {
                    list.AdjacencyList[longestPattern[i]][longestPattern[j]].Remove(transactionID);
                    i++;
                    j++;
                }
                else
                {
                    foreach(string k in list.AdjacencyList[longestPattern[i]].Keys)
                    {
                        if(list.AdjacencyList[longestPattern[i]][k].Contains(transactionID))
                            {
                            list.AdjacencyList[longestPattern[i]][k].Remove(transactionID);
                            break;
                        }
                    }
                    i++;
                    j++;
                }
            }
            if(list.InverseAdjacencyList[longestPattern[j-1]].ContainsKey(longestPattern[j-2]))
            {
                list.InverseAdjacencyList[longestPattern[j - 1]][longestPattern[j - 2]].Remove(transactionID);
            }
            else
            {
                foreach(string s in list.InverseAdjacencyList[longestPattern[j-1]].Keys)
                {
                    if(list.InverseAdjacencyList[longestPattern[j - 1]][s].Contains(transactionID))
                    {
                        list.InverseAdjacencyList[longestPattern[j - 1]][s].Remove(transactionID);
                    }
                }
            }
               
        }
        private void createSanitizationTable()
        {
            string victim;
            sanitizationTable = new Dictionary<string, List<int>>();
            List<int> transactionsToModify;
            List<string> itemsetsAlreadySanitized = new List<string>();
            int ind = 0;
            sanitizationTable = new Dictionary<string, List<int>>();
            while (ind < sensitiveTransactionTable.Count)
            {
                
                //itemsetsAlready stores stores the sensitive itemsets that are previously sanitized
                //it is for avoiding to decrease already sanitized sentive itemsets more than need.
                for (int i = 0; i < sensitiveTransactionTable.Count; i++)
                {
                    if (sensitiveTransactionTable[i].SpCount <= 0 && !itemsetsAlreadySanitized.Contains(sensitiveTransactionTable[i].Sp))
                    {
                        itemsetsAlreadySanitized.Add(sensitiveTransactionTable[i].Sp);
                    }
                }
                if (!itemsetsAlreadySanitized.Contains(sensitiveTransactionTable[ind].Sp))
                { 
                    
                DepthFirstSearch d = new DepthFirstSearch(sensitiveTransactionTable[ind].Sp, list, itemsetsAlreadySanitized);
                transactionsToModify = new List<int>();
                int[] numberSensitiveItemsetsdContains = new int[d.Result.Count];
                //start sanitizing the selected sensitive itemset 
                int nmodify = sensitiveTransactionTable[ind].SpCount;
                //j is the index in d.result
                int j = d.Result.Count - 1;
                   //if there is no deepest path from the vertex itself then;
                    while (nmodify > 0 && j >= 0)
                {
                        transactionsToModify = new List<int>();
                        List<string> sensitiveItemsetsinTheTransaction = d.Result[j].SensitiveItemsets;
                        victim = selectVictim(sensitiveItemsetsinTheTransaction);
                        //the transaction containing maximum amount of sensitive itemset is added to transactionToModify first
                    for (int z = 0; z < nmodify && z < d.Result[j].Transactions.Count; z++)
                    {
                        transactionsToModify.Add(d.Result[j].Transactions[z]);
                    }
                        nmodify = nmodify - transactionsToModify.Count;
                        //fill out the Sanitization Table
                        if (sanitizationTable.ContainsKey(victim))
                        {
                            foreach(int transaction in transactionsToModify)
                            {
                                sanitizationTable[victim].Add(transaction);
                                deleteTransactions(transaction, d.Result[j].SensitiveItemsets);
                                countOfSensitiveItems[victim] = countOfSensitiveItems[victim] - 1;
                            }
                        }
                        else
                        {
                            sanitizationTable[victim] = new List<int>();
                            foreach(int transaction in transactionsToModify)
                            {
                                sanitizationTable[victim].Add(transaction);
                                deleteTransactions(transaction, d.Result[j].SensitiveItemsets);
                                countOfSensitiveItems[victim] = countOfSensitiveItems[victim] - 1;
                            }
                        }
                        List<string> sensitiveItemsetInTransactionNoContainVictim = new List<string>();
                        //update spCount values of itemsets that encapsulates the victim in SensitiveTransaction Table
                        for (int i = 0; i < sensitiveItemsetsinTheTransaction.Count; i++)
                        {
                            List<string> sensitiveItemset = sensitiveItemsetsinTheTransaction[i].Split(' ').ToList();
                            if (sensitiveItemset.Contains(victim))
                            {
                                for (int k = 0; k < sensitiveTransactionTable.Count; k++)
                                {
                                    if (sensitiveTransactionTable[k].Sp == sensitiveItemsetsinTheTransaction[i])
                                    {
                                        sensitiveTransactionTable[k].SpCount = sensitiveTransactionTable[k].SpCount - transactionsToModify.Count;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                sensitiveItemsetInTransactionNoContainVictim.Add(sensitiveItemsetsinTheTransaction[i]);
                            }
                        }
                        
                        //if there are sensitive itemsets that do not contain the victim select another victim for them
                        while (sensitiveItemsetInTransactionNoContainVictim.Count > 0)
                        {
                            victim = selectVictim(sensitiveItemsetInTransactionNoContainVictim);
                            //victim'ı içeren tüm sensitive itemsetleri sensitiveItemsetInTransactionNoContaininVictim'dan sil
                            List<string> itemsetsThatWillbeSanitized = new List<string>();
                            for (int k = 0; k < sensitiveItemsetInTransactionNoContainVictim.Count; k++)
                            {
                                List<string> itemset = sensitiveItemsetInTransactionNoContainVictim[k].Split(' ').ToList();
                                if (itemset.Contains(victim))
                                {
                                    itemsetsThatWillbeSanitized.Add(sensitiveItemsetInTransactionNoContainVictim[k]);
                                }
                            }
                            foreach(string s in itemsetsThatWillbeSanitized)
                            {
                                sensitiveItemsetInTransactionNoContainVictim.Remove(s);
                            }
                            int newModifyValue=0;
                            for(int l=0;l<sensitiveTransactionTable.Count;l++)
                            {
                                if (sensitiveTransactionTable.ElementAt(l).Sp.Contains(itemsetsThatWillbeSanitized[0]))
                                {
                                    newModifyValue = sensitiveTransactionTable[l].SpCount;
                                    break;
                                }
                            }
                            //if there is more than one itemset containg victim select the newmodify as the highest
                            if(itemsetsThatWillbeSanitized.Count>1)
                            {
                                for(int z=1;z<itemsetsThatWillbeSanitized.Count;z++)
                                {
                                    for(int l=0;l<sensitiveTransactionTable.Count;l++)
                                    {
                                        if(sensitiveTransactionTable[l].Sp==itemsetsThatWillbeSanitized[z])
                                        {
                                           if(newModifyValue<sensitiveTransactionTable[l].SpCount)
                                            {
                                                newModifyValue = sensitiveTransactionTable[l].SpCount;
                                                break;
                                            }
                                        }
                                                }
                                }
                            }
                            //select newnmodify finished
                            //Insert transactions into Sanitization Table as the amount of new NModify values
                            int numberofTransactionsModified = 0;
                            for (int a=0;a<transactionsToModify.Count && a<newModifyValue && newModifyValue>0;a++)
                            {
                                if(!sanitizationTable.ContainsKey(victim))
                                {
                                    sanitizationTable[victim] = new List<int>();
                                }
                                sanitizationTable[victim].Add(transactionsToModify[a]);
                                numberofTransactionsModified++;
                                countOfSensitiveItems[victim] = countOfSensitiveItems[victim] - 1;
                            }

                           foreach(string s in itemsetsThatWillbeSanitized)
                            {
                                for(int k=0;k<sensitiveTransactionTable.Count;k++)
                                {
                                    if(sensitiveTransactionTable[k].Sp==s)
                                    {
                                        sensitiveTransactionTable[k].SpCount = sensitiveTransactionTable[k].SpCount - numberofTransactionsModified;
                                    }
                                }
                            }
                        }
                        
                    
                    j--;
                }
                    if (nmodify>0)
                    {
                        List<string> candidatet = new List<string>();
                        candidatet.Add(sensitiveTransactionTable[ind].Sp);
                        victim = selectVictim(candidatet);
                        transactionsToModify = new List<int>();
                        //if there is any vertex that has a self loop
                        if (list.InverseAdjacencyList.ContainsKey(sensitiveTransactionTable[ind].Sp))
                        {
                            if (list.InverseAdjacencyList[sensitiveTransactionTable[ind].Sp].ContainsKey(sensitiveTransactionTable[ind].Sp))
                            {
                                if (nmodify > 0)
                                {
                                    foreach (int t in list.InverseAdjacencyList[sensitiveTransactionTable[ind].Sp][sensitiveTransactionTable[ind].Sp])
                                    {
                                        if (nmodify > 0)
                                        {
                                            transactionsToModify.Add(t);
                                            nmodify--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }


                                }
                                if (!sanitizationTable.ContainsKey(victim))
                                {
                                    sanitizationTable[victim] = new List<int>();
                                }
                                foreach (int tr in transactionsToModify)
                                {
                                    sanitizationTable[victim].Add(tr);
                                }
                            }
                        }
                        //select transactions randomly if self loop transactions on self loop vertices is not enough
                        if (nmodify > 0)
                        {
                            transactionsToModify = new List<int>();
                            if(list.InverseAdjacencyList.ContainsKey(sensitiveTransactionTable[ind].Sp))
                                {
                                foreach (string inverse in list.InverseAdjacencyList[sensitiveTransactionTable[ind].Sp].Keys)
                                {
                                    if (inverse != sensitiveTransactionTable[ind].Sp)
                                        { 
                                        foreach (int tr in list.InverseAdjacencyList[sensitiveTransactionTable[ind].Sp][inverse])
                                        {
                                            if (nmodify > 0)
                                            {
                                                transactionsToModify.Add(tr);
                                                nmodify--;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                }
                                if (nmodify <= 0)
                                {
                                    break;
                                }
                            }
                           
                            if (!sanitizationTable.ContainsKey(victim))
                            {
                                sanitizationTable[victim] = new List<int>();
                            }
                            foreach (int tr in transactionsToModify)
                            {
                                sanitizationTable[victim].Add(tr);
                            }
                        }
                            if (list.AdjacencyList.ContainsKey(sensitiveTransactionTable[ind].Sp))
                            {
                                foreach (string inverse in list.AdjacencyList[sensitiveTransactionTable[ind].Sp].Keys)
                                {
                                    if (inverse != sensitiveTransactionTable[ind].Sp)
                                    {
                                        foreach (int tr in list.AdjacencyList[sensitiveTransactionTable[ind].Sp][inverse])
                                        {
                                            if (nmodify > 0)
                                            {
                                                transactionsToModify.Add(tr);
                                                nmodify--;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (nmodify <= 0)
                                    {
                                        break;
                                    }
                                }
                                
                                if (!sanitizationTable.ContainsKey(victim))
                                {
                                    sanitizationTable[victim] = new List<int>();
                                }
                                foreach (int tr in transactionsToModify)
                                {
                                    sanitizationTable[victim].Add(tr);
                                }
                            }
                        }
                    }
                   
            }
                ind++;
            }
        }
        private void sanitizeDatabase()
        {
            List<int> transactionsToModify;
            string victim;
            for (int i = 0; i < sanitizationTable.Count; i++)
            {
                transactionsToModify = sanitizationTable.ElementAt(i).Value;
                victim = sanitizationTable.ElementAt(i).Key;
                itemsRemoved = itemsRemoved + transactionsToModify.Count;

                for (int j=0;j<transactionsToModify.Count;j++)
                {
                    string[] splittedData = data[transactionsToModify[j]].Split(' ');
                    int index = 0;
                    int ind = 0;
                    bool found = false;
                    for (int a = 0; a < splittedData.Length; a++)
                    {
                        if (splittedData[a] == victim)
                        {
                            ind = a;
                            found = true;
                            break;
                        }
                    }
                        if (ind != 0)
                        {
                            for (int l = 0; l < ind; l++)
                            {
                                index = index + splittedData[l].Length + 1;
                            }
                        }
                        if (found)
                        {
                            if (data[transactionsToModify[j]].Length > index + victim.Length)
                            {

                                data[transactionsToModify[j]] = data[transactionsToModify[j]].Remove(index, victim.Length + 1);
                            }
                        }
                        else
                        {
                            data[transactionsToModify[j]] = data[transactionsToModify[j]].Remove(index, victim.Length);
                        }
                }
            }
        }
        private void writeResult()
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\cs\Desktop\datasets\out.txt");
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] != "")
                {
                    if (data[i][0] == ' ')
                    {
                        data[i] = data[i].Remove(0, 1);
                    }
                }
                writer.Write(data[i]);
                writer.WriteLine();
            }
            writer.Close();
        }
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long memStart = GC.GetTotalMemory(true);
            Program p = new Program();
            p.readSensitiveDataFromTxt();//reads data from sensitive.txt
            p.readDataFromTxt();//reads data from the given dataset
            List<string> vertices = p.sensitive_data.Keys.ToList();
          
            p.createSensitiveTransactionTable(p.databaseLenght);//creates the sensitive transaction table that stores the 
                                                                //neccesary amount of support count decrease need for each different sensitive itemset
            p.createSanitizationTable();//creates the sanitization table which stores; the victim item and transactions that are going to be modified 
            p.sanitizeDatabase();
            stopwatch.Stop();
            Console.WriteLine("execution time {0}", stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine("number of items removed= {0}", p.itemsRemoved);
            long memEnd = GC.GetTotalMemory(true);
            long diff = memEnd - memStart;
            Console.WriteLine("{0:N0} bytes used", diff / 1024);
            p.writeResult();
            Console.WriteLine("operation finished successfully");
            Console.ReadLine();
        }
    }
}
