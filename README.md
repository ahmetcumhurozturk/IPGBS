# IPGBS graph based frequent itemset hiding
Txt files;
	sensitive.txt-->stores sensitive itemsets and their sensitive thresolds. Sensitive itemsets and their
	sensitive thresolds are seperated with "."
	out.txt-->the resulting sanitized database
	dataset.txt-->stores the dataset that will be sanitized
Methods; 
	readSensitiveDataFromTxt();-->reads data from sensitive.txt  
        readDataFromTxt();//reads data from the given dataset

The IPGBS represent sensitive itemset as graph. Sensitive itemsets are represented as vertices. These vertices are connected if 
they exists in the same transaction. The edge labels contain the transaction ids. It is possible to find the transactions containing
maximum amount of sensitive itemsets by uncovering the deepest paths from the graph.
            

