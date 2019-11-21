using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPGBS
{

    class DepthFirstSearch
    {
        List<SensitiveTransactions> result;
        

        private Dictionary<string, List<string>> edgeTo;
            private  List<string> marked;
        private string s;//starting vertex
        //-----------yeni kod üstte
        private List<string> discovered;
        private CreateLists list;
        private List<string> longestPath;
        private List<int> transactions;
        private string mainSensitiveItemset;//dpf'nin başladığı vertex
        public List<SensitiveTransactions> Result
        {
            get{
                return this.result;

            }
        }
        private List<int> findTransactions()//finds pattern with length 2
        {

            List<int> tmpTransactions = new List<int>();
            if (list.AdjacencyList.ContainsKey(mainSensitiveItemset))
            {
                foreach (string w in list.AdjacencyList[mainSensitiveItemset].Keys)
                {
                    if (w.Split(' ').Length > 1)
                    {
                        tmpTransactions = list.AdjacencyList[mainSensitiveItemset][w];
                        foreach (int tr in tmpTransactions)
                        {
                            transactions.Add(tr);
                        }
                    }
                }
            }
            if (list.InverseAdjacencyList.ContainsKey(mainSensitiveItemset))
            {
                foreach (string w in list.AdjacencyList[mainSensitiveItemset].Keys)
                {

                    foreach (int tr in list.AdjacencyList[mainSensitiveItemset][w])
                    {
                        if (!transactions.Contains(tr))
                        {
                            transactions.Add(tr);
                        }
                    }
                }
            }
            
                       
            return transactions;
        }
        public DepthFirstSearch(string v,CreateLists list_,List<string> itemsetsAlreadySanitized)
        {
            marked = new List<string>();
            edgeTo = new Dictionary<string, List<string>>();
            
            mainSensitiveItemset = v;
            list = list_;
            DFS(mainSensitiveItemset,itemsetsAlreadySanitized);
                
        }
        private List<int> intersectTransactions(List<int> listA,List<int>listB)
        {
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
        public void DFS(string v,List<string> itemsetsAlreadySanitized)
        {
            SensitiveTransactions s = new SensitiveTransactions();
            List<int> transactions = new List<int>();
            List<int> t = new List<int>();
            List<string> sensitiveItemsets = new List<string>();
            result= new List<SensitiveTransactions>();
            var queue = new Queue<Tuple<string, List<string>, List<int>>>();
            foreach (string c in list.AdjacencyList[v].Keys)
            {
                if (c != v )
                {
                    transactions = list.AdjacencyList[v][c];
                    sensitiveItemsets = new List<string>();
                    sensitiveItemsets.Add(v);
                    sensitiveItemsets.Add(c);

                    if (transactions.Count > 0)
                    {
                        queue.Enqueue(new Tuple<string, List<string>, List<int>>(c, sensitiveItemsets, transactions));
                    }
                }
            }
            
            while (queue.Any())
            {
                var node = queue.Dequeue();
                if (list.AdjacencyList.ContainsKey(node.Item1))
                {

                    bool eklendi = false;
                    foreach (string child in list.AdjacencyList[node.Item1].Keys)
                    {
                        t = intersectTransactions(node.Item3, list.AdjacencyList[node.Item1][child]);
                        if (child!=node.Item1 && t.Count>0)
                        {
                            eklendi = true;
                            sensitiveItemsets = node.Item2;
                            if (itemsetsAlreadySanitized.Contains(child) )
                            {
                                Tuple<string, List<string>, List<int>> tup = new Tuple<string, List<string>, List<int>>(node.Item1, sensitiveItemsets, t);
                                if (!queue.Contains(tup))
                                {
                                    queue.Enqueue(new Tuple<string, List<string>, List<int>>(child, sensitiveItemsets, t));

                                }

                            }
                            else 
                            {
                                sensitiveItemsets.Add(child);
                                string[] tmpItemsets = sensitiveItemsets.ToArray();
                                queue.Enqueue(new Tuple<string, List<string>, List<int>>( child, tmpItemsets.ToList(), t));
                                sensitiveItemsets.Remove(child);
                                    
                            }
                            
                        }
                    }
                    if(!eklendi)
                    {
                        s = new SensitiveTransactions();
                        s.SensitiveItemsets = node.Item2;
                        s.Transactions = node.Item3;
                        //s.LongestSensitiveItemset = node.Item1;
                        result.Add(s);
                        //nodelar en uzun 12 den başlayan pathleri tutar
                    }
                }
                else
                {
                    s = new SensitiveTransactions();
                    s.LongestSensitiveItemset = node.Item1;
                    s.SensitiveItemsets = node.Item2;
                    s.Transactions = node.Item3;
                    result.Add(s);
                }
            }
            //sort result according to numberofSensitive itemsets it contains
            for (int i = 0; i < result.Count; i++) //sort the table according to sp.count values(bubble sort descending order)
            {
                for (int j = result.Count - 1; j > i; j--)
                {
                    if (result[j].SensitiveItemsets.Count < result[j-1].SensitiveItemsets.Count)
                    {
                        SensitiveTransactions highvalue = result[j];
                        result[j] = result[j - 1];
                        result[j - 1] = highvalue;
                    }
                }
            }
        }
        

    }
}
